using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using SpatialMapper.MediaController;

namespace SpatialMapper.RenderWindow
{
    /// <summary>
    /// Interaction logic for Renderer.xaml
    /// </summary>
    public partial class Renderer : Window
    {
        int _clicks = 0;
        bool _edit = false;
        Layer _selectedLayer;
        MainWindow _parentWindow;

        public Renderer(Model3DCollection Layers, Window parent)
        {
            InitializeComponent();
            models3dgroup.Children = Layers;
            _parentWindow = parent as MainWindow;
        }

        public void editMode(bool inEditMode, Layer layer)
        {
            if (inEditMode)
                this.Cursor = Cursors.Cross;
            else
                this.Cursor = Cursors.None;
            _clicks = 0;
        _edit = inEditMode;
           
        _selectedLayer = layer;  
        }



       Point3D Simple2Dto3D(Viewport3D vp, Point pt)
        {
            OrthographicCamera cam = vp.Camera as OrthographicCamera;
            double scale = cam.Width / vp.ActualWidth;
            double x = scale * (pt.X - vp.ActualWidth / 2) + cam.Position.X;
            double y = scale * (vp.ActualHeight / 2 - pt.Y) + cam.Position.Y;
            return new Point3D(x, y, 0);    
       }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }


        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs args)
        {
            if (_edit && _selectedLayer!=null)
            {
                if (_clicks == 4)
                {
                    _clicks = 0;
                 }
                Point mouse = args.GetPosition(viewport);
                Point3D point3d = Translate2Dto3D(viewport, mouse);
                _selectedLayer.Move(point3d.X, point3d.Y, _clicks++);
                _parentWindow.unsubscribeLayerTransformEvents();
                _parentWindow.loadSliders();
                _parentWindow.subscribeLayerTransformEvents();
            }
            args.Handled = true;
        }


        public Point3D Translate2Dto3D(Viewport3D viewport, Point point)
        {
            OrthographicCamera camera = viewport.Camera as OrthographicCamera;
            double scale = camera.Width / viewport.ActualWidth;
            double x = scale * (point.X - viewport.ActualWidth / 2) + camera.Position.X;
            double y = scale * (viewport.ActualHeight / 2 - point.Y) + camera.Position.Y;

            return new Point3D(x, y, 0);

        }
    }
}
