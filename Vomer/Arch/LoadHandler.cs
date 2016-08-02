using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CefSharp;

namespace Vomer
{
    /// <summary>
    /// Implement this interface to handle events related to browser load status.
    /// The methods of this interface will be called on the CEF UI thread. Blocking in these methods
    /// will likely cause your UI to become unresponsive and/or hang.
    /// </summary>
    public class LoadHandler: ILoadHandler
    {
        /// <summary>
        /// Called when the loading state has changed. This callback will be executed twice
        /// once when loading is initiated either programmatically or by user action,
        /// and once when loading is terminated due to completion, cancellation of failure.
        /// This method will be called on the CEF UI thread.
        /// Blocking this thread will likely cause your UI to become unresponsive and/or hang.
        /// </summary>
        /// <param name="browserControl">The <see cref="IWebBrowser"/> control this popup is related to.</param>
        /// <param name="loadingStateChangedArgs">args</param>
        public void OnLoadingStateChange(IWebBrowser browserControl, LoadingStateChangedEventArgs loadingStateChangedArgs)
        {      

        }

        /// <summary>
        /// Called when the browser begins loading a frame.
        /// The <see cref="FrameLoadEndEventArgs.Frame"/> value will never be empty
        /// Check the <see cref="IFrame.IsMain"/> method to see if this frame is the main frame.
        /// Multiple frames may be loading at the same time. Sub-frames may start or continue loading after the main frame load has ended.
        /// This method may not be called for a particular frame if the load request for that frame fails.
        /// For notification of overall browser load status use <see cref="OnLoadingStateChange"/> instead. 
        /// This method will be called on the CEF UI thread.
        /// Blocking this thread will likely cause your UI to become unresponsive and/or hang.
        /// </summary>
        /// <param name="browserControl">The <see cref="IWebBrowser"/> control this popup is related to.</param>
        /// <param name="frameLoadStartArgs">args</param>
        /// <remarks>Whilst thist may seem like a logical place to execute js, it's called before the DOM has been loaded, implement
        /// <see cref="IRenderProcessMessageHandler.OnContextCreated"/> as it's called when the underlying V8Context is created
        /// (Only called for the main frame at this stage)</remarks>
        public void OnFrameLoadStart(IWebBrowser browserControl, FrameLoadStartEventArgs frameLoadStartArgs)
        {
            
        }

        /// <summary>
        /// Called when the browser is done loading a frame.
        /// The <see cref="FrameLoadEndEventArgs.Frame"/> value will never be empty
        /// Check the <see cref="IFrame.IsMain"/> method to see if this frame is the main frame.
        /// Multiple frames may be loading at the same time. Sub-frames may start or continue loading after the main frame load has ended.
        /// This method will always be called for all frames irrespective of whether the request completes successfully. 
        /// This method will be called on the CEF UI thread.
        /// Blocking this thread will likely cause your UI to become unresponsive and/or hang.
        /// </summary>
        /// <param name="browserControl">The <see cref="IWebBrowser"/> control this popup is related to.</param>
        /// <param name="frameLoadEndArgs">args</param>
        public void OnFrameLoadEnd(IWebBrowser browserControl, FrameLoadEndEventArgs frameLoadEndArgs)
        {
            //It is for test!!!!!!!!!!! Then clear it!
            /*if (browserControl.IsLoading)
            {
                browserControl.ExecuteScriptAsync("var form1 = document.createElement('form')");
                browserControl.ExecuteScriptAsync("document.body.appendChild(form1)");
                browserControl.ExecuteScriptAsync("var input1 = document.createElement('input')");
                browserControl.ExecuteScriptAsync("input1.type = 'text'");
                browserControl.ExecuteScriptAsync("input1.name = 'textNotification'");
                browserControl.ExecuteScriptAsync("input1.value = 'Уведомление'");
                browserControl.ExecuteScriptAsync("form1.appendChild(input1)");
            }*/
        }

        /// <summary>
        /// Called when the resource load for a navigation fails or is canceled.
        /// <see cref="LoadErrorEventArgs.ErrorCode"/> is the error code number, <see cref="LoadErrorEventArgs.ErrorText"/> is the error text and
        /// <see cref="LoadErrorEventArgs.FailedUrl"/> is the URL that failed to load. See net\base\net_error_list.h
        /// for complete descriptions of the error codes.
        /// This method will be called on the CEF UI thread.
        /// Blocking this thread will likely cause your UI to become unresponsive and/or hang.
        /// </summary>
        /// <param name="browserControl">The <see cref="IWebBrowser"/> control this popup is related to.</param>
        /// <param name="loadErrorArgs">args</param>

        public void OnLoadError(IWebBrowser browserControl, LoadErrorEventArgs loadErrorArgs)
        { 
        
        }

    }
}
