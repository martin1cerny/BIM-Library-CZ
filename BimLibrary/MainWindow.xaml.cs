using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Xbim.IO;
using Xbim.Ifc2x3.MaterialResource;
using BimLibrary.Windows;
using Xbim.Ifc2x3.ExternalReferenceResource;
using BimLibrary.ViewModel;
using System.Collections.ObjectModel;
using Microsoft.Windows.Controls.Ribbon;

namespace BimLibrary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        private LibraryModel _library { get { return App.Library; } }
        private XbimModel _model { get { return _library.Model; } }


        public MainWindow()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(MainWindow_Loaded);
            Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //close library and save changes, delete temp files
            _library.Close(true);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //command bindings
            AddCommand(OpenLibrary, ExecutedOpenLibraryCommand, CanExecuteOpenLibraryCommand);
            AddCommand(CloseApplication, ExecutedCloseApplicationCommand, CanExecuteCloseApplicationCommand);
            AddCommand(SaveAs, ExecutedSaveAsCommand, CanExecuteSaveAsCommand);
            AddCommand(Save, ExecutedSaveCommand, CanExecuteSaveCommand);
            AddCommand(ExportIFC, ExecutedExportIFCCommand, CanExecuteExportIFCCommand);
            AddCommand(ExportIFCzip, ExecutedExportIFCzipCommand, CanExecuteExportIFCzipCommand);
            AddCommand(NewLibrary, ExecutedNewLibraryCommand, CanExecuteNewLibraryCommand);
            AddCommand(ImportClassification, ExecutedImportClassificationCommand, CanExecuteImportClassificationCommand);

            SetDataContext();
            SetValue(ClassificationsProperty, App.Library.Classifications);
        }

        #region Commands
        #region OpenLibrary
        public static RoutedCommand OpenLibrary = new RoutedCommand();

        private void ExecutedOpenLibraryCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            dlg.CheckPathExists = true;
            dlg.Multiselect = false;
            dlg.Title = "Select librarty file please...";
            if (dlg.ShowDialog() == true)
            {
                _library.Open(dlg.FileName);
                SetDataContext();
            }
            
        }

        private void CanExecuteOpenLibraryCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #endregion

        #region CloseApplication
        public static RoutedCommand CloseApplication = new RoutedCommand();

        private void ExecutedCloseApplicationCommand(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void CanExecuteCloseApplicationCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #endregion

        #region SaveAs
        public static RoutedCommand SaveAs = new RoutedCommand();

        private void ExecutedSaveAsCommand(object sender, ExecutedRoutedEventArgs e)
        {
            SaveWithDialog();
        }

        private void CanExecuteSaveAsCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #endregion

        #region Save
        public static RoutedCommand Save = new RoutedCommand();

        private void ExecutedSaveCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (_library.HasPath)
                _library.Save();
            else
                _library.LibraryPath = SaveWithDialog();
        }

        private void CanExecuteSaveCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #endregion

        #region ExportIFC
        public static RoutedCommand ExportIFC = new RoutedCommand();

        private void ExecutedExportIFCCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.Title = "Export to IFC 2x3";
            dlg.DefaultExt = _library.DefaultExtension;
            dlg.OverwritePrompt = true;
            if (dlg.ShowDialog() == true)
                _library.ExportToIFC(dlg.FileName);
        }

        private void CanExecuteExportIFCCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #endregion

        #region ExportIFCzip
        public static RoutedCommand ExportIFCzip = new RoutedCommand();

        private void ExecutedExportIFCzipCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.Title = "Export to IFC 2x3";
            dlg.DefaultExt = _library.DefaultExtension;
            dlg.OverwritePrompt = true;
            if (dlg.ShowDialog() == true)
                _library.ExportToIFCzip(dlg.FileName);
        }

        private void CanExecuteExportIFCzipCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #endregion

        #region NewLibrary
        public static RoutedCommand NewLibrary = new RoutedCommand();

        private void ExecutedNewLibraryCommand(object sender, ExecutedRoutedEventArgs e)
        {
            _library.Close(true);
            _library.Clear();
            SetDataContext();
        }

        private void CanExecuteNewLibraryCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #endregion

        #region ImportClassification
        public static RoutedCommand ImportClassification = new RoutedCommand();

        private void ExecutedImportClassificationCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new ClassificationImportWindow();
            dlg.Owner = this;
            dlg.ShowDialog();
        }

        private void CanExecuteImportClassificationCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #endregion
        

        private void AddCommand(ICommand command, ExecutedRoutedEventHandler executed, CanExecuteRoutedEventHandler canExecute)
        {
            var binding = new CommandBinding();
            binding.Command = command;
            binding.Executed += executed;
            binding.CanExecute += canExecute;

            CommandBindings.Add(binding);
        }
        #endregion


        #region Classifications
        public ObservableCollection<ClassificationViewModel> Classifications
        {
            get { return (ObservableCollection<ClassificationViewModel>)GetValue(ClassificationsProperty); }
        }

        // Using a DependencyProperty as the backing store for Classifications.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClassificationsProperty =
            DependencyProperty.Register("Classifications", typeof(ObservableCollection<ClassificationViewModel>), typeof(MainWindow), new UIPropertyMetadata(new ObservableCollection<ClassificationViewModel>()));
        #endregion


        #region Active Classification
        public ClassificationViewModel ActiveClassification
        {
            get { return (ClassificationViewModel)GetValue(ActiveClassificationProperty); }
            set { SetValue(ActiveClassificationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActiveClassification.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActiveClassificationProperty =
            DependencyProperty.Register("ActiveClassification", typeof(ClassificationViewModel), typeof(MainWindow), new UIPropertyMetadata(null));
        #endregion



        private string SaveWithDialog()
        {
            var dlg = new SaveFileDialog();
            dlg.DefaultExt = _library.DefaultExtension;
            dlg.OverwritePrompt = true;
            if (dlg.ShowDialog() == true)
            {
                _library.SaveAs(dlg.FileName);
                return dlg.FileName;
            }
            return null;
        }

        private void ribNewMaterial_Click(object sender, RoutedEventArgs e)
        {
            using (var txn = _model.BeginTransaction("Material creation"))
            {
                var material = _model.Instances.New<IfcMaterial>();
                MaterialWindow win = new MaterialWindow();
                win.Owner = this;
                win.Material = new ViewModel.MaterialViewModel(material);
                var res = win.ShowDialog();

                //commit only is the result is true
                if (res == true)
                {
                    txn.Commit();
                    if (materialsView.Materials != null)
                        materialsView.Materials.Add(new MaterialViewModel(material));
                }
            }

        }

        private void ribNewClassificationSystem_Click(object sender, RoutedEventArgs e)
        {
            using (var txn = _model.BeginTransaction("Classification system creation"))
            {
                var classification = _model.Instances.New<IfcClassification>();
                var win = new ClassificationWindow();
                win.Owner = this;
                win.Classification = new ViewModel.ClassificationViewModel(classification);
                var res = win.ShowDialog();

                //commit only is the result is true
                if (res == true)
                {
                    txn.Commit();
                    Classifications.Add(win.Classification);
                }
            }
        }

        private void SetDataContext()
        {
            var provider = new ObjectDataProvider();
            provider.ObjectInstance = _library;
            DataContext = provider;
        }

        private void ribNewClassItem_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveClassification == null)
            {
                MessageBox.Show("You have to select active classification system first or create one.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            using (var txn = _model.BeginTransaction("Classification system creation"))
            {
                var item = _model.Instances.New<IfcClassificationItem>(ci => {
                    ci.ItemOf = ActiveClassification.IfcClassification;
                    
                });
                if (ActiveClassification.SelectedItem != null)
                {
                    var parent = ActiveClassification.SelectedItem.IfcClassificationItem;
                    if (parent != null)
                    {
                        var relation = parent.IsClassifyingItemIn.FirstOrDefault();
                        if (relation == null)
                            relation = _model.Instances.New<IfcClassificationItemRelationship>();
                        relation.RelatingItem = parent;
                        relation.RelatedItems.Add_Reversible(item);
                    }
                }

                var win = new ClassificationItemWindow();
                win.Owner = this;
                win.ClassificationItem = new ClassificationItemViewModel(item);
                var res = win.ShowDialog();

                //commit only if the result is true
                if (res == true)
                {
                    txn.Commit();
                    if (ActiveClassification.SelectedItem == null)
                        ActiveClassification.RootClassificationItems.Add(win.ClassificationItem);
                    else
                        ActiveClassification.SelectedItem.Children.Add(win.ClassificationItem);
                }
            }
        }

        private void ribNewRootClassItem_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckActiveClassification())
                return;
            using (var txn = _model.BeginTransaction("Classification system creation"))
            {
                var item = _model.Instances.New<IfcClassificationItem>(ci =>
                {
                    ci.ItemOf = ActiveClassification.IfcClassification;

                });

                var win = new ClassificationItemWindow();
                win.ClassificationItem = new ClassificationItemViewModel(item);
                var res = win.ShowDialog();

                //commit only if the result is true
                if (res == true)
                {
                    txn.Commit();
                    ActiveClassification.RootClassificationItems.Add(win.ClassificationItem);
                }
            }
        }

        private bool CheckActiveClassification()
        {
            if (ActiveClassification == null)
            {
                MessageBox.Show(this, "You have to select active classification system first or create one.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
            return true;
        }

        private bool CheckActiveClassificationItem()
        {
            if (!CheckActiveClassification())
                return false;
            if (ActiveClassification.SelectedItem == null)
            {
                MessageBox.Show(this, "You have to select classification item first.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
            return true;
        }

    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType == typeof(Visibility))
            {
                var val = (bool)value;
                if (val)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
