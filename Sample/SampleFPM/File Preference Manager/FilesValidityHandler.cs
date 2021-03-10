using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;

namespace FilesPreferenceManager
{
    public enum FileValidityMode
    {
        Sizing,
        Hashing
    }

    class FilesValidityHandler
    {
        private string saveDirectory;
        private FileValidityMode validityMode;

        public List<PreferenceFile> preferenceFiles { get; private set; }

        public FilesValidityHandler(string saveDirectory, FileValidityMode validityMode)
        {
            this.saveDirectory = saveDirectory;
            this.validityMode = validityMode;

            Directory.CreateDirectory(saveDirectory);
        }
        
        private void FilesValidityReceiver(Uri FilesValidityAddress)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(FilesValidityAddress);

            webRequest.Method = "get";
            webRequest.Accept = "application/json";
            webRequest.UserAgent = "File Preferecne Manager";

            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
            StreamReader stream = new StreamReader(webResponse.GetResponseStream());

            dynamic receivedObject = new JavaScriptSerializer().Deserialize<dynamic>(stream.ReadToEnd());

            webResponse.Close();

            preferenceFiles = new List<PreferenceFile>();
            PreferenceFile preferenceFile = new PreferenceFile();

            foreach (dynamic item in receivedObject["FILES"])
            {
                preferenceFile.Name = item["NAME"].ToString();
                preferenceFile.Directory = item["DIRECTORY"].ToString();

                preferenceFile.Size = long.Parse(item["SIZE"].ToString());
                preferenceFile.Hash = item["HASH"].ToString();

                preferenceFile.Address = new Uri(item["ADDRESS"].ToString());

                FilesValidityInspector(preferenceFile);
            }
        }

        private void FilesValidityInspector(PreferenceFile validityFile)
        {
            if (!string.IsNullOrEmpty(validityFile.Directory))
                Directory.CreateDirectory(Path.Combine(saveDirectory, validityFile.Directory));

            if (!File.Exists(Path.Combine(saveDirectory, validityFile.Directory, validityFile.Name)))
                preferenceFiles.Add(validityFile);
            else
            {
                if (validityMode == FileValidityMode.Sizing) //this method faster than hashing
                {
                    if (new FileInfo(Path.Combine(saveDirectory, validityFile.Directory, validityFile.Name)).Length != validityFile.Size)
                        preferenceFiles.Add(validityFile);
                }
                else if (validityMode == FileValidityMode.Hashing)
                {
                    MD5 crypto = MD5.Create();
                    FileStream stream = File.OpenRead(Path.Combine(saveDirectory, validityFile.Directory, validityFile.Name));

                    byte[] hashcode = crypto.ComputeHash(stream);
                    string hash = BitConverter.ToString(hashcode).Replace("-", string.Empty);

                    if (hash != validityFile.Hash)
                        preferenceFiles.Add(validityFile);
                }
            }
        }

        public bool BeginValidateFiles(Uri FilesValidityAddress)
        {
            Thread validateThread = new Thread(delegate () { FilesValidityReceiver(FilesValidityAddress); });
            validateThread.Start();

            validateThread.Join();

            return true;
        }
    }
}
