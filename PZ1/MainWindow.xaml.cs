using Microsoft.Win32;
using PZ1.Model;
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

namespace PZ1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {     
        public static Uri imageUri = null;
        public static Image newImage = null;
        public static bool cancelFlag = false;
        public static Shape Rendershape = null;
        public static bool changingFlag = false;
        public static object changingObject = null;
        public static Point currentPoint = new Point(0, 0);
        public static PointCollection points = new PointCollection();
        public static SelectedShape selectedShape = SelectedShape.None;
        public static List<Tuple<object, bool>> undo = new List<Tuple<object, bool>>();
        public static List<Tuple<object, bool>> redo = new List<Tuple<object, bool>>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Elipse_Click(object sender, RoutedEventArgs e)
        {
            selectedShape = SelectedShape.Ellipse;
            points = new PointCollection();
        }
        private void Button_Rechtangle_Click(object sender, RoutedEventArgs e)
        {
            selectedShape = SelectedShape.Rectangle;
            points = new PointCollection();
        }
        private void Button_Polygon_Click(object sender, RoutedEventArgs e)
        {
            selectedShape = SelectedShape.Polygon;
            points = new PointCollection();
        }
        private void Button_Image_Click(object sender, RoutedEventArgs e)
        {
            selectedShape = SelectedShape.Image;
            points = new PointCollection();
        }           

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (points.Count == 0)
            {
                changingFlag = true;
                var mouseWasDownOn = e.Source as FrameworkElement;
                if (mouseWasDownOn != null)
                {
                    if (mouseWasDownOn.GetType().Name != "Canvas")
                    {
                        changingObject = mouseWasDownOn;
                        if (changingObject.GetType().Name != "Image")
                        {
                            if(changingObject.GetType().Name == "Polygon")
                                points = (changingObject as Polygon).Points;
                            
                            Rect p = GetCoordsFromExistingObject.GetBoundingBox(mouseWasDownOn, canvas as FrameworkElement);
                            currentPoint.X = p.TopLeft.X;
                            currentPoint.Y = p.TopLeft.Y;
                            InfoWindow infoWindow = new InfoWindow();
                            infoWindow.ShowDialog();
                            
                            if (cancelFlag == false)
                            {
                                canvas.Children.Remove(changingObject as UIElement);
                                canvas.Children.Add(Rendershape);
                                undo.Add(new Tuple<object, bool>(Rendershape, true));
                            }
                        }
                        else
                        {
                            Image oldImage = (mouseWasDownOn as Image);
                            Point oldPosition = new Point(mouseWasDownOn.Margin.Top, mouseWasDownOn.Margin.Left);

                            Rect p = GetCoordsFromExistingObject.GetBoundingBox(mouseWasDownOn, canvas as FrameworkElement);

                            OpenFileDialog dlg = new OpenFileDialog();
                            dlg.InitialDirectory = "c:\\";
                            dlg.Filter = "Image files (*.jpg)|*.jpg|All Files (*.*)|*.*";

                            Nullable<bool> result = dlg.ShowDialog();

                            if (result == true)
                            {
                                string filename = dlg.FileName;
                                imageUri = new Uri(filename);

                                BitmapImage bitmap = new BitmapImage();
                                bitmap.BeginInit();
                                bitmap.UriSource = imageUri;
                                bitmap.EndInit();

                                newImage = new Image()
                                {
                                    Height = oldImage.Height,
                                    Width = oldImage.Width,
                                    Source = bitmap,
                                    Stretch = Stretch.Fill
                                };
                                newImage.Uid = oldImage.Uid;

                                Canvas.SetLeft(newImage, p.TopLeft.X);
                                Canvas.SetTop(newImage, p.TopLeft.Y);

                                canvas.Children.Remove(oldImage);
                                canvas.Children.Add(newImage);
                                undo.Add(new Tuple<object, bool>(newImage, true));

                                imageUri = null;
                                newImage = null;
                            }
                        }
                    }                     
                }                
            }
            else if (selectedShape == SelectedShape.Polygon && points.Count < 3)
            {
                MessageBox.Show("Minimum points for polygon is 3. Try again.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                InfoWindow infoWindow = new InfoWindow();
                infoWindow.ShowDialog();
                if (cancelFlag == false)
                {
                    canvas.Children.Add(Rendershape);
                    undo.Add(new Tuple<object, bool>(Rendershape, false));
                }                
            }            
            Rendershape = null;
            cancelFlag = false;
            changingFlag = false;
            changingObject = null;
            points = new PointCollection();
            currentPoint = new Point(0, 0);
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedShape != SelectedShape.None)
            {
                currentPoint = e.GetPosition((Canvas)sender);

                if (selectedShape != SelectedShape.Image)
                {
                    if (selectedShape != SelectedShape.Polygon)
                    {                        
                        InfoWindow infoWindow = new InfoWindow();
                        infoWindow.ShowDialog();
                        points = new PointCollection();
                        if (cancelFlag == false)
                        {
                            canvas.Children.Add(Rendershape);
                            undo.Add(new Tuple<object, bool>(Rendershape, false));
                        }
                    }
                    else
                        points.Add(e.GetPosition((Canvas)sender));                    
                }
                else
                {
                    OpenFileDialog dlg = new OpenFileDialog();
                    dlg.InitialDirectory = "c:\\";
                    dlg.Filter = "Image files (*.jpg)|*.jpg|All Files (*.*)|*.*";

                    Nullable<bool> result = dlg.ShowDialog();
                    
                    if (result == true)
                    {
                        string filename = dlg.FileName;
                        imageUri = new Uri(filename);
                        InfoWindow infoWindow = new InfoWindow();
                        infoWindow.ShowDialog();

                        if (cancelFlag == false)
                        {
                            canvas.Children.Add(newImage);
                            undo.Add(new Tuple<object, bool>(newImage, false));
                        }
                        imageUri = null;
                        newImage = null;                        
                    }                    
                }
                cancelFlag = false;
                changingFlag = false;
                changingObject = null;
                currentPoint = new Point(0, 0);
            }
        }

        private void Button_Undo_Click(object sender, RoutedEventArgs e)
        {
            if (undo.Count() == 0)
                return;
            Tuple<object, bool> undoObj = undo.Last();
            undo.Remove(undoObj);
            if (undoObj.Item1.GetType().Name.Contains("List"))
            {
                canvas.Children.Clear();
                foreach(UIElement uie in undoObj.Item1 as List<UIElement>)                
                    canvas.Children.Add(uie);
                
                redo.Add(undoObj);
            }
            else
            {
                if (undoObj.Item2 == false)
                {
                    canvas.Children.Remove(undoObj.Item1 as UIElement);
                    redo.Add(undoObj);
                }
                else
                {
                    foreach (Tuple<object, bool> el in undo)
                    {
                        if (el.Item1.GetType().Name.Contains("List"))
                            continue;
                        if ((el.Item1 as UIElement).Uid == (undoObj.Item1 as UIElement).Uid)
                        {
                            canvas.Children.Remove(undoObj.Item1 as UIElement);
                            redo.Add(undoObj);
                            canvas.Children.Add(el.Item1 as UIElement);
                            break;
                        }
                    }
                }
            }
        }

        private void Button_Redo_Click(object sender, RoutedEventArgs e)
        {
            if (redo.Count() == 0)
                return;
            Tuple<object, bool> redoObj = redo.Last();
            redo.Remove(redoObj);
            if (redoObj.Item1.GetType().Name.Contains("List"))
            {
                foreach (UIElement uie in redoObj.Item1 as List<UIElement>)
                {
                    canvas.Children.Remove(uie);
                }
                undo.Add(redoObj);
            }
            else
            {
                if (redoObj.Item2 == false)
                {
                    canvas.Children.Add(redoObj.Item1 as UIElement);
                    undo.Add(redoObj);
                }
                else
                {
                    foreach (UIElement uie in canvas.Children)
                    {
                        if (uie.Uid == (redoObj.Item1 as UIElement).Uid)
                        {
                            canvas.Children.Remove(uie);
                            canvas.Children.Add(redoObj.Item1 as UIElement);
                            undo.Add(redoObj);
                            break;
                        }
                    }
                }
            }
        }

        private void Button_X_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Clear_Click(object sender, RoutedEventArgs e)
        {
            List<UIElement> clearList = new List<UIElement>();
            foreach (UIElement child in canvas.Children)            
                clearList.Add(child);
            canvas.Children.Clear();
            undo.Add(new Tuple<object, bool>(clearList, false));            
        }
    }
}
