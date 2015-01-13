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
using Xbim.XbimExtensions.Interfaces;
using BimLibrary.ViewModel;
using System.Collections.ObjectModel;
using Xbim.Ifc2x3.ExternalReferenceResource;

namespace BimLibrary.UserControls
{
    /// <summary>
    /// Interaction logic for ClassView.xaml
    /// </summary>
    public partial class ClassView : UserControl
    {
        public ClassView()
        {
            InitializeComponent();
        }


        #region Classification
        public ClassificationViewModel Classification
        {
            get { return (ClassificationViewModel)GetValue(ClassificationProperty); }
            set { SetValue(ClassificationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedClassification.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClassificationProperty =
            DependencyProperty.Register("Classification", typeof(ClassificationViewModel), typeof(ClassView), new UIPropertyMetadata(null));
        #endregion

        private void classTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Classification != null)
                Classification.OnPropertyChanged("SelectedItem");
        }


        //#region Classifications
        //public ObservableCollection<ClassificationViewModel> Classifications
        //{
        //    get { return (ObservableCollection<ClassificationViewModel>)GetValue(ClassificationsProperty); }
        //    set { SetValue(ClassificationsProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for Classifications.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ClassificationsProperty =
        //    DependencyProperty.Register("Classifications", typeof(ObservableCollection<ClassificationViewModel>), typeof(ClassView), new UIPropertyMetadata(new ObservableCollection<ClassificationViewModel>()));


        //public void LoadClassifications()
        //{
        //    if (Model == null)
        //        Classifications.Clear();
        //    else
        //    {
        //        var classifications = Model.Instances.OfType<IfcClassification>();
        //        foreach (var classification in classifications)
        //            Classifications.Add(new ClassificationViewModel(classification));
        //    }
        //}
        //#endregion


    }
}
