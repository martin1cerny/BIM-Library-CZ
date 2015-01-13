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
        private bool _isEntityList = false;
        [XmlIgnore]
        public BLModel Model { get { return _model; } }

        internal void SetModel(BLModel model)
        {
            _model = model;
            if (this.All(i => i is BLEntity))
            {
                foreach (var item in this)
                {
                    (item as BLEntity).SetModel(_model);
                }
            }
        }

        internal string Validate()
        {
            var result = "";
            if (this.All(e => e is BLEntity))
                foreach (var e in this)
                    result += (e as BLEntity).Validate();
            return result;
        }

        public BList(BLModel model)
        {
            _model = model;
            _isEntityList = typeof(IBLModelEntity).IsAssignableFrom(typeof(T));
        }

        public BList()
        {
            _isEntityList = typeof(IBLModelEntity).IsAssignableFrom(typeof(T));
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
            if (_model == null)
                doAction();
            else
                _model.Transact<T>(this, doAction, undoAction);
        }

        protected override void InsertItem(int index, T item)
        {
            Action doAction = () =>  base.InsertItem(index, item);
            Action undoAction = () => { var i = this.IndexOf(item); base.RemoveItem(i); };

            if (_model == null)
                doAction();
            else
            {
                if (_isEntityList && (item as IBLModelEntity).Model != _model) throw new Exceptions.ModelOriginException("Entity doesn't live in the same model as this list.");
                _model.Transact<T>(this, doAction, undoAction);
            }
        }

        protected override void SetItem(int index, T item)
        {
            var oldItem = this[index];
            Action doAction = () => base.SetItem(index, item);
            Action undoAction = () => base.SetItem(index, oldItem);

            if (_model == null)
                doAction();
            else
            {
                if (_isEntityList && (item as IBLModelEntity).Model != _model) throw new Exceptions.ModelOriginException("Entity doesn't live in the same model as this list.");
                _model.Transact<T>(this, doAction, undoAction);
            }
        }

        protected override void RemoveItem(int index)
        {
            var oldItem = this[index];
            Action doAction = () => base.RemoveItem(index);
            Action undoAction = () => base.InsertItem(index, oldItem);

            if (_model == null)
                doAction();
            else
                _model.Transact<T>(this, doAction, undoAction);
        }
      
    }
}
