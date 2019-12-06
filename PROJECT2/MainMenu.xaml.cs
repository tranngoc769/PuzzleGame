using Microsoft.Win32;
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
                return;

            }
            else
            {
                imagePath = "resourse/default.png";
            }
            mode = 1;
            defaultimageChoose.IsChecked = true;
            customImageChoose.IsChecked = false;

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
                typeGame = 3; // 9X9
            }
            var mainScreen = new MainWindow(mode, typeGame, imagePath, 1);
            Close();
            mainScreen.ShowDialog();
        }

        private void defaultimageChoose_Click(object sender, RoutedEventArgs e)
        {
            mode = 1;
            imagePath = "resourse/default.png";
        }

        private void quit_Click(object sender, RoutedEventArgs e)
        {

            System.Windows.Application.Current.Shutdown();
        }

        private void load_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var reader = new StreamReader("save");
                reader.Close();
                var mainScreen = new MainWindow(mode, typeGame, imagePath, 0);
                Close();
                mainScreen.ShowDialog();
            }
            catch(Exception ex)
            {
                MessageBox.Show("No game saved");
            }
        }

        private void about_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Developed by : \n1712706 - Tran Ngoc Quang\n1712712 - Nguyen Hoang Quyen");

        }

        private void help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("\t\t\tHELP\t\t\t\n\t\t\tMOVE\t\t\t\n- Use left-mouse click, drag and drop the nearby pieces of the white piece into it\n" +
                "- Use arrow button/key to swap the white key with the true direction of arrow button/key pressed\n"+
                "\t\t\tRULE\t\t\t\n- Move 8/35/80 pieces into right order (similar to the stock image right the play area) to win\n"+
                "- In order to win, you must arrange them before the timer ended");
        }
    }
}
