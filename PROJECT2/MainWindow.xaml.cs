using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
        public class Player
        {
            public static bool isStop
            {
                get;
                set;
            }
            public static bool _isDragging
            {
                get;
                set;
            }
            public static Image _selectedBitmap
            {
                get;
                set;
            }
            public static Image[] _matrixImage
            {
                get;
                set;
            }
            public static Point _lastPosition
            {
                get;
                set;
            }
            public static Point _lastPiece
            {
                get;
                set;
            }
            public static int emptyX
            {
                get;
                set;
            }
            public static int emptyY
            {
                get;
                set;
            }

        }
        public class PlayArea
        {
            //changeabel value
            public static Image StockImage { get; set; }
            public static int Rows
            {
                get;
                set;
            }
            public static int Columns
            {
                get;
                set;
            }
            public static int marginX
            {
                get;
                set;
            }
            public static int marginY
            {
                get;
                set;
            }
            public static int stockImageStartX
            {
                get;
                set;
            }
            public static int stockImageStartY
            {
                get;
                set;
            }
            public const int pieceWidth = 70;
            public const int pieceHeight = 70;
            public const int widthOfImage = 65;
            public const int heightOfImage = 65;
            public const int StockImageWitdh = 210;
            public const int StockImageHeight = 210;
            public const string fileSave = "saveGame.txt";
        }
        int[,] matrix;
        string _maLog = "";
        Timer timer;
        int count = 180;
        private int mode, type;
        private string imagePath;
        public MainWindow(int mode, int type, string imagePath)
        {
            InitializeComponent();
            this.mode = mode;
            this.type = type;
            this.imagePath = imagePath;
            PlayArea.Rows = type * 3;
            PlayArea.Columns = type * 3;
            PlayArea.marginX = 30;
            PlayArea.marginY = 30;
            Player._isDragging = false;
            Player.emptyX = PlayArea.Rows - 1;
            Player.emptyY = PlayArea.Columns - 1;

        }
        private bool checkWin()
        {
            for (int i = 0; i < PlayArea.Rows; i++)
            {
                for (int j = 0; j < PlayArea.Columns; j++)
                {
                    if (matrix[i, j] != i * PlayArea.Columns + j)
                    {
                        Player.isStop = false;
                        return Player.isStop;
                    }

                }
            }
            Player.isStop = true;
            return Player.isStop;
        }
        private void getPos(Point position, ref int i, ref int j)
        {
            i = ((int)position.Y - PlayArea.marginY) / PlayArea.pieceHeight;
            j = ((int)position.X - PlayArea.marginX) / PlayArea.pieceWidth;
        }
        //Timer
        private void TimerStart()
        {
            timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {

            count--;
            if (count == 0)
            {
                Player.isStop = true;
                timer.Stop();
            }
            Dispatcher.Invoke(() => pzTimer.Content = $"Time : {count}");
        }
        //Drop image
        private void DragImageFinished(object sender, MouseButtonEventArgs e)
        {
            Player._isDragging = false;
            var position = e.GetPosition(this);
            int x = 0, y = 0;
            int oldX = 0, oldY = 0;
            getPos(Player._lastPiece, ref oldX, ref oldY);
            getPos(position, ref x, ref y);
            //x,y la vi tri can tha
            //oldX, oldY la vi tri ban dau
            if (x < PlayArea.Rows && y < PlayArea.Columns && (x == oldX && (y == oldY + 1 || y == oldY - 1) || (y == oldY && (x == oldX + 1 || x == oldX - 1))) && matrix[x, y] == (PlayArea.Columns * PlayArea.Rows - 1))
            {
                Canvas.SetLeft(Player._selectedBitmap, PlayArea.marginX + y * PlayArea.pieceWidth + (PlayArea.pieceWidth - PlayArea.widthOfImage) / 2);
                Canvas.SetTop(Player._selectedBitmap, PlayArea.marginY + x * PlayArea.pieceHeight + (PlayArea.pieceHeight - PlayArea.heightOfImage) / 2);
                int temp = matrix[x, y];
                matrix[x, y] = matrix[oldX, oldY];
                matrix[oldX, oldY] = temp;
                Player.emptyX = oldX;
                Player.emptyY = oldY;
            }
            else
            {
                // Tra ve vi tri cu
                Canvas.SetLeft(Player._selectedBitmap, PlayArea.marginX + oldY * PlayArea.pieceWidth + (PlayArea.pieceWidth - PlayArea.widthOfImage) / 2);
                Canvas.SetTop(Player._selectedBitmap, PlayArea.marginY + oldX * PlayArea.pieceHeight + (PlayArea.pieceHeight - PlayArea.heightOfImage) / 2);

            }
            Test();
            if (checkWin() == true)
            {
                MessageBox.Show("WIN");
            }
        }
        private void Test()
        {
            _maLog = "";
            for (int i = 0; i < PlayArea.Rows; i++)
            {
                for (int j = 0; j < PlayArea.Columns; j++)
                {

                    _maLog += $"{matrix[i, j].ToString()} ";
                }
                _maLog += "\n";
            }
            _matrixLog.Content = _maLog;
        }
        //Drag image
        private void StartDragImage(object sender, MouseButtonEventArgs e)
        {
            if (!Player.isStop)
            {
                Player._isDragging = true;
                Player._selectedBitmap = sender as Image;
                Player._lastPosition = e.GetPosition(this);
                Player._lastPiece = e.GetPosition(this);
            }
        }
        //Dragging
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(this);

            int i = 0,
            j = 0;
            getPos(position, ref i, ref j);
            this.Title = $"{position.X} - {position.Y}, a[{i}][{j}]";
            if (Player._isDragging)
            {
                var dx = position.X - Player._lastPosition.X;
                var dy = position.Y - Player._lastPosition.Y;
                var lastLeft = Canvas.GetLeft(Player._selectedBitmap);
                var lastTop = Canvas.GetTop(Player._selectedBitmap);
                Canvas.SetLeft(Player._selectedBitmap, lastLeft + dx);
                Canvas.SetTop(Player._selectedBitmap, lastTop + dy);
                Player._lastPosition = position;
            }
        }
        private void Img_MouseEnter(object sender, MouseButtonEventArgs e)
        {
            //_isDragging = true;
            Player._selectedBitmap = sender as Image;
            Point pos = e.GetPosition(this);
            //_lastPiece = e.GetPosition(this);
            var animation = new DoubleAnimation();
            animation.From = pos.X;
            animation.To = pos.X + 18;
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            animation.AutoReverse = false;
            var story = new Storyboard();
            story.Children.Add(animation);
            Storyboard.SetTargetName(animation, Player._selectedBitmap.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.LeftProperty));
            story.Begin(this);
        }
       
        //Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<int> randList = new List<int>();
            Random rand = new Random();
            matrix = new int[PlayArea.Rows, PlayArea.Columns];
            for (int i = 0; i < PlayArea.Rows; i++)
            {
                for (int j = 0; j < PlayArea.Columns; j++)
                {
                    do
                    {
                        matrix[i, j] = rand.Next(0, PlayArea.Rows * PlayArea.Columns);
                    }
                    while (randList.Contains(matrix[i, j]));
                    randList.Add(matrix[i, j]);
                    _maLog += $"{matrix[i, j].ToString()} ";
                }
                _maLog += "\n";
            }
            Player._matrixImage = new Image[PlayArea.Rows * PlayArea.Columns];
            playArea();
            loadImage(imagePath);
            TimerStart();
        }
        //Draw play area
        private void playArea()
        {
            /*UI*/
            //Draw column
            for (int i = 0; i < PlayArea.Rows + 1; i++)
            {
                var line = new Line();
                line.StrokeThickness = 1;
                line.Stroke = new SolidColorBrush(Colors.Aqua);
                Puzzle.Children.Add(line);
                line.X1 = PlayArea.marginX + i * PlayArea.pieceWidth;
                line.Y1 = PlayArea.marginY;
                line.X2 = PlayArea.marginX + i * PlayArea.pieceWidth;
                line.Y2 = PlayArea.marginY + (PlayArea.Columns) * PlayArea.pieceHeight;
            }

            //Draw row
            for (int i = 0; i < PlayArea.Columns + 1; i++)
            {
                var line = new Line();
                line.StrokeThickness = 1;
                line.Stroke = new SolidColorBrush(Colors.Aqua);
                Puzzle.Children.Add(line);

                line.X1 = PlayArea.marginX;
                line.Y1 = PlayArea.marginY + i * PlayArea.pieceHeight;

                line.X2 = PlayArea.marginX + (PlayArea.Rows) * PlayArea.pieceWidth;
                line.Y2 = PlayArea.marginY + i * PlayArea.pieceHeight;
            }
        }
        // Arrow route
        //Arrow model
        private void ArrowUpHandle()
        {
            if (!Player.isStop)
            {
                if (Player.emptyX != PlayArea.Rows - 1)
                {
                    var img = new Image();
                    img = Player._matrixImage[matrix[Player.emptyX + 1, Player.emptyY]];
                    Canvas.SetLeft(img, PlayArea.marginX + Player.emptyY * PlayArea.pieceWidth + (PlayArea.pieceWidth - PlayArea.widthOfImage) / 2);
                    Canvas.SetTop(img, PlayArea.marginY + Player.emptyX * PlayArea.pieceHeight + (PlayArea.pieceHeight - PlayArea.heightOfImage) / 2);
                    int temp = matrix[Player.emptyX, Player.emptyY];
                    matrix[Player.emptyX, Player.emptyY] = matrix[Player.emptyX + 1, Player.emptyY];
                    matrix[Player.emptyX + 1, Player.emptyY] = temp;
                    Player.emptyX++;
                    Test();
                }

                if (checkWin() == true)
                {
                    MessageBox.Show("WIN");
                }
            }
        }
        private void ArrowDownHandle()
        {
            if (!Player.isStop)
            {
                if (Player.emptyX != 0)
                {
                    var img = new Image();
                    img = Player._matrixImage[matrix[Player.emptyX - 1, Player.emptyY]];
                    Canvas.SetLeft(img, PlayArea.marginX + Player.emptyY * PlayArea.pieceWidth + (PlayArea.pieceWidth - PlayArea.widthOfImage) / 2);
                    Canvas.SetTop(img, PlayArea.marginY + Player.emptyX * PlayArea.pieceHeight + (PlayArea.pieceHeight - PlayArea.heightOfImage) / 2);
                    int temp = matrix[Player.emptyX, Player.emptyY];
                    matrix[Player.emptyX, Player.emptyY] = matrix[Player.emptyX - 1, Player.emptyY];
                    matrix[Player.emptyX - 1, Player.emptyY] = temp;
                    Player.emptyX--;
                    Test();
                }
                if (checkWin() == true)
                {
                    MessageBox.Show("WIN");
                }
            }
        }
        private void ArrowLeftHandle()
        {

            if (!Player.isStop)
            {
                if (Player.emptyY != PlayArea.Columns - 1)
                {
                    var img = new Image();
                    img = Player._matrixImage[matrix[Player.emptyX, Player.emptyY + 1]];
                    Canvas.SetLeft(img, PlayArea.marginX + Player.emptyY * PlayArea.pieceWidth + (PlayArea.pieceWidth - PlayArea.widthOfImage) / 2);
                    Canvas.SetTop(img, PlayArea.marginY + Player.emptyX * PlayArea.pieceHeight + (PlayArea.pieceHeight - PlayArea.heightOfImage) / 2);
                    int temp = matrix[Player.emptyX, Player.emptyY];
                    matrix[Player.emptyX, Player.emptyY] = matrix[Player.emptyX, Player.emptyY + 1];
                    matrix[Player.emptyX, Player.emptyY + 1] = temp;
                    Player.emptyY++;
                    Test();
                }
                if (checkWin() == true)
                {
                    MessageBox.Show("WIN");
                }
            }
        }
        private void ArrowRightHandle()
        {

            if (!Player.isStop)
            {
                if (Player.emptyY != 0)
                {
                    var img = new Image();
                    img = Player._matrixImage[matrix[Player.emptyX, Player.emptyY - 1]];
                    Canvas.SetLeft(img, PlayArea.marginX + Player.emptyY * PlayArea.pieceWidth + (PlayArea.pieceWidth - PlayArea.widthOfImage) / 2);
                    Canvas.SetTop(img, PlayArea.marginY + Player.emptyX * PlayArea.pieceHeight + (PlayArea.pieceHeight - PlayArea.heightOfImage) / 2);
                    int temp = matrix[Player.emptyX, Player.emptyY];
                    matrix[Player.emptyX, Player.emptyY] = matrix[Player.emptyX, Player.emptyY - 1];
                    matrix[Player.emptyX, Player.emptyY - 1] = temp;
                    Player.emptyY--;
                    Test();
                }
                if (checkWin() == true)
                {
                    MessageBox.Show("WIN");
                }
            }
        }
        //Arrow Event
        private void upArrow_Click(object sender, RoutedEventArgs e)
        {
            ArrowUpHandle();
        }
        private void downArrow_Click(object sender, RoutedEventArgs e)
        {
            ArrowDownHandle();
        }
        private void leftArrow_Click(object sender, RoutedEventArgs e)
        {
            ArrowLeftHandle();
        }
        private void rightArrow_Click(object sender, RoutedEventArgs e)
        {
            ArrowRightHandle();
        }
        private void Key_Pressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                ArrowDownHandle();
            }
            else if (e.Key == Key.Up)
            {
                ArrowUpHandle();
            }
            else if (e.Key == Key.Left)
            {
                ArrowLeftHandle();
            }
            else if (e.Key == Key.Right)
            {
                ArrowRightHandle();
            }
        }
        
        private void savebtn_Click(object sender, RoutedEventArgs e)
        {
            //Save image
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)PlayArea.StockImage.Source));
            using (FileStream stream = new FileStream("save.png", FileMode.Create))
            encoder.Save(stream);
            MessageBox.Show("Save success");
            //Save Data
            var writer = new StreamWriter("save");
            writer.WriteLine(type);
            for (int i = 0; i < PlayArea.Rows; i++)
            {
                for (int j = 0; j < PlayArea.Columns; j++)
                {
                    writer.Write($"{matrix[i, j]}");
                    if (j != PlayArea.Columns-1)
                    {
                        writer.Write(" ");

                    }

                }
                writer.WriteLine("");
            }
            writer.Close();
        }

        private void loadbtn_Click(object sender, RoutedEventArgs e)
        {
            mode = 1;
            var images = Puzzle.Children.OfType<Image>().ToList();
            foreach (var image in images)
            {
                Puzzle.Children.Remove(image);
            }
            //Chua try catch
            var reader = new StreamReader("save");
            PlayArea.Rows = 3*int.Parse(reader.ReadLine());
            PlayArea.Columns = PlayArea.Rows;
            for (int i = 0; i < PlayArea.Rows; i++)
            {
                var line = reader.ReadLine().Split(new string[] { " " }, StringSplitOptions.None);
                for (int j = 0; j < PlayArea.Columns; j++)
                {
                    matrix[i, j] = int.Parse(line[j]);
                }
            }
            reader.Close();
            Test();
            loadImage("save.png");
        }

        private void loadImage(string stockImagePath)
        {
            BitmapImage source;
            if (mode == 2)
            {
                source = new BitmapImage(new Uri(stockImagePath, UriKind.Absolute));
            }
            else
            {
                source = new BitmapImage(new Uri(stockImagePath, UriKind.Relative));
            }
            int leng = (int)source.Width;
            if (source.Height < source.Width)
            {
                leng = (int)source.Height;
            }
            PlayArea.StockImage = new Image();
            PlayArea.StockImage.Width = PlayArea.StockImageWitdh;
            PlayArea.StockImage.Height = PlayArea.StockImageHeight;
            PlayArea.StockImage.Source = source;
            // ADD Stock Image
            Puzzle.Children.Add(PlayArea.StockImage);
            PlayArea.StockImage.Tag = Tuple.Create(-1, -1);
            Canvas.SetLeft(PlayArea.StockImage, PlayArea.marginX * 2 + PlayArea.pieceWidth * PlayArea.Columns);
            Canvas.SetTop(PlayArea.StockImage, PlayArea.marginY);
            _matrixLog.Content = _maLog;
            int pos = 0;
            for (int i = 0; i < PlayArea.Rows; i++)
            {
                for (int j = 0; j < PlayArea.Columns; j++)
                {
                    if ((i != PlayArea.Rows - 1) || (j != PlayArea.Columns - 1))
                    {
                        // Duyet i 0--> 3. j 0-->3
                        var h = (int)leng / PlayArea.Rows;
                        var w = (int)leng / PlayArea.Columns;
                        // Crop theo i,j ( o thu i,j tren Image, ô như ô ma trận)
                        var rect = new Int32Rect(j * w, i * h, w, h);
                        var cropBitmap = new CroppedBitmap(source, rect);
                        var img = new Image();
                        img.Stretch = Stretch.Fill;
                        img.Width = PlayArea.widthOfImage;
                        img.Height = PlayArea.heightOfImage;
                        img.Source = cropBitmap;
                        img.Tag = new Tuple<int,
                        int>(i, j);
                        Player._matrixImage[pos++] = img;
                    }
                }
            }
            string a = "";
            for (int i = 0; i < PlayArea.Rows; i++)
            {
                for (int j = 0; j < PlayArea.Columns; j++)
                {
                    if (matrix[i, j] != PlayArea.Rows * PlayArea.Columns - 1)
                    {
                        a += $"{matrix[i, j]} ";
                        var img = new Image();
                        img = Player._matrixImage[matrix[i, j]];
                        Puzzle.Children.Add(img);
                        string name = "Name_" + matrix[i, j].ToString();
                        Canvas.SetLeft(img, PlayArea.marginX + j * PlayArea.pieceWidth + (PlayArea.pieceHeight - PlayArea.widthOfImage) / 2);
                        Canvas.SetTop(img, PlayArea.marginY + i * PlayArea.pieceHeight + (PlayArea.pieceHeight - PlayArea.heightOfImage) / 2);
                        img.MouseLeftButtonDown += StartDragImage;
                        img.PreviewMouseLeftButtonUp += DragImageFinished;
                        img.MouseRightButtonUp += Img_MouseEnter;
                    }
                    else
                    {
                        Player.emptyX = i;
                        Player.emptyY = j;

                    }
                }
            }

        }
        
    }
}