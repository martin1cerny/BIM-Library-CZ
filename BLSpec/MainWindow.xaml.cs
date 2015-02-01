using System;
using System.Collections.Generic;
using System.IO.Compression;
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
using Microsoft.Win32;
using BLData.Comments;

namespace BLSpec
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public BLModel Model
        {
            get { return (BLModel)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Model.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register("Model", typeof(BLModel), typeof(MainWindow), new PropertyMetadata(new BLModel()));



        public string Lang
        {
            get { return (string)GetValue(LangProperty); }
            set { SetValue(LangProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Lang.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LangProperty =
            DependencyProperty.Register("Lang", typeof(string), typeof(MainWindow), new PropertyMetadata("en-US"));

        

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            var location = GetType().Assembly.Location;
            location = System.IO.Path.GetDirectoryName(location);
            var plugLoc = System.IO.Path.Combine(location, "Plugins");
            if (System.IO.Directory.Exists(plugLoc))
            {
                var files = System.IO.Directory.EnumerateFiles(plugLoc, "*.dll", System.IO.SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    var assembly = System.Reflection.Assembly.LoadFrom(file);
                    var commands = assembly.GetTypes().Where(t => typeof(IExternalCommand).IsAssignableFrom(t));
                    foreach (var c in commands)
                    {
                        var command = Activator.CreateInstance(c) as IExternalCommand;
                        RegisterPlugin(command);
                    }
                }
            }
            
            //register built-in plugins
            var builtInComands = GetType().Assembly.GetTypes().Where(t => typeof(IExternalCommand).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
            foreach (var t in builtInComands)
            {
                var command = Activator.CreateInstance(t) as IExternalCommand;
                RegisterPlugin(command);
            }
        }

        private Dictionary<Guid, IExternalCommand> _externalCommands = new Dictionary<Guid,IExternalCommand>();
        private void RegisterPlugin(IExternalCommand command)
        {
            var name = command.Name;
            var firstLevel = false;
            if (name.StartsWith("..")) firstLevel = true;
            var path = name.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (!path.Any())
            {
                MessageBox.Show("Attempted to load plug-in with no name specified.");
                return;
            }

            if (_externalCommands.Keys.Contains(command.ID))
            {
                MessageBox.Show("Plug-in with the same ID has been loaded already. Name: " + name);
                return;
            }

            _externalCommands.Add(command.ID, command);

            var menuItem = firstLevel ? menu as ItemsControl : Plugins as ItemsControl;
            foreach (var part in path)
            {
                var item = menuItem.Items.OfType<MenuItem>().FirstOrDefault(mi => (mi.Header as String) == part);
                if (item == null)
                {
                    item = new MenuItem() { Header = part };
                    menuItem.Items.Add(item);
                }
                menuItem = item;
            }
            if (menuItem is MenuItem)
                (menuItem as MenuItem).Click += (s, a) =>
                {
                    ExecutePlugin(command.ID);
                };
        }

        private void ExecutePlugin(Guid id)
        {
            var command = _externalCommands[id];
            var restorePoint = Model.Session.GetRestorePoint();
            try
            {
                command.Execute(Model, new UIHelper(this));
            }
            catch (Exception e)
            {
                //reverse any changes made by the plugin
                Model.Session.RestoreToPoint(restorePoint);
                MessageBox.Show("Plugin execution failed: \n" + e.Message, "Plug-in error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void miAbout_Click(object sender, RoutedEventArgs e)
        {
            var win = new About() { Owner = this};
            win.ShowDialog();
        }

        private string _path;

        private void miOpen_Click(object sender, RoutedEventArgs e)
        {
            miCloseModel_Click(sender, e);
            var dlg = new OpenFileDialog()
            {
                AddExtension = true,
                DefaultExt = ".bls",
                Filter = "BIM specification|*.bls|Compressed BIM specification|*.blsx",
                Title = "Otevřít BIM specifikaci...",
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false,
                ShowReadOnly = false,
            };

            if (dlg.ShowDialog(this) == true)
                Open(dlg.FileName);
        }

        private void miSave_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(_path))
                Save(_path);
            else
                miSaveAs_Click(sender, e);
        }

        private void miSaveAs_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog()
            {
                AddExtension = true,
                DefaultExt = ".bls",
                Filter = "BIM specification|*.bls|Compressed BIM specification|*.blsx",
                OverwritePrompt = true,
                Title = "Uložit BIM specifikaci..."
            };

            if (dlg.ShowDialog(this) == true)
                Save(dlg.FileName);
        }

        private void Save(string path)
        {
            if (String.IsNullOrEmpty(path)) return;
            using (var stream = System.IO.File.Create(path))
            {
                if (System.IO.Path.GetExtension(path).ToLower() == ".blsx")
                {
                    //save as compressed zip file
                    using (var archive = new ZipArchive(stream, ZipArchiveMode.Create))
                    {
                        var entry = archive.CreateEntry("specification.xml");
                        using (var entryStream = entry.Open())
                        {
                            Model.Save(entryStream);
                            entryStream.Close();
                        }
                    }
                    stream.Close();
                }
                else
                {
                    Model.Save(stream);
                    stream.Close();
                }
            }
            _path = path;
        }

        private void Open(string path)
        {
            if (String.IsNullOrEmpty(path)) return;
            using (var stream = System.IO.File.Open(path, System.IO.FileMode.Open))
            {
                if (System.IO.Path.GetExtension(path).ToLower() == ".blsx")
                {
                    //save as compressed zip file
                    using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
                    {
                        var entry = archive.Entries.FirstOrDefault(e => e.Name == "specification.xml");
                        using (var entryStream = entry.Open())
                        {
                            Model = BLModel.Open(entryStream);
                            entryStream.Close();
                        }
                    }
                    stream.Close();
                }
                else
                {
                    Model = BLModel.Open(stream);
                    stream.Close();
                }
            }
            _path = path;
        }

        private void miCloseModel_Click(object sender, RoutedEventArgs e)
        {
            //there are changes to be saved
            if (Model.Session.IsDirty)
            {
                if (MessageBox.Show("Chcete uložit poslední změny?", "Dotaz", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                    miSave_Click(sender, e);
            }
            Model = new BLModel();
            _path = null;
        }

        private void miExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            //close model first
            miCloseModel_Click(null, null);
            base.OnClosing(e);
        }

        private void EntityActiveHandler(object sender, BLEntityActiveEventArgs args)
        {
            var entity = args.Entity;
            if (entity == null)
                return;

            aliasesControl.NameAliases = args.NameAliases;
            aliasesControl.DefinitionAliases = args.DefinitionAliases;

            //set entity for comments
            var comments = entity.Model.Get<BLComment>(c => c._forEntityId == entity.Id);

        }
    }
}
