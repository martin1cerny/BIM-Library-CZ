using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BLData
{
    public abstract class BLModelEntity : IBLModelEntity
    {
        [XmlIgnore]
        public BLModel Model
        {
            get { return _model; }
        }

        private Guid _id;
        public Guid Id
        {
            get { return _id; }
            set { Set("_id", _id, value); }
        }

        public BLModelEntity()
        {
            _id = Guid.NewGuid();
        }

        internal BLModel _model;
        protected void Set(string fieldName, object oldValue, object newValue)
        {
            if (_model == null) //no transaction. This is for deserialization only
            {
                var field = GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic);
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
    }
}
