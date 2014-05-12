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
using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.ExternalReferenceResource;

namespace BimLibrary.Windows
{
    /// <summary>
    /// Interaction logic for ClassificationWindow.xaml
    /// </summary>
    public partial class ClassificationWindow : Window
    {
        public ClassificationWindow()
        {
            InitializeComponent();
        }




        public IModel Model
        {
            get { return (IModel)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Model.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register("Model", typeof(IModel), typeof(ClassificationWindow), new UIPropertyMetadata(null));

        


        public bool New
        {
            get { return (bool)GetValue(NewProperty); }
            set { SetValue(NewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for New.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NewProperty =
            DependencyProperty.Register("New", typeof(bool), typeof(ClassificationWindow), new UIPropertyMetadata(false));



        public ClassificationViewModel Classification
        {
            get { return (ClassificationViewModel)GetValue(ClassificationProperty); }
            set { SetValue(ClassificationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Classification.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClassificationProperty =
            DependencyProperty.Register("Classification", typeof(ClassificationViewModel), typeof(ClassificationWindow), new UIPropertyMetadata(null));

        private static void ClassificationChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            var window = sender as ClassificationWindow;
            var classification = args.NewValue as ClassificationViewModel;
            if (window != null)
            {
                var provider = new ObjectDataProvider();
                provider.ObjectInstance = classification;
                window.DataContext = provider;
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (New && Model != null)
            {
                var classifications = Model.Instances.Where<IfcClassification>(c => c.Name == txtClassificationName.Text);
                //there shouldn't be any classification with the same name
                if (classifications.Any())
                {
                    DialogResult = false;
                    throw new Exception("There shouldn't be more than one classification of the same name.");
                }
                else
                {
                    DialogResult = true;
                    Close();
                }

            }

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
