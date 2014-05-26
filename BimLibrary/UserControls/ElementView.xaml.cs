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
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.Ifc2x3.ProductExtension;

namespace BimLibrary.UserControls
{
    /// <summary>
    /// Interaction logic for ElementView.xaml
    /// </summary>
    public partial class ElementView : UserControl
    {
        public ElementView()
        {
            InitializeComponent();
        }
    }

    public class ElementTypes : List<Type>
    {
        public ElementTypes()
        {
            var assembly = typeof(IfcElementType).Assembly;
            var types = assembly.GetTypes().Where(t => typeof(IfcElementType).IsAssignableFrom(t));
            AddRange(types);
        }
    }
}
