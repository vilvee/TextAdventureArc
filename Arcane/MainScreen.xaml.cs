using Engine;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Arcane
{
    /// <summary>
    /// Interaction logic for MainScreen.xaml
    /// </summary>
    public partial class MainScreen : Window
    {
        public MainScreen()
        {
            InitializeComponent();
        }


        private void btnNewGame_Click(object sender, RoutedEventArgs e)
        {
            GameWindow gameWindow = new GameWindow();
            gameWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            gameWindow.ShowDialog();
        }

        private void btnContinueGame_Click(object sender, RoutedEventArgs e)
        {
            GameWindow gameWindow = new GameWindow();
            gameWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            gameWindow.ShowDialog();
        }

        #region WindowControls

        /// <summary>
        /// Triggers window movement when the left mouse button is pressed.
        /// </summary>
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


        /// <summary>
        /// Event handler for maximizing or restoring the window when the user clicks on it.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void TriggerMaximize(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Maximized)
            {
                // If the window is already maximized, restore it to normal state.
                WindowState = System.Windows.WindowState.Normal;
            }
            else if (WindowState == System.Windows.WindowState.Normal)
            {
                // If the window is in normal state, maximize it.
                WindowState = System.Windows.WindowState.Maximized;
            }
        }

        /// <summary>
        /// Event handler for closing the window.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void TriggerClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Event handler for minimizing the window.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void TriggerMinimize(object sender, RoutedEventArgs e)
        {
            // Minimize the window.
            WindowState = System.Windows.WindowState.Minimized;
        }

        #endregion
    }
}
