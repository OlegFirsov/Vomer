using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;
using System.Diagnostics;
using System.Windows.Forms;

namespace Vomer
{
    public class DownloadHandler : IDownloadHandler
    {
        private string sessionCookie = null;
        private string suggestedFilename = null;
        // without the session, the file cannot be downloaded
        public void setSessionCookie(string sessionCookie)
        {
            this.sessionCookie = sessionCookie;
        }

        public void OnBeforeDownload(IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
        {
            if (!callback.IsDisposed)
            {
                /*using (callback)
                {
                    callback.Continue(downloadItem.SuggestedFileName, showDialog: true);
                }*/
                Process downloadProcess = new Process();
                try
                {
                    downloadProcess.StartInfo.UseShellExecute = true;
                    downloadProcess.StartInfo.FileName = downloadItem.Url;
                    downloadProcess.Start();                    
                }
                catch (Exception e)
                {
                    MessageBox.Show("Не удалось запустить броузер по умолчанию." + "  " + e.Message.ToString());
                }
            }
        }

        public void OnDownloadUpdated(IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
        {
            
        }
    }
}
