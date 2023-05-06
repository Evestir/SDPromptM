using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SDPromptM.src
{
    internal class Functions
    {
        public static void log(string content)
        {
            string targetPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SDPromptM\\logs.txt";

            if (File.Exists(targetPath))
            {
                using (StreamWriter writer = new StreamWriter(targetPath))
                {
                    // Write some text to the file
                    writer.WriteLine($"[{DateTime.Now.ToString()}] {content}", true);
                }
            }
        }
        public static void Notify(string message, int second)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.Snackbar.MessageQueue?.Enqueue(message, null, null, null, false, true, TimeSpan.FromSeconds(second));
            }
        }
    }
}
