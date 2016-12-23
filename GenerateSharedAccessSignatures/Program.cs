namespace GenerateSharedAccessSignatures
{
	using System;
	using System.IO;
	using Microsoft.Azure;
	using Microsoft.WindowsAzure;
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
	}
}
