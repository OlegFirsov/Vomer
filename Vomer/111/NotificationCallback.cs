using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CefSharp;

namespace Vomer
{
    public class Callback
    {        
      ///
      // Notify the associated JavaScript onSuccess callback that the query has
      // completed successfully with the specified |response|.
      ///
      public void Success(string response)
        {
    
        }
      ///
      // Notify the associated JavaScript onFailure callback that the query has
      // failed with the specified |error_code| and |error_message|.
      ///
      public void Failure(int error_code, string error_message)
        {
    
        }
    }
}
