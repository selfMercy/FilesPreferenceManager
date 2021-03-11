using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace FilesPreferenceManager
{
    public struct PreferenceFile
    {
        public string Name;
        public string Directory;

        public long Size;
        public string Hash;

        public Uri Address;
    }

    class PreferenceFilesDownloader
    {
        private string SaveDirectory;
        List<PreferenceFile> PreferenceFiles;
        private PreferenceTrackingHandler PreferenceTracker;

        private long TotalBytes;
        private long TotalBytesConfirmed;

        public Thread DownloadThread;

        public PreferenceFilesDownloader(string SaveDirectory, List<PreferenceFile> PreferenceFiles, ref PreferenceTrackingHandler PreferenceTracker)
        {
            this.SaveDirectory = SaveDirectory;
            this.PreferenceFiles = PreferenceFiles;
            this.PreferenceTracker = PreferenceTracker;

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
            if (index >= PreferenceFiles.Count)
            {
                ///Tracking change to download status
                PreferenceTracker.DownloadEngineStatusArgs.DownloadStatus = DownloadStatusFlags.Finished;
                PreferenceTracker.DownloadEngineStatusChange(PreferenceTracker.DownloadEngineStatusArgs);
                
                return;
            }

            ///Tracking change to downloading file
            PreferenceTracker.DownloadEngineFileArgs.Name = PreferenceFiles[index].Name;
            PreferenceTracker.DownloadEngineFileArgs.Directory = PreferenceFiles[index].Directory;

            PreferenceTracker.DownloadEngineFileChange(PreferenceTracker.DownloadEngineFileArgs);
            

            WebClient client = new WebClient();
            long LastBytesReceived = 0;
            
            client.DownloadFileCompleted += (s, e) => DownloadInjureFile(++index);

            client.DownloadProgressChanged += (s, e) =>
            {
                TotalBytesConfirmed += (e.BytesReceived - LastBytesReceived);
                LastBytesReceived = e.BytesReceived;

                int Percentage = Convert.ToInt32((double)TotalBytesConfirmed / (double)TotalBytes * 100.0);

                ///Tracking change to downloading progress
                PreferenceTracker.DownloadEngineProgressArgs.Size = PreferenceFiles[index].Size;
                PreferenceTracker.DownloadEngineProgressArgs.DownloadedSize = e.BytesReceived;

                PreferenceTracker.DownloadEngineProgressArgs.Percentage = Percentage;

                PreferenceTracker.DownloadEngineProgressChange(PreferenceTracker.DownloadEngineProgressArgs);
            };

            client.DownloadFileAsync(PreferenceFiles[index].Address, Path.Combine(SaveDirectory, PreferenceFiles[index].Directory, PreferenceFiles[index].Name));
        }

        public void Start()
        {
            ///Tracking change to download status
            PreferenceTracker.DownloadEngineStatusArgs.DownloadStatus = DownloadStatusFlags.Downloading;
            PreferenceTracker.DownloadEngineStatusChange(PreferenceTracker.DownloadEngineStatusArgs);
            

            DownloadThread = new Thread(delegate () { DownloadInjureFile(); });
            DownloadThread.Start();

            DownloadThread.Join();
        }
    }
}
