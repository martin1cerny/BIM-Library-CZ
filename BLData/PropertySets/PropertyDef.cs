﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BLData.PropertySets
{
    /// <summary>
    /// The element of property definition.
    /// </summary>
    public class PropertyDef : QuantityPropertyDef
    {
        private ValueDef _valueDef;
        /// <summary>
        /// Not in use. This element is deprecated.
        /// </summary>
        [Obsolete]
        public ValueDef ValueDef {
            get { return _valueDef; }
            set { var old = _valueDef; Set("ValueDef", () => _valueDef = value, () => _valueDef = old); }
        }


        private PropertyType _type;
        /// <summary>
        /// The container element of property type.
        /// </summary>
        public PropertyType PropertyType {
            get { return _type; }
            set { var old = _type; Set("PropertyType", () => _type = value, () => _type = old); }
        }


        private string _ifdGuid;

        [XmlAttribute("ifdguid")]
        public string IfdGuid {
            get { return _ifdGuid; }
            set { var old = _ifdGuid; Set("IfdGuid", () => _ifdGuid = value, () =>_ifdGuid = old); } 
        }

        internal override void SetModel(BLModel model)
        {
            if (_valueDef != null) _valueDef.SetModel(model);
            if (_type != null) _type.SetModel(model);
            base.SetModel(model);
        }

        public override string Validate()
        {
            var result = "";
            result += base.Validate();
            if (_valueDef != null) result += _valueDef.Validate();
            if (_type != null) result += _type.Validate();
            return result;
        }
    }
}
