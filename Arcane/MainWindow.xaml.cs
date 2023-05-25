using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Engine;

namespace Arcane
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Player _player;
        public MainWindow()
        {
            InitializeComponent();
            Location location = new(1, "Home", "This is your home");

            //initialize player at level one
            _player = new(1,0,20,10,10);

            //display stats in fields
            lbHitPoints.Content = _player.CurrentHitPoints.ToString();
            lbGold.Content = _player.Gold.ToString();
            lbExperience.Content = _player.ExperiencePoints.ToString();
            lbLevel.Content = _player.Level.ToString();
        }

    }
}
