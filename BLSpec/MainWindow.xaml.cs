using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BLData;
using BLData.Comments;
using Microsoft.Win32;

namespace BLSpec
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        public BLModel Model
        {
            get { return (BLModel)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Model.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register("Model", typeof(BLModel), typeof(MainWindow), new PropertyMetadata(new BLModel()));



        public string SecondaryLanguage
        {
            get { return (string)GetValue(SecondaryLanguageProperty); }
            set { SetValue(SecondaryLanguageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SecondaryLanguage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SecondaryLanguageProperty =
            DependencyProperty.Register("SecondaryLanguage", typeof(string), typeof(MainWindow), new PropertyMetadata("en-US"));

        

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

#if !DEBUG
            //Show TACR branding for a sec
            Thread.Sleep(2000);
#endif
            //load plugins from the subfolder
            var location = GetType().Assembly.Location;
            location = Path.GetDirectoryName(location);
            if (location != null)
            {
                var plugLoc = Path.Combine(location, "Plugins");
                if (Directory.Exists(plugLoc))
                {
                    var files = Directory.EnumerateFiles(plugLoc, "*.dll", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        var assembly = Assembly.LoadFrom(file);
                        var commands = assembly.GetTypes().Where(t => typeof(IExternalCommand).IsAssignableFrom(t));
                        foreach (var command in commands.Select(c => Activator.CreateInstance(c) as IExternalCommand))
                        {
                            RegisterPlugin(command);
                        }
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

            if (!_pluginsExist) Plugins.Visibility = Visibility.Collapsed;

            //load model from command line if specified
            var allowedExtensions = new[] { ".blsx", ".bls"};
            if (!string.IsNullOrEmpty(App.arg) && System.IO.File.Exists(App.arg))
            {
                var ext = Path.GetExtension(App.arg).ToLower();
                if (allowedExtensions.Contains(ext))
                    try
                    {
                        Open(App.arg);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(this, "Neplatný formát souboru");
                        Model = new BLModel();
                    }
                return;
            }

            //try to load default model from install dir
            var actualPath = Assembly.GetExecutingAssembly().CodeBase;
            if (string.IsNullOrWhiteSpace(actualPath))
                return;
            actualPath = (new Uri(actualPath)).LocalPath;

            var actualDir = Path.GetDirectoryName(actualPath);
            if (actualDir == null) return;

            var defaultFile = Directory.EnumerateFiles(actualDir, "*.blsx", SearchOption.TopDirectoryOnly).FirstOrDefault() ??
                              Directory.EnumerateFiles(actualDir, "*.bls", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (defaultFile != null)
            {
                Open(defaultFile);
                //set path to null so that if user wants to save the data he has to choose the name and location
                _path = null;
            }
        }

        private readonly Dictionary<Guid, IExternalCommand> _externalCommands = new Dictionary<Guid,IExternalCommand>();
        private bool _pluginsExist;
        private void RegisterPlugin(IExternalCommand command)
        {
            var name = command.Name;
            var firstLevel = name.StartsWith("..");
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

            var menuItem = firstLevel ? menu : Plugins as ItemsControl;
            foreach (var part in path)
            {
                var item = menuItem.Items.OfType<MenuItem>().FirstOrDefault(mi => (mi.Header as string) == part);
                if (item == null)
                {
                    item = new MenuItem { Header = part };
                    menuItem.Items.Add(item);
                }
                menuItem = item;
            }
            var mItem = menuItem as MenuItem;
            if (mItem != null)
            {
                _pluginsExist = true;
                mItem.Click += (s, a) =>
                {
                    ExecutePlugin(command.ID);
                };}
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
            var win = new About { Owner = this};
            win.ShowDialog();
        }

        private string _path;

        private void miOpen_Click(object sender, RoutedEventArgs e)
        {
            miCloseModel_Click(sender, e);
            var dlg = new OpenFileDialog
            {
                AddExtension = true,
#if DEBUG
                DefaultExt = ".bls",
                Filter = "BIM specification|*.bls|Compressed BIM specification|*.blsx",
#else
                DefaultExt = ".blsx",
                Filter = "BIM specification|*.blsx",
#endif
                Title = "Otevřít BIM specifikaci...",
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false,
                ShowReadOnly = false
            };

            if (dlg.ShowDialog(this) == true)
                Open(dlg.FileName);
        }

        private void miSave_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_path))
                Save(_path);
            else
                miSaveAs_Click(sender, e);
        }

        // ReSharper disable UnusedParameter.Local
        private void miSaveAs_Click(object sender, RoutedEventArgs e)
        // ReSharper restore UnusedParameter.Local
        {
            var dlg = new SaveFileDialog
            {
                AddExtension = true,
#if DEBUG
                DefaultExt = ".bls",
                Filter = "BIM specification|*.bls|Compressed BIM specification|*.blsx",
#else
                DefaultExt = ".blsx",
                Filter = "BIM specification|*.blsx",
#endif
                OverwritePrompt = true,
                Title = "Uložit BIM specifikaci..."
            };

            if (dlg.ShowDialog(this) == true)
                Save(dlg.FileName);
        }

        private void Save(string path)
        {
            if (string.IsNullOrEmpty(path)) return;
            using (var stream = System.IO.File.Create(path))
            {
                if (Path.GetExtension(path).ToLower() == ".blsx")
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
            if (string.IsNullOrEmpty(path)) return;
            using (var stream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read))
            {
                if (Path.GetExtension(path).ToLower() == ".blsx")
                {
                    //save as compressed zip file
                    using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
                    {
                        var entry = archive.Entries.FirstOrDefault(e => e.Name == "specification.xml");
                        if (entry != null)
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

        protected override void OnClosing(CancelEventArgs e)
        {
            //close model first
            miCloseModel_Click(null, null);
            base.OnClosing(e);
        }

        private void EntityActiveHandler(object sender, BLEntityActiveEventArgs args)
        {
            var entity = args.Entity;
            if (entity == null)
            {
                aliasesControl.NameAliases = null;
                aliasesControl.DefinitionAliases = null;
                commentsControl.Entity = null;

                txtSelectedPath.Text = null;
                return;
            } 

            aliasesControl.NameAliases = args.NameAliases;
            aliasesControl.DefinitionAliases = args.DefinitionAliases;
            commentsControl.Entity  = entity as BLEntity;

            var cls = entity as BLData.Classification.BLClassificationItem;
            if (cls != null)
                txtSelectedPath.Text = cls.Name;

            var qpset = entity as BLData.PropertySets.QuantityPropertySetDef;
            if (qpset != null)
                txtSelectedPath.Text = qpset.Name;

            var qp = entity as BLData.PropertySets.QuantityPropertyDef;
            if (qp != null)
            {
                var set = Model.Get<BLData.PropertySets.QuantityPropertySetDef>(s => s.Definitions.Contains(qp)).FirstOrDefault();
                txtSelectedPath.Text = set.Name + "." + qp.Name;
            }
        }

        private void miExportAllComments_Click(object sender, RoutedEventArgs e)
        {
            var exchange = new CommentsExchangeModel();
            exchange.LoadFromModel(Model);
            var dlg = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = ".blcx",
                Filter = "Komentáře|*.blcx",
                OverwritePrompt = true,
                Title = "Uložit komentáře..."
            };

            if (dlg.ShowDialog(this) == true)
                exchange.SaveAs(dlg.FileName);

        }

        private void miExportUsersComments_Click(object sender, RoutedEventArgs e)
        {
            var person = commentsControl.Person;
            if (person == null)
            {
                MessageBox.Show(this, "Není vybrán uživatel.");
                return;
            }

            var exchange = new CommentsExchangeModel();
            exchange.LoadFromModel(Model, person);
            var dlg = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = ".blcx",
                Filter = "Komentáře|*.blcx",
                OverwritePrompt = true,
                Title = "Uložit komentáře..."
            };

            if (dlg.ShowDialog(this) == true)
                exchange.SaveAs(dlg.FileName);

        }

        private void miImportComments_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                AddExtension = true,
                DefaultExt = ".blcx",
                Filter = "Komentáře|*.blcx",
                Title = "Importovat komentáře...",
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false,
                ShowReadOnly = false
            };

            if (dlg.ShowDialog(this) == true)
            {
                using (var txn = Model.BeginTansaction("Comments import"))
                {
                    try
                    {
                        var exchange = CommentsExchangeModel.LoadFromFile(dlg.FileName);
                        var msg = exchange.AddToModel(Model);
                        if (!string.IsNullOrEmpty(msg))
                        {
                            if (MessageBox.Show(this,"Během importu se vyskytly chyby. Chcete vrátit provedené změny? \n\n" + msg,
                                "Varovani", 
                                MessageBoxButton.YesNo, 
                                MessageBoxImage.Warning, 
                                MessageBoxResult.No) == MessageBoxResult.Yes)
                                txn.Commit();
                            else
                                txn.RollBack();
                        }
                        else
                            txn.Commit();
                    }
                    catch (Exception)
                    {
                        txn.RollBack();
                        throw;
                    }    
                }
                
            }
        }

        private void CanExecuteAllways(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandSave_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            miSave_Click(null, null);
        }

        private void CommandSaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            miSaveAs_Click(null, null);
        }

        private void CommandOpen_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            miOpen_Click(null, null);
        }

        private void CommandClose_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            miCloseModel_Click(null, null);
        }

        private void miShowComments_Click(object sender, RoutedEventArgs e)
        {
            cdComments.Width = new GridLength(350);
        }

        private void miHideComments_Click(object sender, RoutedEventArgs e)
        {
            cdComments.Width = new GridLength(0);
        }

        private void btnCopyPath_Click(object sender, RoutedEventArgs e)
        {
            txtSelectedPath.Focus();
            txtSelectedPath.SelectAll();
            Clipboard.SetText(txtSelectedPath.Text);
            e.Handled = true;
        }
    }
}
