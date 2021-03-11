# Files Preference Manager
Assistant in downloading files and checking their integrity.
As well as tracking downloads and deleting unnecessary files.

# Sample
```csharp
// The path to the folder for downloading and checking files.
string SavePath = @"C:\Sample folder";

// Link to json file, it is generated using PreferenceGenerator.php
Uri AddressToJson = new Uri("http://test1.ru/preferenceFiles.json");

// There are two modes for checking files. (Hashing/Sizing)
// The first is more accurate, but the second is much faster.
FileValidityMode ValidityMode = FileValidityMode.Hashing; //or FileValidityMode.Sizing;

PreferenceTrackingHandler PreferenceTracker = new PreferenceTrackingHandler();

PreferenceTracker.DownloadEngineStatusChanged += (s, e) => // Fires when the download state changes.
{
    Console.WriteLine(string.Format($"Status: {e.DownloadStatus}"));
};

PreferenceTracker.DownloadEngineFileChanged += (s, e) => // Fires when the next file starts downloading.
{
    Console.WriteLine(string.Format($"Download now: {Path.Combine(e.Directory, e.Name)}"));
};

PreferenceTracker.DownloadEngineProgressChanged += (s, e) => // Fires when new bytes are received.
{
    Console.WriteLine(string.Format($"Downloaded: {e.DownloadedSize} / {e.Size} | Percentage: {e.Percentage}"));
};

// Declaring the main instance of the manager class and initializing the download engine.
FilesPreferenceManager PreferenceManager = new FilesPreferenceManager(SavePath, AddressToJson, ValidityMode, PreferenceTracker);
PreferenceManager.InitializeDownloadEngine();
```
