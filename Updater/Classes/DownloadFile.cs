using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Updater
{
    public delegate void OnChangeSpeedAndPercentDownloading(double percent);

    class WorkingWithFiles
    {
        public static event OnChangeSpeedAndPercentDownloading eventPercentAndSpeedOnChange;

        public static async Task DownloadFile(string sourceUrl, string destinationPath)
        {
            if (File.Exists(destinationPath)) File.Delete(destinationPath);

            Stream saveFileStream = null, responseStream = null;
            HttpWebResponse response = null;

            string tempPath = destinationPath + ".downloading";

            try
            {
                sw.Start();
                saveFileStream = new FileStream(tempPath, FileMode.Append, FileAccess.Write, FileShare.Read);
                var request = (HttpWebRequest)WebRequest.Create(sourceUrl);
                request.AddRange(saveFileStream.Position);
                response = (HttpWebResponse)(await request.GetResponseAsync());
                responseStream = response.GetResponseStream();

                long iFileSize = response.ContentLength;
                long iExistLen = 0;

                int iByteSize;
                byte[] downBuffer = new byte[1024 * 1000];

                while ((iByteSize = responseStream.Read(downBuffer, 0, downBuffer.Length)) > 0)
                {
                    saveFileStream.Write(downBuffer, 0, iByteSize);
                    iExistLen += iByteSize;
                    eventPercentAndSpeedOnChange((double)iExistLen / (double)iFileSize * 100);
                }
                sw.Stop();
            }
            finally
            {
                if (responseStream != null) responseStream.Close();
                if (response != null) response.Dispose();
                if (saveFileStream != null) saveFileStream.Close();
            }

            File.Move(tempPath, destinationPath);
        }

        public static Stopwatch sw = new Stopwatch();
    }
}