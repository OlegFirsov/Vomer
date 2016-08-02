using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;

namespace Vomer
{
    public class JsDialogHandler: IJsDialogHandler
    {
        public bool OnJSDialog(IWebBrowser browserControl, IBrowser browser, string originUrl, string acceptLang, CefJsDialogType dialogType, string messageText, string defaultPromptText, IJsDialogCallback callback, ref bool suppressMessage)
        {
            if (messageText.Contains("Web notification not supported"))
            {
                suppressMessage = true;
                //browserControl.ExecuteScriptAsync("notification.requestPermission(function(permission){});");
                //browserControl.ExecuteScriptAsync("toastr.info('Notification works!')");
            }
            if (messageText.Contains("error"))
            {
                suppressMessage = true;
            }
            return false;
        }

        public bool OnJSBeforeUnload(IWebBrowser browserControl, IBrowser browser, string message, bool isReload, IJsDialogCallback callback)
        {
            //NOTE: No need to execute the callback if you return false
            // callback.Continue(true);

            //NOTE: Returning false will trigger the default behaviour, you need to return true to handle yourself.
            return false;
        }

        public void OnResetDialogState(IWebBrowser browserControl, IBrowser browser)
        {

        }

        public void OnDialogClosed(IWebBrowser browserControl, IBrowser browser)
        {

        }
    }
}
