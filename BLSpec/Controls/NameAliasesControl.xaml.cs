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

namespace BLSpec.Controls
{
    /// <summary>
    /// Interaction logic for NameAliasesControl.xaml
    /// </summary>
    public partial class NameAliasesControl : UserControl
    {
        public NameAliasesControl()
        {
            InitializeComponent();
        }


        private void SetLanguage() { 
        var code = String.IsNullOrEmpty(Lang) ? "en-us" : Lang;
            if (NameAliases != null)
                foreach (var alias in NameAliases)
                {
                    if (alias.Lang == code)
                    {
                        SetValue(ActiveNameAliasProperty, alias);
                        break;
                    }
                }
            if (DefinitionAliases != null)
                foreach (var alias in DefinitionAliases)
                {
                    if (alias.Lang == code)
                    {
                        SetValue(ActiveDefinitionAliasProperty, alias);
                        break;
                    }
                }
        }

        public string Lang
        {
            get { return (string)GetValue(LangProperty); }
            set { SetValue(LangProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Lang.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LangProperty =
            DependencyProperty.Register("Lang", typeof(string), typeof(NameAliasesControl), new PropertyMetadata("en-us", (s, a) => {
                var ctrl = s as NameAliasesControl;
                if (ctrl != null) ctrl.SetLanguage();
            }));

        

        public IEnumerable<NameAlias> NameAliases
        {
            get { return (IEnumerable<NameAlias>)GetValue(NameAliasesProperty); }
            set { SetValue(NameAliasesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NameAliases.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NameAliasesProperty =
            DependencyProperty.Register("NameAliases", typeof(IEnumerable<NameAlias>), typeof(NameAliasesControl), new PropertyMetadata(null, (s, a) =>
            {
                var ctrl = s as NameAliasesControl;
                if (ctrl != null) ctrl.SetLanguage();
            }));



        public IEnumerable<NameAlias> DefinitionAliases
        {
            get { return (IEnumerable<NameAlias>)GetValue(DefinitionAliasesProperty); }
            set { SetValue(DefinitionAliasesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DefinitionAliases.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefinitionAliasesProperty =
            DependencyProperty.Register("DefinitionAliases", typeof(IEnumerable<NameAlias>), typeof(NameAliasesControl), new PropertyMetadata(null, (s, a) =>
            {
                var ctrl = s as NameAliasesControl;
                if (ctrl != null) ctrl.SetLanguage();
            }));




        public NameAlias ActiveNameAlias
        {
            get { return (NameAlias)GetValue(ActiveNameAliasProperty); }
            set { SetValue(ActiveNameAliasProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActiveNameAlias.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActiveNameAliasProperty =
            DependencyProperty.Register("ActiveNameAlias", typeof(NameAlias), typeof(NameAliasesControl), new PropertyMetadata(null));



        public NameAlias ActiveDefinitionAlias
        {
            get { return (NameAlias)GetValue(ActiveDefinitionAliasProperty); }
            set { SetValue(ActiveDefinitionAliasProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActiveDefinitionAlias.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActiveDefinitionAliasProperty =
            DependencyProperty.Register("ActiveDefinitionAlias", typeof(NameAlias), typeof(NameAliasesControl), new PropertyMetadata(null));

        


    }
}
