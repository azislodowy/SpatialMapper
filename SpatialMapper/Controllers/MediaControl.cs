using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace SpatialMapper.MediaController
{
    class MediaControl
    {
        #region variables
        public List<Media> _mediaElements;
        #endregion

        #region constructor

        public MediaControl(Media defaultGrid)
        {
            _mediaElements = new List<Media>();
           
            _mediaElements.Add(defaultGrid);
        }
        #endregion

        #region Creation texture&media

        public void NewTextureForLayer(string MediaName,Layers layer) // todo: if there is no layers
        {
            Media selectedMedia = getMediaByName(MediaName);
            selectedMedia.newTexture(layer.layerName);

        }


        public void NewMediaSource(Media mediaElement)
        {
            _mediaElements.Add(mediaElement);
        }

        #endregion

        #region FindGet layermedia&mediabyName

        public MediaTexture getLayerMedia(string MediaName, string LayerName) // returns Media's material for specified layer if exists
        {
            return getMediaByName(MediaName).getMediaTextureByLayerName(LayerName);
        }

    

        public Media getMediaByName(string MediaName) // returns Media by its name
        {
            
            foreach (Media m in _mediaElements)
            {
                if (m.mediaName == MediaName)
                    return m;
            }
            return null; // should never happen
        }

        #endregion

        #region RemoveDelete mediasource&layerstextures

        public void DeleteMediaSource(Media mediaElement) // todo: remove all layer textures that belonge to this media element
        {
            mediaElement.Close();
            _mediaElements.Remove(mediaElement);

        }

        public void deleteAllLayersTextures(string LayerName) //removes all textures of specified layer name
        {
            foreach (Media m in _mediaElements)
            {
                m.removeLayerTexture(LayerName);
            }
            
        }

        #endregion

        #region alter media renameLayersMediaTexture& renameMedia
        
        public void renameAllMediaTexturesForLayer(string layerName, string newLayerName)
        {

            foreach (Media m in _mediaElements)
            {
                foreach (MediaTexture t in m.mediaTextures)
                {
                    if (t.parentLayer == layerName)
                        t.parentLayer = newLayerName;
                }
            }
        }

        public void renameMedia(string mediaName, string newMediaName)
        {
            foreach (Media m in _mediaElements)
            {
                if (m.mediaName == mediaName)
                    m.mediaName = newMediaName;
            }
        }

        public List<MediaTexture> getMediaListForLayer(string layerName)
        {
            List<MediaTexture> tempList = new List<MediaTexture>();
            foreach (Media media in _mediaElements)
            {
               MediaTexture mediaTexture = media.getMediaTextureByLayerName(layerName);
               if (mediaTexture != null)
               {
                   tempList.Add(mediaTexture);
               }

            }
            return tempList;
        }

        #endregion

        #region ERROR RELOAD MEDIA

        public void reloadMedia()
        {
            foreach (Media m in _mediaElements)
            {

                Uri temp = m.Source;
                m.Close();
                m.Source  =temp ;
                m.Play();

            }

        }

        #endregion

    }
}
