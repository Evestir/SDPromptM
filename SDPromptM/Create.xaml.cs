using Microsoft.Win32;
using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

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
        private void Placeholder_Shadow(bool Cum)
        {
            if (Cum)
            {
                var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1) };
                double x = 0;

                timer.Tick += (s, e) =>
                {
                    x += 0.1;
                    if (x >= 0.5)
                    {
                        timer.Stop();
                        Placeholder.Opacity = 0.5;
                    }
                    else
                    {
                        PlaceHolderText.Opacity = x * 2;
                        Placeholder.Opacity = 1 - x;
                        ImageBlurAmount.Radius = x * 20;
                    }
                };

                timer.Start();
            }
            else
            {
                var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1) };
                double y = 0;

                timer.Tick += (s, e) =>
                {
                    y += 0.1;
                    if (y >= 1)
                    {
                        timer.Stop();
                        Placeholder.Opacity = 1;
                    }
                    else
                    {
                        PlaceHolderText.Opacity = 1 - y * 2;
                        Placeholder.Opacity = 0.5 + y;
                        ImageBlurAmount.Radius = 10 - y * 20;
                    }
                };

                timer.Start();
            }
        }
        private void Placeholder_MouseEnter(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Hand;
            Placeholder_Shadow(true);
        }
        private void Placeholder_MouseLeave(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            Placeholder_Shadow(false);
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
