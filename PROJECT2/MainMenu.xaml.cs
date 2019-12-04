using Microsoft.Win32;
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
using System.Windows.Shapes;

namespace PROJECT2
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {

        private int mode = 1; // 1 la mode default, 2 la mode tuy chon
        private int typeGame = 1; // 3x3
        private string imagePath = "resourse/default.png";
        public MainMenu()
        {
            InitializeComponent();
        }

        private void customImageChoose_Click(object sender, RoutedEventArgs e)
        {
            mode = 2;
            OpenFileDialog chooseImage = new OpenFileDialog();
            chooseImage.Filter = "Images File(*.png;*.jpg;*.jpeg;*.bmp*)|*.png;*.jpg;*.jpeg;*.bmp*";
            chooseImage.FilterIndex = 1;
            chooseImage.Multiselect = false;

            if (chooseImage.ShowDialog() == true)
            {
                imagePath = chooseImage.FileName;
            }
            else
            {
                imagePath = "resourse/default.png";
            }
        }

        private void start_Click(object sender, RoutedEventArgs e)
        {
            if (type3.IsChecked == true)
            {
                typeGame = 1; // 3x3
            }
            if (type6.IsChecked == true)
            {
                typeGame = 2; // 6x6
            }
            if (type9.IsChecked == true)
            {
                typeGame = 3; // 6x6
            }
            var mainScreen = new MainWindow(mode, typeGame, imagePath);
            Close();
            mainScreen.ShowDialog();
        }

        private void defaultimageChoose_Click(object sender, RoutedEventArgs e)
        {
            mode = 1;
            imagePath = "resourse/default.png";
        }
    }
}
