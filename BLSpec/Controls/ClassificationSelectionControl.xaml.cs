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
    /// Interaction logic for ClassificationSelectionControl.xaml
    /// </summary>
    public partial class ClassificationSelectionControl : UserControl
    {
        public ClassificationSelectionControl()
        {
            InitializeComponent();
        }



        public BLModel Model
        {
            get { return (BLModel)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Model.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register("Model", typeof(BLModel), typeof(ClassificationSelectionControl), new PropertyMetadata(new BLModel(), (s, a) => {
                var ctrl = s as ClassificationSelectionControl;
                var val = a.NewValue as BLModel;

                if (val == null)
                    ctrl.Classifications = new List<BLClassification>();
                else
                {
                    ctrl.Classifications = val.Get<BLClassification>();
                    val.ModelEntitiesCollectionChanged += (args) => {
                        if (args.EntityType == typeof(BLClassification))
                        {
                            ctrl.Classifications = val.Get<BLClassification>();
                        }
                    };

                    //set first as visible so that user sees something has happened
                    ctrl.cmbSelectClassification.SelectedItem = ctrl.Classifications.FirstOrDefault();
                }
            }));


        
        public IEnumerable<BLClassification> Classifications
        {
            get { return (IEnumerable<BLClassification>)GetValue(ClassificationsProperty); }
            set { SetValue(ClassificationsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Classifications.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClassificationsProperty =
            DependencyProperty.Register("Classifications", typeof(IEnumerable<BLClassification>), typeof(ClassificationSelectionControl), new PropertyMetadata(new List<BLClassification>()));



        public BLClassification SelectedClassification
        {
            get { return (BLClassification)GetValue(SelectedClassificationProperty); }
            set { SetValue(SelectedClassificationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedClassification.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedClassificationProperty =
            DependencyProperty.Register("SelectedClassification", typeof(BLClassification), typeof(ClassificationSelectionControl), new PropertyMetadata(null));

        
        
    }
}
