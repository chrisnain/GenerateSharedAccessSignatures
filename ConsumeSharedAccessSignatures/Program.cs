namespace ConsumeSharedAccessSignatures
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using Microsoft.WindowsAzure.Storage;
	using Microsoft.WindowsAzure.Storage.Blob;

	class Program
	{
		static void Main(string[] args)
		{
			const string ContainerSas =
				@"https://factondevcontentblob.blob.core.windows.net/sascontainer?sv=2016-05-31&sr=c&sig=n7zHrHc8l8gcvzS1LRQ5YODho4OJLTFACIrLwD7QJ6Y%3D&se=2017-01-04T10%3A54%3A20Z&sp=wl";
			const string BlobSas =
				@"https://factondevcontentblob.blob.core.windows.net/sascontainer/sasblob.txt?sv=2016-05-31&sr=b&sig=PpnMeRyzmRd%2FoDIJrjj5JbtiIJJdfH4od5dnJUZ9%2F8Q%3D&st=2017-01-03T10%3A49%3A20Z&se=2017-01-04T10%3A54%3A20Z&sp=rw";
			const string ContainerSasWithAccessPolicy =
				@"https://factondevcontentblob.blob.core.windows.net/sascontainer?sv=2016-05-31&sr=c&si=tutorialpolicy&sig=t8TM596qd%2FmoWhb7ZIWQNW%2F02zyhNCJW%2FsD8G1s%2FuOM%3D";
			const string BlobSasWithAccessPolicy =
				@"https://factondevcontentblob.blob.core.windows.net/sascontainer/sasblobpolicy.txt?sv=2016-05-31&sr=b&si=tutorialpolicy&sig=qsV4UzHMHSlvZtiHqG9FIY%2BBVLvrOsF05XCOpt63zjs%3D";

			// Call the test methods with the shared access signatures created on the container, with and without the access policy.
			UseContainerSas(ContainerSas);
			UseContainerSas(ContainerSasWithAccessPolicy);

			Console.ReadLine();
		}

		static void UseContainerSas(string sas)
		{
			// Try performing container operations with the SAS provided.

			// Return a reference to the container using the SAS URI.
			var blobContainer = new CloudBlobContainer(new Uri(sas));

			// Create a list to store blob URIs returned by a listing operation on the container.
			var blobList = new List<IListBlobItem>();

			// Write operation: write a new blob to the container.
			try
			{
				var blob = blobContainer.GetBlockBlobReference("blobCreatedViaSas.txt");
				const string BlobContent = "This blob was created with a shared access signature granting write permissions to the container. ";
				var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(BlobContent)) { Position = 0 };

				using (memoryStream)
				{
					blob.UploadFromStream(memoryStream);
				}

				Console.WriteLine("Write operation succeeded for SAS " + sas);
				Console.WriteLine();
			}
			catch (StorageException exception)
			{
				Console.WriteLine("Write operation failed for SAS " + sas);
				Console.WriteLine("Addition error information: " + exception.Message);
				Console.WriteLine();
			}

			// List operation: List the blobs in the container.
			try
			{
				blobList.AddRange(blobContainer.ListBlobs());

				Console.WriteLine("List operation succeeded for SAS " + sas);
				Console.WriteLine();
			}
			catch (StorageException exception)
			{
				Console.WriteLine("List operation failed for SAS " + sas);
				Console.WriteLine("Addition error information: " + exception.Message);
				Console.WriteLine();
			}

			// Read operation: Get a reference to one of the blobs in the container and read it.
			try
			{
				var blob = blobContainer.GetBlockBlobReference(((ICloudBlob)blobList[0]).Name);
				var memoryStream = new MemoryStream { Position = 0 };

				using (memoryStream)
				{
					blob.DownloadToStream(memoryStream);
					Console.WriteLine(memoryStream.Length);
				}

				Console.WriteLine("Read operation succeeded for SAS " + sas);
				Console.WriteLine();
			}
			catch (StorageException exception)
			{
				Console.WriteLine("Read operation failed for SAS " + sas);
				Console.WriteLine("Addition error information: " + exception.Message);
				Console.WriteLine();
			}
			Console.WriteLine();

			// Delete operation: Delete blob in the container.
			try
			{
				var blob = blobContainer.GetBlockBlobReference(((ICloudBlob)blobList[0]).Name);
				blob.Delete();

				Console.WriteLine("Delete operation succeeded for SAS " + sas);
				Console.WriteLine();
			}
			catch (StorageException exception)
			{
				Console.WriteLine("Delete operation failed for SAS " + sas);
				Console.WriteLine("Addition error information: " + exception.Message);
				Console.WriteLine();
			}
		}
	}
}
