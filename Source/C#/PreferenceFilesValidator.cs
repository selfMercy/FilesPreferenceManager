using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace FilesPreferenceManager
{
    public enum FileValidityMode
    {
        Sizing,
        Hashing
    }

    class PreferenceFilesValidator
    {
        /// <summary>
        /// This method allows you to check the integrity of the files, the list of which it received
        /// </summary>
        /// <param name="SaveDirectory">File check root path</param>
        /// <param name="PreferenceFiles">List of preference files</param>
        /// <param name="ValidityMode">File check mode</param>
        /// <returns>List of files edited as a result of the method</returns>
        public List<PreferenceFile> ValidateFiles(string SaveDirectory, List<PreferenceFile> PreferenceFiles, FileValidityMode ValidityMode, ref PreferenceTrackingHandler PreferenceTracker)
        {
            ///Tracking change to download status
            PreferenceTracker.DownloadEngineStatusArgs.DownloadStatus = DownloadStatusFlags.Validating;
            PreferenceTracker.DownloadEngineStatusChange(PreferenceTracker.DownloadEngineStatusArgs);

            List<PreferenceFile> PreferenceInjuredFiles = new List<PreferenceFile>();

            foreach (PreferenceFile ValidityFile in PreferenceFiles)
            {
                if (!string.IsNullOrEmpty(ValidityFile.Directory))
                    Directory.CreateDirectory(Path.Combine(SaveDirectory, ValidityFile.Directory));

                if (!File.Exists(Path.Combine(SaveDirectory, ValidityFile.Directory, ValidityFile.Name)))
                {
                    PreferenceInjuredFiles.Add(ValidityFile);
                    continue;
                }
                

                if (ValidityMode == FileValidityMode.Sizing)
                {
                    if (new FileInfo(Path.Combine(SaveDirectory, ValidityFile.Directory, ValidityFile.Name)).Length != ValidityFile.Size)
                        PreferenceInjuredFiles.Add(ValidityFile);
                }
                else if (ValidityMode == FileValidityMode.Hashing)
                {
                    MD5 Crypto = MD5.Create();
                    FileStream Stream = File.OpenRead(Path.Combine(SaveDirectory, ValidityFile.Directory, ValidityFile.Name));

                    byte[] Hashcode = Crypto.ComputeHash(Stream);
                    string Hash = (BitConverter.ToString(Hashcode).Replace("-", string.Empty)).ToLower();
                    
                    if (Hash != ValidityFile.Hash)
                        PreferenceInjuredFiles.Add(ValidityFile);
                }
            }
            return PreferenceInjuredFiles;
        }
    }
}
