using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using BLData.Exceptions;

namespace BLData
{
    [XmlRoot("Model")]
    public class BLModel
    {
        private Session _session = new Session();
        [XmlIgnore]
        public Session Session { get { return _session; } }
        private Transaction CurrentTransaction { get { 
            var txn = _session.CurrentTransaction; 
            if (txn == null || txn.State != Transaction.StateEnum.OPENED)
                return null;
            return txn;
        } }

        [XmlIgnore]
        public bool IsTransacting { get { return CurrentTransaction != null; } }

        public Transaction BeginTansaction(string name)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException("Transaction name has to be defined.", "name");
            if (CurrentTransaction != null && !CurrentTransaction.IsFinished) throw new Exceptions.TransactionNotFinishedException(CurrentTransaction.Name);

            _session.Add(new Transaction(name));
            return CurrentTransaction;
        }

        internal void Transact(IBLModelEntity entity, Action doAction, Action undoAction)
        {
            //check if entity is bound to this model
            if (entity.Model != this) throw new ModelOriginException("Entity can't use transaction of this model. It is not bound to this model.");

            if (CurrentTransaction == null) throw new Exceptions.NoTransactionException("Transaction must be opened to be able to change the model.");

            CurrentTransaction.AddAction(undoAction, doAction);
            doAction();
        }

        internal void Transact<T>(BList<T> list, Action doAction, Action undoAction)
        {
            //check if entity is bound to this model
            if (list.Model != this) throw new ModelOriginException("Entity can't use transaction of this model. It is not bound to this model.");
            if (CurrentTransaction == null) throw new Exceptions.NoTransactionException("Transaction must be opened to be able to change the model.");

            CurrentTransaction.AddAction(undoAction, doAction);
            doAction();
        }

        private List<BLEntityList> _entities = new List<BLEntityList>();
        [XmlArrayItem("EntityList")]
        public List<BLEntityList> EntityDictionary
        {
            get { return _entities; }
            set { _entities = value; }
        }

        public IEnumerable<T> Get<T>() where T : BLEntity
        {
            if (typeof(BLModelEntity).IsAssignableFrom(typeof(T)))
            {
                var resources = _entities.Where(r => typeof(T).IsAssignableFrom(Type.GetType(r.Type)));
                foreach (var resource in resources)
                {
                    if (resource != null)
                        foreach (var item in resource.Items)
                            yield return item as T;    
                }
            }
            else
            {
                foreach (var res in _entities)
                {
                    foreach (var item in res.Items)
                    {
                        if (typeof(T).IsAssignableFrom(item.GetType()))
                            yield return item as T;

                        foreach (var entity in item.GetChildren())
                        {
                            if (typeof(T).IsAssignableFrom(entity.GetType()))
                                yield return entity as T;
                        }
                    }
                }
            }
        }

        public IEnumerable<T> Get<T>(Func<T, bool> selector) where T : BLEntity
        {
            foreach (var item in Get<T>())
                if (selector(item)) yield return item;
        }

        public T Get<T>(Guid id) where T : BLEntity
        {
            return Get<T>().FirstOrDefault(e => e.Id == id);
        }

        public T New<T>(Action<T> init = null) where T : BLEntity, new()
        {
            //check for transaction
            if (CurrentTransaction == null) throw new Exceptions.NoTransactionException("Transaction must be opened to be able to change the model.");

            var entity = new T();
            entity.SetModel(this);

            //add to resources of the model in transaction
            var modelEntity = entity as BLModelEntity;
            if (modelEntity != null)
            {
                var resource = _entities.FirstOrDefault(r => r.Type == typeof(T).FullName);
                if (resource == null)
                {
                    resource = new BLEntityList(this) { Type = typeof(T).FullName};
                    _entities.Add(resource);
                }
                resource.Items.Add(modelEntity);
                OnModelEntitiesCollectionChanged(modelEntity, ChangeType.NEW);
            }
            //init if defined
            if (init != null)
                init(entity);
            return entity;
        }

        internal BLEntityList GetResource<T>() where T : BLModelEntity
        {
            var result = EntityDictionary.FirstOrDefault(res => res.Type == typeof(T).FullName);
            if (result == null)
            {
                result = new BLEntityList(this) { Type = typeof(T).FullName };
                EntityDictionary.Add(result);
            }
            if (result.Items == null)
                result.Items = new BList<BLModelEntity>();
            return result;
        }

        /// <summary>
        /// This will remove object from resources but it won't to anything with related and relating objects. It will delete any nested objects.
        /// </summary>
        /// <param name="instance">Instance to be deleted</param>
        /// <returns>TRUE if deletion was successfull, FALSE otherwise.</returns>
        public bool Delete(BLModelEntity instance)
        {
            //check for transaction
            if (CurrentTransaction == null) throw new Exceptions.NoTransactionException("Transaction must be opened to be able to change the model.");

            //remove from resources in transaction
            var resource = _entities.FirstOrDefault(r => r.Type == instance.GetType().FullName);
            if (resource == null) return false;
            var entity = resource.Items.FirstOrDefault(e => e.Id == instance.Id);
            if (entity == null) return false;
            var result = resource.Items.Remove(entity);
            OnModelEntitiesCollectionChanged(entity, ChangeType.DELETED);
            return result;
        }

        public static BLModel Open(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(BLModel));
            var model = serializer.Deserialize(stream) as BLModel;
        
            //set model for all root entities
            foreach (var item in model.EntityDictionary)
                foreach (var e in item.Items)
                    e.SetModel(model);
         
            return model;
        }

        /// <summary>
        /// This will serialize model to the stream as an XML
        /// </summary>
        /// <param name="stream">target stream</param>
        public void Save(Stream stream)
        {
            if (CurrentTransaction != null && !CurrentTransaction.IsFinished)
            {
                throw new Exceptions.TransactionNotFinishedException(CurrentTransaction.Name);
            }

            //set session transactions as saved - this is to be used to indicate if model has been changed 
            if (_session.CurrentTransaction != null)
                _session.CurrentTransaction.IsSaved = true;

            var serializer = new XmlSerializer(GetType());
            serializer.Serialize(stream, this);
            
        }

        public string Validate()
        {
            var msg = "";

            //validate all root entities
            foreach (var item in this.EntityDictionary)
                foreach (var e in item.Items)
                    msg += e.Validate();

            //additional validation rules
            if (Information == null)
                msg += String.Format("There should be at least one project information object. \n");

            return msg;
        }

        [XmlIgnore]
        public BLModelInformation Information
        {
            get
            {
                var result = Get<BLModelInformation>().FirstOrDefault();
                if (result == null) 
                {
                    Action a = () => 
                    result = New<BLModelInformation>();
                    if (CurrentTransaction == null)
                        using (var txn = BeginTansaction("Creation of the model information"))
                        {
                            try
                            {
                                a();
                                txn.Commit();
                            }
                            catch (Exception)
                            {
                                txn.RollBack();
                                throw;
                            }
                        }
                    else
                        a();
                }
                return result;
            }
        }

        public event ModelEntitiesCollectionChangedHandler ModelEntitiesCollectionChanged;
        private void OnModelEntitiesCollectionChanged(BLModelEntity entity, ChangeType type)
        {
            if (ModelEntitiesCollectionChanged != null)
                ModelEntitiesCollectionChanged(new ModelEntitiesCollectionChangedEventArgs(entity, type));
        }

    }

    public delegate void ModelEntitiesCollectionChangedHandler (ModelEntitiesCollectionChangedEventArgs args);

    public class ModelEntitiesCollectionChangedEventArgs
    {
        public BLModelEntity Entity { get; internal set; }
        public ChangeType ChangeType { get; internal set; }

        public Type EntityType { get { return Entity.GetType(); } }

        public ModelEntitiesCollectionChangedEventArgs(BLModelEntity entity, ChangeType type)
        {
            Entity = entity;
            ChangeType = type;
        }
    }

    public enum ChangeType
    {
        UNDEFINED,
        NEW,
        DELETED
    }
}
