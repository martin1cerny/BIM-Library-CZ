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
using Xbim.XbimExtensions.Interfaces;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.Ifc2x3.MeasureResource;

namespace BimLibrary.UserControls
{
    /// <summary>
    /// Interaction logic for MetaPropertiesView.xaml
    /// </summary>
    public partial class MetaPropertiesView : UserControl
    {
        public MetaPropertiesView()
        {
            InitializeComponent();
        }


        #region Classification
        public ClassificationItemViewModel ClassificationItem
        {
            get { return (ClassificationItemViewModel)GetValue(ClassificationProperty); }
            set { SetValue(ClassificationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Classification.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClassificationProperty =
            DependencyProperty.Register("ClassificationItem", typeof(ClassificationItemViewModel), typeof(MetaPropertiesView), new UIPropertyMetadata(null));

        #endregion

    }

    public class PropertyTypes : List<Type>
    {
        public PropertyTypes()
        {
            var assembly = typeof(IfcLabel).Assembly;
            var types = assembly.GetTypes().Where(t => typeof(IfcValue).IsAssignableFrom(t));
            AddRange(types);
        }
    }
}
