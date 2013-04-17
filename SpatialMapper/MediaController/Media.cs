using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;

namespace SpatialMapper.MediaController
{
    class Media : MediaElement
    {
        #region variables

        private List<MediaTexture> _mediaTextures;
        private bool _loop = true;
        private bool _mute = false;
        private Brush _brush;

        #endregion

        #region properties

        public string fileUrl
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

        #region Constructor

        public Media(string url)
        {
            this._mediaTextures = new List<MediaTexture>();
            this.fileUrl = url;
            this.loop = true;
            this.mute = false;
            this.Source = new Uri(url);
            this.LoadedBehavior = MediaState.Pause;
            this.UnloadedBehavior = MediaState.Manual;
            this.MediaEnded += new System.Windows.RoutedEventHandler(Media_MediaEnded);
            this.MediaFailed += new EventHandler<System.Windows.ExceptionRoutedEventArgs>(Media_MediaFailed);

            mediaName = Path.GetFileName(url);
            _brush = new VisualBrush(this);
            newTexture("$previewTexture$", -999);

        }

        #endregion

        #region events

        private void Media_MediaFailed(object sender, System.Windows.ExceptionRoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show(e.ErrorException.Message);
        }

        private void Media_MediaEnded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.loop)
            {
                this.Position = TimeSpan.Zero;
                this.Play();

            }
        }

        #endregion
       


        public void newTexture(string layerName, int position)
        {
            if (getMediaTextureByLayerName(layerName) == null)
            {
                _mediaTextures.Add(new MediaTexture(this.getBrush.Clone(), layerName, this.mediaName,position));
            }
            
        }

        public MediaTexture getMediaTextureByLayerName(string layerName)
        {
            foreach (MediaTexture m in _mediaTextures)
            {
                if (m.parentLayer == layerName)
                    return m;
            }
            return null; 
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
       
    }
}
