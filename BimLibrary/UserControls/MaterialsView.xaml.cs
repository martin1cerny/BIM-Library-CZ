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
using Xbim.IO;
using BimLibrary.ViewModel;
using System.Collections.ObjectModel;
using Xbim.Ifc2x3.MaterialResource;

namespace BimLibrary.UserControls
{
    /// <summary>
    /// Interaction logic for MaterialsView.xaml
    /// </summary>
    public partial class MaterialsView : UserControl
    {
        public MaterialsView()
        {
            InitializeComponent();

            SetValue(MaterialsProperty, App.Library.Materials);
        }

        #region Materials
        public ObservableCollection<MaterialViewModel> Materials
        {
            get { return (ObservableCollection<MaterialViewModel>)GetValue(MaterialsProperty); }
        }

        // Using a DependencyProperty as the backing store for Materials.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaterialsProperty =
            DependencyProperty.Register("Materials", typeof(ObservableCollection<MaterialViewModel>), typeof(MaterialsView), new UIPropertyMetadata(new ObservableCollection<MaterialViewModel>()));
        #endregion

    }
}
