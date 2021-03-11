using System;

namespace FilesPreferenceManager
{
    public enum FileValidityMode
    {
        Sizing,
        Hashing
    }

    public struct PreferenceFile
    {
        public string Name;
        public string Directory;

        public long Size;
        public string Hash;

        public Uri Address;
    }

    public enum DownloadStatusFlags
    {
        Waiting,
        Validating,
        Downloading,
        Finished
    }

    public class DownloadEngineEventArgs : EventArgs
    {
        public string Name = string.Empty;
        public string Directory = string.Empty;

        public double Size = 0;
        public int Percentage = 0;

        public DownloadStatusFlags DownloadStatus = DownloadStatusFlags.Waiting;
    }
}
