
using FilesPreferenceManager;
using System;
using System.IO;

namespace SampleFPM
{
    class FilesPreferenceManager
    {
        static void Main(string[] args)
        {
            string sampleDirectory = @"C:\Sample folder";
            Uri sampleFilesAddress = new Uri("http://test1.ru/preferenceFiles.json");

            FilesValidityHandler validityHandler = new FilesValidityHandler(sampleDirectory, FileValidityMode.Sizing);

            if (validityHandler.BeginValidateFiles(sampleFilesAddress))
            {
                FilesDownloadHandler downloadHandler = new FilesDownloadHandler(sampleDirectory, validityHandler.preferenceFiles);

                downloadHandler.DownloadTracker += (s, e) =>
                {
                    Console.WriteLine(string.Format($"Status: {e.DownloadStatus}"));
                    Console.WriteLine(string.Format($"Downloading file: {Path.Combine(e.Directory, e.Name)} | Size: {e.Size} | Percentage: {e.Percentage}"));

                    Console.WriteLine();
                };

                downloadHandler.BeginDownloadFiles();
            }
            Console.ReadKey();
        }
    }
}
