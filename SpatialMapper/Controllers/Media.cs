using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections;

namespace SpatialMapper.MediaController
{
    class Media : MediaElement
    {
        private List<MediaTexture> _mediaTextures;
        private bool _loop = true;
        private bool _mute = false;
        private Brush _brush;
        #region properties

        public string fileUrl
        {
            get;
            set;
        }

        public Image thumbnail
        {
            get;
            set;
        }

        public List<MediaTexture> mediaTextures
        {
            get { return _mediaTextures; }
        }

        public string mediaName
        {
            get;
            set;
        }



        public Brush getBrush
        {
            get
            {
                return _brush;

            }

        }
        public bool loop
        {
            get { return _loop; }
            set
            {
                if (!value)
                {
                    this.MediaEnded -= Media_MediaEnded; _loop = false;
                }
                else
                    this.MediaEnded += Media_MediaEnded; _loop = true;
            }
        }

        public bool mute
        {
            get { return _mute; }
            set
            {
                if (value)
                {
                    this.Volume = 0; _mute = true;
                }
                else
                this.Volume=80; _mute = false;

            }
        }
        #endregion

     

        public Media(string url)
        {
            this._mediaTextures = new List<MediaTexture>();
            this.fileUrl = url;
            this.loop = true;
            this.mute = true;
            this.Source = new Uri(url);
            this.LoadedBehavior = MediaState.Manual;
            this.UnloadedBehavior = MediaState.Manual; 
            mediaName = Path.GetFileName(url);
            _brush = new VisualBrush(this);
            this.MediaEnded += new System.Windows.RoutedEventHandler(Media_MediaEnded);
            
        }

  
 
        void Media_MediaFailed(object sender, System.Windows.ExceptionRoutedEventArgs e)
        {
      
        }

        public void newTexture(string layerName) // creates copy of media brush 
        {
            //TODO: Check if there is MediaTexture from the same Media, for the same Layer IF there is don't create another one
            _mediaTextures.Add(new MediaTexture(_brush.Clone(), layerName, this.mediaName));
        }

        public MediaTexture getMediaTextureByLayerName(string layerName)
        {
            foreach (MediaTexture m in _mediaTextures)
            {
                if (m.parentLayer == layerName)
                    return m;
            }
            return null; //should never happen
        }

        public void removeLayerTexture(string LayerName)
        {
            foreach (MediaTexture m in _mediaTextures)
            {
                if (m.parentLayer == LayerName)
                {
                    _mediaTextures.Remove(m);
                    return;
                }
            }
        }

        void Media_MediaEnded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.loop)
            {
                this.Position = TimeSpan.Zero;
                this.Play();

            }
        }
    }
}
