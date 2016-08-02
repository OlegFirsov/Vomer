using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.IO;
using CefSharp;

namespace Vomer
{
    internal class MyCustomSchemeHandler : IResourceHandler
    {

        private static readonly IDictionary<string, string> ResourceDictionary;

        private string mimeType;
        private MemoryStream stream;

        static MyCustomSchemeHandler()
        {
            ResourceDictionary = new Dictionary<string, string>
        {
            //{ "/home.html", Properties.Resources.index},
            //{ "/assets/css/style.css", Properties.Resources.style}
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
        }

        public Stream Stream { get; set; }
        public int StatusCode { get; set; }
        public string StatusText { get; set; }
        public string MimeType { get; set; }
        public NameValueCollection Headers { get; private set; }

        public Stream GetResponse(IResponse response, out long responseLength, out string redirectUrl)
        {
            redirectUrl = null;
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

            return Stream;
        }

        public bool ProcessRequestAsync(IRequest request, ICallback callback)
        {
            // The 'host' portion is entirely ignored by this scheme handler.
            var uri = new Uri(request.Url);
            var fileName = uri.AbsolutePath;
            string resource;

            if (ResourceDictionary.TryGetValue(fileName, out resource) && !string.IsNullOrEmpty(resource))
            {
                var resourceHandler = ResourceHandler.FromString(resource);
                stream = (MemoryStream)resourceHandler.Stream;

                var fileExtension = Path.GetExtension(fileName);
                mimeType = ResourceHandler.GetMimeType(fileExtension);

                callback.Continue();
                return true;
            }
            else
            {
                callback.Dispose();
            }

            return false;
        }

        void GetResponseHeaders(IResponse response, out long responseLength, out string redirectUrl)
        {
            responseLength = stream == null ? 0 : stream.Length;
            redirectUrl = null;

            response.StatusCode = (int)StatusCode;
            response.StatusText = "OK";
            response.MimeType = mimeType;
        }

        bool ReadResponse(Stream dataOut, out int bytesRead, ICallback callback)
        {
            //Dispose the callback as it's an unmanaged resource, we don't need it in this case
            callback.Dispose();

            if (stream == null)
            {
                bytesRead = 0;
                return false;
            }

            //Data out represents an underlying buffer (typically 32kb in size).
            var buffer = new byte[dataOut.Length];
            bytesRead = stream.Read(buffer, 0, buffer.Length);

            dataOut.Write(buffer, 0, buffer.Length);

            return bytesRead > 0;
        }

        bool CanGetCookie(Cookie cookie)
        {
            return true;
        }

        bool CanSetCookie(Cookie cookie)
        {
            return true;
        }

        void Cancel()
        {

        }

    }
}
