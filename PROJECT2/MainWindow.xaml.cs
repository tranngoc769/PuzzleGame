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
        int[,] matrix;
        Timer timer;
        private int mode, type,state;
        private string imagePath;
        private int count;
        public class Player
        {
            public static int time { get; set; }
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
            public const int marginX = 30;
            public const int marginY = 50;
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
            public static int pieceWidth { get; set; }
            public static int pieceHeight { get; set; }
            public const int StockImageWitdh = 360;
            public const int StockImageHeight = 360;
            public const string fileSave = "saveGame.txt";
        }
        public MainWindow(int mode, int type, string imagePath, int state)
        {
            InitializeComponent();
            this.mode = mode;
            this.type = type;
            this.imagePath = imagePath;
            this.state = state;

        }
        //Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            wiw.Visibility = Visibility.Hidden;
            lost.Visibility = Visibility.Hidden;
            PlayArea.Rows = type * 3;
            PlayArea.Columns = type * 3;
            Player._isDragging = false;
            Player.isStop = false;
            Player.time = type * 180;
            count = Player.time;
            Player.emptyX = PlayArea.Rows - 1;
            Player.emptyY = PlayArea.Columns - 1;
            Player._matrixImage = new Image[PlayArea.Rows * PlayArea.Columns];
            PlayArea.pieceHeight = PlayArea.StockImageHeight / PlayArea.Columns;
            PlayArea.pieceWidth = PlayArea.pieceHeight;
            Noti.Content = "Save successfully";
            Noti.Content = "Use mouse or button/arrow key to play";
            TimerStart();
            if (state == 1)
            {
                RandomMatrix();
                loadImage(imagePath);
            }
            else
            {
                loadGame();
                TimerReStart();
            }
        }
        //Others
        private void RandomMatrix()
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
                }
            }
        }
        private void Swap(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
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
        private void positionToIndex(Point position, ref int i, ref int j)
        {
            i = ((int)position.Y - PlayArea.marginY) / PlayArea.pieceHeight;
            j = ((int)position.X - PlayArea.marginX) / PlayArea.pieceWidth;
        }
        //Timer
        private void TimerReStart()
        {
            timer.Stop();
            count =Player.time;
            timer.Start();
        }
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
                lost.Visibility = Visibility.Visible;
                timer.Stop();
            }
            int min = count / 60;
            string sec = (count % 60).ToString();
            if (count % 60 < 10) sec = "0" + sec;
            Dispatcher.Invoke(() => pzTimer.Content = $"Time : 0{min}:{sec}");
        }
        //Drop image
        private void DragImageFinished(object sender, MouseButtonEventArgs e)
        {
            if (!Player.isStop)
            {
                Player._isDragging = false;
                var position = e.GetPosition(this);
                int x = 0, y = 0;
                int oldX = 0, oldY = 0;
                positionToIndex(Player._lastPiece, ref oldX, ref oldY);
                positionToIndex(position, ref x, ref y);
                //x,y la vi tri can tha
                //oldX, oldY la vi tri ban dau
                if (x < PlayArea.Rows && y < PlayArea.Columns && (x == oldX && (y == oldY + 1 || y == oldY - 1) || (y == oldY && (x == oldX + 1 || x == oldX - 1))) && matrix[x, y] == (PlayArea.Columns * PlayArea.Rows - 1))
                {
                    Canvas.SetLeft(Player._selectedBitmap, PlayArea.marginX + y * PlayArea.pieceWidth);
                    Canvas.SetTop(Player._selectedBitmap, PlayArea.marginY + x * PlayArea.pieceHeight);
                    Swap(ref matrix[x, y], ref matrix[oldX, oldY]);
                    Player.emptyX = oldX;
                    Player.emptyY = oldY;
                    if (checkWin() == true)
                    {
                        wiw.Visibility = Visibility.Visible;
                        timer.Stop();
                    }
                }
                else
                {
                    // Tra ve vi tri cu
                    Canvas.SetLeft(Player._selectedBitmap, PlayArea.marginX + oldY * PlayArea.pieceWidth);
                    Canvas.SetTop(Player._selectedBitmap, PlayArea.marginY + oldX * PlayArea.pieceHeight);

                }
            }
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
            positionToIndex(position, ref i, ref j);
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

        //Play area
        private void clearPlayArea()
        {
            var images = Puzzle.Children.OfType<Image>().ToList();
            foreach (var image in images)
            {
                Puzzle.Children.Remove(image);
            }
            var lines = Puzzle.Children.OfType<Line>().ToList();
            foreach (var line in lines)
            {
                Puzzle.Children.Remove(line);
            }
        }
        private void playArea()
        {
            /*UI*/
            //Draw column
            for (int i = 0; i < PlayArea.Rows + 1; i++)
            {
                var line = new Line();
                line.StrokeThickness = 3;
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
                line.StrokeThickness = 3;
                line.Stroke = new SolidColorBrush(Colors.Aqua);
                Puzzle.Children.Add(line);

                line.X1 = PlayArea.marginX;
                line.Y1 = PlayArea.marginY + i * PlayArea.pieceHeight;

                line.X2 = PlayArea.marginX + (PlayArea.Rows) * PlayArea.pieceWidth;
                line.Y2 = PlayArea.marginY + i * PlayArea.pieceHeight;
            }
        }
        // Arrow route
        private void ArrowUpHandle()
        {
            if (!Player.isStop)
            {
                if (Player.emptyX != PlayArea.Rows - 1)
                {
                    var img = new Image();
                    img = Player._matrixImage[matrix[Player.emptyX + 1, Player.emptyY]];
                    Canvas.SetLeft(img, PlayArea.marginX + Player.emptyY * PlayArea.pieceWidth);
                    Canvas.SetTop(img, PlayArea.marginY + Player.emptyX * PlayArea.pieceHeight);
                    Swap(ref matrix[Player.emptyX, Player.emptyY], ref matrix[Player.emptyX + 1, Player.emptyY]);
                    Player.emptyX++;
                }

                if (checkWin() == true)
                {

                    wiw.Visibility = Visibility.Visible;
                    timer.Stop();
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
                    Canvas.SetLeft(img, PlayArea.marginX + Player.emptyY * PlayArea.pieceWidth);
                    Canvas.SetTop(img, PlayArea.marginY + Player.emptyX * PlayArea.pieceHeight);
                    Swap(ref matrix[Player.emptyX, Player.emptyY], ref matrix[Player.emptyX - 1, Player.emptyY]);
                    Player.emptyX--;
                }
                if (checkWin() == true)
                {

                    wiw.Visibility = Visibility.Visible;
                    timer.Stop();
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
                    Canvas.SetLeft(img, PlayArea.marginX + Player.emptyY * PlayArea.pieceWidth);
                    Canvas.SetTop(img, PlayArea.marginY + Player.emptyX * PlayArea.pieceHeight);
                    Swap(ref matrix[Player.emptyX, Player.emptyY], ref matrix[Player.emptyX, Player.emptyY + 1]);
                    Player.emptyY++;
                }
                if (checkWin() == true)
                {

                    wiw.Visibility = Visibility.Visible;
                    timer.Stop();
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
                    Canvas.SetLeft(img, PlayArea.marginX + Player.emptyY * PlayArea.pieceWidth);
                    Canvas.SetTop(img, PlayArea.marginY + Player.emptyX * PlayArea.pieceHeight);
                    Swap(ref matrix[Player.emptyX, Player.emptyY], ref matrix[Player.emptyX, Player.emptyY - 1]);
                    Player.emptyY--;
                }
                if (checkWin() == true)
                {
                    wiw.Visibility = Visibility.Visible;
                    timer.Stop();
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
        //Button control
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
            //Save Data
            var writer = new StreamWriter("save");
            writer.WriteLine(type);
            writer.WriteLine(count);
            for (int i = 0; i < PlayArea.Rows; i++)
            {
                for (int j = 0; j < PlayArea.Columns; j++)
                {
                    writer.Write($"{matrix[i, j]}");
                    if (j != PlayArea.Columns - 1)
                    {
                        writer.Write(" ");

                    }

                }
                writer.WriteLine("");
            }
            writer.Close();
            
            Noti.Content = "Save successfully";
            Task.Delay(2000).ContinueWith(_ =>
            {
                Dispatcher.Invoke(() =>
                {
                    Noti.Content = "Use mouse or button/arrow key to play";
                });
            }
);
        }
        private bool loadGame()
        {
            //Chua try catch
            try
            {
                mode = 1;
                var reader = new StreamReader("save");
                PlayArea.Rows = 3 * int.Parse(reader.ReadLine());
                PlayArea.Columns = PlayArea.Rows;
                Player.time = int.Parse(reader.ReadLine());
                matrix = new int[PlayArea.Rows, PlayArea.Columns];
                for (int i = 0; i < PlayArea.Rows; i++)
                {
                    var line = reader.ReadLine().Split(new string[] { " " }, StringSplitOptions.None);
                    for (int j = 0; j < PlayArea.Columns; j++)
                    {
                        matrix[i, j] = int.Parse(line[j]);
                    }
                }
                reader.Close();
                //De clear o duoi tranh bi anh huong boi try catch
                clearPlayArea();
                Player._matrixImage = new Image[PlayArea.Rows * PlayArea.Columns];
                PlayArea.pieceHeight = PlayArea.StockImageHeight / PlayArea.Columns;
                PlayArea.pieceWidth = PlayArea.pieceHeight;
                loadImage("save.png");
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        private void loadbtn_Click(object sender, RoutedEventArgs e)
        {
            if (loadGame() == true)
            {

                wiw.Visibility = Visibility.Hidden;
                lost.Visibility = Visibility.Hidden;
                TimerReStart();
                Noti.Content = "Load successfully";
                Task.Delay(2000).ContinueWith(_ =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        Noti.Content = "Use mouse or button/arrow key to play";
                    });
                });
            }
            else
            {
                Noti.Content = "No game saved";
                Task.Delay(2000).ContinueWith(_ =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        Noti.Content = "Use mouse or button/arrow key to play";
                    });
                });
            }
        }
        private void mainMenu_Click(object sender, RoutedEventArgs e)
        {
            var mainmenu = new MainMenu();
            Close();
            mainmenu.ShowDialog();
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
            int leng = (int)source.PixelWidth;
            if (source.PixelHeight < source.PixelWidth)
            {
                leng = (int)source.PixelHeight;
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
                        img.Width = PlayArea.pieceWidth;
                        img.Height = PlayArea.pieceHeight;
                        img.Source = cropBitmap;
                        img.Tag = new Tuple<int,
                        int>(i, j);
                        Player._matrixImage[pos++] = img;
                    }
                }
            }
            for (int i = 0; i < PlayArea.Rows; i++)
            {
                for (int j = 0; j < PlayArea.Columns; j++)
                {
                    if (matrix[i, j] != PlayArea.Rows * PlayArea.Columns - 1)
                    {
                        var img = new Image();
                        img = Player._matrixImage[matrix[i, j]];
                        Puzzle.Children.Add(img);
                        string name = "Name_" + matrix[i, j].ToString();
                        Canvas.SetLeft(img, PlayArea.marginX + j * PlayArea.pieceWidth);
                        Canvas.SetTop(img, PlayArea.marginY + i * PlayArea.pieceHeight);
                        img.MouseLeftButtonDown += StartDragImage;
                        img.PreviewMouseLeftButtonUp += DragImageFinished;
                    }
                    else
                    {
                        Player.emptyX = i;
                        Player.emptyY = j;
                    }
                }
            }
            playArea();
        }
        private void reStart_Click(object sender, RoutedEventArgs e)
        {
            wiw.Visibility = Visibility.Hidden;
            lost.Visibility = Visibility.Hidden;
            Player.time = type * 180;
            Player.isStop = false;
            clearPlayArea();
            RandomMatrix();
            loadImage(imagePath);
            TimerReStart();
        }
    }
}