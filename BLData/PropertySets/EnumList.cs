using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BLData.PropertySets
{
    public class EnumList : BLEntity
    {
        private string _name;

        [XmlAttribute("name")]
        public string Name 
        {
            get { return _name; }
            set { var old = _name; Set("Name", () => _name = value, () => _name = old); }
        }

        private BList<string> _items = new BList<string>();

        [XmlElement("EnumItem")]
        public BList<string> Items 
        {
            get { return _items; }
            set { var old = _items; Set("Items", () => _items = value, () => _items = old); } 
        }

        internal override void SetModel(BLModel model)
        {
            _model = model;
            if (_items != null) _items.SetModel(model);
        }

        public override string Validate()
        {
            var result = "";
            if (_items != null) result += _items.Validate();
            return result;
        }

        internal override IEnumerable<BLEntity> GetChildren()
        {
            yield break;
        }
    }
}
