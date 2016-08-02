using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;

namespace Vomer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static Mutex m_instance;
        private const string m_appName = "VOMER";  
        [STAThread]
        static void Main()
        {
            bool tryCreateNewApp;            
            m_instance = new Mutex(true, m_appName,
                    out tryCreateNewApp);
            if (tryCreateNewApp)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
                return;
            }           
        }
    }
}
