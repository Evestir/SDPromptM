using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SDPromptM.src;

namespace SDPromptM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, [In] ref bool attrValue, int attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int DWMWA_CAPTION_COLOR = 35;

        private static bool UseImmersiveDarkMode(IntPtr handle, bool enabled)
        {
            if (IsWindows10OrGreater(17763))
            {
                var attribute = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
                if (IsWindows10OrGreater(18985))
                {
                    attribute = DWMWA_USE_IMMERSIVE_DARK_MODE;
                }

                bool value = enabled;

                int useImmersiveDarkMode = enabled ? 1 : 0;
                if (DwmSetWindowAttribute(handle, (int)attribute, ref value, Marshal.SizeOf<bool>()) == 1) return true;
                else return false;
            }

            return false;
        }

        private static bool IsWindows10OrGreater(int build = -1)
        {
            return Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= build;
        }

        private void CreateEventHandlers()
        {
            PromptsRadio.Click += new RoutedEventHandler(PromptsRadio_Click);
            CreationRadio.Click += new RoutedEventHandler(CreationRadio_Click);
        }

        public MainWindow()
        {
            InitializeComponent();
            CreateEventHandlers();
            this.SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Ideal);

            // Enalbe Dark Mode
            IntPtr hWnd = new WindowInteropHelper(this).EnsureHandle();
            UseImmersiveDarkMode(hWnd, true);

            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            if (Directory.Exists(appdata + "\\SDPromptM"))
            {
                Functions.log(appdata);
            }
            else
            {
                Directory.CreateDirectory(appdata + "\\SDPromptM");
                File.CreateText(appdata + "\\SDPromptM\\logs.txt");

                Thread.Sleep(10);

                Functions.log($"[{DateTime.Now.ToString()}] Created a folder and a log.");
            }
        }
        private void LinearOpacityPages(int ver)
        {
            double x = 0;

            if (ver == 0)
            {
                while (x < 1)
                {
                    x = x + 0.02;
                    this.Dispatcher.Invoke(() =>
                    {
                        PromptsPage.Opacity = x;
                        CreationPage.Opacity = 1 - x;
                    });
                    Thread.Sleep(1);
                }
                this.Dispatcher.Invoke(() =>
                {
                    CreationPage.Visibility = Visibility.Collapsed;
                    PromptsPage.Opacity = 1;
                });
            }
            else if (ver == 1)
            {
                while (x < 1)
                {
                    x = x + 0.02;
                    this.Dispatcher.Invoke(() =>
                    {
                        CreationPage.Opacity = x;
                        PromptsPage.Opacity = 1 - x;
                    });
                    Thread.Sleep(1);
                }
                this.Dispatcher.Invoke(() =>
                {
                    PromptsPage.Visibility = Visibility.Collapsed;
                    CreationPage.Opacity = 1;
                });
            }
        }

        private void PromptsRadio_Click(object sender, RoutedEventArgs e)
        {
            if (PromptsPage.Opacity != (float)1)
            {
                PromptsPage.Visibility = Visibility.Visible;

                Thread threandnfkasdf = new Thread(delegate ()
                {
                    LinearOpacityPages(0);
                });

                threandnfkasdf.Start();
            }
        }

        private void CreationRadio_Click(object sender, RoutedEventArgs e)
        {
            if (CreationPage.Opacity != (float)1)
            {
                CreationPage.Visibility = Visibility.Visible;

                Thread asfdmsadfke4qwf = new Thread(delegate ()
                {
                    LinearOpacityPages(1);
                });

                asfdmsadfke4qwf.Start();
            }
        }
    }
}
