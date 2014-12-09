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
        
        protected void Set(string propertyName, Action doAction, Action undoAction)
        {
            if (_model == null) //no transaction. This is for deserialization only
            {
                doAction();
            }
            else
                _model.Transact(this, doAction, undoAction);
            OnPropertyChanged(propertyName);
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
            if (GetType() != obj.GetType()) return false;
            return _id.Equals((obj as BLModelEntity).Id);
        }

        public abstract string Validate();
    }
}
