
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace EditMaps
{
    class Map
    {
        public static objects brush;
        private int width, height;
        public VisibleObject[,] field;
        private int countOfPlayers;
        private int time;
        private Tuple<int, int> coordsFirstPressedKey;
        public Map(int h, int w)
        {
            width = w;
            height = h;
            field = new VisibleObject[h, w];
        }
        public Map()
        {
            width = 0;
            height = 0;
        }
        public int Width
        {
            get { return width; }
        }
        public int Height
        {
            get { return height; }
        }
        public int CountOfPlayers
        {
            get { return countOfPlayers; }
            set { countOfPlayers = value; }
        }
        public int Time
        {
            get { return time; }
            set { time = value; }
        }
        public void ResizeMap(int h, int w)
        {
            if (field == null)
            {
                width = w;
                height = h;
                field = new VisibleObject[h, w];
                for (int i = 0; i < h; i++)
                {
                    for (int j = 0; j < w; j++)
                    {
                        field[i, j] = new VisibleObject(i, j, objects.empty);
                        field[i, j].eventMouseDown += MouseDown;
                        field[i, j].eventMouseUp += MouseUp;
                    }
                }
            }
            else
            {
                VisibleObject[,] temp = field;
                field = new VisibleObject[h, w];
                for (int i = 0; i < h; i++)
                {
                    for (int j = 0; j < w; j++)
                    {
                        field[i, j] = new VisibleObject(i, j, objects.empty);
                    }
                }
                for (int i = 0; i < Math.Min(h, height); i++)
                {
                    for (int j = 0; j < Math.Min(w, width); j++)
                    {
                        field[i, j] = temp[i, j];
                    }
                }
                width = w;
                height = h;
            }
        }
        public int[,] BuildGraphOnTheMap()
        {
            int[,] graph = new int[Height + 2, Width + 2];

            for (int h = 0; h < Height; h++)
            {
                for (int w = 0; w < Width; w++)
                {
                    if (field[h, w].type == objects.empty ||
                        field[h, w].type == objects.player)
                    {
                        graph[h + 1, w + 1] = 0;
                    }
                    else if (field[h, w].type == objects.exit)
                    {
                        graph[h + 1, w + 1] = 2;
                    }
                    else
                    {
                        graph[h + 1, w + 1] = 1;
                    }
                }
            }

            for (int h = 0; h < Height + 2; h++)
            {
                graph[h, 0] = 1;
                graph[h, Width + 1] = 1;
            }
            for (int w = 0; w < Width + 2; w++)
            {
                graph[0, w] = 1;
                graph[Height + 1, w] = 1;
            }

            return graph;
        }
        public bool CheckConnectivity(ref string points, ref bool presentExit)
        {
            int[,] graph = BuildGraphOnTheMap();

            Queue<Tuple<int, int>> nodes = new Queue<Tuple<int, int>>();

            for (int h = 1; h <= Height; h++)
            {
                for (int w = 1; w <= Width; w++)
                {
                    if (graph[h, w] == 2)
                    {
                        nodes.Enqueue(new Tuple<int, int>(h, w));
                        graph[h, w] = -1;
                    }
                }
            }

            if (nodes.Count == 0)
            {
                presentExit = false;
                return false;
            }

            while (nodes.Count != 0)
            {
                Tuple<int, int> node = nodes.Dequeue();

                if (graph[node.Item1 - 1, node.Item2] == 0)
                {
                    graph[node.Item1 - 1, node.Item2] = -1;
                    nodes.Enqueue(new Tuple<int, int>(node.Item1 - 1, node.Item2));
                }
                if (graph[node.Item1 + 1, node.Item2] == 0)
                {
                    graph[node.Item1 + 1, node.Item2] = -1;
                    nodes.Enqueue(new Tuple<int, int>(node.Item1 + 1, node.Item2));
                }
                if (graph[node.Item1, node.Item2 - 1] == 0)
                {
                    graph[node.Item1, node.Item2 - 1] = -1;
                    nodes.Enqueue(new Tuple<int, int>(node.Item1, node.Item2 - 1));
                }
                if (graph[node.Item1, node.Item2 + 1] == 0)
                {
                    graph[node.Item1, node.Item2 + 1] = -1;
                    nodes.Enqueue(new Tuple<int, int>(node.Item1, node.Item2 + 1));
                }
            }
            bool flag = true;
            for (int h = 1; h <= Height; h++)
            {
                for (int w = 1; w <= Width; w++)
                {
                    if (graph[h, w] == 0)
                    {
                        points += String.Format("({0}, {1})\n", h, w);
                        flag = false;
                    }
                }
            }
            return flag;
        }
        public void DrawFieldByMap()
        {
            for (int h = 0; h < Height; h++)
            {
                for (int w = 0; w < Width; w++)
                {
                    field[h, w].Height = 10;
                    field[h, w].Width = 10;
                    switch (field[h, w].type)
                    {
                        case objects.empty:
                            field[h, w].Background = new SolidColorBrush(Colors.White);
                            break;
                        case objects.wall:
                            field[h, w].Background = new SolidColorBrush(Colors.Black);
                            break;
                        case objects.player:
                            field[h, w].Background = new SolidColorBrush(Colors.Green);
                            break;
                        case objects.exit:
                            field[h, w].Background = new SolidColorBrush(Colors.Red);
                            break;
                    }                    
                }
            }
        }  
        public StringBuilder SaveMap(string time)
        {
            StringBuilder str = new StringBuilder();
            List<Tuple<int, int>> coordsOfPlayers = new List<Tuple<int, int>>();
            str.AppendLine(Height + " " + Width);
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    switch (field[i, j].type)
                    {
                        case objects.empty:
                            str.Append("0");
                            break;
                        case objects.wall:
                            str.Append("1");
                            break;
                        case objects.player:
                            coordsOfPlayers.Add(new Tuple<int, int>(i, j));
                            str.Append("0");
                            break;
                        case objects.exit:
                            str.Append("2");
                            break;
                    }
                }
                str.AppendLine("");
            }
            CountOfPlayers = coordsOfPlayers.Count;
            str.AppendLine(CountOfPlayers.ToString());
            for (int q = 0; q < CountOfPlayers; q++)
            {
                str.AppendLine(coordsOfPlayers[q].Item1 + " " + coordsOfPlayers[q].Item2);
            }
            if (Int32.TryParse(time, out this.time) && this.time > 0)
            {
                str.AppendLine(time.ToString());
            }
            else
            {
                str.AppendLine("0");
                MessageBox.Show("Некорректно введенные данные. Время может быть представлено только целым положительным числом. По умолчанию сохранено значение 0");
            }
            return str;
        }
        public void OpenMap(StreamReader sr)
        {
            string[] str = sr.ReadLine().Split(' ');                   
            height = Int32.Parse(str[0]);
            width = Int32.Parse(str[1]);
            field = new VisibleObject[height, width];
            
            string temp;

            for (int h = 0; h < Height; h++)
            {
                temp = sr.ReadLine();
                for (int w = 0; w < Width; w++)
                {
                    switch (temp[w])
                    {
                        case '0':
                            field[h, w] = new VisibleObject(h, w, objects.empty);
                            break;
                        case '1':
                            field[h, w] = new VisibleObject(h, w, objects.wall);
                            break;
                        case '2':
                            field[h, w] = new VisibleObject(h, w, objects.exit);
                            break;
                        default:
                            throw new FormatException("Incorrect map format!");
                    }
                    field[h, w].eventMouseDown += MouseDown;
                    field[h, w].eventMouseUp += MouseUp;
                }
            }

            temp = sr.ReadLine();

            if (temp != "")
            {
                CountOfPlayers = Int32.Parse(temp);
                for (int i = 0; i < CountOfPlayers; i++)
                {
                    temp = sr.ReadLine();
                    str = temp.Split(' ');
                    field[Int32.Parse(str[0]), Int32.Parse(str[1])] = new VisibleObject(Int32.Parse(str[0]), Int32.Parse(str[1]), objects.player);
                }

                temp = sr.ReadLine();
                if (temp != "")
                {
                    Time = Int32.Parse(temp);
                }
            }
        }
        public void MouseDown(int y, int x)
        {
            coordsFirstPressedKey = new Tuple<int, int>(y, x);
        }
        public void MouseUp(int y, int x)
        {
            if(coordsFirstPressedKey.Item1 == y)
            {
                for(int i = Math.Min(coordsFirstPressedKey.Item2, x); i <= Math.Max(coordsFirstPressedKey.Item2, x); i++)
                {
                    field[y, i].type = brush;
                }
            }
            if(coordsFirstPressedKey.Item2 == x)
            {
                for (int i = Math.Min(coordsFirstPressedKey.Item1, y); i <= Math.Max(coordsFirstPressedKey.Item1, y); i++)
                {
                    field[i, x].type = brush;
                }
            }
            DrawFieldByMap();
            coordsFirstPressedKey = null;
        }
    }
}
