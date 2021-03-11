using System;
using System.Collections.Generic;
using System.IO;

namespace FilesPreferenceManager
{
    class FilesPreferenceManager
    {
        private string SaveDirectory;
        private Uri AddressToFiles;
        private FileValidityMode ValidityMode;

        /// <summary>
        /// Files preference manager allows you to check the integrity of files, download missing or injured ones, as well as delete unnecessary ones and track all this
        /// </summary>
        /// <param name="SaveDirectory">Root path for downloading and checking files</param>
        /// <param name="AddressToFiles">URL address to json file</param>
        /// <param name="ValidityMode">File check mode</param>
        public FilesPreferenceManager(string SaveDirectory, Uri AddressToFiles, FileValidityMode ValidityMode)
        {
            this.SaveDirectory = SaveDirectory;
            this.AddressToFiles = AddressToFiles;
            this.ValidityMode = ValidityMode;

            Directory.CreateDirectory(SaveDirectory);
        }

        /// <summary>
        /// If you need to get a list of injured files, then use this method
        /// </summary>
        /// <returns>List of injured files</returns>
        public List<PreferenceFile> GetInjuredFiles() =>
            new PreferenceFilesValidator().ValidateFiles(SaveDirectory, new PreferenceFilesReceiver().Receive(AddressToFiles), ValidityMode);

        public void InitializeDownloadEngine()
        {
            PreferenceFilesDownloader DownloadEngine = new PreferenceFilesDownloader(SaveDirectory, GetInjuredFiles());

            ///TODO: Download tracker
            ///TODO: Deleting unnecessary files

            DownloadEngine.Start();
        }
    }
}
