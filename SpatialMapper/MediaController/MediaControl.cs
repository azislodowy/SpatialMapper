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

        private static int CompareMediaTextures(MediaTexture mt1, MediaTexture mt2)
        {
            if (mt1.position < mt2.position)
                return 1;
            else if (mt1.position > mt2.position)
                return -1;
            else return 0;
          
        }

        public MediaControl(Media defaultGrid)
        {
            _mediaElements = new List<Media>();
            _mediaElements.Add(defaultGrid);
        }
        #endregion


        #region Creation texture&media

       
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

        public List<MediaTexture> getMediaListForLayer(string layerName, int layerTextureCount)
        {
            List<MediaTexture> tempList = new List<MediaTexture>();
            foreach (Media m in _mediaElements)
            {
                foreach (MediaTexture mt in m.mediaTextures)
                {
                    if (mt.parentLayer == layerName)
                        tempList.Add(mt);
                }
     
                //tempList.Sort(new Comparison<MediaTexture>(MediaTexture mt1, MediaTexture mt2) { return mt1. });
                tempList.Sort(CompareMediaTextures);

            }
            return tempList;
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
                {
                    foreach (MediaTexture t in m.mediaTextures)
                    {
                        t.parentMedia = newMediaName;
                    }
                    m.mediaName = newMediaName;

                }

            }
        }

        #endregion
 

    }
}
