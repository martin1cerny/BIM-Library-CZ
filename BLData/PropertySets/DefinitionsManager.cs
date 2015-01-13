using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using BLData.Exceptions;

namespace BLData.PropertySets
{
    /// <summary>
    /// This is the main class used to work with property set definitions.
    /// It implements buildingSMART data model for definition of properties and quantities for IFC4
    /// It also extends the model with a few deprecated values so that it is backwards compatible with 
    /// IFC 2x3 and it supports its property sets. Definitions of all property sets for IFC 2x3 and IFC4 and quantity sets 
    /// for IFC4 are part of the library so it is independent. It can also be used to define new
    /// property sets and to save them in the format compliant with buildingSMART data specification.
    /// </summary>
    public class DefinitionsManager<T> where T : QuantityPropertySetDef, new()
    {
        private List<T> _definitions;
        private XmlSerializer _serializer;
        private Version _version;

        public DefinitionsManager(Version version)
        {
            _definitions = new List<T>();
            _version = version;
            switch (version)
            {
                case Version.IFC4:
                    var ns = "";
                    if (typeof(T) == typeof(PropertySetDef))
                        ns = "http://buildingSMART-tech.org/xml/psd/PSD_IFC4.xsd";
                    if (typeof(T) == typeof(QtoSetDef))
                        ns = "http://www.buildingsmart-tech.org/xml/qto/QTO_IFC4.xsd";
                    _serializer = new XmlSerializer(typeof(T), ns);
                    break;
                case Version.IFC2x3:
                    _serializer = new XmlSerializer(typeof(T));
                    break;
                default:
                    break;
            }
        }

        public void SetModel(BLModel model)
        {
            if (!_definitions.Any()) return;

            //add item to entity dictonary
            var resource = model.EntityDictionary.FirstOrDefault(r => r.Type == typeof(T).FullName);
            if (resource == null)
            {
                resource = new BLEntityList(model) { Type = typeof(T).FullName };
                model.EntityDictionary.Add(resource);
            }

            foreach (var item in this._definitions)
            {
                if (item.Model != null) throw new ModelOriginException("Property sets has a model defined already");
                item.SetModel(model);
                resource.Items.Add(item);
            }
        }

        public void Add(T item)
        {
            _definitions.Add(item);
        }

        public void Add(IEnumerable<T> items)
        {
            _definitions.AddRange(items);
        }

        public IEnumerable<T> DefinitionSets
        {
            get
            {
                foreach (var item in _definitions)
                {
                    yield return item;
                }
            }
        }

        public T this[string name]
        {
            get
            {
                return _definitions.Where(d => d.Name == name).FirstOrDefault();
            }
        }

        public void LoadFromDirectory(string directory, SearchOption option = SearchOption.TopDirectoryOnly)
        {
            if (!Directory.Exists(directory))
                throw new ArgumentException("Directory doesn't exist.");

            var files = Directory.EnumerateFiles(directory, "*.xml", option);
            foreach (var file in files)
            {
                Load(file);
            }
        }

        public void Load(string path)
        {
            if (!File.Exists(path))
                throw new ArgumentException("Invalid path to file");

            using (var file = File.OpenRead(path))
            {
                Load(file);
                file.Close();
            }
        }

        public void Load(Stream stream)
        {
            var data = (T)_serializer.Deserialize(stream);
            if (data != null)
            {
                this.Add(data);
            }
        }

        public void Load(TextReader reader)
        {
            var data = (T)_serializer.Deserialize(reader);
            if (data != null)
            {
                this.Add(data);
            }
        }

        public void Save(string path, string name)
        {
            var def = this[name];
            if (def != null)
                Save(path, def);
        }

        public void Save(string path, T pSet)
        {
            using (var file = File.Create(path))
            {
                Save(file, pSet);
                file.Close();
            }
        }

        public void Save(Stream stream, T pSet)
        {
            _serializer.Serialize(stream, pSet);
        }

        public void SaveToDirectory(string directory)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            foreach (var pDef in this._definitions)
            {
                var file = Path.Combine(directory, (pDef.Name ?? "Unknown") + ".xml");
                Save(file, pDef);
            }
        }

        public IEnumerable<TP> GetAllProperties<TP>(bool nested = true) where TP : QuantityPropertyDef
        {
            return GetPropertiesWhere<TP>(p => true, nested);
        }

        public IEnumerable<TP> GetPropertiesWhere<TP>(Func<TP, bool> predicate, bool nested = true) where TP : QuantityPropertyDef
        {
            foreach (var set in this._definitions)
            {
                foreach (var qpDef in set.Definitions)
                {
                    var def = qpDef as TP;
                    if (def == null)
                        continue;

                    //check predicate
                    if (predicate(def))
                        yield return def;

                    if (!nested)
                        continue;

                    var pDef = qpDef as PropertyDef;
                    if (pDef == null)
                        continue;

                    //process nested properties in complex properties
                    var cType = pDef.PropertyType.PropertyValueType as TypeComplexProperty;
                    if (cType != null)
                    {
                        foreach (var nestedDef in cType.Properties)
                        {
                            //check predicate
                            var nDef = nestedDef as TP;
                            if (nDef == null)
                                continue;

                            if (predicate(nDef))
                                yield return nDef;
                        }
                    }
                }
            }
        }
    }

    public enum Version
    {
        IFC2x3,
        IFC4
    }
}
