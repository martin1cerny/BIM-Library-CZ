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
                if (ctrl != null)
                    ctrl.SetComments();
            }));

        private void SetComments()
        {
            if (Person != null && Entity != null)
                SetValue(CommentsProperty, Model.Get<BLComment>(c => c._forEntityId == Entity.Id && c._issuePersonId == Person.Id).OrderBy(c => c.IssueDate));
            else
                SetValue(CommentsProperty, null);
        }

        public IEnumerable<BLComment> Comments
        {
            get { return (IEnumerable<BLComment>)GetValue(CommentsProperty); }
            set { SetValue(CommentsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Comments.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommentsProperty =
            DependencyProperty.Register("Comments", typeof(IEnumerable<BLComment>), typeof(CommentsControl), new PropertyMetadata(null));



        public BLPerson Person
        {
            get { return (BLPerson)GetValue(PersonProperty); }
            set { SetValue(PersonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Person.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PersonProperty =
            DependencyProperty.Register("Person", typeof(BLPerson), typeof(CommentsControl), new PropertyMetadata(null, (s, a) => {
                var ctrl = s as CommentsControl;
                var person = a.NewValue as BLPerson;
                if (ctrl != null)
                    ctrl.SetComments();

            }));



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
                {
                    txn.Commit();
                    cbPersons.ItemsSource = Model.Get<BLPerson>();
                    Person = person;
                }
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

            using (var txn = Model.BeginTansaction("Edit person"))
            {
                var dlg = new PersonDialog(person);
                if (dlg.ShowDialog() == true)
                    txn.Commit();
                else
                    txn.RollBack();
            }

        }

        private void btnNewComment_Click(object sender, RoutedEventArgs e)
        {
            var person = cbPersons.SelectedItem as BLPerson;
            if (person == null)
            {
                MessageBox.Show("Musíte vybrat autora připomínky");
                return;
            }
            using (var txn = Model.BeginTansaction("New comment"))
            {
                try
                {
                    var comment = Model.New<BLComment>();
                    comment.IssuedBy = person;
                    comment.IssueDate = DateTime.Now;
                    comment.State = CommentStateEnum.ISSUE;
                    comment.ForEntity = Entity;

                    var dlg = new CommentNewDialog(comment);
                    if (dlg.ShowDialog() == true)
                    {
                        SetComments();
                        txn.Commit();
                    }
                    else
                        txn.RollBack();
                }
                catch (Exception ex)
                {
                    txn.RollBack();
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnDeleteComment_Click(object sender, RoutedEventArgs e)
        {
            var comment = lbComments.SelectedItem as BLComment;
            if (comment == null)
            {
                MessageBox.Show("Musíte vybrat komentář.");
                return;
            }

            var result = MessageBox.Show("Opravdu chcete tento komentář smazat?", "Ujištění", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            if (result == MessageBoxResult.No)
                return;

            using (var txn = Model.BeginTansaction("Delete comment"))
            {
                try
                {
                    Model.Delete(comment as BLModelEntity);
                    txn.Commit();
                    SetComments();
                }
                catch (Exception ex)
                {
                    txn.RollBack();
                    MessageBox.Show("Error: " + ex.Message);
                    throw;
                }
                
            }

        }

        private void btnEditComment_Click(object sender, RoutedEventArgs e)
        {
            var comment = lbComments.SelectedItem as BLComment;
            if (comment == null)
            {
                MessageBox.Show("Musíte vybrat komentář.");
                return;
            }
            using (var txn = Model.BeginTansaction("New comment"))
            {
                try
                {
                    comment.IssueDate = DateTime.Now;

                    var dlg = new CommentNewDialog(comment);
                    if (dlg.ShowDialog() == true)
                    {
                        SetComments();
                        txn.Commit();
                    }
                    else
                        txn.RollBack();
                }
                catch (Exception ex)
                {
                    txn.RollBack();
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnResolveComment_Click(object sender, RoutedEventArgs e)
        {

        }

        
    }
}
