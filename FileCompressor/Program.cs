using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCompressor
{
	using System.IO;
	using System.IO.Compression;

	class Program
	{
		private const string TempFolder = "D:\\Temp\\";

		static void Main()
		{
			var fileStorage = new List<string> { "exchangeRates.txt", "machines.txt" };
			CompressFileStorage("test.zip", fileStorage);
		}

		public static string CompressFileStorage(string zipFileName, List<string> fileStorage)
		{
			var zipFile = new FileInfo(TempFolder + zipFileName);
			var fileStream = zipFile.Create();

			using (var zip = new ZipArchive(fileStream, ZipArchiveMode.Create))
			{
				foreach (var fileName in fileStorage)
				{
					zip.CreateEntryFromFile(TempFolder + fileName, fileName, CompressionLevel.Optimal);
				}
			}

			return zipFile.Name;
		}
	}
}
