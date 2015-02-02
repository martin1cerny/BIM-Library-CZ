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
using BLData;
using BLData.Classification;
using BLData.PropertySets;

namespace BLSpec.Controls
{
    /// <summary>
    /// Interaction logic for PropertySetsControl.xaml
    /// </summary>
    public partial class PropertySetsControl : UserControl
    {
        public PropertySetsControl()
        {
            InitializeComponent();
        }



        public BLClassificationItem ClassificationItem
        {
            get { return (BLClassificationItem)GetValue(ClassificationItemProperty); }
            set { SetValue(ClassificationItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ClassificationItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClassificationItemProperty =
            DependencyProperty.Register("ClassificationItem", typeof(BLClassificationItem), typeof(PropertySetsControl), new PropertyMetadata(null));




        public PropertySetDef SelectedPropertySet
        {
            get { return (PropertySetDef)GetValue(SelectedPropertySetProperty); }
            set { SetValue(SelectedPropertySetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedPropertySet.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedPropertySetProperty =
            DependencyProperty.Register("SelectedPropertySet", typeof(PropertySetDef), typeof(PropertySetsControl), new PropertyMetadata(null));

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var entity = e.AddedItems.Cast<PropertySetDef>().FirstOrDefault();
            if (entity != null)
                OnEntityActive(entity);
        }


        public event BLEntityActiveHandler EntityActive;
        private void OnEntityActive(INamedEntity entity)
        {
            if (EntityActive != null)
                EntityActive(this, new BLEntityActiveEventArgs(entity));
        }

    }
}
