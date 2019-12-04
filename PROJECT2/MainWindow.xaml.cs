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

namespace PROJECT2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string _maLog = "";
        bool _isDragging = false;
        Image _selectedBitmap = null;
        Point _lastPosition;
        Point _lastPiece;
        int[,] matrix;
        private int mode, type;
        private string imagePath;
        public MainWindow(int mode, int type, string imagePath)
        {
            InitializeComponent();
            this.mode = mode;
            this.type = type;
            this.imagePath = imagePath;
            UIView.rowNumber = type * 3;
            UIView.colNumber = type * 3;
            UIView.marginX = 30;
            UIView.marginY = 30;


        }
        public class UIView
        {
            //changeabel value
            public static int rowNumber { get; set; }
            public static int colNumber { get; set; }
            public static int marginX { get; set; }
            public static int marginY { get; set; }
            public static int stockImageStartX { get; set; }
            public static int stockImageStartY { get; set; }

            //Constant for specifications
            //public const int topOffset = 40;
            //public const int leftOffset = 40;
            public const int pieceWidth = 70;
            public const int pieceHeight = 70;
            public const int widthOfImage = 65;
            public const int heightOfImage = 65;
            public const int widthOfPreviewImage = 210;
            public const int heightOfPreviewImage = 210;
            public const string fileSave = "saveGame.txt";
        }
        private void getPos(Point position, ref int i, ref int j)
        {
            i = ((int)position.Y - UIView.marginY) / UIView.pieceHeight;
            j = ((int)position.X - UIView.marginX) / UIView.pieceWidth;
        }
        private void Image_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            var position = e.GetPosition(this);
            int x = 0, y = 0;
            getPos(position, ref x, ref y);
            if (x < 3 && y < 3 && x*3+y+1 != matrix[2,2])
            {
                Canvas.SetLeft(_selectedBitmap, UIView.marginX + y * UIView.pieceWidth + (UIView.pieceWidth - UIView.widthOfImage) / 2);
                Canvas.SetTop(_selectedBitmap, UIView.marginY + x * UIView.pieceHeight + (UIView.pieceHeight - UIView.heightOfImage) / 2);
                _log.Content = $"{x} {y} {matrix[2,2]}";
            }
            else
            {
                // Tra ve vi tri cu
                getPos(_lastPiece, ref x, ref y);
                Canvas.SetLeft(_selectedBitmap, UIView.marginX + y * UIView.pieceWidth + (UIView.pieceWidth - UIView.widthOfImage) / 2);
                Canvas.SetTop(_selectedBitmap, UIView.marginY + x * UIView.pieceHeight + (UIView.pieceHeight - UIView.heightOfImage) / 2);

            }
        }
        private void CropImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _selectedBitmap = sender as Image;
            _lastPosition = e.GetPosition(this);
            _lastPiece  = e.GetPosition(this);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<int> randList = new List<int>();
            Random rand = new Random();
            matrix = new int[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    do
                    {
                        matrix[i, j] = rand.Next(1,10);
                    }
                    while (randList.Contains(matrix[i, j]));
                    randList.Add(matrix[i, j]);
                    _maLog += $"{matrix[i, j].ToString()} ";
                }
                _maLog += "\n";
            }
            playArea();
            loadImage(imagePath);

        }
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(this);

            int i = 0, j = 0;
            getPos(position, ref i, ref j);
            this.Title = $"{position.X} - {position.Y}, a[{i}][{j}]";
            if (_isDragging)
            {
                var dx = position.X - _lastPosition.X;
                var dy = position.Y - _lastPosition.Y;
                var lastLeft = Canvas.GetLeft(_selectedBitmap);
                var lastTop = Canvas.GetTop(_selectedBitmap);
                Canvas.SetLeft(_selectedBitmap, lastLeft + dx);
                Canvas.SetTop(_selectedBitmap, lastTop + dy);
                _lastPosition = position;
            }
        }
        private void playArea()
        {
            /*UI*/
            //Draw column
            for (int i = 0; i < UIView.rowNumber + 1; i++)
            {
                var line = new Line();
                line.StrokeThickness = 1;
                line.Stroke = new SolidColorBrush(Colors.Aqua);
                Puzzle.Children.Add(line);

                line.X1 = UIView.marginX + i * UIView.pieceWidth;
                line.Y1 = UIView.marginY;

                line.X2 = UIView.marginX + i * UIView.pieceWidth;
                line.Y2 = UIView.marginY + (UIView.colNumber) * UIView.pieceHeight;
            }

            //Draw row
            for (int i = 0; i < UIView.colNumber + 1; i++)
            {
                var line = new Line();
                line.StrokeThickness = 1;
                line.Stroke = new SolidColorBrush(Colors.Aqua);
                Puzzle.Children.Add(line);

                line.X1 = UIView.marginX;
                line.Y1 = UIView.marginY + i * UIView.pieceHeight;

                line.X2 = UIView.marginX + (UIView.rowNumber) * UIView.pieceWidth;
                line.Y2 = UIView.marginY + i * UIView.pieceHeight;
            }

        }
        private void loadImage(string tmpPreviewName)
        {
                BitmapImage source;
                if (mode == 2)
                {
                    source = new BitmapImage(new Uri(tmpPreviewName, UriKind.Absolute));
                }
                else
                {
                    source = new BitmapImage(new Uri(tmpPreviewName, UriKind.Relative));
                }
                
            int leng = (int)source.Width;
            if (source.Height < source.Width)
            {
                leng = (int)source.Height;
            }
            
            var previewImg = new Image();
            previewImg.Width = UIView.widthOfPreviewImage;
            previewImg.Height = UIView.heightOfPreviewImage;
            previewImg.Source = source;
            Puzzle.Children.Add(previewImg);
            previewImg.Tag = Tuple.Create(-1, -1);
            Canvas.SetLeft(previewImg, UIView.marginX*2 + UIView.pieceWidth*UIView.colNumber);
            Canvas.SetTop(previewImg, UIView.marginY);

            _matrixLog.Content = _maLog;
            for (int i = 0; i < UIView.rowNumber; i++)
            {
                for (int j = 0; j < UIView.colNumber; j++)
                {
                    if ((i != UIView.rowNumber - 1) || (j != UIView.colNumber - 1))
                    {
                        for (int m = 0; m < UIView.rowNumber; m++)
                        {
                            for (int n = 0; n < UIView.colNumber; n++)
                            {
                                if (m * 3 + n + 1 == matrix[i, j])
                                {
                                    // Duyet i 0--> 3. j 0-->3
                                    var h = (int)leng / UIView.rowNumber;
                                    var w = (int)leng / UIView.colNumber;
                                    // Crop theo i,j ( o thu i,j tren Image, ô như ô ma trận)
                                    var rect = new Int32Rect(j * w, i * h, w, h);
                                    var cropBitmap = new CroppedBitmap(source, rect);

                                    var img = new Image();
                                    img.Stretch = Stretch.Fill;
                                    img.Width = UIView.widthOfImage;
                                    img.Height = UIView.heightOfImage;
                                    img.Source = cropBitmap;
                                    Puzzle.Children.Add(img);
                                    // Gắn vô vị trí của ô (i,j)
                                    Canvas.SetLeft(img, UIView.marginX + n * UIView.pieceWidth + (UIView.pieceHeight - UIView.widthOfImage) / 2);
                                    Canvas.SetTop(img, UIView.marginY + m * UIView.pieceHeight + (UIView.pieceHeight - UIView.heightOfImage) / 2);
                                    img.MouseLeftButtonDown += CropImage_MouseLeftButtonDown;
                                    img.PreviewMouseLeftButtonUp += Image_PreviewMouseLeftButtonUp;
                                    img.Tag = new Tuple<int, int>(i, j);
                                }
                            }
                        }

                       
                    }
                }
            }
        }


    }
}

