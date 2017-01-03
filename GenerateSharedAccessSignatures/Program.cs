namespace GenerateSharedAccessSignatures
{
	using System;
	using System.IO;
	using System.Text;
	using Microsoft.Azure;
	using Microsoft.WindowsAzure.Storage;
	using Microsoft.WindowsAzure.Storage.Blob;

	class Program
	{
		static void Main(string[] args)
		{
			// Parse the connection string and return a reference to the storage account.
			var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

			// Create the blob client object.
			var blobClient = storageAccount.CreateCloudBlobClient();

			// Get a reference to a container to use for the sample code, and create it if it does not exist.
			var blobContainer = blobClient.GetContainerReference("sascontainer");
			blobContainer.CreateIfNotExists();

			// Insert calls to the methods created below here...

			// Generate a SAS URI for the container, without a stored access policy.
			var sasUri = GetContainerSasUri(blobContainer);
			Console.WriteLine("Container SAS URI: " + sasUri);
			Console.WriteLine();

			// Generate a SAS URI for a blob within the container, without a stored access policy.
			var sasBlobUri = GetBlobSasUri(blobContainer);
			Console.WriteLine("Blob SAS URI: " + sasBlobUri);
			Console.WriteLine();

			// Clear any existing access policies on container.
			var permissions = blobContainer.GetPermissions();
			permissions.SharedAccessPolicies.Clear();
			blobContainer.SetPermissions(permissions);

			// Create a new access policy on the container, which may be optionally used to provide constraints for
			// shared access signatures on the container and the blob.
			const string SharedAccessPolicyName = "tutorialpolicy";
			CreateSharedAccessPolicy(blobClient, blobContainer, SharedAccessPolicyName);

			// Generate a SAS URI for the container, using a stored access policy to set constraints on the SAS.
			var sasBlobContainerUri = GetContainerSasUriWithPolicy(blobContainer, SharedAccessPolicyName);
			Console.WriteLine("Container SAS URI using stored access policy: " + sasBlobContainerUri);
			Console.WriteLine();

			// Generate a SAS URI for a blob within the container, using a stored access policy to set constraints on the SAS.
			var sasBlobUriWithStoredAccess = GetBlobSasUriWithPolicy(blobContainer, SharedAccessPolicyName);
			Console.WriteLine("Blob SAS URI using stored access policy: " + sasBlobUriWithStoredAccess);
			Console.WriteLine();

			// Require user input before closing the console window.
			Console.ReadLine();
		}

		static string GetContainerSasUri(CloudBlobContainer blobContainer)
		{
			// Set the expiry time and permission for the container.
			// In this case no start time is specified, so the shared access signature becomes valid immediately.
			var sasContraints = new SharedAccessBlobPolicy
			{
				SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
				Permissions = SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.List
			};

			// Generate the shared access signature on the container, setting the constraints directly on the signature.
			var sasContainerToken = blobContainer.GetSharedAccessSignature(sasContraints);

			// Return the URE string for the container, including  the SAS token.
			return blobContainer.Uri + sasContainerToken;
		}

		static string GetBlobSasUri(CloudBlobContainer blobContainer)
		{
			// Get a reference to a blob within the container.
			var blob = blobContainer.GetBlockBlobReference("sasblob.txt");

			// Upload text to the blob. If the blob does not yet exist, is will be created.
			// If the blob does exist, its existing content will be overwritten.
			const string BlobContent = "This blob will be accessible to clients via a Shared Access Signature.";
			var stream = new MemoryStream(Encoding.UTF8.GetBytes(BlobContent)) { Position = 0 };

			using (stream)
			{
				blob.UploadFromStream(stream);
			}

			// Set the expiry time and permissions for the blob.
			// In this case the start time is specified as a few minutes in the past, to mitigate clock skew.
			// The shared access signature will be valid immediately.
			var sasContraints = new SharedAccessBlobPolicy
			{
				SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5),
				SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
				Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write
			};

			// Generate the shared access signature on the blob, setting the constraints directly on the signature.
			var sasBlobToken = blob.GetSharedAccessSignature(sasContraints);

			// Return the URI string for the container, including the SAS token.
			return blob.Uri + sasBlobToken;
		}

		static void CreateSharedAccessPolicy(CloudBlobClient blobClient, CloudBlobContainer blobContainer, string policyName)
		{
			// Create a new shared access policy and define its constraints.
			var sharedPolicy = new SharedAccessBlobPolicy()
			{
				SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
				Permissions = SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.List | SharedAccessBlobPermissions.Read
			};

			// Get the containers existing permissions.
			var permissions = blobContainer.GetPermissions();

			// Add the new policy to the containers permissions, and set the containers permissions.
			permissions.SharedAccessPolicies.Add(policyName, sharedPolicy);
			blobContainer.SetPermissions(permissions);
		}

		static string GetContainerSasUriWithPolicy(CloudBlobContainer blobContainer, string policyName)
		{
			// Generate the shared access signature on the container. In this case, all of the constraints for the
			// shared access signature are specified on the stored access policy.
			var sasContainerToken = blobContainer.GetSharedAccessSignature(null, policyName);

			// Return the URI string for the container, including the SAS token.
			return blobContainer.Uri + sasContainerToken;
		}

		static string GetBlobSasUriWithPolicy(CloudBlobContainer blobContainer, string policyName)
		{
			// Get a reference to a blob within the container.
			var blob = blobContainer.GetBlockBlobReference("sasblobpolicy.txt");

			// Upload text to the blob. If the blob does not yet exist, it will be created.
			// If the blob does exist, its existing content will be overwritten.
			const string BlobContent = "This blob will be accessible to clients via a shared access signature. "
										+ "A stored access policy defines the constraints for the signature.";

			var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(BlobContent)) { Position = 0 };

			using (memoryStream)
			{
				blob.UploadFromStream(memoryStream);
			}

			// Generate the shared access signature on the blob.
			var sasBlobToken = blob.GetSharedAccessSignature(null, policyName);

			// Return the URI for the container, including the SAS token.
			return blob.Uri + sasBlobToken;
		}
	}
}