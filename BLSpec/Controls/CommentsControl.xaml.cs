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

        
    }
}
