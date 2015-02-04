using BLData;
using BLData.Comments;
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
using BLSpec.Dialogs;

namespace BLSpec.Controls
{
    /// <summary>
    /// Interaction logic for CommentsControl.xaml
    /// </summary>
    public partial class CommentsControl : UserControl
    {
        public CommentsControl()
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
            DependencyProperty.Register("Model", typeof(BLModel), typeof(CommentsControl), new PropertyMetadata(null, (s, a) => {
                var ctrl = s as CommentsControl;
                var val = a.NewValue as BLModel;

                if (val != null)
                {
                    var dict = val.EntityDictionary.Where(e => e.Type == typeof(BLPerson).FullName);
                    if (dict == null)
                        val.EntityDictionary.Add(new BLEntityList(val) { Type = typeof(BLPerson).FullName, Items = new BList<BLModelEntity>()});
                }

                if (ctrl != null && val != null)
                    ctrl.cbPersons.ItemsSource = val.Get<BLPerson>();
                if (ctrl != null && val == null)
                    ctrl.cbPersons.ItemsSource = null;


            }));

        

        public BLEntity Entity
        {
            get { return (BLEntity)GetValue(EntityProperty); }
            set { SetValue(EntityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Entity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EntityProperty =
            DependencyProperty.Register("Entity", typeof(BLEntity), typeof(CommentsControl), new PropertyMetadata(null, (s, a) => {
                var ctrl = s as CommentsControl;
                var value = a.NewValue as BLEntity;
                if (value == null)
                    ctrl.SetValue(CommentsProperty, null);
                else
                {
                    var comments = value.Model.Get<BLComment>(c => c._forEntityId == value.Id);
                    ctrl.SetValue(CommentsProperty, comments);
                }
            }));

        

        public IEnumerable<BLComment> Comments
        {
            get { return (IEnumerable<BLComment>)GetValue(CommentsProperty); }
            set { SetValue(CommentsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Comments.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommentsProperty =
            DependencyProperty.Register("Comments", typeof(IEnumerable<BLComment>), typeof(CommentsControl), new PropertyMetadata(null));

        private void btnAddPerson_Click(object sender, RoutedEventArgs e)
        {
            if (Model == null) 
            {
                MessageBox.Show("Model not set to comments control.");
                return;
            }
            using (var txn = Model.BeginTansaction("New person"))
            {
                var person = Model.New<BLPerson>();
                var dlg = new PersonDialog(person);
                if (dlg.ShowDialog() == true)
                    txn.Commit();
                else
                    txn.RollBack();
            }
        }

        private void btnEditPerson_Click(object sender, RoutedEventArgs e)
        {
            if (Model == null)
            {
                MessageBox.Show("Model not set to comments control.");
                return;
            }

            var person = cbPersons.SelectedItem as BLPerson;
            if (person == null)
            {
                MessageBox.Show("No user selected");
                return;
            }

            using (var txn = Model.BeginTansaction("New person"))
            {
                var dlg = new PersonDialog(person);
                if (dlg.ShowDialog() == true)
                    txn.Commit();
                else
                    txn.RollBack();
            }

        }

        
    }
}
