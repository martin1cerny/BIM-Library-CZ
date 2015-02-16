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
using BLData.Comments;

namespace BLSpec.Dialogs
{
    /// <summary>
    /// Interaction logic for CommentNewDialog.xaml
    /// </summary>
    public partial class CommentNewDialog : Window
    {
        BLComment _commentToSet;
        public CommentNewDialog(BLComment comment)
        {
            _commentToSet = comment;
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            SetValue(CommentProperty, _commentToSet);
            base.OnInitialized(e);
        }

        public BLComment Comment
        {
            get { return (BLComment)GetValue(CommentProperty); }
            set { SetValue(CommentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Comment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommentProperty =
            DependencyProperty.Register("Comment", typeof(BLComment), typeof(CommentNewDialog), new PropertyMetadata(null));

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(Comment.Issue))
            {
                MessageBox.Show(this, "Musíte zadat komentář");
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

        
    }
}
