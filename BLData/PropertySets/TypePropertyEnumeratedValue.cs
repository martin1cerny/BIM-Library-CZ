using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BLData.PropertySets
{
    /// <summary>
    /// The element of IfcPropertyEnumeratedValue.
    /// </summary>
    public class TypePropertyEnumeratedValue : BLEntity, IPropertyValueType
    {
        private EnumList _enum;
        /// <summary>
        /// The container element of enumeration list.
        /// </summary>
        public EnumList EnumList 
        {
            get { return _enum; }
            set { var old = _enum; Set("EnumList", () => _enum = value, () => _enum = old); }
        }

        private BList<ConstantDef> _constantList = new BList<ConstantDef>();
        /// <summary>
        /// The container element of enumeration constants with localized names and descriptions.
        /// </summary>
        public BList<ConstantDef> ConstantList 
        {
            get { return _constantList; }
            set { var old = _constantList; Set("ConstantList", () => _constantList = value, () => _constantList = old); }
        }

        internal override void SetModel(BLModel model)
        {
            _model = model;
            if (_enum != null) _enum.SetModel(model);
            if (_constantList != null) _constantList.SetModel(model);
        }

        public override string Validate()
        {
            var result = "";
            if (_enum != null) result +=  _enum.Validate();
            if (_constantList != null) result += _constantList.Validate();
            return result;
        }

        internal override IEnumerable<BLEntity> GetChildren()
        {
            if (_enum != null) yield return EnumList;
            if (_constantList != null) foreach (var item in _constantList) yield return item;
        }
    }
}
