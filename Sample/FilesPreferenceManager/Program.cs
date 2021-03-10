using System;

namespace FilesPreferenceManager
{
    class Program
    {
        static void Main(string[] args)
        {
            string SavePath = @"C:\Sample folder";
            Uri AddressToJson = new Uri("http://test1.ru/preferenceFiles.json");

            FileValidityMode ValidityMode = FileValidityMode.Hashing; //Hashing is better, but sizing faster that hashing
            
            FilesPreferenceManager PreferenceManager = new FilesPreferenceManager(SavePath, AddressToJson, ValidityMode);
            PreferenceManager.InitializeDownloadEngine();

            Console.ReadKey();
        }
    }
}
