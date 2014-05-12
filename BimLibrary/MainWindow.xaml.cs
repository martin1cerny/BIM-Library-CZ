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

namespace BimLibrary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LibraryModel _library;
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
            if (_library == null)
                _library = LibraryModel.Create();

            //command bindings
            AddCommand(OpenLibrary, ExecutedOpenLibraryCommand, CanExecuteOpenLibraryCommand);
            AddCommand(CloseApplication, ExecutedCloseApplicationCommand, CanExecuteCloseApplicationCommand);
            AddCommand(SaveAs, ExecutedSaveAsCommand, CanExecuteSaveAsCommand);
            AddCommand(Save, ExecutedSaveCommand, CanExecuteSaveCommand);
            AddCommand(ExportIFC, ExecutedExportIFCCommand, CanExecuteExportIFCCommand);
            AddCommand(ExportIFCzip, ExecutedExportIFCzipCommand, CanExecuteExportIFCzipCommand);
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
                if (_library == null)
                    _library = new LibraryModel();
                else
                    _library.Close(true);
                _library.Open(dlg.FileName);
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
                SaveWithDialog();
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

        private void AddCommand(ICommand command, ExecutedRoutedEventHandler executed, CanExecuteRoutedEventHandler canExecute)
        {
            var binding = new CommandBinding();
            binding.Command = command;
            binding.Executed += executed;
            binding.CanExecute += canExecute;

            CommandBindings.Add(binding);
        }
        #endregion

        private void SaveWithDialog()
        {
            var dlg = new SaveFileDialog();
            dlg.DefaultExt = _library.DefaultExtension;
            dlg.OverwritePrompt = true;
            if (dlg.ShowDialog() == true)
                _library.SaveAs(dlg.FileName);
        }

        private void ribNewMaterial_Click(object sender, RoutedEventArgs e)
        {
            using (var txn = _model.BeginTransaction("Material creation"))
            {
                var material = _model.Instances.New<IfcMaterial>();
                MaterialWindow win = new MaterialWindow();
                win.Material = new ViewModel.MaterialViewModel(material);
                var res = win.ShowDialog();

                //commit only is the result is true
                if (res == true)
                    txn.Commit();
            }

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
