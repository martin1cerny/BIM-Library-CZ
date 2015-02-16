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
using BLData.PropertySets;
using BLData;

namespace BLSpec.Controls
{
    /// <summary>
    /// Interaction logic for PropertiesControl.xaml
    /// </summary>
    public partial class PropertiesControl : UserControl
    {
        public PropertiesControl()
        {
            InitializeComponent();
        }



        public QuantityPropertyDef SelectedProperty
        {
            get { return (QuantityPropertyDef)GetValue(SelectedPropertyProperty); }
            set { SetValue(SelectedPropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedPropertyProperty =
            DependencyProperty.Register("SelectedProperty", typeof(QuantityPropertyDef), typeof(PropertiesControl), new PropertyMetadata(null));




        public QuantityPropertySetDef PropertySet
        {
            get { return (QuantityPropertySetDef)GetValue(PropertySetProperty); }
            set { SetValue(PropertySetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PropertySet.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropertySetProperty =
            DependencyProperty.Register("PropertySet", typeof(QuantityPropertySetDef), typeof(PropertiesControl), new PropertyMetadata(null));

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var entity = e.AddedItems.Cast<QuantityPropertyDef>().FirstOrDefault();
            if (entity != null)
                OnEntityActive(entity);
        }

        public event BLEntityActiveHandler EntityActive;
        private void OnEntityActive(INamedEntity entity)
        {
            if (EntityActive != null)
                EntityActive(this, new BLEntityActiveEventArgs(entity));
        }

        private void dgProperties_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OnEntityActive(dgProperties.SelectedItem as INamedEntity);
        }
        
    }
}
