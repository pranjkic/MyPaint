using PZ1.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PZ1
{
    /// <summary>
    /// Interaction logic for InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow : Window
    {
        public InfoWindow()
        {
            InitializeComponent();
            if (MainWindow.selectedShape == SelectedShape.Polygon || MainWindow.changingFlag)
            {
                WidthLabel.Visibility = Visibility.Collapsed;
                Width.Visibility = Visibility.Collapsed;
                HeightLabel.Visibility = Visibility.Collapsed;
                Height.Visibility = Visibility.Collapsed;

                if(MainWindow.changingFlag)
                {
                    Shape changingShape = (MainWindow.changingObject as Shape);
                    FillColor.SelectedColor = FromHexToColorNameConverter.Convert(changingShape.Fill);
                    BorderColor.SelectedColor = FromHexToColorNameConverter.Convert(changingShape.Stroke);
                    BorderThickness.Text = changingShape.StrokeThickness.ToString();                    
                }
            }
            else if(MainWindow.selectedShape == SelectedShape.Image)
            {
                FillColorLabel.Visibility = Visibility.Collapsed;
                FillColor.Visibility = Visibility.Collapsed;
                BorderColorLabel.Visibility = Visibility.Collapsed;
                BorderColor.Visibility = Visibility.Collapsed;
                BorderThicknessLabel.Visibility = Visibility.Collapsed;
                BorderThickness.Visibility = Visibility.Collapsed;
            }
        }

        private void Button_Draw_Click(object sender, RoutedEventArgs e)
        {
            Shape Rendershape = null;
            Image newImage = null;
            if (MainWindow.changingFlag == false)
            {
                switch (MainWindow.selectedShape)
                {
                    case (SelectedShape.Ellipse):

                        if(string.IsNullOrWhiteSpace(Height.Text) || string.IsNullOrWhiteSpace(Width.Text) || string.IsNullOrWhiteSpace(BorderThickness.Text))
                        {
                            MessageBox.Show("All fields are required.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                        Rendershape = new Ellipse()
                        {
                            Height = Double.Parse(Regex.Replace(Height.Text, " ", "")),
                            Width = Double.Parse(Regex.Replace(Width.Text, " ", "")),
                            Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(FillColor.SelectedColor.ToString()),
                            Stroke = (SolidColorBrush)new BrushConverter().ConvertFromString(BorderColor.SelectedColor.ToString()),
                            StrokeThickness = Int32.Parse(Regex.Replace(BorderThickness.Text, " ", ""))
                        };
                        Rendershape.Uid = Guid.NewGuid().ToString();

                        Canvas.SetLeft(Rendershape, MainWindow.currentPoint.X);
                        Canvas.SetTop(Rendershape, MainWindow.currentPoint.Y - Rendershape.Height);                
                        MainWindow.Rendershape = Rendershape;
                        break;

                    case (SelectedShape.Rectangle):

                        if (string.IsNullOrWhiteSpace(Height.Text) || string.IsNullOrWhiteSpace(Width.Text) || string.IsNullOrWhiteSpace(BorderThickness.Text))
                        {
                            MessageBox.Show("All fields are required.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                        Rendershape = new Rectangle()
                        {
                            Height = Double.Parse(Regex.Replace(Height.Text, " ", "")),
                            Width = Double.Parse(Regex.Replace(Width.Text, " ", "")),
                            Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(FillColor.SelectedColor.ToString()),
                            Stroke = (SolidColorBrush)new BrushConverter().ConvertFromString(BorderColor.SelectedColor.ToString()),
                            StrokeThickness = Int32.Parse(Regex.Replace(BorderThickness.Text, " ", "")),
                            RadiusX = 12,
                            RadiusY = 12
                        };
                        Rendershape.Uid = Guid.NewGuid().ToString();

                        Canvas.SetLeft(Rendershape, MainWindow.currentPoint.X);
                        Canvas.SetTop(Rendershape, MainWindow.currentPoint.Y - Rendershape.Height);

                        MainWindow.Rendershape = Rendershape;
                        break;

                    case (SelectedShape.Polygon):

                        if (string.IsNullOrWhiteSpace(BorderThickness.Text))
                        {
                            MessageBox.Show("All fields are required.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                        Rendershape = new Polygon()
                        {
                            Points = MainWindow.points,
                            Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(FillColor.SelectedColor.ToString()),
                            Stroke = (SolidColorBrush)new BrushConverter().ConvertFromString(BorderColor.SelectedColor.ToString()),
                            StrokeThickness = Int32.Parse(Regex.Replace(BorderThickness.Text, " ", ""))                           
                        };
                        Rendershape.Uid = Guid.NewGuid().ToString();

                        MainWindow.Rendershape = Rendershape;
                        break;

                    case (SelectedShape.Image):

                        if (string.IsNullOrWhiteSpace(Height.Text) || string.IsNullOrWhiteSpace(Width.Text))
                        {
                            MessageBox.Show("All fields are required.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = MainWindow.imageUri;
                        bitmap.EndInit();

                        newImage = new Image()
                        {
                            Height = Double.Parse(Regex.Replace(Height.Text, " ", "")),
                            Width = Double.Parse(Regex.Replace(Width.Text, " ", "")),
                            Source = bitmap,
                            Stretch = Stretch.Fill
                        };
                        newImage.Uid = Guid.NewGuid().ToString();

                        Canvas.SetLeft(newImage, MainWindow.currentPoint.X);
                        Canvas.SetTop(newImage, MainWindow.currentPoint.Y - newImage.Height);

                        MainWindow.newImage = newImage;
                        break;
                }
            }
            else
            {
                switch(MainWindow.changingObject.GetType().Name)
                {
                    case ("Ellipse"):

                        if (string.IsNullOrWhiteSpace(BorderThickness.Text))
                        {
                            MessageBox.Show("StrokeThickness is required.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                        Ellipse ellipse = MainWindow.changingObject as Ellipse;
                        Rendershape = new Ellipse()
                        {
                            Height = ellipse.Height,
                            Width = ellipse.Width,
                            Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(FillColor.SelectedColor.ToString()),
                            Stroke = (SolidColorBrush)new BrushConverter().ConvertFromString(BorderColor.SelectedColor.ToString()),
                            StrokeThickness = Int32.Parse(BorderThickness.Text)
                        };
                        Rendershape.Uid = (MainWindow.changingObject as UIElement).Uid;

                        Canvas.SetLeft(Rendershape, MainWindow.currentPoint.X);
                        Canvas.SetTop(Rendershape, MainWindow.currentPoint.Y);

                        MainWindow.Rendershape = Rendershape;
                        break;

                    case ("Rectangle"):

                        if (string.IsNullOrWhiteSpace(BorderThickness.Text))
                        {
                            MessageBox.Show("StrokeThickness is required.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                        Rectangle rectangle = MainWindow.changingObject as Rectangle;
                        Rendershape = new Rectangle()
                        {
                            Height = rectangle.Height,
                            Width = rectangle.Width,
                            Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(FillColor.SelectedColor.ToString()),
                            Stroke = (SolidColorBrush)new BrushConverter().ConvertFromString(BorderColor.SelectedColor.ToString()),
                            StrokeThickness = Int32.Parse(BorderThickness.Text),
                            RadiusX = 12,
                            RadiusY = 12
                        };
                        Rendershape.Uid = (MainWindow.changingObject as UIElement).Uid;

                        Canvas.SetLeft(Rendershape, MainWindow.currentPoint.X);
                        Canvas.SetTop(Rendershape, MainWindow.currentPoint.Y);

                        MainWindow.Rendershape = Rendershape;
                        break;

                    case ("Polygon"):

                        if (string.IsNullOrWhiteSpace(BorderThickness.Text))
                        {
                            MessageBox.Show("StrokeThickness is required.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                        Rendershape = new Polygon()
                        {
                            Points = MainWindow.points,
                            Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(FillColor.SelectedColor.ToString()),
                            Stroke = (SolidColorBrush)new BrushConverter().ConvertFromString(BorderColor.SelectedColor.ToString()),
                            StrokeThickness = Int32.Parse(BorderThickness.Text)
                        };
                        Rendershape.Uid = (MainWindow.changingObject as UIElement).Uid;

                        MainWindow.Rendershape = Rendershape;
                        break;
                }
            }
            this.Close();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.cancelFlag = true;
            this.Close();
        }

        private void Width_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Height_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void BorderThickness_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
