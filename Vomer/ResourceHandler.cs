using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using CefSharp;
using System.IO;
using System.Net;

namespace Vomer
{
    //Get Notification from site for Desktop!!!!!! 
    


    /// <summary>
    /// Class ResourceHandler.
    /// </summary>
    internal class ResourceHandler : IResourceHandler
    {
        /// <summary>
        /// MimeType to be used if none provided
        /// </summary>
        private HttpWebRequest webRequest;
        private HttpWebResponse webResponse;
        private Stream requestStream;
        private byte[] requestBytes;
        ICallback callback;
        private const string DefaultMimeType = "text/html";

        /// <summary>
        /// Path of the underlying file
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Gets or sets the Mime Type.
        /// </summary>
        /// <value>The Mime Type.</value>
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the resource stream.
        /// </summary>
        /// <value>The stream.</value>
        public Stream Stream { get; private set; }

        /// <summary>
        /// Gets or sets the http status code.
        /// </summary>
        /// <value>The http status code.</value>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the status text.
        /// </summary>
        /// <value>The status text.</value>
        public string StatusText { get; set; }

        /// <summary>
        /// Gets or sets the headers.
        /// </summary>
        /// <value>The headers.</value>
        public NameValueCollection Headers { get; private set; }

        /// <summary>
        /// Specify which type of resource handle represnets
        /// </summary>
        public ResourceHandlerType Type { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceHandler"/> class.
        /// </summary>
        public ResourceHandler(string mimeType, ResourceHandlerType type)
        {
            if (string.IsNullOrEmpty(mimeType))
            {
                throw new ArgumentNullException("mimeType", "Please provide a valid mimeType");
            }
            
            StatusCode = 200;
            StatusText = "OK";
            MimeType = mimeType;
            Headers = new NameValueCollection();
            Type = type;
        }

        /// <summary>
        /// Gets the resource from the file.
        /// </summary>
        /// <param name="fileName">Location of the file.</param>
        /// <param name="fileExtension">The file extension.</param>
        /// <returns>ResourceHandler.</returns>
        public static ResourceHandler FromFileName(string fileName, string fileExtension = null)
        {
            var mimeType = string.IsNullOrEmpty(fileExtension) ? DefaultMimeType : GetMimeType(fileExtension);
            return new ResourceHandler(mimeType, ResourceHandlerType.File) { FilePath = fileName };
        }

        /// <summary>
        /// Gets the resource from the string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="fileExtension">The file extension.</param>
        /// <returns>ResourceHandler.</returns>
        public static ResourceHandler FromString(string text, string fileExtension)
        {
            var mimeType = GetMimeType(fileExtension);
            return FromString(text, Encoding.UTF8, false, mimeType);
        }

        /// <summary>
        /// Gets a <see cref="ResourceHandler"/> that represents a string.
        /// Without a Preamble, Cef will use BrowserSettings.DefaultEncoding to load the html.
        /// </summary>
        /// <param name="text">The html string</param>
        /// <param name="encoding">Character Encoding</param>
        /// <param name="includePreamble">Include encoding preamble</param>
        /// <param name="mimeType">Mime Type</param>
        /// <returns>ResourceHandler</returns>
        public static ResourceHandler FromString(string text, Encoding encoding = null, bool includePreamble = true, string mimeType = DefaultMimeType)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return new ResourceHandler(mimeType, ResourceHandlerType.Stream) { Stream = GetStream(text, encoding, includePreamble) };
        }

        /// <summary>
        /// Gets the resource from a stream.
        /// </summary>
        /// <param name="stream">A stream of the resource.</param>
        /// <param name="mimeType">Type of MIME.</param>
        /// <returns>ResourceHandler.</returns>
        public static ResourceHandler FromStream(Stream stream, string mimeType = DefaultMimeType)
        {
            return new ResourceHandler(mimeType, ResourceHandlerType.Stream) { Stream = stream };
        }

        private static MemoryStream GetStream(string text, Encoding encoding, bool includePreamble)
        {
            if (includePreamble)
            {
                var preamble = encoding.GetPreamble();
                var bytes = encoding.GetBytes(text);

                var memoryStream = new MemoryStream(preamble.Length + bytes.Length);

                memoryStream.Write(preamble, 0, preamble.Length);
                memoryStream.Write(bytes, 0, bytes.Length);

                memoryStream.Position = 0;

                return memoryStream;
            }

            return new MemoryStream(encoding.GetBytes(text));
        }

        //TODO: Replace with call to CefGetMimeType (little difficult at the moment with no access to the CefSharp.Core class from here)
        private static readonly IDictionary<string, string> Mappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) 
        {
            // Combination of values from Windows 7 Registry and  C:\Windows\System32\inetsrv\config\applicationHost.config
            {".bmp", "image/bmp"},
            {".css", "text/css"},
            {".gif", "image/gif"},
            {".htm", "text/html"},
            {".html", "text/html"},
            {".ico", "image/x-icon"},
            {".jpeg", "image/jpeg"},
            {".jpg", "image/jpeg"},
            {".js", "application/x-javascript"},
            {".json", "application/json"},
            {".png", "image/png"},
            {".xml", "text/xml"}            
        };

        /// <summary>
        /// Gets the MIME type of the content.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.ArgumentNullException">extension</exception>
        public static string GetMimeType(string extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException("extension");
            }
            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }
            string mime;
            return Mappings.TryGetValue(extension, out mime) ? mime : "application/octet-stream";
        }

        /// <summary>
        /// Read response data. If data is available immediately copy to
        /// dataOut, set bytesRead to the number of bytes copied, and return true.
        /// To read the data at a later time set bytesRead to 0, return true and call ICallback.Continue() when the
        /// data is available. To indicate response completion return false.
        /// </summary>
        /// <param name="dataOut">Stream to write to</param>
        /// <param name="bytesRead">Number of bytes copied to the stream</param>
        /// <param name="callback">The callback used to Continue or Cancel the request (async).</param>
        /// <returns>If data is available immediately copy to dataOut, set bytesRead to the number of bytes copied,
        /// and return true.To indicate response completion return false.</returns>
        /// <remarks>Depending on this size of your response this method may be called multiple times</remarks>
        bool IResourceHandler.ReadResponse(Stream dataOut, out int bytesRead, ICallback callback)
        {
            //We don't need the callback, as it's an unmanaged resource we should dispose it (could wrap it in a using statement).
            callback.Dispose();

            if (Stream == null)
            {
                bytesRead = 0;

                return false;
            }

            //Data out represents an underlying buffer (typically 32kb in size).
            var buffer = new byte[dataOut.Length];
            bytesRead = Stream.Read(buffer, 0, buffer.Length);

            dataOut.Write(buffer, 0, buffer.Length);

            return bytesRead > 0;
        }

        void IResourceHandler.GetResponseHeaders(IResponse response, out long responseLength, out string redirectUrl)
        {
            Stream = GetResponse(response, out responseLength, out redirectUrl);

            if (Stream != null && Stream.CanSeek)
            {
                //Reset the stream position to 0
                Stream.Position = 0;
            }
        }

        bool ProcessRequestAsync(IRequest request, ICallback callback)
        {
            this.callback = callback;
            webRequest = (HttpWebRequest)WebRequest.Create(request.Url);
            webRequest.Method = request.Method;
            foreach (string key in request.Headers)
            {
                string value = request.Headers[key];
                // HttpWebRequest doesn't like it if you try to set these values through the Headers collection
                if ("Accept".Equals(key))
                    webRequest.Accept = value;
                else if ("User-Agent".Equals(key))
                    webRequest.UserAgent = value;
                else if ("Referer".Equals(key))
                    webRequest.Referer = value;
                else if ("Content-Type".Equals(key))
                    webRequest.ContentType = value;
                else if ("Content-Length".Equals(key))
                    webRequest.ContentLength = Convert.ToInt32(value);
                else
                    webRequest.Headers.Add(key, value);
            }
            if (!String.IsNullOrWhiteSpace(request.PostData.Elements.FirstOrDefault().GetBody()))
            {
                this.requestBytes = Encoding.UTF8.GetBytes(request.PostData.Elements.FirstOrDefault().GetBody());
                webRequest.BeginGetRequestStream(new AsyncCallback(SendRequestBody), null);
            }
            else
            {
                webRequest.BeginGetResponse(new AsyncCallback(Response), null);
            }
            return true;


            /*callback.Continue();
            return true;*/
        }

        public bool ProcessRequest(IRequest request, ICallback callback)
        {
            return ProcessRequestAsync(request, callback);
        }

        Stream GetResponse(IResponse response, out long responseLength, out string redirectUrl)
        {
            responseLength = webResponse.ContentLength;
            redirectUrl = null;
            string type = webResponse.ContentType;
            // strip off the encoding, if present
            if (type.IndexOf("; ") > 0)
                type = type.Substring(0, type.IndexOf("; "));
            response.MimeType = type;
            // only a direct assignment works here, don't try to use other methods of the name/value collection;
            response.ResponseHeaders = webResponse.Headers;
            response.StatusCode = (int)webResponse.StatusCode;
            response.StatusText = webResponse.StatusDescription;
            // TODO return a wrapper around this stream to capture the response inline.
            return webResponse.GetResponseStream();

            /*redirectUrl = null;
            responseLength = -1;
            response.MimeType = MimeType;
            response.StatusCode = StatusCode;
            response.StatusText = StatusText;
            response.ResponseHeaders = Headers;
            var memoryStream = Stream as MemoryStream;
            if (memoryStream != null)
            {
                responseLength = memoryStream.Length;
            }
            return Stream;*/
        }

        private void SendRequestBody(IAsyncResult result)
        {
            requestStream = webRequest.EndGetRequestStream(result);
            requestStream.BeginWrite(requestBytes, 0, requestBytes.Length, RequestSent, null);
        }

        private void RequestSent(IAsyncResult ar)
        {
            requestStream.EndWrite(ar);
            requestStream.Close();
            webRequest.BeginGetResponse(new AsyncCallback(Response), null);
        }

        private void Response(IAsyncResult ar)
        {
            webResponse = (HttpWebResponse)webRequest.EndGetResponse(ar);
            callback.Continue();
        }

        /// <summary>
        /// Return true if the specified cookie can be sent with the request or false
        /// otherwise. If false is returned for any cookie then no cookies will be sent
        /// with the request.
        /// </summary>
        /// <param name="cookie">cookie</param>
        /// <returns>Return true if the specified cookie can be sent with the request or false
        /// otherwise. If false is returned for any cookie then no cookies will be sent
        /// with the request.</returns>
        bool IResourceHandler.CanGetCookie(CefSharp.Cookie cookie)
        {
            return false;
        }

        /// <summary>
        /// Return true if the specified cookie returned with the response can be set or false otherwise.
        /// </summary>
        /// <param name="cookie">cookie</param>
        /// <returns>Return true if the specified cookie returned with the response can be set or false otherwise.</returns>
        bool  IResourceHandler.CanSetCookie(CefSharp.Cookie cookie)
        {
            return false;
        }

        /// <summary>
        /// Request processing has been canceled.
        /// </summary>
        void IResourceHandler.Cancel()
        { 
        }
        public virtual void Dispose()
        {
        }

    }

}
