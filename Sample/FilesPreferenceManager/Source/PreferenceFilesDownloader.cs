using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FilesPreferenceManager
{
    class PreferenceFilesDownloader
    {
        private string SaveDirectory;
        List<PreferenceFile> PreferenceFiles;

        private long TotalBytes;
        private long TotalBytesConfirmed;

        public Thread DownloadThread;

        public PreferenceFilesDownloader(string SaveDirectory, List<PreferenceFile> PreferenceFiles)
        {
            this.SaveDirectory = SaveDirectory;
            this.PreferenceFiles = PreferenceFiles;

            TotalBytes = GetTotalBytes();
        }

        private long GetTotalBytes()
        {
            long bytes = 0;
            
            foreach (PreferenceFile i in PreferenceFiles)
                bytes += i.Size;

            return bytes;
        }

        private void DownloadInjureFile(int index = 0)
        {
            if (index + 1 >= PreferenceFiles.Count)
                return;
            
            WebClient client = new WebClient();
            long LastBytesReceived = 0;
            
            client.DownloadFileCompleted += (s, e) => DownloadInjureFile(++index);

            client.DownloadProgressChanged += (s, e) =>
            {
                TotalBytesConfirmed += (e.BytesReceived - LastBytesReceived);
                LastBytesReceived = e.BytesReceived;
            };

            client.DownloadFileAsync(PreferenceFiles[index].Address, Path.Combine(SaveDirectory, PreferenceFiles[index].Directory, PreferenceFiles[index].Name));
        }

        public void Start()
        {
            DownloadThread = new Thread(delegate () { DownloadInjureFile(); });
            DownloadThread.Start();

            DownloadThread.Join();
        }
    }
}
