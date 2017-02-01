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


        private void SetLanguage() 
        {
            var secondaryCode = String.IsNullOrEmpty(SecondaryLanguage) ? "en-US" : SecondaryLanguage;
            var primaryCode = String.IsNullOrEmpty(PrimaryLanguage) ? "en-US" : PrimaryLanguage;
            if (NameAliases != null)
            {
                var alias = NameAliases.FirstOrDefault(a => a.Lang == secondaryCode);
                SetValue(ActiveNameAliasProperty, alias);

                var name = NameAliases.FirstOrDefault(a => a.Lang == primaryCode);
                SetValue(ActiveNameProperty, name);

                if (name == null || string.IsNullOrWhiteSpace(name.Value))
                    grdColumnLeft.Width = new GridLength(0);

                if (alias == null || string.IsNullOrWhiteSpace(alias.Value))
                    grdColumnRight.Width = new GridLength(0);
            }
            if (DefinitionAliases != null)
            {
                var alias = DefinitionAliases.FirstOrDefault(a => a.Lang == secondaryCode);
                SetValue(ActiveDefinitionAliasProperty, alias);

                var definition = DefinitionAliases.FirstOrDefault(a => a.Lang == primaryCode);
                SetValue(ActiveDefinitionProperty, definition);
            }
        }



        public string PrimaryLanguage
        {
            get { return (string)GetValue(PrimaryLanguageProperty); }
            set { SetValue(PrimaryLanguageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PrimaryLanguage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrimaryLanguageProperty =
            DependencyProperty.Register("PrimaryLanguage", typeof(string), typeof(NameAliasesControl), new PropertyMetadata("en-US", (s, a) =>
                {
                    var ctrl = s as NameAliasesControl;
                    if (ctrl != null) ctrl.SetLanguage();
                }));



        public string SecondaryLanguage
        {
            get { return (string)GetValue(SecondaryLanguageProperty); }
            set { SetValue(SecondaryLanguageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Lang.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SecondaryLanguageProperty =
            DependencyProperty.Register("SecondaryLanguage", typeof(string), typeof(NameAliasesControl), new PropertyMetadata("en-US", (s, a) =>
            {
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




        public NameAlias ActiveName
        {
            get { return (NameAlias)GetValue(ActiveNameProperty); }
            set { SetValue(ActiveNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActiveName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActiveNameProperty =
            DependencyProperty.Register("ActiveName", typeof(NameAlias), typeof(NameAliasesControl), new PropertyMetadata(null));




        public NameAlias ActiveDefinition
        {
            get { return (NameAlias)GetValue(ActiveDefinitionProperty); }
            set { SetValue(ActiveDefinitionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActiveDefinition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActiveDefinitionProperty =
            DependencyProperty.Register("ActiveDefinition", typeof(NameAlias), typeof(NameAliasesControl), new PropertyMetadata(null));



    }
}
