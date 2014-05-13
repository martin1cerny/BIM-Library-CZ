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
    /// Interaction logic for ClassificationItemWindow.xaml
    /// </summary>
    public partial class ClassificationItemWindow : Window
    {
        public ClassificationItemWindow()
        {
            InitializeComponent();
        }


        #region Classification Item
        public ClassificationItemViewModel ClassificationItem
        {   
            get { return (ClassificationItemViewModel)GetValue(ClassificationItemProperty); }
            set { SetValue(ClassificationItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ClassificationItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClassificationItemProperty =
            DependencyProperty.Register("ClassificationItem", typeof(ClassificationItemViewModel), typeof(ClassificationItemWindow), new UIPropertyMetadata(null, ClassificationItemChanged));

        private static void ClassificationItemChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            var win = sender as ClassificationItemWindow;
            var item = args.NewValue as ClassificationItemViewModel;
            if (win != null)
            {
                var provider = new ObjectDataProvider();
                provider.ObjectInstance = item;
                win.DataContext = provider;
            }
        }
        #endregion

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
