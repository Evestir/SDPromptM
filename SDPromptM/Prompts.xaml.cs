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
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SDPromptM.src;

namespace SDPromptM
{
    /// <summary>
    /// Interaction logic for Prompts.xaml
    /// </summary>
    public partial class Prompts : UserControl
    {
        string[] files = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SDPromptM", "*.cfg");
        string[] strings = null;
        string[] ImageExtensions = { ".png", ".webp", ".jpg", ".jpeg" };

        public Prompts()
        {
            InitializeComponent();

            this.SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Ideal);
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);

            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            if (Directory.Exists(appdata + "\\SDPromptM"))
            {
                // get the list of text files in the folder

                foreach (string file in files)
                {
                    try {
                        strings = File.ReadAllLines(file);
                    }
                    catch (Exception){
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

                        if (ImagePath == null)
                        {
                            ImagePath = new Uri("pack://application:,,,/Images/unavailable.png").ToString();
                        }
                    }

                    // create a card
                    CardItem(ImagePath, System.IO.Path.GetFileNameWithoutExtension(file), strings[2], strings[0], strings[1]);
                }

                Functions.log(appdata);

                InitializeComponent();
            }
            else
            {
                Directory.CreateDirectory(appdata + "\\SDPromptM");
                File.CreateText(appdata + "\\SDPromptM\\logs.txt");

                Functions.log($"[{DateTime.Now.ToString()}] Created a folder and a log.");
            }
        }

        public void CardItem(string imagePath, string title, string description, string posprompt, string negprompt)
        {
            // create the card control
            var card = new MaterialDesignThemes.Wpf.Card
            {
                Width = 200,
                Height = 400,
                Foreground = new SolidColorBrush(Color.FromRgb(246, 232, 234)),
                Margin = new Thickness(20),
                Effect = new DropShadowEffect { BlurRadius = 40, Direction = 0, Opacity = 1.0, Color = Colors.Black }
            };

            // create the card content
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(200) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var image = new Image { Width = 200, Height = 200, Source = new BitmapImage(new Uri(imagePath)), Stretch = Stretch.UniformToFill };
            grid.Children.Add(image);

            var button = new Button { Margin = new Thickness(0, 0, 16, -20), HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom };
            button.Style = (Style)Application.Current.Resources["MaterialDesignFloatingActionMiniSecondaryButton"];
            button.Content = new MaterialDesignThemes.Wpf.PackIcon { Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrangeBringForward };
            grid.Children.Add(button);
            button.SetValue(Grid.RowProperty, 0);
            button.Click += Button_Click;
            button.PreviewMouseRightButtonDown += Button_MouseRightButtonDown;

            void Button_Click(object sender, RoutedEventArgs e)
            {
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.Snackbar.MessageQueue?.Enqueue($"ℹ️ Copied The Positive Prompt of {title}.", null, null, null, false, true, TimeSpan.FromSeconds(2));
                }

                Clipboard.SetText(posprompt);
            }
            void Button_MouseRightButtonDown(object sender, RoutedEventArgs e)
            {
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.Snackbar.MessageQueue?.Enqueue($"ℹ️ Copied The Negative Prompt of {title}.", null, null, null, false, true, TimeSpan.FromSeconds(2));
                }

                Clipboard.SetText(negprompt);
            }

            var stackPanel = new StackPanel { Margin = new Thickness(8, 6, 8, 0) };

            GridLengthConverter gridLengthConverter = new GridLengthConverter();

            stackPanel.Children.Add(new TextBlock { FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Noto Sans Medium"), FontSize = 18, Text = title });
            stackPanel.SetValue(Grid.RowProperty, 1);
            stackPanel.Children.Add(new TextBlock { VerticalAlignment = VerticalAlignment.Center, Text = description, FontSize=14, FontWeight = FontWeight.FromOpenTypeWeight(100),TextWrapping = TextWrapping.Wrap });
            grid.Children.Add(stackPanel);

            card.Content = grid;
            CardsHolder.Children.Add(card);
        }
    }
}
