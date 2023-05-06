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
using SDPromptM.src;

namespace SDPromptM
{
    /// <summary>
    /// Interaction logic for Create.xaml
    /// </summary>
    public partial class Create : UserControl
    {
        string PathImg = null;
        void CreateEventHandlers()
        {
            Placeholder.MouseDown += Placeholder_Click;
            Placeholder.MouseEnter += Placeholder_MouseEnter;
            Placeholder.MouseLeave += Placeholder_MouseLeave;
        }

        public Create()
        {
            InitializeComponent();
            CreateEventHandlers();
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
        }
        public void Placeholder_Shadow(bool Cum)
        {
            double x = 0;

            bool IsCurOpacityOne = false;

            this.Dispatcher.Invoke(() =>
            {
                if (Placeholder.Opacity == (double)1)
                {
                    IsCurOpacityOne = true;
                }
            });

            if (Cum && IsCurOpacityOne)
            {
                while (x < 0.5)
                {
                    x += 0.07;
                    this.Dispatcher.Invoke(() =>
                    {
                        PlaceHolderText.Opacity = x * 2;
                        Placeholder.Opacity = (double)1 - x;
                        ImageBlurAmount.Radius = x * 10;
                    });
                    Thread.Sleep(1);
                }
                this.Dispatcher.Invoke(() =>
                {
                    Placeholder.Opacity = (float)0.5;
                });
            }
            else if (Cum == false && IsCurOpacityOne == false)
            {
                while (x < 0.5)
                {
                    x += 0.07;
                    this.Dispatcher.Invoke(() =>
                    {
                        PlaceHolderText.Opacity = 1 - x * 2;
                        Placeholder.Opacity = (double)0.5 + x;
                        ImageBlurAmount.Radius = 5 - x * 10;
                    });
                    Thread.Sleep(1);
                }
                this.Dispatcher.Invoke(() =>
                {
                    Placeholder.Opacity = (float)1;
                    ImageBlurAmount.Radius = 0;
                });
            }
        }
        private void Placeholder_MouseEnter(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Hand;

            Thread thread = new Thread(delegate ()
            {
                Placeholder_Shadow(true);
            });

            thread.Start();
        }
        private void Placeholder_MouseLeave(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Arrow;

            Thread thread = new Thread(delegate ()
            {
                Placeholder_Shadow(false);
            });

            thread.Start();
        }
        private void Placeholder_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            var response = openFileDialog.ShowDialog();
            if (response == true)
            {
                PathImg = openFileDialog.FileName;
                Placeholder.Source = new BitmapImage(new Uri(PathImg));
            }
        }
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            string Title = TitleField.Text;
            string Description = DescriptionField.Text;
            string PositiveProm = PositivePrompt.Text;
            string NegativeProm = NegativePrompt.Text;

            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            string targetPath = appdata + "\\SDPromptM\\" + Title + ".cfg";

            if (string.IsNullOrEmpty(Title) || string.IsNullOrEmpty(Description) || string.IsNullOrEmpty(PositiveProm) || string.IsNullOrEmpty(NegativeProm))
            {
                SDPromptM.src.Functions.Notify("⚠️ Please fill in every field.", 3);
                return;
            }

            if (!File.Exists(targetPath))
            {
                using (StreamWriter writer = File.CreateText(targetPath))
                {
                    // Write some text to the file
                    writer.WriteLine(PositiveProm);
                    writer.WriteLine(NegativeProm);
                    writer.WriteLine(Description);
                }
            }
            else
            {
                SDPromptM.src.Functions.Notify("⚠️ A config already exists. Ovewriting..", 1);
                using (StreamWriter writer = new StreamWriter(targetPath))
                {
                    writer.WriteLine(PositiveProm);
                    writer.WriteLine(NegativeProm);
                    writer.WriteLine(Description);
                }
            }

            if (PathImg != null)
            {
                try
                {
                    System.IO.File.Copy(PathImg, appdata + "\\SDPromptM\\" + Title + System.IO.Path.GetExtension(PathImg));
                }
                catch (Exception ex)
                {
                    SDPromptM.src.Functions.Notify(ex.ToString(), 3);
                    throw;
                }
            }

            SDPromptM.src.Functions.Notify("ℹ️ Successfully created a new config.", 1);
        }
    }
}
