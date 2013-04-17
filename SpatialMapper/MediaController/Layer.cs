using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using System.IO;

namespace SpatialMapper.MediaController
{

   public class Layer
   {
        #region variables

        private MeshGeometry3D _mesh;
        private GeometryModel3D _Layer3DModel;
        private MatrixTransform3D _matrixTransform;
        public int numTextures = -1;
        public int position = 0;
        public Point3D[] _pointsTransformed;
        public MaterialGroup materials;

       #endregion

        #region properties
        public string layerName
        {
            get;
            set;
        }

    public MeshGeometry3D mesh
    {
    get { return _mesh; }
    set { _mesh = value; }
    }

    public GeometryModel3D Layer3DModel
    {
    get { return _Layer3DModel; }
    set { _Layer3DModel = value; }
    }

        #endregion

        #region Constructor
        public Layer(string name, int pos)
        {
            _matrixTransform = new MatrixTransform3D();
            materials = new MaterialGroup();
            createLayer(name);
            _pointsTransformed = new Point3D[4];
            for (int i = 0; i < 4; i++)
                _pointsTransformed[i] = _matrixTransform.Transform(_mesh.Positions[i]);
           _matrixTransform.Matrix = CalculateNonAffineTransform(_pointsTransformed);
           position = pos;

           
        }

#endregion

        #region BackGroundHelpers(GRID) SHOW/HIDE

        public void showBackGroundHelper(Material _backgroundHelper)
        {
            if(!materials.Children.Contains(_backgroundHelper)) // if there is alredy there is no need to add another one
            materials.Children.Add(_backgroundHelper);
        }
        public void hideBackGroundHelper(Material _backgroundHelper)
        {
            materials.Children.Remove(_backgroundHelper);
        }

        #endregion

        #region LayerTRANSFORMATIONS

        public void Move(double offsetX, double offsetY, int index)
        {
            Point3D pt = _pointsTransformed[index];
            pt.X = offsetX;
            pt.Y = offsetY;
            _pointsTransformed[index] = pt;
            _matrixTransform.Matrix = CalculateNonAffineTransform(_pointsTransformed);

        }

        Matrix3D CalculateNonAffineTransform(Point3D[] points)
        {
           
            Matrix3D A = new Matrix3D();
            A.M11 = points[2].X - points[0].X;
            A.M12 = points[2].Y - points[0].Y;
            A.M21 = points[1].X - points[0].X;
            A.M22 = points[1].Y - points[0].Y;
            A.OffsetX = points[0].X;
            A.OffsetY = points[0].Y;
            A.OffsetZ = points[0].Z;

            double d = A.M11 * A.M22 - A.M12 * A.M21;
            double a = (A.M22 * points[3].X - A.M21 * points[3].Y +
                        A.M21 * A.OffsetY - A.M22 * A.OffsetX) / d;

            double b = (A.M11 * points[3].Y - A.M12 * points[3].X +
                        A.M12 * A.OffsetX - A.M11 * A.OffsetY) / d;

            Matrix3D B = new Matrix3D();
            B.M11 = a / (a + b - 1);
            B.M22 = b / (a + b - 1);
            B.M14 = B.M11 - 1;
            B.M24 = B.M22 - 1;

            return B * A;
        }

        #endregion

        #region LayerCREATION

        public void createLayer(string name)
        {
            _mesh = new MeshGeometry3D();
            _mesh.Positions.Add(new Point3D(0, 0, 0));
            _mesh.Positions.Add(new Point3D(0, 1, 0));
            _mesh.Positions.Add(new Point3D(1, 0, 0));
            _mesh.Positions.Add(new Point3D(1, 1, 0));
            _mesh.TriangleIndices.Add(0);
            _mesh.TriangleIndices.Add(2);
            _mesh.TriangleIndices.Add(1);
            _mesh.TriangleIndices.Add(2);
            _mesh.TriangleIndices.Add(3);
            _mesh.TriangleIndices.Add(1);
            _mesh.TextureCoordinates.Add(new Point(0, 1));
            _mesh.TextureCoordinates.Add(new Point(0, 0));
            _mesh.TextureCoordinates.Add(new Point(1, 1));
            _mesh.TextureCoordinates.Add(new Point(1, 0));
          
           
    
            _Layer3DModel = new GeometryModel3D(mesh, new DiffuseMaterial(Brushes.White));
           
           
            _Layer3DModel.Transform = _matrixTransform;
          
          
          _Layer3DModel.Material = materials;
           layerName = name;
         

        }
        #endregion

        #region Layer Materials

        public void addMaterial(MediaTexture texture)
        {
           
            materials.Children.Add(texture.materialTexture);
            numTextures++;
        }

        #endregion
   }
}
