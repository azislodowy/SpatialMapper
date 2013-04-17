using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows;
using System.Windows.Controls;


namespace SpatialMapper.MediaController
{
    class LayerController
    {
        #region variables
        public List<Layers> _Layers;
        private MediaControl _MediaControl;
        #endregion

        #region constructor

        public LayerController(MediaControl mediaController)
        {
            _MediaControl = mediaController;
            _Layers = new List<Layers>();
        
        }

        #endregion

        #region Creation Layers Media // MOVE NEW MEDIA SOMEWHERE ELSE

        public void newLayer(string name)
        {
            Layers Layer = new Layers(name);
            _Layers.Add(Layer);
        }

        public void newMedia(string filename)
        {
            _MediaControl.NewMediaSource(new Media(filename));
        }

        #endregion

        #region FIND ELEMENTS LAYERS/MEDIA
        public Layers findLayer(string byName)
        {
            foreach (Layers geo in _Layers)
            {
                if (geo.layerName == byName)
                {
                    return geo;
                }
            }
            return null;
        }


        public Media findMedia(string byName) // chuj wi
        {
            return _MediaControl.getMediaByName(byName);
        }

        #endregion

        #region GRIDHELPER

        public void HideHelpers(MediaTexture helper)
        {
            foreach (Layers layer in _Layers)
            {
                layer.hideBackGroundHelper(helper.materialTexture);
            }
        }

        public void ShowHelpers(MediaTexture helper)
        {
            foreach (Layers layer in _Layers)
            {
                layer.showBackGroundHelper(helper.materialTexture);
            }

        }

        #endregion

        #region DELETE/REMOVAL layer&textures

        public bool removeLayer(Layers _Layer) //removes layer and all underlying textures
        {
            _MediaControl.deleteAllLayersTextures(_Layer.layerName);
            return this._Layers.Remove(_Layer);
        }

        public bool removeTexture(string MediaName, string LayerName)
        {

            DiffuseMaterial mat = _MediaControl.getLayerMedia(MediaName, LayerName).materialTexture as DiffuseMaterial;
            findLayer(LayerName).materials.Children.Remove(mat);
            _MediaControl.getMediaByName(MediaName).removeLayerTexture(LayerName); //removes Layers Media->MediaTexture
            return true;
        
        }

        #endregion

        #region ADD materials

        public void addMaterial(Media media, Layers Layer)
        {
            media.newTexture(Layer.layerName);
            Layer.materials.Children.Add(_MediaControl.getLayerMedia(media.mediaName, Layer.layerName).materialTexture);
            media.Play();
        }

       public void addMaterial(Media media, Layers Layer, bool defaultGrid)
       {
           Layer.materials.Children.Add(_MediaControl.getLayerMedia(media.mediaName, "allLayers").materialTexture);
           media.Play();

       }
        #endregion

        #region Alter layer

       public void renameLayer(Layers layer, string newName) //podpiac do kontrolek
       {
           layer.layerName = newName;
           _MediaControl.renameAllMediaTexturesForLayer(layer.layerName, newName);   
       }

       #endregion

       
       

    }
}
