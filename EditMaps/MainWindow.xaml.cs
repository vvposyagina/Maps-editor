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
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace EditMaps
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {        
        Map newMap;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void create_Click(object sender, RoutedEventArgs e)
        {
            height.IsEnabled = true;
            width.IsEnabled = true;
            createField.IsEnabled = true;
            clearField.IsEnabled = true;            
            time.IsEnabled = true;
            newMap = new Map();
        }
        private void createField_Click(object sender, RoutedEventArgs e)
        {
            int w = 0, h = 0;
            if (Int32.TryParse(width.Text, out w) && Int32.TryParse(height.Text, out h) && w > 0 && h > 0 )
            {
                canvasForField.Children.Clear();
                newMap.ResizeMap(h, w);
                canvasForField.Width = w * 10;
                canvasForField.Height = h * 10;
                scrollForCanvas.IsEnabled = true;
                save.IsEnabled = true;
                brushes.IsEnabled = true;
                checkСonnectivity.IsEnabled = true;

                newMap.DrawFieldByMap();

                for (int i = 0; i < newMap.Height; i++)
                {
                    for (int j = 0; j < newMap.Width; j++)
                    {
                        newMap.field[i, j].Content = "";
                        newMap.field[i, j].Margin = new Thickness(j * 10, i * 10, 0, 0);
                        canvasForField.Children.Add(newMap.field[i, j]);
                    }
                }
            }
            else 
            {
                MessageBox.Show("Некорректно введенные данные. Введите целое положительное число");
            }
        }
        private void clearField_Click(object sender, RoutedEventArgs e)
        {
            canvasForField.Children.Clear();
        }
        private void wall_Click(object sender, RoutedEventArgs e)
        {
            Map.brush = objects.wall;
        }
        private void player_Click(object sender, RoutedEventArgs e)
        {
            Map.brush = objects.player;
        }
        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Map.brush = objects.exit;
        }
        private void empty_Click(object sender, RoutedEventArgs e)
        {
            Map.brush = objects.empty;
        }
        private void save_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog windowForSaving = new Microsoft.Win32.SaveFileDialog();
            windowForSaving.Filter = "map files (*.map)|*.map";
            windowForSaving.FilterIndex = 1;
            windowForSaving.RestoreDirectory = true;
            windowForSaving.ShowDialog();

            if (windowForSaving.FileName != "")
            {
                StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(windowForSaving.FileName);
                    string strForTime = time.Text;
                    sw.WriteLine(newMap.SaveMap(strForTime));
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Ошибка записи");
                }
                finally
                {
                    sw.Close();
                }
                
            }
        }
        private void open_Click(object sender, RoutedEventArgs e)
        {
            time.IsEnabled = true;
            height.IsEnabled = true;
            width.IsEnabled = true;
            createField.IsEnabled = true;
            clearField.IsEnabled = true;
            brushes.IsEnabled = true;
            scrollForCanvas.IsEnabled = true;
            save.IsEnabled = true;
            checkСonnectivity.IsEnabled = true;
            Microsoft.Win32.OpenFileDialog windowForOpening = new Microsoft.Win32.OpenFileDialog();
            windowForOpening.Filter = "map files (*.map)|*.map";
            windowForOpening.FilterIndex = 1;
            windowForOpening.RestoreDirectory = true;

            if (windowForOpening.ShowDialog() == true)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(windowForOpening.FileName))
                    {
                        newMap = new Map();
                        newMap.OpenMap(sr);
                        height.Text = newMap.Height.ToString();
                        width.Text = newMap.Width.ToString();

                        newMap.DrawFieldByMap();
                        for (int h = 0; h < newMap.Height; h++)
                        {
                            for (int w = 0; w < newMap.Width; w++)
                            {
                                newMap.field[h, w].Content = "";
                                newMap.field[h, w].Margin = new Thickness(w * 10, h * 10, 0, 0);
                                canvasForField.Children.Add(newMap.field[h, w]);
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Неверный формат файла");
                } 
            }
        }          
        private void checkСonnectivity_Click(object sender, RoutedEventArgs e)
        {
            string points = "";
            bool presentExit = true;
            if (newMap.CheckConnectivity(ref points, ref presentExit))
            {
                MessageBox.Show("Граф связный");
            }
            else if(!presentExit)
            {
                MessageBox.Show("Добавьте точки выхода");
            }
            else 
            {
                new InfoAfterCheck(points).ShowDialog();
            }
        }   
    }
}
