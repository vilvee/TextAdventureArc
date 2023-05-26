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

namespace Arcane
{
    /// <summary>
    /// Interaction logic for WorldMap.xaml
    /// </summary>
    public partial class WorldMap : Window
    {

        readonly Assembly _thisAssembly = Assembly.GetExecutingAssembly();

        public WorldMap()
        {
            InitializeComponent();
            SetImage(pic_0_2, "HerbalistsGarden");
            SetImage(pic_1_2, "HerbalistsHut");
            SetImage(pic_2_0, "FarmFields");
            SetImage(pic_2_1, "Farmhouse");
            SetImage(pic_2_2, "TownSquare");
            SetImage(pic_2_3, "TownGate");
            SetImage(pic_2_4, "Bridge");
            SetImage(pic_2_5, "SpiderForest");
            SetImage(pic_3_2, "Home");
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
