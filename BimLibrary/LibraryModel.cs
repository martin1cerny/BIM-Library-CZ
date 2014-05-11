using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BimLibrary.MetadataModel;
using Xbim.IO;
using Xbim.XbimExtensions.Interfaces;
using Ionic.Zip;
using System.IO;

namespace BimLibrary
{
    public class LibraryModel
    {
        private XbimModel _model;
        private MetaPropertyMappings _propertyMappings;
        private string _path;


        private const string _propMapFile = "property_mappings.xml";
        private const string _libFile = "library.ifc";
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
                _model = XbimModel.CreateModel(tempModelPath);
            }
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
            _model.SaveAs(modelPath, XbimStorageType.IFC);

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

            if (File.Exists(modelPath))
                File.Delete(modelPath);
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
        }

        public static LibraryModel Create()
        {
            var lib = new LibraryModel();
            lib._model = XbimModel.CreateTemporaryModel();
            lib._propertyMappings = new MetaPropertyMappings();

            return lib;
        }
    }
}
