using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using BLData.Classification;

namespace BLData
{
    public class BLEntityList
    {
        public BLEntityList()
        {
            _items = new BList<BLModelEntity>();
        }

        public BLEntityList(BLModel model)
        {
            _items = new BList<BLModelEntity>(model);
        }

        internal void SetModel(BLModel model)
        {
            _items.SetModel(model);
        }

        private String _type;

        [XmlAttribute]
        public String Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private BList<BLModelEntity> _items;

        [XmlElement("Classification", typeof(BLClassification))]
        [XmlElement("ClassificationItem", typeof(BLClassificationItem))]
        [XmlElement("PropertySetDefinition", typeof(PropertySets.PropertySetDef))]
        [XmlElement("QuantitySetDefinition", typeof(PropertySets.QtoSetDef))]
        [XmlElement("Person", typeof(Actors.BLPerson))]
        [XmlElement("Comment", typeof(Comments.BLComment))]
        public BList<BLModelEntity> Items
        {
            get { return _items; }
            set { _items = value; }
        }
        
    }
}
