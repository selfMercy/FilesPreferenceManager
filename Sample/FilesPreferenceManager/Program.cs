using System;
using System.IO;

namespace FilesPreferenceManager
{
    class Program
    {
        static void Main(string[] args)
        {
            string SavePath = @"C:\Sample folder";
            Uri AddressToJson = new Uri("http://test1.ru/preferenceFiles.json");

            //Hashing is better, but sizing faster than hashing
            FileValidityMode ValidityMode = FileValidityMode.Hashing;

            PreferenceTrackingHandler PreferenceTracker = new PreferenceTrackingHandler();
            
            PreferenceTracker.DownloadEngineStatusChanged += (s, e) =>
            {
                Console.WriteLine(string.Format($"Status: {e.DownloadStatus}"));
            };

            PreferenceTracker.DownloadEngineFileChanged += (s, e) =>
            {
                Console.WriteLine(string.Format($"Download now: {Path.Combine(e.Directory, e.Name)}"));
            };

            PreferenceTracker.DownloadEngineProgressChanged += (s, e) =>
            {
                Console.WriteLine(string.Format($"Downloaded: {e.DownloadedSize} / {e.Size} | Percentage: {e.Percentage}"));
            };

            FilesPreferenceManager PreferenceManager = new FilesPreferenceManager(SavePath, AddressToJson, ValidityMode, PreferenceTracker);
            PreferenceManager.InitializeDownloadEngine();

            Console.ReadKey();
        }
    }
}
