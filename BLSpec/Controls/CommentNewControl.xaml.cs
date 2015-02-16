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
using BLData.Comments;

namespace BLSpec.Controls
{
    /// <summary>
    /// Interaction logic for CommentNewControl.xaml
    /// </summary>
    public partial class CommentNewControl : UserControl
    {
        public CommentNewControl()
        {
            InitializeComponent();
        }

        public BLComment Comment
        {
            get { return (BLComment)GetValue(CommentProperty); }
            set { SetValue(CommentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Comment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommentProperty =
            DependencyProperty.Register("Comment", typeof(BLComment), typeof(CommentNewControl), new PropertyMetadata(null));

    }
}
