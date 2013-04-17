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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using SpatialMapper.MediaController;
using System.Windows.Media.Media3D;
using System.Reflection;
//using SpatialMapper.ProjectSerializer;
using System.Timers;
using System.Windows.Threading;
using System.Threading;


namespace SpatialMapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

         #region variables

        private LayerControl _layerController;
        private MediaControl _mediaController; 
        private Media _selectedMedia;
        private Layer _selectedLayer;
        private Media _defaultGrid;
        private RenderWindow.Renderer _projectionWindow;
        private Viewport3D _viewport;
        private DispatcherTimer _videoTrackbarTimer;
        private  int _nameIterator = 0;
        private bool _editMode = false;
       
        #endregion

         #region Constructor
        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion
      
         #region WindowEvents

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string defaultGrid = AppDomain.CurrentDomain.BaseDirectory + @"Contents\grid.png";
            EventHandler _videoTrackbarUpdate = new EventHandler(_videoTrackbarOnUpdate);
            _defaultGrid = new Media(defaultGrid);
            _defaultGrid.newTexture("$allLayers$", -1); // doesn't count
            _mediaController = new MediaControl(_defaultGrid);
            _layerController = new LayerControl(_mediaController);
            _videoTrackbarTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 20), DispatcherPriority.Normal, _videoTrackbarUpdate, MediaController_Slider_Progress.Dispatcher);
            _videoTrackbarTimer.Stop();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
 {
     if (_projectionWindow != null)
     {
         _projectionWindow.Close();
     }
 }

        #endregion      

         #region ContextMenus

         private void ContextMenuMedia_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            switch (menuItem.Name)
            {
                case "MediaList_AddNewMedia":
                    {
                        addNewMedia();
                        break; 
                    }
                case "MediaList_RemoveMedia":
                    {
                        _videoTrackbarTimer.Stop();
                        if (MediaListBox.SelectedIndex != -1)
                        {
                            foreach (Layer layer in _layerController._Layers)
                            {
                                if (_selectedMedia.getMediaTextureByLayerName(layer.layerName) != null)
                                {
                                    layer.materials.Children.Remove(_selectedMedia.getMediaTextureByLayerName(layer.layerName).materialTexture);
                                }
                            }
                            MediaListBox.Items.RemoveAt(MediaListBox.SelectedIndex);
                            _mediaController.DeleteMediaSource(_selectedMedia);
                            _selectedMedia = null;
                            reloadLayersMediaList();
                        }
                        break; 
                    
                    }
                case "MediaList_RenameMedia":
                    {
                        if(_selectedMedia!=null)
                        {
                            IODialogs.MediaChangeNameDialog textio = new IODialogs.MediaChangeNameDialog(_selectedMedia.mediaName);
                            textio.nameHasChanged += new IODialogs.MediaChangeNameDialog.MediaNameChanged(textio_nameHasChanged);
                            textio.ShowDialog();
                            }
                        break;
                    }


            }
      
        }
 
         private void LayersListContext_Click(object sender, RoutedEventArgs e) //fix
        {
            MenuItem mi = (MenuItem)sender;
            switch (mi.Name)
            {
                case "LayersList_AddNewLayer":
                    {
                        addNewLayer();
                        break; 
                    }
             
                case "LayersList_RemoveLayer":
                    {
                        removeLayer();
                        break;
                    }
                case "LayersList_RenameLayer":
                    {
                        
                        if (_selectedLayer != null)
                        {
                            IODialogs.LayerNameChangeDialog changeLayerNameDialog = new IODialogs.LayerNameChangeDialog(_selectedLayer.layerName);
                            changeLayerNameDialog.nameHasChanged += new IODialogs.LayerNameChangeDialog.LayerNameChanged(changeLayerNameDialog_nameHasChanged);
                            changeLayerNameDialog.ShowDialog();
                        }
                        break;
                    }
              

            }
        }

         private void LayersMediaListContext_Click(object sender, RoutedEventArgs e)
         {
             MenuItem mi = (MenuItem)sender;
             switch (mi.Name)
             {

                 case "LayersMediaList_ApplyTexture":
                     {
                         applyTexture();
                         break;
                     }
                 case "LayersMediaList_RemoveTexture":
                     {
                         removeTexture();
                         break;
                     }
                 case "LayerMediaList_MoveTextureDown":
                     {
                         MoveTexture(false);
                         break;

                     }
                 case "LayerMediaList_MoveTextureUp":
                     {
                         MoveTexture(true);
                         break;
                     }
             }

         }


          #endregion

         #region Menu

         private void MenuEditItem_Click(object sender, RoutedEventArgs e)
         {
             MenuItem item = (MenuItem)sender;

             switch (item.Name)
             {
                 case "Edit_MenuItem_NewWindow":
                     {
                         if (_projectionWindow == null)
                         {
                             foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
                             {
                                 if (screen != System.Windows.Forms.Screen.PrimaryScreen)
                                 {
                                     _projectionWindow = new RenderWindow.Renderer(models3dgroup.Children, this);

                                     _projectionWindow.Show();
                                     _projectionWindow.Top = screen.Bounds.Top;
                                     _projectionWindow.Left = screen.Bounds.Left;
                                     _projectionWindow.WindowState = System.Windows.WindowState.Maximized;
                                     _projectionWindow.WindowStyle = System.Windows.WindowStyle.None;

                                     if (_projectionWindow != null)
                                     {
                                         this._editMode = !_editMode;

                                         _projectionWindow.editMode(_editMode, _selectedLayer);
                                     }
                                 }
                                 else
                                 {
                                     MessageBox.Show("Plug in projector in extended display mode");
                                 }
                             }

                         }
                         break;
                     }
                 case "Edit_MenuItem_HideBackGround":
                     {
                         _layerController.HideHelpers(_defaultGrid.getMediaTextureByLayerName("$allLayers$"));
                         break;
                     }
                 case "Edit_MenuItem_ShowBackGround":
                     {

                         _layerController.ShowHelpers(_defaultGrid.getMediaTextureByLayerName("$allLayers$"));
                         break;

                     }
                 case "Edit_MenuItem_HidePreview":
                     {
                         if (_viewport == null)
                         {
                             _viewport = viewport;
                         }
                         viewportStackPanel.Children.Remove(viewport);
                         break;

                     }
                 case "Edit_MenuItem_ShowPreview":
                     {
                         if (_viewport != null)
                         {
                             viewportStackPanel.Children.Add(_viewport);

                         }
                         break;
                     }
             }
         }
         private void menu_Click(object sender, RoutedEventArgs e)
         {
             MenuItem temp = (MenuItem)sender;
             switch (temp.Name)
             {
                 case "menuNewLayer":
                     addNewLayer();
                     break;
                 case "menuNewMedia":
                     addNewMedia();
                     break;
                 case "menuExit":
                     this.Close();
                     break;
             }
         }

         #endregion

         #region ListEvents

         private void LayersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
         {
             ListBox glist = (ListBox)sender;
             if (glist.SelectedIndex != -1)
             {
                 string name = glist.SelectedItem.ToString();


                 Layer layer = _layerController.findLayer(name);
                 if (layer != null)
                 {
                     _selectedLayer = layer;
                     if (_editMode && _projectionWindow != null) // jeśli w trybie edycji -> Renderer.xaml.cs -> edit(layer)
                     { _projectionWindow.editMode(_editMode, layer); }

                     reloadLayersMediaList();


                     unsubscribeLayerTransformEvents();
                     loadSliders();
                     subscribeLayerTransformEvents();
                 }
                 //fix for empty selection
             }
         }
         private void SelectedLayersMediaListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
         {

             ListBox selectedList = (sender as ListBox);


             if (selectedList.SelectedIndex != -1)
             {
                 string mediaName = selectedList.SelectedItem.ToString();

                 MediaListBox.SelectedItem = mediaName;
                 Media med = _mediaController.getMediaByName(mediaName);
                 MediaTexture mediaTexture = med.getMediaTextureByLayerName(_selectedLayer.layerName);
                 if (med != null)
                 {

                     unsubscribeMediaControllerEvents();
                     if (mediaTexture != null)
                     {
                         MediaController_Slider_Opacity.Value = mediaTexture.Opacity;
                         FillPreviewRectangle(mediaTexture);
                     }

                     reloadMediaControllers();
                 }

                 subscribeMediaControllerEvents();
             }
             else
                 unsubscribeMediaControllerEvents();

         }
         private void MediaListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
         {
             _videoTrackbarTimer.Stop();
             ListBox mlist = (ListBox)sender;
             bool isInLayersListBox = false;
             if (mlist.SelectedIndex != -1)
             {
                 string name = mlist.SelectedItem.ToString();

                 Media med = _mediaController.getMediaByName(name);

                 if (med != null)
                 {
                     _selectedMedia = med;
                     reloadMediaControllers();
                     foreach (object item in SelectedLayersMediaListBox.Items)
                     {
                         if (item.ToString() == name)
                         {
                             SelectedLayersMediaListBox.SelectedItem = item.ToString();
                             isInLayersListBox = true;
                         }
                     }
                     if (!isInLayersListBox)
                     {
                         MediaTexture medTexture = _selectedMedia.getMediaTextureByLayerName("$previewTexture$");
                         if (medTexture != null)
                         {
                             SelectedLayersMediaListBox.SelectedIndex = -1;
                             FillPreviewRectangle(medTexture);
                         }
                     }
                 }
                 selectedMediaChanged(_selectedMedia);



             }
         }


         #endregion

         #region sliders

         private void transformsliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
         {
             if (LayersListBox.SelectedIndex != -1)
             {
                 Slider temp = (sender as Slider);
                 switch (temp.Name)
                 {
                     case "sliderTopLeftX":
                         _selectedLayer.Move(temp.Value, _selectedLayer._pointsTransformed[1].Y, 1);
                         break;
                     case "sliderTopLeftY":
                         _selectedLayer.Move(_selectedLayer._pointsTransformed[1].X, temp.Value, 1);
                         break;
                     case "sliderTopRightX":
                         _selectedLayer.Move(temp.Value, _selectedLayer._pointsTransformed[3].Y, 3);
                         break;
                     case "sliderTopRightY":
                         _selectedLayer.Move(_selectedLayer._pointsTransformed[3].X, temp.Value, 3);
                         break;
                     case "sliderBottomLeftX":
                         _selectedLayer.Move(temp.Value, _selectedLayer._pointsTransformed[0].Y, 0);
                         break;
                     case "sliderBottomLeftY":
                         _selectedLayer.Move(_selectedLayer._pointsTransformed[0].X, temp.Value, 0);
                         break;
                     case "sliderBottomRightX":
                         _selectedLayer.Move(temp.Value, _selectedLayer._pointsTransformed[2].Y, 2);
                         break;
                     case "sliderBottomRightY":
                         _selectedLayer.Move(_selectedLayer._pointsTransformed[2].X, temp.Value, 2);
                         break;
                 }
             }
         }
         private void MediaController_Sliders_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
         {

             Slider slide = sender as Slider;
             switch (slide.Name)
             {
                 case "MediaController_Slider_Opacity":
                     {
                         if ((SelectedLayersMediaListBox.Items.Count > 0 && SelectedLayersMediaListBox.SelectedIndex > -1) && (LayersListBox.SelectedIndex > -1))
                         {
                             string selectedMedia = SelectedLayersMediaListBox.SelectedItem.ToString();
                             MediaTexture mediaTexture = this._mediaController.getMediaByName(selectedMedia).getMediaTextureByLayerName(_selectedLayer.layerName);

                             if (mediaTexture != null)
                                 mediaTexture.Opacity = MediaController_Slider_Opacity.Value;
                             else
                                 slide.Value = e.OldValue;

                         }
                         break;
                     }
                 case "MediaSpeedSlider":
                     {
                         if (_selectedMedia != null)
                         {
                             _selectedMedia.SpeedRatio = Math.Round(MediaSpeedSlider.Value, 1);
                         }
                         break;
                     }


             }
             e.Handled = true;
         }
         private void MediaController_Slider_Progress_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
         {
             _videoTrackbarTimer.Stop();

         }
         private void MediaController_Slider_Progress_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
         {
             if (_selectedMedia != null)
             {
                 _selectedMedia.Position = new TimeSpan(0, 0, 0, 0, (int)(sender as Slider).Value * 10);
                 _videoTrackbarTimer.Start();
             }
         }
         private void _videoTrackbarOnUpdate(object sender, EventArgs args)
         {
             MediaController_Slider_Progress.Value = _selectedMedia.Position.TotalMilliseconds / 10;
         }

        #endregion

         #region event subscribe/unsubscribe

         public void unsubscribeLayerTransformEvents()
         {
             sliderTopLeftX.ValueChanged -= transformsliderValueChanged;
             sliderTopLeftY.ValueChanged -= transformsliderValueChanged;
             sliderTopRightX.ValueChanged -= transformsliderValueChanged;
             sliderTopRightY.ValueChanged -= transformsliderValueChanged;
             sliderBottomLeftX.ValueChanged -= transformsliderValueChanged;
             sliderBottomLeftY.ValueChanged -= transformsliderValueChanged;
             sliderBottomRightX.ValueChanged -= transformsliderValueChanged;
             sliderBottomRightY.ValueChanged -= transformsliderValueChanged;

         }
         public void subscribeLayerTransformEvents()
         {
             sliderTopLeftX.ValueChanged += transformsliderValueChanged;
             sliderTopLeftY.ValueChanged += transformsliderValueChanged;
             sliderTopRightX.ValueChanged += transformsliderValueChanged;
             sliderTopRightY.ValueChanged += transformsliderValueChanged;
             sliderBottomLeftX.ValueChanged += transformsliderValueChanged;
             sliderBottomLeftY.ValueChanged += transformsliderValueChanged;
             sliderBottomRightX.ValueChanged += transformsliderValueChanged;
             sliderBottomRightY.ValueChanged += transformsliderValueChanged;

         }
         private void unsubscribeMediaControllerEvents()
         {
             MediaController_Slider_Opacity.ValueChanged -= MediaController_Sliders_ValueChanged;
             MediaSpeedSlider.ValueChanged -= MediaController_Sliders_ValueChanged;
             MediaMuteCkBox.Click -= mediaOptionsCkBox;
             MediaLoopCkBox.Click -= mediaOptionsCkBox;
             MediaPreviewCkBox.Click -= mediaOptionsCkBox;
         }
         private void subscribeMediaControllerEvents()
         {
             MediaSpeedSlider.ValueChanged += MediaController_Sliders_ValueChanged;
             MediaController_Slider_Opacity.ValueChanged += MediaController_Sliders_ValueChanged;
             MediaMuteCkBox.Click += mediaOptionsCkBox;
             MediaLoopCkBox.Click += mediaOptionsCkBox;
             MediaPreviewCkBox.Click += mediaOptionsCkBox;
         }

         #endregion

         #region controls reload
         private void reloadLayersList()
         {
             int selectedIndex = LayersListBox.SelectedIndex;
             LayersListBox.Items.Clear();
             List<Layer> layers = _layerController.getLayersList;
             foreach (Layer layer in layers)
             {
                 LayersListBox.Items.Add(layer.layerName);
             }
             LayersListBox.SelectedIndex = selectedIndex;
         }
         private void reloadLayersMediaList()
         {
             int selectedIndex = SelectedLayersMediaListBox.SelectedIndex;
             SelectedLayersMediaListBox.Items.Clear();
             if (_selectedLayer != null)
             {
                 List<MediaTexture> layerTextures = _mediaController.getMediaListForLayer(_selectedLayer.layerName, _selectedLayer.numTextures);
                 foreach (MediaTexture m in layerTextures)
                 {
                     SelectedLayersMediaListBox.Items.Add(m.parentMedia);
                 }

                 if ((SelectedLayersMediaListBox.SelectedIndex == -1) && (SelectedLayersMediaListBox.Items.Count > 0))
                 {
                     SelectedLayersMediaListBox.SelectedIndex = 0;
                 }

             }

         }
         private void reloadMediaList()
         {
             int selectedIndex = MediaListBox.SelectedIndex;
             MediaListBox.Items.Clear();
             foreach (Media m in _mediaController._mediaElements)
             {
                 if (m.mediaName != "grid.png")
                     MediaListBox.Items.Add(m.mediaName);
             }
             MediaListBox.SelectedIndex = selectedIndex;
         }
         public void loadSliders()
         {
             sliderTopLeftX.Value = _selectedLayer._pointsTransformed[1].X;
             sliderTopLeftY.Value = _selectedLayer._pointsTransformed[1].Y;
             sliderTopRightX.Value = _selectedLayer._pointsTransformed[3].X;
             sliderTopRightX.Value = _selectedLayer._pointsTransformed[3].Y;
             sliderBottomLeftX.Value = _selectedLayer._pointsTransformed[0].X;
             sliderBottomLeftY.Value = _selectedLayer._pointsTransformed[0].Y;
             sliderBottomRightX.Value = _selectedLayer._pointsTransformed[2].X;
             sliderBottomRightY.Value = _selectedLayer._pointsTransformed[2].Y;

         }
         private void reloadMediaControllers()
         {
             if (_selectedMedia != null)
             {
                 unsubscribeMediaControllerEvents();
                 MediaSpeedSlider.Value = _selectedMedia.SpeedRatio;
                 MediaMuteCkBox.IsChecked = _selectedMedia.mute;
                 MediaLoopCkBox.IsChecked = _selectedMedia.loop;
                 subscribeMediaControllerEvents();
             }
         }

         #endregion

         #region media controls events

         private void mediaOptionsCkBox(object sender, RoutedEventArgs e)
         {
             CheckBox ck = sender as CheckBox;
             switch (ck.Name)
             {
                 case "MediaMuteCkBox":
                     this._selectedMedia.mute = ck.IsChecked.Value;
                     break;
                 case "MediaLoopCkBox":
                     this._selectedMedia.loop = ck.IsChecked.Value;
                     break;
                 case "MediaPreviewCkBox":
                     if (!ck.IsChecked.Value)
                     { this.MediaController_Rectangle_VideoPreview.Fill = null; }
                     else
                     { this.MediaController_Rectangle_VideoPreview.Fill = _selectedMedia.getBrush; }
                     break;
             }
         }
         private void mediaController_PlaybackButtons(object sender, RoutedEventArgs e)
         {
             Button btn = sender as Button;

             if (_selectedMedia != null)
             {

                 switch (btn.Name)
                 {
                     case "MediaController_PlayButton":
                         this._selectedMedia.Play();
                         break;
                     case "MediaController_PauseButton":
                         if (this._selectedMedia.CanPause)
                             this._selectedMedia.Pause();
                         break;
                     case "MediaController_StopButton":
                         this._selectedMedia.Stop();
                         break;
                     case "ResetToDefault":
                         {
                             this._selectedMedia.SpeedRatio = 1;
                             this._selectedMedia.mute = false;
                             this._selectedMedia.loop = true;
                             this.MediaController_Slider_Opacity.Value = 1;
                             reloadMediaControllers();
                             break;
                         }
                 }

             }
         }

        #endregion

         #region namechanged events

       private void textio_nameHasChanged(string name)
         {
             bool nameExistsInList = false;
             foreach (object listitem in MediaListBox.Items)
             {
                 if (listitem.ToString() == name)
                     nameExistsInList = true;
             }

             if (!nameExistsInList)
             {
                 _mediaController.renameMedia(_selectedMedia.mediaName, name);
                 reloadMediaList();
                 reloadLayersMediaList();
             }
         }
       private void changeLayerNameDialog_nameHasChanged(string name)
       {
           bool nameExists = false;
           foreach (object listitem in LayersListBox.Items)
           {
               if (listitem.ToString() == name)
                   nameExists = true;
           }
           if (!nameExists)
           {
               _layerController.renameLayer(_selectedLayer, name);
           }
           reloadLayersList();
       }  

         #endregion
 
         #region Layer Operations

       private void addNewLayer()
       {

           _layerController.newLayer("layer" + _nameIterator++.ToString());
           _layerController.addMaterial(_defaultGrid, _layerController.findLayer("layer" + (_nameIterator - 1).ToString()), true);
           models3dgroup.Children.Add(_layerController._Layers[_layerController._Layers.Count - 1].Layer3DModel);
           LayersListBox.Items.Insert(0, _layerController._Layers[_layerController._Layers.Count - 1].layerName);
           LayersListBox.SelectedIndex = 0;
       }
       private void removeLayer()
       {
           if (_selectedLayer != null)
           {
               this.LayersListBox.Items.Remove(_selectedLayer.layerName);
               models3dgroup.Children.Remove(_selectedLayer.Layer3DModel);
               _layerController.removeLayer(_selectedLayer); //check
               _selectedLayer = null;

               reloadLayersMediaList();
           }
       }

       #endregion

         #region Media Operations and Events

       private void MoveTexture(bool UP)
       {
           if (SelectedLayersMediaListBox.SelectedIndex != -1)
           {

               int itemsCount = SelectedLayersMediaListBox.Items.Count;

               int selectedIndex = SelectedLayersMediaListBox.SelectedIndex;
               string selectedItem = SelectedLayersMediaListBox.SelectedItem.ToString();
               MediaTexture mtCurrent = _mediaController.getLayerMedia(selectedItem, _selectedLayer.layerName);
               if (UP)
               {
                   if (selectedIndex > 0)
                   {
                       MediaTexture mtNext = _mediaController.getLayerMedia(SelectedLayersMediaListBox.Items[selectedIndex - 1].ToString(), _selectedLayer.layerName);
                       int tempPos = mtCurrent.position;
                       mtCurrent.position = mtNext.position;
                       mtNext.position = tempPos;
                       SelectedLayersMediaListBox.Items.Remove(SelectedLayersMediaListBox.SelectedItem);
                       SelectedLayersMediaListBox.Items.Insert(selectedIndex - 1, selectedItem);
                       SelectedLayersMediaListBox.SelectedIndex = selectedIndex - 1;
                       int positionMoveTo = _selectedLayer.materials.Children.IndexOf(mtNext.materialTexture);
                       _selectedLayer.materials.Children.Remove(mtCurrent.materialTexture);
                       _selectedLayer.materials.Children.Insert(positionMoveTo, mtCurrent.materialTexture);
                       reloadLayersMediaList();
                       SelectedLayersMediaListBox.SelectedIndex = selectedIndex - 1;

                   }
               }
               else
               {
                   if (itemsCount > 1 && !(itemsCount - 1 == selectedIndex))
                   {
                       MediaTexture mtPrev = _mediaController.getLayerMedia(SelectedLayersMediaListBox.Items[selectedIndex + 1].ToString(), _selectedLayer.layerName);
                       int tempPos = mtCurrent.position;
                       mtCurrent.position = mtPrev.position;
                       mtPrev.position = tempPos;
                       int positionMoveTo = _selectedLayer.materials.Children.IndexOf(mtPrev.materialTexture);
                       _selectedLayer.materials.Children.Remove(mtCurrent.materialTexture);
                       _selectedLayer.materials.Children.Insert(positionMoveTo, mtCurrent.materialTexture);
                       reloadLayersMediaList();
                       SelectedLayersMediaListBox.SelectedIndex = selectedIndex + 1;
                   }
               }

           }
       }
       private void addNewMedia()
       {
           string[] filenames = GetFilePath();

           foreach (string file in filenames)
           {


               string filename = System.IO.Path.GetFileName(file);
               if (!MediaListBox.Items.Contains(filename))
               {

                   _selectedMedia = new Media(file);
                   _selectedMedia.MediaOpened += new RoutedEventHandler(_selectedMedia_MediaOpened);
                   _mediaController.NewMediaSource(_selectedMedia);
                   unsubscribeMediaControllerEvents();
                   MediaListBox.Items.Add(System.IO.Path.GetFileName(filename));
                   reloadMediaControllers();
                   MediaListBox.SelectedIndex = MediaListBox.Items.Count - 1;
                   subscribeMediaControllerEvents();
               }


           }
       }
       private string[] GetFilePath()
       {
           OpenFileDialog ofd = new OpenFileDialog();
           ofd.Multiselect = true;
           ofd.Filter = " Movie Files|*.avi; *.mpeg; *.mp4;*.wmv; *.mpg;| Image Files |*.jpg;*.jpeg;*.png;*.bmp";
           ofd.ShowDialog();
           return ofd.FileNames;
       }
       private void applyTexture()
       {
           bool ret = false;
           if (_selectedLayer != null && _selectedMedia != null)
           {
               if (SelectedLayersMediaListBox.Items.Count > 0)
               {

                   foreach (object listObj in SelectedLayersMediaListBox.Items)
                   {
                       if (listObj.ToString() == _selectedMedia.mediaName)
                       {
                           ret = true;
                       }
                   }
               }
               if (!ret)
               {
                   _layerController.addMaterial(_selectedMedia, _selectedLayer, _selectedLayer.numTextures);
                   reloadLayersMediaList();
               }
               else
               {
                   //już jest taki element
               }
           }
       }
       private void removeTexture()
       {
           if (_selectedLayer != null && _selectedMedia != null)
           {
               _layerController.removeTexture(_selectedMedia.mediaName, _selectedLayer.layerName);
               reloadLayersMediaList();


           }
       }
       private void _selectedMedia_MediaOpened(object sender, RoutedEventArgs e)
       {
           selectedMediaChanged(sender as Media);
       }

       private void selectedMediaChanged(Media media)
       {
           if (media.NaturalDuration.HasTimeSpan)
           {
               MediaController_Slider_Progress.Dispatcher.Invoke(new Action(delegate() { MediaController_Slider_Progress.Maximum = media.NaturalDuration.TimeSpan.TotalMilliseconds / 10; }), DispatcherPriority.Normal, null);
               _videoTrackbarTimer.Start();
           }
       }


       #endregion

         #region preview

       private void FillPreviewRectangle(MediaTexture mediaPreview)
       {
           if (MediaPreviewCkBox.IsChecked.Value)
           {
               MediaController_Rectangle_VideoPreview.Fill = mediaPreview.getBrush;
           }
       }

        #endregion


 













































    }

    
}
