using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CefSharp;

namespace Vomer
{
    class LocalSchemeHandlerFactory: ISchemeHandlerFactory
    {
        public const string SchemeName = "error";
        public const string SchemeNameTest = "test";
        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            //To read a file of disk no need to implement your own handler
            if (schemeName == SchemeName && request.Url.EndsWith("ErrorLoad\\index.html", System.StringComparison.OrdinalIgnoreCase))
            {
                //Display the CefSharp.Core.xml file in the browser
                return ResourceHandler.FromFileName("ErrorLoad\\index.html", ".html");
            }
            return new ResourceHandler("text/html", ResourceHandlerType.File);
        }
    }
}
