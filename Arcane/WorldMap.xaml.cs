using Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using Path = System.IO.Path;
using Engine;
namespace Arcane
{
    /// <summary>
    /// Interaction logic for WorldMap.xaml
    /// </summary>
    public partial class WorldMap : Window
    {

        readonly Assembly _thisAssembly = Assembly.GetExecutingAssembly();

        public WorldMap(Player player)
        {
            InitializeComponent();
            SetImage(pic_0_2, player.LocationsVisited.Contains(5) ? "HerbalistsGarden" : "FogLocation");
            SetImage(pic_1_2, player.LocationsVisited.Contains(4) ? "HerbalistsHut" : "FogLocation");
            SetImage(pic_2_0, player.LocationsVisited.Contains(7) ? "FarmFields" : "FogLocation");
            SetImage(pic_2_1, player.LocationsVisited.Contains(6) ? "Farmhouse" : "FogLocation");
            SetImage(pic_2_2, player.LocationsVisited.Contains(2) ? "TownSquare" : "FogLocation");
            SetImage(pic_2_3, player.LocationsVisited.Contains(3) ? "TownGate" : "FogLocation");
            SetImage(pic_2_4, player.LocationsVisited.Contains(8) ? "Bridge" : "FogLocation");
            SetImage(pic_2_5, player.LocationsVisited.Contains(9) ? "SpiderForest" : "FogLocation");
            SetImage(pic_3_2, player.LocationsVisited.Contains(1) ? "Home" : "FogLocation");
        }

        private void SetImage(Image pictureBox, string imageName)
        {
            string solutionDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName; 
            string imagePath = Path.Combine(solutionDirectory, "Images", imageName + ".png");
            BitmapImage bitmapImage = new BitmapImage();

            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute);
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();

            pictureBox.Source = bitmapImage;
        }




    }
}
