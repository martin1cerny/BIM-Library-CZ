using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BLData
{
    public abstract class BLEntity : IBLModelEntity
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
            Set(new string[] { propertyName }, doAction, undoAction);
        }

        protected void Set(string[] propertyNames, Action doAction, Action undoAction)
        {
            Action doA = () => { doAction(); foreach(var propertyName in propertyNames) OnPropertyChanged(propertyName); };
            Action undoA = () => { undoAction(); foreach (var propertyName in propertyNames) OnPropertyChanged(propertyName); };
            if (_model == null) //no transaction. This is for deserialization only
            {
                doA();
            }
            else
                _model.Transact(this, doA, undoA);
        }


        private Dictionary<string, BList<Guid>> _entityIds;
        private Dictionary<string, BList<BLEntity>> _entityLists;
        protected BList<T> GetBLEntityList<T>(string name) where T: BLEntity
        {
            //try to get existing one
            BList<BLEntity> result;
            if (_entityLists.TryGetValue(name, out result))
            {
                return result as BList<T>;
            }

            //try to create from ids
            BList<Guid> ids;
            if (_entityIds.TryGetValue(name, out ids))
            {
                var entityList = new BList<T>(_model);
                foreach (var id in ids)
                {
                    var entity = _model.Get<T>(id);
                    entityList.Add(entity);
                }
                SetBLEntityList<T>(name, entityList);
                return entityList;
            }

            //create new empty one
            var entities = new BList<T>(_model);
            _entityLists.Add(name, entities as BList<BLEntity>);
            return entities;
        }
        protected void SetBLEntityList<T>(string name, BList<T> value) where T : BLEntity
        {
            BList<BLEntity> old;
            _entityLists.TryGetValue(name, out old);

            Action doAction = () => {
                if (_entityLists.Keys.Contains(name))
                    _entityLists[name] = value as BList<BLEntity>;
                else
                    _entityLists.Add(name, value as BList<BLEntity>);
                if (value != null)
                    value.SetModel(_model);
            };
            Action undoAction = () => {
                if (old == null)
                    _entityLists.Remove(name);
                else
                    _entityLists[name] = old;
            };
            //set value in transaction
            Set(name, doAction, undoAction);
        }
        protected BList<Guid> GetGuidList(string name)
        {
            BList<Guid> ids;
            if (_entityIds.TryGetValue(name, out ids))
                return ids;
            else
            {
                ids = new BList<Guid>();
                SetGuidList(name, ids);
                return ids;
            }
        }
        protected void SetGuidList(string name, BList<Guid> value)
        {
            if (_entityIds.Keys.Contains(name))
                _entityIds[name] = value;
            else
                _entityIds.Add(name, value);
            
            //keep watching if this collection is being used only for serialization and deserialization
            value.CollectionChanged += (s, a) =>
            {
                if (_model != null)
                    throw new Exceptions.InvalidModelOperationException("Guid list " + name + " can't be changed when model is defined already. This property is only to be used for serialization and deserialization.");
            };
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
            if (obj == null) return false;
            if (GetType() != obj.GetType()) return false;
            return _id.Equals((obj as BLEntity).Id);
        }

        public abstract string Validate();

        internal abstract IEnumerable<BLEntity> GetChildren();
    }
}
