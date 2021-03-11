using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesPreferenceManager
{
    public enum DownloadStatusFlags
    {
        Waiting,
        Validating,
        Downloading,
        Finished
    }

    public class DownloadEngineStatusChangedEventArgs : EventArgs
    {
        public DownloadStatusFlags DownloadStatus = DownloadStatusFlags.Waiting;
    }

    public class DownloadEngineProgressChangedEventArgs : EventArgs
    {
        public long Size = 0;
        public long DownloadedSize = 0;

        public int Percentage = 0;
    }
    
    public class DownloadEngineFileChangedEventArgs : EventArgs
    {
        public string Name = string.Empty;
        public string Directory = string.Empty;
    }
    
    class PreferenceTrackingHandler
    {
        public event EventHandler<DownloadEngineStatusChangedEventArgs> DownloadEngineStatusChanged;
        public DownloadEngineStatusChangedEventArgs DownloadEngineStatusArgs = new DownloadEngineStatusChangedEventArgs();

        public event EventHandler<DownloadEngineProgressChangedEventArgs> DownloadEngineProgressChanged;
        public DownloadEngineProgressChangedEventArgs DownloadEngineProgressArgs = new DownloadEngineProgressChangedEventArgs();

        public event EventHandler<DownloadEngineFileChangedEventArgs> DownloadEngineFileChanged;
        public DownloadEngineFileChangedEventArgs DownloadEngineFileArgs = new DownloadEngineFileChangedEventArgs();
        
        public void DownloadEngineStatusChange(DownloadEngineStatusChangedEventArgs args) =>
            DownloadEngineStatusChanged?.Invoke(this, args);
        
        public void DownloadEngineProgressChange(DownloadEngineProgressChangedEventArgs args) =>
            DownloadEngineProgressChanged?.Invoke(this, args);

        public void DownloadEngineFileChange(DownloadEngineFileChangedEventArgs args) =>
            DownloadEngineFileChanged?.Invoke(this, args);
    }
}
