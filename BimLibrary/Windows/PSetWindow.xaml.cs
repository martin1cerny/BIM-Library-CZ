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

namespace BimLibrary.Windows
{
    /// <summary>
    /// Interaction logic for PSetWindow.xaml
    /// </summary>
    public partial class PSetWindow : Window
    {
        public PSetWindow()
        {
            InitializeComponent();
        }



        public MetadataModel.MetaPropertySet PropertySet
        {
            get { return (MetadataModel.MetaPropertySet)GetValue(PropertySetProperty); }
            set { SetValue(PropertySetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PropertySet.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropertySetProperty =
            DependencyProperty.Register("PropertySet", typeof(MetadataModel.MetaPropertySet), typeof(PSetWindow), new UIPropertyMetadata(null, PSetChanged));

        private static void PSetChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            var win = sender as PSetWindow;
            var pSet = args.NewValue as MetadataModel.MetaPropertySet;

            if (win != null)
            {
                var provider = new ObjectDataProvider();
                provider.ObjectInstance = pSet;
                win.DataContext = provider;
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
