using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLData.Exceptions;

namespace BLData
{
    public class BLModel
    {
        private Session _session = new Session();
        internal Transaction CurrentTransaction { get { 
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

        internal void Set(IBLModelEntity entity, string field, object oldValue, object newValue)
        {
            //check if entity is bound to this model
            if (entity.Model != this) throw new ModelOriginException("Entity can't use transaction of this model. It is not bound to this model.");

            if (CurrentTransaction == null) throw new Exceptions.NoTransactionException("Transaction must be opened to be able to change the model.");

            var type = entity.GetType();
            var info = type.GetField(field, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
            if (info == null)
                throw new Exceptions.FieldNotFoundException(field);

            //check value
            var entVal = newValue as IBLModelEntity;
            if (entVal != null && entVal.Model != this) throw new ModelOriginException("Entity can't be assigned. It is not bound to the same model.");

            var listVal = newValue as BList<object>;
            if (listVal != null && listVal.Model != this) throw new ModelOriginException("List can't be assigned. It is not bound to the same model.");


            Action undoAction = () => { info.SetValue(entity, oldValue); };
            Action doAction = () => { info.SetValue(entity, newValue); };
            CurrentTransaction.AddAction(undoAction, doAction);
            doAction();

            //get property name
            var property = field.TrimStart('_').ToCharArray();
            property[0] = property[0].ToString().ToUpper()[0];
            var name = new String(property);

            //signal property changed
            entity.OnPropertyChanged(name);
        }

        public T New<T>(Action<T> init = null) where T: BLModelEntity, new()
        {
            //check for transaction
            if (CurrentTransaction == null) throw new Exceptions.NoTransactionException("Transaction must be opened to be able to change the model.");

            var entity = new T();
            entity._model = this;

            //add to resources of the model in transaction
            throw new NotImplementedException();

            //init if defined
            if (init != null)
                init(entity);

            return entity;
        }

        public void Delete<T>(T instance)
        {
            //check for transaction
            if (CurrentTransaction == null) throw new Exceptions.NoTransactionException("Transaction must be opened to be able to change the model.");

            //remove from resources in transaction
            throw new NotImplementedException();
        }

        public void Open(Stream stream)
        {
            //check if there is no pending transaction open
            if (CurrentTransaction != null && !CurrentTransaction.IsFinished)
            {
                throw new Exceptions.TransactionNotFinishedException(CurrentTransaction.Name);
            }

                throw new NotImplementedException();
        }

        public void Save(Stream stream)
        {
            if (CurrentTransaction != null && !CurrentTransaction.IsFinished)
            {
                throw new Exceptions.TransactionNotFinishedException(CurrentTransaction.Name);
            }

            
            throw new NotImplementedException();
        }
    }
}
