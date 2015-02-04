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
using BLData.Actors;

namespace BLSpec.Controls
{
    /// <summary>
    /// Interaction logic for PersonControl.xaml
    /// </summary>
    public partial class PersonControl : UserControl
    {
        public PersonControl()
        {
            InitializeComponent();
        }



        public BLPerson Person
        {
            get { return (BLPerson)GetValue(PersonProperty); }
            set { SetValue(PersonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Person.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PersonProperty =
            DependencyProperty.Register("Person", typeof(BLPerson), typeof(PersonControl), new PropertyMetadata(null, (s, a) => {
                var ctrl = s as PersonControl;
                var val = a.NewValue as BLPerson;

                if (ctrl != null)
                    ctrl.DataContext = new ObjectDataProvider() { ObjectInstance = val };
            }));

        
    }
}
