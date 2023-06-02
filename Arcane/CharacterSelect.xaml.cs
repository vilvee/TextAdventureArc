using Engine;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace Arcane
{

    // filesPath = "../../../files/LargeFirstNameDescendingOrder.txt";

    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class CharacterSelect : Window
    {
        private Player _player;
        private string enteredUsername;
        private string characterImagePath;
        private bool isBorderWhite = false;
        private Dictionary<Image, string> _imagePaths;

        public CharacterSelect()
        {
            InitializeComponent();
           
            _imagePaths= new Dictionary<Image, string>
            {
                { imgMaleScavanger, "../../../Images/Characters/MaleScavanger.jpg" },
                { imgMaleAristocrat, "../../../Images/Characters/MaleAristocrat.jpg" },
                { imgMaleMage, "../../../Images/Characters/MaleMage.jpg" },
                { imgFemaleScavanger, "../../../Images/Characters/FemaleScavanger.jpg" },
                { imgFemaleAristocrat, "../../../Images/Characters/FemaleAristocrat.jpg" },
                { imgFemaleWitch, "../../../Images/Characters/FemaleWitch.jpg" }
            };

            foreach (var entry in _imagePaths)
            {
                SetImage(entry.Key, entry.Value);
            }
        }



        private void SetImage(Image img, string imagePath)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(imagePath, UriKind.Relative);
            bitmapImage.EndInit();

            img.Source = bitmapImage;
        }

        private void imageControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Windows.Controls.Image image = (System.Windows.Controls.Image)sender;
            Border imageBorder = FindParentBorder(image);

            if (imageBorder != null)
            {
                if (isBorderWhite)
                {
                    // Change the border color back to the previous color (e.g., #343661)
                    imageBorder.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("#343661");
                    isBorderWhite = false;
                }
                else
                {
                    // Change the border color to white
                    imageBorder.BorderBrush = Brushes.White;
                    isBorderWhite = true;
                }

                // Save the path of the image
                characterImagePath = image.Source.ToString();
            }
        }

        private Border FindParentBorder(FrameworkElement element)
        {
            FrameworkElement parent = (FrameworkElement)element.Parent;

            while (parent != null)
            {
                if (parent is Border border)
                {
                    return border;
                }

                parent = (FrameworkElement)parent.Parent;
            }

            return null;
        }



        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_player.CharacterImagePath == null)
            {
                btnSave.IsEnabled = false;
            }
            else
            {
                btnSave.IsEnabled = true;
                _player.CharacterImagePath = characterImagePath;
                enteredUsername = txtCharacterName.Text;
                _player = Player.CreateDefaultPlayer(enteredUsername, _player.CharacterImagePath);
                GameWindow gameSession = new(_player);
                gameSession.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                this.Close();
                gameSession.ShowDialog();
            }


        }


        private void TriggerMoveWindow(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (WindowState == System.Windows.WindowState.Maximized)
                {
                    WindowState = System.Windows.WindowState.Normal;

                    double pct = PointToScreen(e.GetPosition(this)).X /
                                 System.Windows.SystemParameters.PrimaryScreenWidth;
                    Top = 0;
                    Left = e.GetPosition(this).X - (pct * Width);
                }

                DragMove();
            }
        }

        private void TriggerMaximize(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Maximized)
                WindowState = System.Windows.WindowState.Normal;
            else if (WindowState == System.Windows.WindowState.Normal)
                WindowState = System.Windows.WindowState.Maximized;
        }

        private void TriggerClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TriggerMinimize(object sender, RoutedEventArgs e)
        {
            WindowState = System.Windows.WindowState.Minimized;
        }
    }
}
