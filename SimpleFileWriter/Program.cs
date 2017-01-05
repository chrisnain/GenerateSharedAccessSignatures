namespace SimpleFileWriter
{
	using System.IO;

	class Program
	{
		static void Main()
		{
			using (var streamWriter = new StreamWriter("D:\\Temp\\sasUrl.txt"))
			{
				streamWriter.Write("http://stackoverflow.com/users/909980/christoph-br%c3%bcckmann");
			}
		}
	}
}
