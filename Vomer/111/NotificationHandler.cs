using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CefSharp;

namespace Vomer
{
    class Handler
    {        
      ///
      // Executed when a new query is received. |query_id| uniquely identifies the
      // query for the life span of the router. Return true to handle the query
      // or false to propagate the query to other registered handlers, if any. If
      // no handlers return true from this method then the query will be
      // automatically canceled with an error code of -1 delivered to the
      // JavaScript onFailure callback. If this method returns true then a
      // Callback method must be executed either in this method or asynchronously
      // to complete the query.
      ///
      public bool OnQuery(IBrowser browser,
                          IFrame frame,
                          Int64 query_id,
                          IRequest request,
                          bool persistent,
                          Callback callback)
        {
          string notif = request.ToString();
          MessageBox.Show("Уведомление: " + notif);          
          return false;
        }

      ///
      // Executed when a query has been canceled either explicitly using the
      // JavaScript cancel function or implicitly due to browser destruction,
      // navigation or renderer process termination. It will only be called for
      // the single handler that returned true from OnQuery for the same
      // |query_id|. No references to the associated Callback object should be
      // kept after this method is called, nor should any Callback methods be
      // executed.
      ///
      public void OnQueryCanceled(IBrowser browser,
                                   IFrame frame,
                                   Int64 query_id) {}
        }
}
