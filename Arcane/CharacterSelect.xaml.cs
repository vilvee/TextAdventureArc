using Engine;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
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
using System.Windows.Shapes;

namespace Arcane
{
    public struct ImagePath
    {
        public string Path { get; set; }
    }

    public class CharacterSelectViewModel
    {

        public ImagePath MaleSacavger { get; set; }
        public ImagePath MaleAristocrat { get; set; }
        public ImagePath MaleMage { get; set; }
        public ImagePath FemaleScavanger { get; set; }
        public ImagePath FemaleAristocrat { get; set; }
        public ImagePath FemaleWitch { get; set; }

    }


    // filesPath = "../../../files/LargeFirstNameDescendingOrder.txt";

    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class CharacterSelect : Window
    {
        private Player _player;
        private string enteredUsername;
        private List<string> _imagePaths;

        public CharacterSelect()
        {
          InitializeComponent();
            // Specify the image source path
            // string imagePath = "../../../Images/Characters/MaleScavanger.png";
            string imagePath = "C:\\Users\\vvile\\Documents\\Arcane\\Arcane\\Images\\Characters\\MaleScavanger.jpg";

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(imagePath, UriKind.Absolute);
            bitmapImage.EndInit();

            imgMaleScavanger.Source = bitmapImage;
        }


/*
        private void imageControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image clickedImage = (Image)sender;
            // Retrieve the ImagePath from the clicked Image control
            if (clickedImage.DataContext is ImagePath imagePath)
            {
                _player.CharacterImagePath = imagePath.Path; // Set the player's image path to the retrieved image path
            }
        }*/

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_player.CharacterImagePath == null)
            { 
                btnSave.IsEnabled = false;
            }
            else
            {
                btnSave.IsEnabled = true;
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

                    double pct = PointToScreen(e.GetPosition(this)).X / System.Windows.SystemParameters.PrimaryScreenWidth;
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
