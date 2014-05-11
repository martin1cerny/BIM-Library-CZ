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

namespace BimLibrary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LibraryModel _library;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(MainWindow_Loaded);
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
        }

        #region Commands
        #region OpenLibrary
        public static RoutedCommand OpenLibrary = new RoutedCommand();

        private void ExecutedOpenLibraryCommand(object sender, ExecutedRoutedEventArgs e)
        {
            _library = new LibraryModel();
            var dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            dlg.CheckPathExists = true;
            dlg.Multiselect = false;
            dlg.Title = "Select librarty file please...";
            if (dlg.ShowDialog() == true)
            {
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
            if (_library.HasPath)
                _library.Save();
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
    }
}
