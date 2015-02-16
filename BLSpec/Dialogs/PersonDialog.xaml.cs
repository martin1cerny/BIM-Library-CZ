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
using System.Windows.Shapes;
using BLData;
using BLData.Actors;

namespace BLSpec.Dialogs
{
    /// <summary>
    /// Interaction logic for PersonDialog.xaml
    /// </summary>
    public partial class PersonDialog : Window
    {
        private BLPerson _person;
        private Transaction _txn;
        public PersonDialog(BLPerson person)
        {
            if (person == null) throw new ArgumentNullException("person","You have to specify the person.");
            _person = person;
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            ctrlPerson.Person = _person;
            if (!_person.Model.IsTransacting)
                _txn = _person.Model.BeginTansaction("Editing person information");
            base.OnInitialized(e);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            //check if email is defined
            if (String.IsNullOrEmpty(_person.Email))
            {
                MessageBox.Show(this, "Email musí být vyplněný.");
                return;
            }

            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            if (DialogResult != true && _txn != null)
                _txn.RollBack();
            if (DialogResult == true && _txn != null)
                _txn.Commit();

            base.OnClosed(e);
        }
    }
}
