using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BLData.PropertySets
{
    /// <summary>
    /// Version information of IFC release and sub schema.
    /// </summary>
    public class IfcVersion : BLEntity
    {
        private string _version;

        /// <summary>
        /// The version information of IFC, i.e., "2x3 TC1", "IFC4".
        /// </summary>
        [XmlAttribute("version")]
        public string Version 
        {
            get { return _version; }
            set { var old = _version; Set("Version", () => _version = value, () => _version = old); }
        }


        private string _schema;
        /// <summary>
        /// The sub schema name,  i.e., "IfcSharedBldgElements".
        /// </summary>
        [XmlAttribute("schema")]
        public string Schema 
        {
            get { return _schema; }
            set { var old = _schema; Set("Schema", () => _schema = value, () => _schema = old); }
        }

        internal override void SetModel(BLModel model)
        {
            _model = model;
        }

        public override string Validate()
        {
            var msg = "";
            if (String.IsNullOrEmpty(_version) || String.IsNullOrEmpty(_schema))
                msg += String.Format("Version and format have to be defined \n");
            return msg;
        }

        internal override IEnumerable<BLEntity> GetChildren()
        {
            yield break;
        }
    }
}
