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

            //TODO: Set Materials to static materials list in Library
        }

        #region Materials
        public ObservableCollection<MaterialViewModel> Materials
        {
            get { return (ObservableCollection<MaterialViewModel>)GetValue(MaterialsProperty); }
            set { SetValue(MaterialsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Materials.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaterialsProperty =
            DependencyProperty.Register("Materials", typeof(ObservableCollection<MaterialViewModel>), typeof(MaterialsView), new UIPropertyMetadata(new ObservableCollection<MaterialViewModel>()));
        #endregion

        #region Model
        public XbimModel Model
        {
            get { return (XbimModel)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Model.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register("Model", typeof(XbimModel), typeof(MaterialsView), new UIPropertyMetadata(null, OnModelChanged));

        private static void OnModelChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            var view = sender as MaterialsView;
            var model = args.NewValue as XbimModel;
            if (view != null)
            {
                if (model == null)
                    view.Materials.Clear();
                else
                {
                    var matCollection = new ObservableCollection<MaterialViewModel>();
                    var ifcMaterials = model.Instances.OfType<IfcMaterial>();
                    foreach (var ifcMaterial in ifcMaterials)
                    {
                        view.Materials.Add(new MaterialViewModel(ifcMaterial));
                    }
                }
            }
        }
        #endregion
    }
}
