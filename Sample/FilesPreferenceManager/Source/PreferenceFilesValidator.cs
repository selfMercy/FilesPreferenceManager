using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace FilesPreferenceManager
{
    class PreferenceFilesValidator
    {
        /// <summary>
        /// The method allows you to check the integrity of the files, the list of which it received
        /// </summary>
        /// <param name="SaveDirectory">File check root path</param>
        /// <param name="PreferenceFiles">List of preference files</param>
        /// <param name="ValidityMode">File check mode</param>
        /// <returns>List of files edited as a result of the method</returns>
        public List<PreferenceFile> ValidateFiles(string SaveDirectory, List<PreferenceFile> PreferenceFiles, FileValidityMode ValidityMode)
        {
            foreach (PreferenceFile ValidityFile in PreferenceFiles)
            {
                if (!string.IsNullOrEmpty(ValidityFile.Directory))
                    Directory.CreateDirectory(Path.Combine(SaveDirectory, ValidityFile.Directory));

                if (!File.Exists(Path.Combine(SaveDirectory, ValidityFile.Directory, ValidityFile.Name)))
                    continue;

                //Hashing method is better, but sizing faster
                if (ValidityMode == FileValidityMode.Sizing)
                {
                    if (new FileInfo(Path.Combine(SaveDirectory, ValidityFile.Directory, ValidityFile.Name)).Length == ValidityFile.Size)
                        PreferenceFiles.Remove(ValidityFile);
                }
                else if (ValidityMode == FileValidityMode.Hashing)
                {
                    MD5 crypto = MD5.Create();
                    FileStream stream = File.OpenRead(Path.Combine(SaveDirectory, ValidityFile.Directory, ValidityFile.Name));

                    byte[] hashcode = crypto.ComputeHash(stream);
                    string hash = BitConverter.ToString(hashcode).Replace("-", string.Empty);

                    if (hash == ValidityFile.Hash)
                        PreferenceFiles.Remove(ValidityFile);
                }
            }
            return PreferenceFiles;
        }
    }
}
