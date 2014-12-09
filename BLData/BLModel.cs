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
        public Session Session { get { return _session; } }
        private Transaction CurrentTransaction { get { 
            var txn = _session.CurrentTransaction; 
            if (txn == null || txn.State != Transaction.StateEnum.OPENED)
                return null;
            return txn;
        } }

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

        public IEnumerable<T> Get<T>() where T : BLModelEntity, new()
        {
            var resource = _entities.FirstOrDefault(r => r.Type == typeof(T).Name);
            if (resource != null)
                foreach (var item in resource.Items)
                    yield return (T)item; ;
        }

        public IEnumerable<T> Get<T>(Func<T, bool> selector) where T : BLModelEntity, new()
        {
            var resource = _entities.FirstOrDefault(r => r.Type == typeof(T).Name);
            if (resource != null) 
                foreach (var item in resource.Items)
                {
                    var entity = (T)item;
                    if (selector(entity)) yield return entity;
                }
        }

        public T Get<T>(Guid id) where T : BLModelEntity, new()
        {
            var resource = _entities.FirstOrDefault(r => r.Type == typeof(T).Name);
            if (resource == null) return null;
            return resource.Items.FirstOrDefault(e => e.Id == id) as T;
        }

        public T New<T>(Action<T> init = null) where T : BLModelEntity, new()
        {
            //check for transaction
            if (CurrentTransaction == null) throw new Exceptions.NoTransactionException("Transaction must be opened to be able to change the model.");

            var entity = new T();
            entity.SetModel(this);

            //add to resources of the model in transaction
            var resource = _entities.FirstOrDefault(r => r.Type == typeof(T).Name);
            if (resource == null)
            {
                resource = new BLEntityList(this) { Type = typeof(T).Name};
                _entities.Add(resource);
            }
            resource.Items.Add(entity);

            //init if defined
            if (init != null)
                init(entity);

            return entity;
        }

        /// <summary>
        /// This will remove object from resources but it won'd to anything with related and relating objects
        /// </summary>
        /// <typeparam name="T">Type of instance to be removed</typeparam>
        /// <param name="instance">Instance to be deleted</param>
        /// <returns>TRUE if deletion was successfull, FALSE otherwise.</returns>
        public bool Delete<T>(T instance) where T : BLModelEntity
        {
            //check for transaction
            if (CurrentTransaction == null) throw new Exceptions.NoTransactionException("Transaction must be opened to be able to change the model.");

            //remove from resources in transaction
            var resource = _entities.FirstOrDefault(r => r.Type == typeof(T).Name);
            if (resource == null) return false;
            var entity = resource.Items.FirstOrDefault(e => e.Id == instance.Id);
            if (entity == null) return false;
            return resource.Items.Remove(entity);
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

        public void Save(Stream stream)
        {
            if (CurrentTransaction != null && !CurrentTransaction.IsFinished)
            {
                throw new Exceptions.TransactionNotFinishedException(CurrentTransaction.Name);
            }

            //set session transactions as saved - this is to be used to indocate weather model has been changed 
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
                if (result == null && CurrentTransaction != null) 
                {
                    result = New<BLModelInformation>();
                }
                return result;
            }
        }
    }
}
