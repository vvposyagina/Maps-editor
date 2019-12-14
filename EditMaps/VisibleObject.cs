using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace EditMaps
{
    enum objects {wall, player, exit, empty};
    class VisibleObject : Button
    {
        public int coordX, coordY;
        public delegate void deleagateForMouseDown(int coordRow, int coordCol);
        public delegate void deleagateForMouseUp(int coordRow, int coordCol);
        
        public objects type;    
        public event deleagateForMouseDown eventMouseDown;
        public event deleagateForMouseUp eventMouseUp;
        
        public VisibleObject(int x, int y)
        {
            coordX = x;
            coordY = y;
            type = objects.empty;
            Click += CellReaction;
            PreviewMouseDown += methodeMouseDown;
            PreviewMouseUp += methodeMouseUp;
        }

        public VisibleObject(int x, int y, objects t)
        {
            coordX = x;
            coordY = y;
            type = t;
            Click += CellReaction;
            PreviewMouseDown += methodeMouseDown;
            PreviewMouseUp += methodeMouseUp;
        }

        public void CellReaction(object sender, RoutedEventArgs e)
        {
            type = Map.brush;
            switch(type)
            {
                case objects.wall:
                    Background = new SolidColorBrush(Colors.Black);
                    break;
                case objects.player:
                    Background = new SolidColorBrush(Colors.Green);
                    break;
                case objects.exit:
                    Background = new SolidColorBrush(Colors.Red);
                    break;
                case objects.empty:
                    Background = new SolidColorBrush(Colors.White);
                    break;
                default:
                    break;
            }
           
        }

        public void methodeMouseDown(object sender, RoutedEventArgs e)
        {
            eventMouseDown(coordX, coordY);
            Console.WriteLine("mouse down");
            //using(var writer = new StreamWriter("md.txt"))
            //{
            //    writer.WriteLine("md " + coordX + " " + coordY);
            //}
            //MessageBox.Show("md");
        }
        public void methodeMouseUp(object sender, RoutedEventArgs e)
        {
            eventMouseUp(coordX, coordY);
            Console.WriteLine("mouse up");
            //using (var writer = new StreamWriter("mu.txt"))
            //{
            //    writer.WriteLine("mu " + coordX + " " + coordY);
            //}
            //MessageBox.Show("mu");
        }
    }
}
