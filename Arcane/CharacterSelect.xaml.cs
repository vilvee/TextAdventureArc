using Engine;
using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class CharacterSelect : Window
    {
        private Player _player;
        private string enteredUsername;

        public CharacterSelect()
        {
            InitializeComponent();

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            enteredUsername = txtCharacterName.Text;
            _player = Player.CreateDefaultPlayer(enteredUsername);
            GameWindow gameSession = new(_player);
            gameSession.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Close();
            gameSession.ShowDialog();

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
