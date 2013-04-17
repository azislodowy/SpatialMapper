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
    class LayerControl
    {
        #region variables
        public List<Layer> _Layers;
        private int index = 0;
        private MediaControl _MediaControl;
        #endregion

        #region SORTING

        private static int CompareLayersList(Layer layer1, Layer layer2)
        {
            if (layer1.position < layer2.position)
                return 1;
            else if (layer1.position > layer2.position)
                return -1;
            else return 0;
        }

        #endregion

        #region constructor

        public LayerControl(MediaControl mediaController)
        {
            _MediaControl = mediaController;
            _Layers = new List<Layer>();
        
        }

        #endregion   

        #region Creation Layers Media // MOVE NEW MEDIA SOMEWHERE ELSE

        public void newLayer(string name)
        {
            Layer Layer = new Layer(name, index++);
            _Layers.Add(Layer);
        }

        #endregion

        #region FIND ELEMENTS LAYERS/MEDIA

        public List<Layer> getLayersList
        {
            get
            {
                _Layers.Sort(CompareLayersList);
                return _Layers;
            }

        }

        public Layer findLayer(string byName)
        {
            foreach (Layer geo in _Layers)
            {
                if (geo.layerName == byName)
                {
                    return geo;
                }
            }
            return null;
        }

        #endregion

        #region GRIDHELPER

        public void HideHelpers(MediaTexture helper)
        {
            foreach (Layer layer in _Layers)
            {
                layer.hideBackGroundHelper(helper.materialTexture);
            }
        }

        public void ShowHelpers(MediaTexture helper)
        {
            foreach (Layer layer in _Layers)
            {
                layer.showBackGroundHelper(helper.materialTexture);
            }

        }

        #endregion

        #region DELETE/REMOVAL layer&textures

        public bool removeLayer(Layer _Layer) //removes layer and all underlying textures
        {
            _MediaControl.deleteAllLayersTextures(_Layer.layerName);
            return this._Layers.Remove(_Layer);
        }

        public bool removeTexture(string MediaName, string LayerName)
        {
            MediaTexture tempMediaTexture = _MediaControl.getLayerMedia(MediaName, LayerName);
            if (tempMediaTexture != null)
            {
                DiffuseMaterial mat = tempMediaTexture.materialTexture as DiffuseMaterial;
                findLayer(LayerName).materials.Children.Remove(mat);
                _MediaControl.getMediaByName(MediaName).removeLayerTexture(LayerName);
                return true;
            }
            else
                return false;
        
        }

        #endregion

        #region ADD materials

        public void addMaterial(Media media, Layer Layer, int pos)
        {
            media.newTexture(Layer.layerName, pos);
            
            Layer.addMaterial(_MediaControl.getLayerMedia(media.mediaName, Layer.layerName));
            media.Play();
        }

       public void addMaterial(Media media, Layer Layer, bool defaultGrid)
       {
           Layer.addMaterial(_MediaControl.getLayerMedia(media.mediaName, "$allLayers$"));
           media.Play();

       }
        #endregion

        #region Alter layer

       public void renameLayer(Layer layer, string newName)
       {
           _MediaControl.renameAllMediaTexturesForLayer(layer.layerName, newName);
           layer.layerName = newName;
       }

       #endregion

       
       

    }
}
