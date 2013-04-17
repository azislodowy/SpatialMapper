using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace SpatialMapper.MediaController
{
     class MediaTexture
    {
        private Brush _videoBrush;
        private double _opacity = 1;
        public Material materialTexture
        {
            get;
            set;
        }
        public string parentLayer
        {
            get;
            set;
        }

        public string parentMedia
        {
            get;
            set;
        }

        public Brush getBrush
        {
            get
            {
                return _videoBrush;
            }
        }

        public double Opacity
        {
            set
            {
                this._opacity = value;
                this._videoBrush.Opacity = value;
            }
            get
            {
                return _opacity;
            }
        }


        public MediaTexture(Brush videoBrush, string parentLayerName, string parentMediaName)
        {
            _videoBrush = videoBrush;
            materialTexture = new DiffuseMaterial(_videoBrush);
            parentMedia = parentMediaName;
            parentLayer = parentLayerName;
        }

      
    }
}
