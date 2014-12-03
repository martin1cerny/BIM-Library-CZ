using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BLData
{
    public class BList<T> : ObservableCollection<T>
    {
        private BLModel _model;
        [XmlIgnore]
        public BLModel Model { get { return _model; } }

        internal void SetModel(BLModel model)
        {
            _model = model;
        }

        public BList(BLModel model)
        {
            _model = model;
        }

        public BList()
        {

        }

        protected override void ClearItems()
        {
            var items = this.ToList();
            Action doAction = () => {
                base.ClearItems();
            };
            Action undoAction = () => {
                for(var i = 0; i < items.Count; i++)
                    base.InsertItem(i, items[i]);
            };
            if (_model != null && _model.CurrentTransaction == null) throw new Exceptions.NoTransactionException("Transaction must be opened to be able to change this list.");
            if (_model != null) _model.CurrentTransaction.AddAction(undoAction, doAction);
            doAction();
        }

        protected override void InsertItem(int index, T item)
        {
            Action doAction = () =>  base.InsertItem(index, item);
            Action undoAction = () => { var i = this.IndexOf(item); base.RemoveItem(i); };

            if (_model != null && _model.CurrentTransaction == null) throw new Exceptions.NoTransactionException("Transaction must be opened to be able to change this list.");
            if (_model != null)
            {
                var entity = item as IBLModelEntity;
                if (entity != null && entity.Model != _model) throw new Exceptions.ModelOriginException("Entity doesn't live in the same model as this list.");
                _model.CurrentTransaction.AddAction(undoAction, doAction);
            }
            doAction();
        }

        protected override void SetItem(int index, T item)
        {
            var oldItem = this[index];
            Action doAction = () => base.SetItem(index, item);
            Action undoAction = () => base.SetItem(index, oldItem);

            if (_model != null && _model.CurrentTransaction == null) throw new Exceptions.NoTransactionException("Transaction must be opened to be able to change this list.");
            if (_model != null)
            {
                var entity = item as IBLModelEntity;
                if (entity != null && entity.Model != _model) throw new Exceptions.ModelOriginException("Entity doesn't live in the same model as this list.");
                _model.CurrentTransaction.AddAction(undoAction, doAction);
            }
            doAction();

        }

        protected override void RemoveItem(int index)
        {
            var oldItem = this[index];
            Action doAction = () => base.RemoveItem(index);
            Action undoAction = () => base.InsertItem(index, oldItem);

            if (_model != null && _model.CurrentTransaction == null) throw new Exceptions.NoTransactionException("Transaction must be opened to be able to change this list.");
            if (_model != null) _model.CurrentTransaction.AddAction(undoAction, doAction);
            doAction();
        }
      
    }
}
