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
using BimLibrary.ViewModel;

namespace BimLibrary.UserControls
{
    /// <summary>
    /// Interaction logic for PropertySetsView.xaml
    /// </summary>
    public partial class PropertySetsView : UserControl
    {
        public PropertySetsView()
        {
            InitializeComponent();
        }



        public ElementTypeViewModel Element
        {
            get { return (ElementTypeViewModel)GetValue(ElementProperty); }
            set { SetValue(ElementProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Element.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ElementProperty =
            DependencyProperty.Register("Element", typeof(ElementTypeViewModel), typeof(PropertySetsView), new UIPropertyMetadata(null, OnElementChanged));


        private static void OnElementChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            var view = sender as PropertySetsView;
            var element = args.NewValue as ElementTypeViewModel;
            if (view != null)
            {
                var provider = new ObjectDataProvider();
                provider.ObjectInstance = element != null ? element.PropertySets : null;
                view.lvPSets.DataContext = provider;
            }
        }



        public MaterialViewModel Material
        {
            get { return (MaterialViewModel)GetValue(MaterialProperty); }
            set { SetValue(MaterialProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Material.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaterialProperty =
            DependencyProperty.Register("Material", typeof(MaterialViewModel), typeof(PropertySetsView), new UIPropertyMetadata(null, OnMaterialChanged));

        private static void OnMaterialChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            var view = sender as PropertySetsView;
            var material = args.NewValue as MaterialViewModel;
            if (view != null)
            {
                var provider = new ObjectDataProvider();
                provider.ObjectInstance = material != null ? material.PropertySets : null;
                view.lvPSets.DataContext = provider;
            }
        }

    }
}
