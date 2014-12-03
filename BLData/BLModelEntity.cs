using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BLData
{
    [XmlInclude(typeof(BLData.Classification.BLClassificationItem))]
    public abstract class BLModelEntity : IBLModelEntity
    {
        [XmlIgnore]
        public BLModel Model
        {
            get { return _model; }
        }

        private Guid _id  = Guid.NewGuid();
        [XmlAttribute]
        public Guid Id
        {
            get { return _id; }
            //set should only happen during deserialization when model is null
            set { if (_model != null) throw new InvalidOperationException("ID can't be assigned when object is bound to model."); _id = value; }
        }

        protected BLModel _model;
        //this has to be implemented in all inherited classes
        internal abstract void SetModel(BLModel model);
        
        protected void Set(string fieldName, object oldValue, object newValue)
        {
            if (_model == null) //no transaction. This is for deserialization only
            {
                var field = GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (fieldName == null) throw new Exceptions.FieldNotFoundException(fieldName);
                field.SetValue(this, newValue);
            }
            else
                _model.Set(this, fieldName, oldValue, newValue);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return _id.Equals(obj);
        }

        public abstract string Validate();
    }
}
