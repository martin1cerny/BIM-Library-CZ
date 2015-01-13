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
using System.Windows.Shapes;
using System.IO;

namespace BimLibrary.Windows
{
    /// <summary>
    /// Interaction logic for ClassificationImport.xaml
    /// </summary>
    public partial class ClassificationImportWindow : Window
    {
        public ClassificationImportWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(ClassificationImport_Loaded);
        }

        private class ClassificationAvailable
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }

        void ClassificationImport_Loaded(object sender, RoutedEventArgs e)
        {
            var existing = Directory.EnumerateFiles("Classifications", "*", SearchOption.AllDirectories);
            var source = new List<ClassificationAvailable>();
            foreach (var ex in existing)
            {
                var path = ex;
                var name = System.IO.Path.GetFileNameWithoutExtension(ex);
                source.Add(new ClassificationAvailable() { Path = path, Name = name });
            }
            var provider = new ObjectDataProvider();
            provider.ObjectInstance = source;
            DataContext = provider;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var file = cmbClassifications.SelectedValue as string;
                if (File.Exists(file))
                {
                    var importer = new ClassificationImporter();
                    importer.ImportCSV(file);
                }
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "It was not possible to import classification: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                DialogResult = false;
            }
            
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
