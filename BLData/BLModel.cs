using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLData
{
    public class BLModel
    {
        private Session _session = new Session();
        internal Transaction CurrentTransaction { get { return _session.CurrentTransaction; } }


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
            if (CurrentTransaction == null || CurrentTransaction.IsFinished) throw new Exceptions.NoTransactionException("Transaction must be opened to be able to change the model.");

            var type = entity.GetType();
            var info = type.GetField(field, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
            if (info == null)
                throw new Exceptions.FieldNotFoundException(field);


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

        public T New<T>(Action<T> init) where T: BLModelEntity, new()
        {
            var entity = new T();
            entity._model = this;

            //add to resources of the model in transaction

            return entity;
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
