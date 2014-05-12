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


        #region Model
        public IModel Model
        {
            get { return (IModel)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Model.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register("Model", typeof(IModel), typeof(ClassView), new UIPropertyMetadata(null, ModelChanged));

        private static void ModelChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var view = sender as ClassView;
            if (view != null)
            {
                var model = e.NewValue as IModel;
                if (model == null)
                    view.Classifications.Clear();
                else
                {
                    view.LoadClassifications();
                }
            }
        }
        #endregion


        #region SelectedClassification
        public ClassificationViewModel SelectedClassification
        {
            get { return (ClassificationViewModel)GetValue(SelectedClassificationProperty); }
            set { SetValue(SelectedClassificationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedClassification.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedClassificationProperty =
            DependencyProperty.Register("SelectedClassification", typeof(ClassificationViewModel), typeof(ClassView), new UIPropertyMetadata(null));
        #endregion



        #region Classifications
        public ObservableCollection<ClassificationViewModel> Classifications
        {
            get { return (ObservableCollection<ClassificationViewModel>)GetValue(ClassificationsProperty); }
            set { SetValue(ClassificationsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Classifications.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClassificationsProperty =
            DependencyProperty.Register("Classifications", typeof(ObservableCollection<ClassificationViewModel>), typeof(ClassView), new UIPropertyMetadata(new ObservableCollection<ClassificationViewModel>()));


        public void LoadClassifications()
        {
            if (Model == null)
                Classifications.Clear();
            else
            {
                var classifications = Model.Instances.OfType<IfcClassification>();
                foreach (var classification in classifications)
                    Classifications.Add(new ClassificationViewModel(classification));
            }
        }
        #endregion

    }
}
