using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using CefSharp;
using System.IO;

namespace Vomer
{
    /// <summary>
    /// Class ResourceHandler.
    /// </summary>
    public class ResourceHandler : IResourceHandler
    {
        /// <summary>
        /// MimeType to be used if none provided
        /// </summary>
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
        private ResourceHandler(string mimeType, ResourceHandlerType type)
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

        bool IResourceHandler.ProcessRequestAsync(IRequest request, ICallback callback)
        {
            callback.Continue();

            return true;
        }

        Stream IResourceHandler.GetResponse(IResponse response, out long responseLength, out string redirectUrl)
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
    }

}
