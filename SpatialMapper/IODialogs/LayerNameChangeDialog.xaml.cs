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

namespace SpatialMapper.IODialogs
{
    /// <summary>
    /// Interaction logic for LayerNameChangeDialog.xaml
    /// </summary>
    public partial class LayerNameChangeDialog : Window
    {
        public delegate void LayerNameChanged(string name);
        public event LayerNameChanged nameHasChanged;
       
        public LayerNameChangeDialog(string currentName)
        {
            InitializeComponent();
            currentText.Text=currentName;
            newText.Focus();
        }   

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            string newTxt = newText.Text;
            newTxt = newTxt.Replace(" ", "");
            if (currentText.Text != newText.Text && newTxt!="")
            {
                nameHasChanged(newTxt);
                this.Close();
            }
            else
            {
                MessageBox.Show("New name cannot be equal to the old name");
            }
        }

        private void newText_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            submitButton_Click(null, null);
        }
    }
    }

