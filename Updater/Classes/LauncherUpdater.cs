using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace Updater
{
    class LauncherUpdater
    {
        private string _urlFiles = "https://rqsteam.tk/binder/Updater";
        public async Task UpdaterLauncher(MainWindow _parentWindow)
        {
            string path = Directory.GetCurrentDirectory();
            XmlTextReader reader = new XmlTextReader(_urlFiles + "/meta.xml");
            List<string> filespath = new List<string>();
            List<string> fileshash = new List<string>();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    while (reader.MoveToNextAttribute())
                    {
                        if (reader.Name == "path")
                        {
                            filespath.Add(reader.Value);
                        }
                        if (reader.Name == "hash")
                        {
                            fileshash.Add(reader.Value);
                        }
                    }
                }
            }
            try
            {
                for (int i = 0; i < filespath.Count; i++)
                {
                    for (int j = 1; j < filespath[i].Split('\\').Length - 1; j++)
                    {
                        Directory.CreateDirectory(path + "\\" + filespath[i].Split('\\')[j]);
                    }
                    if (File.Exists(path + filespath[i]))
                    {
                        if (fileshash[i] != ComputeMD5Checksum(path + filespath[i]))
                        {
                            await WorkingWithFiles.DownloadFile(_urlFiles + filespath[i], path + filespath[i]);
                        }
                    }
                    else
                    {
                        await WorkingWithFiles.DownloadFile(_urlFiles + filespath[i], path + filespath[i]);
                    }
                }
                Process.Start(Directory.GetCurrentDirectory() + "\\Binder.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            _parentWindow.Dispatcher.Invoke(() => Application.Current.Shutdown());
        }

        public static string ComputeMD5Checksum(string path)
        {
            using (FileStream fs = File.OpenRead(path))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] fileData = new byte[fs.Length];
                fs.Read(fileData, 0, (int)fs.Length);
                byte[] checkSum = md5.ComputeHash(fileData);
                string result = BitConverter.ToString(checkSum).Replace("-", String.Empty);
                return result;
            }
        }
    }
}