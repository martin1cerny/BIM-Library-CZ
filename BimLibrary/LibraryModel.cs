using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BimLibrary.MetadataModel;
using Xbim.IO;
using Xbim.XbimExtensions.Interfaces;
using Ionic.Zip;
using System.IO;
using System.ComponentModel;
using BimLibrary.ViewModel;
using System.Collections.ObjectModel;
using Xbim.Ifc2x3.ExternalReferenceResource;
using Xbim.Ifc2x3.MaterialResource;

namespace BimLibrary
{
    public class LibraryModel : INotifyPropertyChanged
    {
        private XbimModel _model;
        private MetaPropertyMappings _propertyMappings;
        private string _path;


        private const string _propMapFile = "property_mappings.xml";
        private const string _libFile = "library.xbim";
        private const string _defaultExtension = ".xbl";
        private const string _tempDir = "BimLibraryData";

        private string TempDataDir
        {
            get 
            {
                var path = Path.GetTempPath();
                path = Path.Combine(path, _tempDir);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }

        public bool HasPath { get { return !String.IsNullOrEmpty(_path); } }
        public string DefaultExtension { get { return _defaultExtension; } }
        public string LibraryPath { get { return _path; } set { _path = value; } }

        public XbimModel Model { get { return _model; } }
        public MetaPropertyMappings PropertyMappings { get { return _propertyMappings; } }

        private void CleanTempDir()
        {
            foreach (var file in Directory.EnumerateFiles(TempDataDir, "*", SearchOption.AllDirectories))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception)
                {
                }
            }
            try
            {
                Directory.Delete(TempDataDir, true);
            }
            catch (Exception)
            {
            }
        }


        public void Open(string file)
        {
            if (String.IsNullOrEmpty(file))
                throw new ArgumentNullException("File has to be specified");
            if (!File.Exists(file))
                throw new ArgumentException("File has to be specified");

            Close(true);
            _path = file;

            var tempModelPath = Path.Combine(TempDataDir, _libFile);
            using (ZipFile zip = ZipFile.Read(file))
            {
                var propMapEntry = zip[_propMapFile];
                using (var reader = propMapEntry.OpenReader())
                {
                    _propertyMappings = MetaPropertyMappings.Open(reader);
                    reader.Close();
                }

                var modelEntry = zip[_libFile];
                modelEntry.Extract(TempDataDir);
                if (_model == null)
                    _model = new XbimModel();
                _model.Open(tempModelPath, Xbim.XbimExtensions.XbimDBAccess.ReadWrite);
                OnPropertyChanged("Model");
            }

            Init();
        }

        public void Save()
        {
            if (String.IsNullOrEmpty(_path))
                throw new Exception("Model wasn't saved before. Unknown target location.");

            SaveAs(_path);
        }

        public void SaveAs(string file)
        {
            if (String.IsNullOrEmpty(Path.GetExtension(file)))
                file = Path.ChangeExtension(file, _defaultExtension);

            var modelPath = Path.Combine(TempDataDir, _libFile);
            
            //temp model
            if (_model.DatabaseName != modelPath)
                _model.SaveAs(modelPath, XbimStorageType.XBIM);
            else
                _model.CacheStop();
            _model.Close();

            using (ZipFile zip = new ZipFile())
            {
                var propertyStream = new MemoryStream();
                _propertyMappings.Save(propertyStream);
                propertyStream.Position = 0;

                ZipEntry e = zip.AddEntry(_propMapFile, propertyStream);
                e.Comment = "Template property mappings.";
                zip.AddFile(modelPath, "");
                    
                zip.Save(file);
            }

            //reopen closed model
            _model.Open(modelPath, Xbim.XbimExtensions.XbimDBAccess.ReadWrite);

            OnPropertyChanged("Model");
            OnPropertyChanged("PropertyMappings");
        }

        public void Close(bool save)
        {
            if (save && HasPath)
                Save();
            if (_model != null)
                _model.Close();

            CleanTempDir();

            _model = null;
            _propertyMappings = null;

            Init();
            OnPropertyChanged("Model");
            OnPropertyChanged("PropertyMappings");
        }

        public static LibraryModel Create()
        {
            var lib = new LibraryModel();
            lib._model = XbimModel.CreateTemporaryModel();
            lib._propertyMappings = new MetaPropertyMappings();

            lib.Init();
            lib.OnPropertyChanged("Model");
            lib.OnPropertyChanged("PropertyMappings");

            return lib;
        }

        public void Clear()
        {
            _model = XbimModel.CreateTemporaryModel();
            _propertyMappings = new MetaPropertyMappings();

            Init();
            OnPropertyChanged("Model");
            OnPropertyChanged("PropertyMappings");
        }

        public void ExportToIFC(string path)
        {
            path = Path.ChangeExtension(path, ".ifc");
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException();
            _model.SaveAs(path, XbimStorageType.IFC);
        }

        public void ExportToIFCzip(string path)
        {
            path = Path.ChangeExtension(path, ".ifczip");
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException();
            _model.SaveAs(path, XbimStorageType.IFCZIP);
        }

        #region Materials
        private ObservableCollection<MaterialViewModel> _Materials;

        public ObservableCollection<MaterialViewModel> Materials
        {
            get {
                if (_Materials == null)
                    _Materials = new ObservableCollection<MaterialViewModel>();
                return _Materials; 
            }
            set { _Materials = value; OnPropertyChanged("Materials"); }
        }
        #endregion

        #region Classifications
        private ObservableCollection<ClassificationViewModel> _classifications;

        public ObservableCollection<ClassificationViewModel> Classifications
        {
            get {
                if (_classifications == null)
                    _classifications = new ObservableCollection<ClassificationViewModel>();
                return _classifications; }
            set { _classifications = value; OnPropertyChanged("Classifications"); }
        }
        #endregion

        #region Element Types
        private ObservableCollection<ElementTypeViewModel> _elementTypes;

        public ObservableCollection<ElementTypeViewModel> ElementTypes
        {
            get
            {
                if (_elementTypes == null)
                    _elementTypes = new ObservableCollection<ElementTypeViewModel>();
                return _elementTypes;
            }
            set { _elementTypes = value; OnPropertyChanged("ElementTypes"); }
        }
        #endregion

        private void Init()
        {
            //clear first
            Classifications.Clear();
            Materials.Clear();

            if (_model != null)
            {
                //classifications
                var cls = _model.Instances.OfType<IfcClassification>();
                foreach (var item in cls)
                {
                    Classifications.Add(new ClassificationViewModel(item));
                }

                //materials
                var mats = _model.Instances.OfType<IfcMaterial>();
                foreach (var item in mats)
                {
                    Materials.Add(new MaterialViewModel(item));
                }
            }

        }

        #region Property Changed implementation
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
        #endregion


    }
}
