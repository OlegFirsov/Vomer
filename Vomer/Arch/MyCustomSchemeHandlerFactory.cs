using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CefSharp;

namespace Vomer
{
    internal class MyCustomSchemeHandlerFactory : ISchemeHandlerFactory
    {
        public const string SchemeName = "custom";

        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            return new ResourceHandler("application/json", ResourceHandlerType.Stream); 
        }
    }
}
