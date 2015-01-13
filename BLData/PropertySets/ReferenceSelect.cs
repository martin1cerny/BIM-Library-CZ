using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BLData.PropertySets
{
    /// <summary>
    /// The element of reference select.
    /// </summary>
    public class ReferenceSelect: BLEntity
    {
        #region RefType
        private RefTypeEnum? _reftype;

        /// <summary>
        /// The reference type based on SELECT Type IfcObjectReferenceSelect.
        /// </summary>
        [XmlAttribute("reftype")]
        public string _Value
        {
            get
            {
                return _reftype.ToString();
            }
            set
            {
                if (_model != null)
                    throw new InvalidOperationException();

                RefTypeEnum type = RefTypeEnum.IfcExternalReference;
                if (String.IsNullOrEmpty(value))
                {
                    _reftype = null;
                }
                else if (Enum.TryParse<RefTypeEnum>(value, out type))
                {
                    _reftype = type;
                }
                else
                    throw new ArgumentOutOfRangeException(value);

            }
        }
        
        [XmlIgnore]
        public RefTypeEnum? ReferenceType
        {
            get { return _reftype; }
            set {
                var old = _reftype; Set("ReferenceType", () => _reftype = value, () => _reftype = old);
            }
        }
        #endregion

        private string _guid;
        /// <summary>
        /// The GUID for reference.
        /// </summary>
        [XmlAttribute("guid")]
        public string Guid {
            get { return _guid; }
            set { var old = _guid; Set("Guid", () => _guid = value, () => _guid = old); }
        }

        private string _url;
        /// <summary>
        /// The URL for reference.
        /// </summary>
        [XmlAttribute("URL")]
        public string URL {
            get { return _url; }
            set { var old = _url; Set("URL", () => _url = value, () => _url = old); }
        }

        private string _libName;
        /// <summary>
        /// The library name for reference.
        /// </summary>
        [XmlAttribute("libraryname")]
        public string LibraryName {
            get { return _libName; }
            set { var old = _libName; Set("LibraryName", () => _libName = value, () => _libName = old); }
        }

        private string _secRef;
        /// <summary>
        /// The section information of reference.
        /// </summary>
        [XmlAttribute("sectionref")]
        public string SectionReference {
            get { return _secRef; }
            set { var old = _secRef; Set("SectionReference", () => _secRef = value, () => _secRef = old); }
        }

        internal override void SetModel(BLModel model)
        {
            _model = model;
        }

        public override string Validate()
        {
            return "";
        }

        internal override IEnumerable<BLEntity> GetChildren()
        {
            yield break;
        }
    }

    public enum RefTypeEnum
    {
        IfcMaterialDefinition,
        IfcPerson,
        IfcOrganization,
        IfcPersonAndOrganization,
        IfcExternalReference,
        IfcTimeSeries,
        IfcAddress,
        IfcAppliedValue,

        [Obsolete]
        IfcMaterial,
        [Obsolete]
        IfcDateAndTime,
        [Obsolete]
        IfcMaterialList,
        [Obsolete]
        IfcCalendarDate,
        [Obsolete]
        IfcLocalTime,
        [Obsolete]
        IfcMaterialLayer,
        [Obsolete]
        IfcClassificationReference
    }
}
