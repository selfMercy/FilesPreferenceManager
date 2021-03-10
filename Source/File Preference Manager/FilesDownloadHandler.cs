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

    class FilesDownloadHandler
    {
        #region DownloadEvent

        public event EventHandler<DownloadHandlerEventArgs> DownloadTracker;

        public enum DownloadStatusFlags
        {
            Waiting,
            Downloading,
            Finished
        }

        public class DownloadHandlerEventArgs : EventArgs
        {
            public string Name = string.Empty;
            public string Directory = string.Empty;

            public double Size = 0;
            public int Percentage = 0;

            public DownloadStatusFlags DownloadStatus = DownloadStatusFlags.Waiting;
        }

        private DownloadHandlerEventArgs TrackingArgs = new DownloadHandlerEventArgs();

        #endregion

        private string saveDirectory;
        private List<PreferenceFile> preferenceFiles;

        private double totalBytes = 0;
        private double totalBytesConfirmed = 0;

        public FilesDownloadHandler(string saveDirectory, List<PreferenceFile> preferenceFiles)
        {
            this.saveDirectory = saveDirectory;
            this.preferenceFiles = preferenceFiles;

            foreach (PreferenceFile i in preferenceFiles)
                totalBytes += i.Size;
        }
        
        private void DownloadInjureFile(int index = 0)
        {
            if (index + 1 >= preferenceFiles.Count)
            {
                if (DownloadTracker != null)
                {
                    TrackingArgs.DownloadStatus = DownloadStatusFlags.Finished;
                    TrackingArgs.Percentage = 100;

                    DownloadTracker(this, TrackingArgs);
                }
                return;
            }
            WebClient client = new WebClient();

            client.DownloadFileCompleted += (s, e) => DownloadInjureFile(++index);

            long lastBytesReceived = 0;

            client.DownloadProgressChanged += (s, e) =>
            {
                totalBytesConfirmed += (e.BytesReceived - lastBytesReceived);
                lastBytesReceived = e.BytesReceived;

                if (DownloadTracker != null)
                {
                    TrackingArgs.Percentage = Convert.ToInt32(totalBytesConfirmed / totalBytes * 100.0);
                    DownloadTracker(this, TrackingArgs);
                }
            };

            if (DownloadTracker != null)
            {
                TrackingArgs.Name = preferenceFiles[index].Name;
                TrackingArgs.Directory = preferenceFiles[index].Directory;
                TrackingArgs.Size = preferenceFiles[index].Size;

                DownloadTracker(this, TrackingArgs);
            }

            client.DownloadFileAsync(preferenceFiles[index].Address, Path.Combine(saveDirectory, preferenceFiles[index].Directory, preferenceFiles[index].Name));
        }

        public bool BeginDownloadFiles()
        {
            if (DownloadTracker != null)
            {
                TrackingArgs.DownloadStatus = DownloadStatusFlags.Downloading;
                DownloadTracker(this, TrackingArgs);
            }

            Thread downloadThread = new Thread(delegate () { DownloadInjureFile(); });
            downloadThread.Start();

            downloadThread.Join();

            return true;
        }
    }
}
