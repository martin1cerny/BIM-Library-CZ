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
using BimLibrary.ViewModel;

namespace BimLibrary.Windows
{
    /// <summary>
    /// Interaction logic for MaterialWindow.xaml
    /// </summary>
    public partial class MaterialWindow : Window
    {
        public MaterialWindow()
        {
            InitializeComponent();
        }



        public MaterialViewModel Material
        {
            get { return (MaterialViewModel)GetValue(MaterialProperty); }
            set { SetValue(MaterialProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Material.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaterialProperty =
            DependencyProperty.Register("Material", typeof(MaterialViewModel), typeof(MaterialWindow), new UIPropertyMetadata(null, MaterialChanged));

        private static void MaterialChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            var window = sender as MaterialWindow;
            var material = args.NewValue as MaterialViewModel;
            if (window != null)
            {
                var provider = new ObjectDataProvider();
                provider.ObjectInstance = material;
                window.DataContext = provider;
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
