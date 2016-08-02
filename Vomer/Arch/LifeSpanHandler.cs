using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;
using System.Diagnostics;
using System.Windows.Forms;
using CefSharp.WinForms;

namespace Vomer
{
    public class LifeSpanHandler : ILifeSpanHandler
    {
        public bool OnBeforePopup(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {            
            var chromiumWebBrowser = (ChromiumWebBrowser)browserControl;
            ChromiumWebBrowser newChromiumBrowser = new ChromiumWebBrowser(targetUrl);
            /*chromiumWebBrowser.Invoke(new Action(() =>
            {
                newChromiumBrowser =  chromiumWebBrowser.GetMainFrame();
            }));*/
            newBrowser = newChromiumBrowser;            
            //string path = request.Url;
            //if (request.TransitionType == TransitionType.LinkClicked && request.ResourceType == ResourceType.MainFrame)
            {
                Process downloadProcess = new Process();
                try
                {
                    downloadProcess.StartInfo.UseShellExecute = true;
                    downloadProcess.StartInfo.FileName = targetUrl;
                    downloadProcess.Start();
                    return false;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Не удалось запустить броузер по умолчанию." + "  " + e.Message.ToString());
                }
            }    
              
            return false;//true - block popup    false - allow popup
        }

        public void OnAfterCreated(IWebBrowser browserControl, IBrowser browser)
        {

        }

        public bool DoClose(IWebBrowser browserControl, IBrowser browser)
        {
            return false;
        }

        public void OnBeforeClose(IWebBrowser browserControl, IBrowser browser)
        {
            
        }
        
    }
}
