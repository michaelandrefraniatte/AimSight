using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX.XInput;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using WebView2 = Microsoft.Web.WebView2.WinForms.WebView2;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace AimSight
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        public static extern uint TimeBeginPeriod(uint ms);
        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        public static extern uint TimeEndPeriod(uint ms);
        [DllImport("ntdll.dll", EntryPoint = "NtSetTimerResolution")]
        public static extern void NtSetTimerResolution(uint DesiredResolution, bool SetResolution, ref uint CurrentResolution);
        public static uint CurrentResolution = 0;
        public static bool closed = false;
        public static int x, y, Width, Height;
        public WebView2 webView21 = new WebView2();
        public static List<double> List_A = new List<double>(), List_B = new List<double>(), List_X = new List<double>(), List_Y = new List<double>(), List_LB = new List<double>(), List_RB = new List<double>(), List_LT = new List<double>(), List_RT = new List<double>(), List_MAP = new List<double>(), List_MENU = new List<double>(), List_LSTICK = new List<double>(), List_RSTICK = new List<double>(), List_DU = new List<double>(), List_DD = new List<double>(), List_DL = new List<double>(), List_DR = new List<double>(), List_XBOX = new List<double>();
        public static double Controller_RX, Controller_RY;
        private static Controller[] controller = new Controller[] { null };
        public static int xnum;
        private static State state;
        public static double Controller1ThumbRightX;
        public static double Controller1ThumbRightY;
        private async void Form1_Shown(object sender, EventArgs e)
        {
            TimeBeginPeriod(1);
            NtSetTimerResolution(1, true, ref CurrentResolution);
            Width = Screen.PrimaryScreen.Bounds.Width;
            Height = Screen.PrimaryScreen.Bounds.Height;
            CoreWebView2EnvironmentOptions options = new CoreWebView2EnvironmentOptions("--disable-web-security", "--allow-file-access-from-files", "--allow-file-access");
            CoreWebView2Environment environment = await CoreWebView2Environment.CreateAsync(null, null, options);
            await webView21.EnsureCoreWebView2Async(environment);
            webView21.CoreWebView2.SetVirtualHostNameToFolderMapping("appassets", "assets", CoreWebView2HostResourceAccessKind.DenyCors);
            webView21.CoreWebView2.Settings.AreDevToolsEnabled = false;
            webView21.Source = new Uri("https://appassets/index.html");
            webView21.Dock = DockStyle.Fill;
            webView21.DefaultBackgroundColor = Color.Transparent;
            this.Controls.Add(webView21);
            this.Location = new Point(Width / 2 - this.Size.Width / 2, Height / 2 - this.Size.Height / 2);
            try
            {
                var controllers = new[] { new Controller(UserIndex.One) };
                xnum = 0;
                foreach (var selectControler in controllers)
                {
                    if (selectControler.IsConnected)
                    {
                        controller[xnum] = selectControler;
                        xnum++;
                        if (xnum > 0)
                        {
                            break;
                        }
                    }
                }
            }
            catch { }
        }
        private static double Scale(double value, double min, double max, double minScale, double maxScale)
        {
            double scaled = minScale + (double)(value - min) / (max - min) * (maxScale - minScale);
            return scaled;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            taskEmulate();
        }
        public async void SetController(double Controller1ThumbRightX, double Controller1ThumbRightY)
        {
            try
            {
                Controller_RX = Controller1ThumbRightX;
                Controller_RY = Controller1ThumbRightY;
                await execScriptHelper($"setController('{Controller_RX.ToString()}', '{Controller_RY.ToString()}');");
            }
            catch { }
        }
        private async Task<String> execScriptHelper(String script)
        {
            var x = await webView21.ExecuteScriptAsync(script).ConfigureAwait(false);
            return x;
        }
        private async void taskEmulate()
        {
            try
            {
                for (int inc = 0; inc < xnum; inc++)
                {
                    state = controller[inc].GetState();
                    if (inc == 0)
                    {
                        Controller1ThumbRightX = state.Gamepad.RightThumbX;
                        Controller1ThumbRightY = state.Gamepad.RightThumbY;
                        SetController(Controller1ThumbRightX, Controller1ThumbRightY);
                    }
                }
            }
            catch { }
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            closed = true;
        }
    }
}