using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;

namespace FilesPreferenceManager
{
    class PreferenceFilesReceiver
    {
        /// <summary>
        /// The method allows you to get a json object from a web server
        /// </summary>
        /// <param name="AddressToFiles">Direct address to json file</param>
        /// <returns>List of files from json object</returns>
        public List<PreferenceFile> Receive(Uri AddressToFiles)
        {
            HttpWebRequest HttpRequest = (HttpWebRequest)WebRequest.Create(AddressToFiles);
            
            HttpRequest.Method = "GET";
            HttpRequest.Accept = "application/json";
            HttpRequest.UserAgent = "Files Preference Manager";

            HttpWebResponse WebResponse = (HttpWebResponse)HttpRequest.GetResponse();
            StreamReader Stream = new StreamReader(WebResponse.GetResponseStream());

            dynamic ReceivedObject = new JavaScriptSerializer().Deserialize<dynamic>(Stream.ReadToEnd());
            
            WebResponse.Close();
            
            List<PreferenceFile> PreparedFiles = new List<PreferenceFile>();
            PreferenceFile File = new PreferenceFile();
            
            foreach (dynamic item in ReceivedObject["FILES"])
            {
                File.Name = item["NAME"].ToString();
                File.Directory = item["DIRECTORY"].ToString();

                File.Size = long.Parse(item["SIZE"].ToString());
                File.Hash = item["HASH"].ToString();

                File.Address = new Uri(item["ADDRESS"].ToString());

                PreparedFiles.Add(File);
            }
            return PreparedFiles;
        }
    }
}
