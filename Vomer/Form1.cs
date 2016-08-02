using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;


namespace Vomer
{
    public partial class Form1 : Form
    {        
        private readonly ChromiumWebBrowser browser;        
        public Form1()
        {
            InitializeComponent();                       
            if (VomerIni.Read("Error", "LoadError") == "1")
            {
                if (File.Exists(AppPath + "\\Cache\\Cookies"))
                {
                    File.Delete(AppPath + "\\Cache\\Cookies");//clear cookies
                }
                VomerIni.Write("Error", "0","LoadError");
            }
            var settings = new CefSettings();
            settings.PersistSessionCookies = true;            
            settings.CachePath = AppPath + "\\Cache\\";
            settings.CefCommandLineArgs.Add("enable-media-stream", "1"); //Enable WebRTC
            settings.CefCommandLineArgs.Add("enable-usermedia-screen-capturing", "enable-usermedia-screen-capturing");
            //settings.CefCommandLineArgs.Add("disable-web-security", "disable-web-security");//!!!
            //settings.CefCommandLineArgs.Add("disable-web-security", "disable-web-security");//!!!
            settings.CefCommandLineArgs.Add("enable-web-notification-custom-layouts", "enable-web-notification-custom-layouts");
            settings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = MyCustomSchemeHandlerFactory.SchemeName,
                SchemeHandlerFactory = new MyCustomSchemeHandlerFactory()
            });
            Cef.Initialize(settings);           

            browser = new ChromiumWebBrowser("vomer.com.ua")
            {                
                //for ErrorLoad
                BrowserSettings = new BrowserSettings(){
                    FileAccessFromFileUrls = CefSharp.CefState.Enabled,
                    UniversalAccessFromFileUrls = CefSharp.CefState.Enabled                    
                    //,WebSecurity = CefState.Disabled !!!!!
                    },

                Dock = DockStyle.Fill                
            };
            //browser.RegisterJsObject("desktopNotification", new desktopNotification());
            //Create events
            browser.RequestHandler = new RequestHandler();
            browser.DownloadHandler = new DownloadHandler();
            browser.LifeSpanHandler = new LifeSpanHandler();
            browser.JsDialogHandler = new JsDialogHandler();
            browser.DialogHandler = new DialogHandler();
            browser.LoadError += OnLoadError;

            //ResourceHandler.FromStream();
          
            //browser.LoadHandler = new LoadHandler();
                                                    
            string s1 = Cef.CefVersion;//3.2623.1396
            string s2 = Cef.ChromiumVersion;//49.0.2623.110
                                        
            toolStripContainer1.ContentPanel.Controls.Add(browser);

            notifyIcon1.Text = "VOMER";
            notifyIcon1.Icon = new Icon("logo_zelen_kolo 2.ico");
            //подписываемся на событие клика мышкой по значку в трее
            notifyIcon1.MouseClick += new MouseEventHandler(notifyIcon1_MouseClick);
            //подписываемся на событие изменения размера формы
            this.Resize += new EventHandler(Form1_Resize);            
        }

        
        
        void OnLoadError(object sender, LoadErrorEventArgs loadErrorArgs)
        //void ILoadHandler.OnLoadError(IWebBrowser browserControl, LoadErrorEventArgs loadErrorArgs)
        {
            var txt = loadErrorArgs.ErrorText;
            var cd = loadErrorArgs.ErrorCode;
            //if (loadErrorArgs.ErrorCode == CefErrorCode.ConnectionTimedOut || loadErrorArgs.ErrorCode == CefErrorCode.ConnectionFailed)
            if(loadErrorArgs.FailedUrl.Contains("https://vomer.com.ua/"))
            {
                string str = "file:///" + Uri.EscapeDataString(AppPath) + "\\ErrorLoad\\index.html";                          
                browser.Load(str);
                VomerIni.Write("Error", "1", "LoadError");//load with error 
            }
        }      
    
            /// здесь хранится состояние окна до сворачивания (максимизированное или нормальное)
            private FormWindowState _OldFormState;
            private int wdth, hght, locX,locY;
            public static string AppPath = Application.StartupPath.ToString();
            IniFile VomerIni = new IniFile(AppPath + "\\vomer.ini");
       
            // обрабатываем событие клика мышью по значку в трее
            void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
            {
                //проверяем, какой кнопкой было произведено нажатие
                if (e.Button == MouseButtons.Left)//если левой кнопкой мыши
                {
                    //проверяем текущее состояние окна
                    if (WindowState == FormWindowState.Normal || WindowState == FormWindowState.Maximized)//если оно развернуто
                    {
                        //сохраняем текущее состояние
                        _OldFormState = WindowState;
                        
                        //сворачиваем окно
                        WindowState = FormWindowState.Minimized;
                        //скрываться в трей оно будет по событию Resize (изменение размера), которое сгенерировалось после минимизации строчкой выше
                    }
                    else//в противном случае
                    {
                        //и показываем на нанели задач
                        Show();
                        //разворачиваем (возвращаем старое состояние "до сворачивания")
                        WindowState = _OldFormState;
                        if (WindowState == FormWindowState.Normal)
                        {
                            this.Width = wdth + 1;
                            this.Height = hght + 1;
                            this.Left = locX;
                            this.Top = locY;
                        }
                    }
                }
            }
            // обрабатываем событие изменения размера
            void Form1_Resize(object sender, EventArgs e)
            {
                if (FormWindowState.Minimized == WindowState)//если окно "свернуто"
                {
                    //то скрываем его
                    //Hide();
                }
                if (FormWindowState.Normal == WindowState)
                {
                    wdth = this.Width;
                    hght = this.Height;
                    locX = this.Left;
                    locY = this.Top;
                    _OldFormState = WindowState;
                    
                }
                if (FormWindowState.Maximized == WindowState)
                {
                    _OldFormState = WindowState;
                }
            }       
            
        
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            //this.Show();
            //this.Close();
            //notifyIcon1.Visible = false;
            Application.Exit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {               
                VomerIni.Write("X", this.Left.ToString(), "Location");
                VomerIni.Write("Y", this.Top.ToString(), "Location");
                VomerIni.Write("Width", this.Width.ToString(), "Size");
                VomerIni.Write("Height", this.Height.ToString(), "Size");                
                WindowState = FormWindowState.Minimized;
                e.Cancel = true;
                notifyIcon1.Visible = true;
                this.Hide();
                
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //var VomerIni = new IniFile(AppPath + "\\vomer.ini");
            if (!File.Exists(AppPath + "\\vomer.ini"))
            {
                File.Create(AppPath + "\\vomer.ini").Close();                
                VomerIni.Write("X","0","Location");
                VomerIni.Write("Y", "0", "Location");
                VomerIni.Write("Width", "1055","Size");
                VomerIni.Write("Height", "717","Size");
                //VomerIni.Write("Error", "0", "LoadError");//load without error 
            }            

        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.Left = Convert.ToInt32(VomerIni.Read("X", "Location"));
            this.Top = Convert.ToInt32(VomerIni.Read("Y", "Location"));
            this.Width = Convert.ToInt32(VomerIni.Read("Width", "Size"));
            this.Height = Convert.ToInt32(VomerIni.Read("Height", "Size"));
        }

        
                       
    }

    

}

