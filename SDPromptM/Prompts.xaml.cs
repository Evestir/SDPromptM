using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;
using SDPromptM.src;
using XamlFlair;

namespace SDPromptM
{
    /// <summary>
    /// Interaction logic for Prompts.xaml
    /// </summary>
    public partial class Prompts : UserControl
    {
        string[] files = null;
        string[] strings = null;
        string[] ImageExtensions = { ".png", ".webp", ".jpg", ".jpeg" };

        private void LoadForm()
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            if (Directory.Exists(appdata + "\\SDPromptM"))
            {
                // get the list of text files in the folder
                try
                {
                    files = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SDPromptM", "*.cfg");
                    foreach (string file in files)
                    {
                        try
                        {
                            strings = File.ReadAllLines(file);
                        }
                        catch (Exception)
                        {
                            SDPromptM.src.Functions.Notify($"⚠️ Failed to read configs.", 1);
                            throw;
                        }

                        string ImagePath = null;

                        foreach (string extension in ImageExtensions)
                        {
                            string Expath = appdata + "\\SDPromptM\\" + System.IO.Path.GetFileNameWithoutExtension(file) + extension;

                            if (File.Exists(Expath))
                            {
                                ImagePath = Expath;
                                break;
                            }
                        }

                        if (ImagePath == null)
                        {
                            ImagePath = new Uri("pack://application:,,,/Images/unavailable.png").ToString();
                        }

                        // create a card
                        CardItem(ImagePath, System.IO.Path.GetFileNameWithoutExtension(file), strings[2], strings[0], strings[1]);
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }

                InitializeComponent();
            }
            else
            {
                NothingToSeeHere.Opacity = 1;
                Directory.CreateDirectory(appdata + "\\SDPromptM");
            }
        }

        public Prompts()
        {
            InitializeComponent();

            this.SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Ideal);
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.Fant);

            LoadForm();
        }

        private void ReloadForm(bool CleanUpImg, string title)
        {
            var timmer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1) };
            var timer_ex = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1) };
            double x = 0;

            timer_ex.Tick += (s, e) =>
            {
                x += 0.1;
                if (x >= 1)
                {
                    timer_ex.Stop();
                    this.CardsHolder.Opacity = 1;
                    if (CleanUpImg)
                    {
                        string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                        string ImagePath = null;

                        foreach (string extension in ImageExtensions)
                        {
                            string Expath = appdata + "\\SDPromptM\\" + title + extension;

                            if (File.Exists(Expath))
                            {
                                ImagePath = Expath;
                                try
                                {
                                    File.Delete(ImagePath);

                                }
                                catch (Exception ex)
                                {
                                    SDPromptM.src.Functions.Notify(ex.ToString(), 2);
                                    throw;
                                }
                                break;
                            }
                        }
                    }
                }
                else
                {
                    this.CardsHolder.Opacity = x;
                }
            };

            timmer.Tick += (s, e) =>
            {
                x += 0.1;
                if (x >= 1)
                {
                    this.CardsHolder.Opacity = 0;
                    CardsHolder.Children.Clear();
                    LoadForm();
                    x = 0;
                    timer_ex.Start();
                    timmer.Stop();
                }
                else
                {
                    this.CardsHolder.Opacity = ((double)1 - x);
                }
            };

            timmer.Start();
        }

        public void CardItem(string imagePath, string title, string description, string posprompt, string negprompt)
        {
            // create the card control
            var card = new MaterialDesignThemes.Wpf.Card
            {
                Width = 240,
                Height = 450,
                Foreground = new SolidColorBrush(Color.FromRgb(246, 232, 234)),
                Margin = new Thickness(20),
                Effect = new DropShadowEffect { BlurRadius = 40, Direction = 0, ShadowDepth = 0, Opacity = 0.4, Color = Colors.Black }
            };

            // create the card content
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(300) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });


            var stackPanel = new StackPanel { Margin = new Thickness(8, 6, 8, 0) };
            var grid_rev = new Grid();

            stackPanel.Children.Add(new TextBlock { FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Noto Sans Medium"), FontSize = 18, Text = title });
            stackPanel.SetValue(Grid.RowProperty, 1);
            stackPanel.Children.Add(new TextBlock { VerticalAlignment = VerticalAlignment.Center, Text = description, FontSize = 14, FontWeight = FontWeight.FromOpenTypeWeight(100), TextWrapping = TextWrapping.Wrap });
            grid.Children.Add(stackPanel);

            // Caching Image
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bi.UriSource = new Uri(imagePath);
            bi.EndInit();

            var new_grid = new Grid { Effect = new DropShadowEffect { BlurRadius = 40, ShadowDepth = 0, Direction = 0, Opacity = 1.0, Color = Colors.Black } };
            var image = new Image { Width = 240, Source = bi, Stretch = Stretch.UniformToFill};
            new_grid.Children.Add(image);
            grid.Children.Add(new_grid);
            XamlFlair.Animations.SetPrimary(image, Animations.GetPrimary(ImageExa));
            XamlFlair.Animations.SetSecondary(image, Animations.GetSecondary(ImageExa));

            // Copy Btn
            var button = new Button { Margin = new Thickness(0, 0, 16, -20), HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom, Effect = new DropShadowEffect { BlurRadius = 40, ShadowDepth = 0, Direction = 0, Opacity = 1.0, Color = Colors.Black } };
            button.Style = (Style)Application.Current.Resources["MaterialDesignFloatingActionMiniSecondaryButton"];
            button.Content = new MaterialDesignThemes.Wpf.PackIcon { Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrangeBringForward };
            grid.Children.Add(button);

            button.SetValue(Grid.RowProperty, 0);
            button.Click += Button_Click;
            button.PreviewMouseRightButtonDown += Button_MouseRightButtonDown;

            // Delete Btn
            var button_rev = new Button { Margin = new Thickness(0, 0, 6, -144), Width=31, Height=31, Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 255, 62, 62)), BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 66, 66, 66)), Background = new SolidColorBrush(Color.FromArgb(0xFF, 66, 66, 66)), HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom, Effect = new DropShadowEffect { BlurRadius = 0, ShadowDepth = 0, Direction = 0, Opacity = 0 } };
            button_rev.Style = (Style)Application.Current.Resources["MaterialDesignIconForegroundButton"];
            button_rev.Content = new MaterialDesignThemes.Wpf.PackIcon { Kind = MaterialDesignThemes.Wpf.PackIconKind.Delete };
            grid_rev.Children.Add(button_rev);
            grid.Children.Add(grid_rev);

            button_rev.Click += Button_Rev_Click;

            void Button_Click(object sender, RoutedEventArgs e)
            {
                SDPromptM.src.Functions.Notify($"ℹ️ Copied The Positive Prompt of {title}.", 1);

                Clipboard.SetText(posprompt);
            }
            void Button_MouseRightButtonDown(object sender, RoutedEventArgs e)
            {
                SDPromptM.src.Functions.Notify($"ℹ️ Copied The Negative Prompt of {title}.", 1);

                Clipboard.SetText(negprompt);
            }
            void Button_Rev_Click(object sender, RoutedEventArgs e)
            {
                string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string ImagePath = null;
                image.Source = null;
                //foreach (string extension in ImageExtensions)
                //{
                //    string Expath = appdata + "\\SDPromptM\\" + title + extension;

                //    if (File.Exists(Expath))
                //    {
                //        ImagePath = Expath;
                //        try
                //        {
                //            using (FileStream fs = new FileStream(ImagePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                //            {
                //                fs.Close();
                //            }
                //            File.Delete(ImagePath);

                //        }
                //        catch (Exception ex) 
                //        {
                //            SDPromptM.src.Functions.Notify(ex.ToString(), 2);
                //            throw;
                //        }
                //        break;
                //    }
                //}

                try
                {
                    File.Delete(appdata + "\\SDPromptM\\" + title + ".cfg");
                }
                catch (Exception ex)
                {
                    SDPromptM.src.Functions.Notify(ex.ToString(), 2);
                    throw;
                }
                ReloadForm(true, title);

                SDPromptM.src.Functions.Notify($"ℹ️ Successfully Removed {title}.", 1);
            }

            card.Content = grid;
            CardsHolder.Children.Add(card);
        }

        private void ReloadBtn_Click(object sender, RoutedEventArgs e)
        {
            ReloadForm(false, null);
            SDPromptM.src.Functions.Notify($"ℹ️ Successfully reloaded the page.", 1);
        }
    }
}
