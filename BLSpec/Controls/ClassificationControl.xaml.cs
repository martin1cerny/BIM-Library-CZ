using BLData;
using BLData.Classification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BLSpec.Controls
{
    /// <summary>
    /// Interaction logic for ClassificationControl.xaml
    /// </summary>
    public partial class ClassificationControl : UserControl
    {
        public ClassificationControl()
        {
            InitializeComponent();
        }

        public BLClassification Classification
        {
            get { return (BLClassification)GetValue(ClassificationProperty); }
            set { SetValue(ClassificationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Classification.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClassificationProperty =
            DependencyProperty.Register("Classification", typeof(BLClassification), typeof(ClassificationControl), new PropertyMetadata(new BLClassification()));

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SetValue(SelectedItemProperty, e.NewValue);
            OnEntityActive(e.NewValue as BLClassificationItem);
        }


        public BLClassificationItem SelectedItem
        {
            get { return (BLClassificationItem)GetValue(SelectedItemProperty); }
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(BLClassificationItem), typeof(ClassificationControl), new PropertyMetadata(null));


        public event BLEntityActiveHandler EntityActive;
        private void OnEntityActive(INamedEntity entity)
        {
            if (EntityActive != null)
                EntityActive(this, new BLEntityActiveEventArgs(entity));
        }

        private void TreeView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OnEntityActive(twClassification.SelectedItem as INamedEntity);
        }
    }
}
