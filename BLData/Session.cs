using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLData
{
    public class Session
    {
        private  List<Transaction> _session = new List<Transaction>();
        private  int _current = -1;
        internal  Transaction CurrentTransaction { get { return _current > -1 ? _session[_current] : null; } }

        public  bool HasUndo { get { return _current >= 0; } }
        public  bool HasRedo { get { return _current < _session.Count - 1; } }
        public bool CanUndo { get {return HasUndo && CurrentTransaction.State != Transaction.StateEnum.OPENED;}}
        public bool IsDirty { get { return CurrentTransaction != null && !CurrentTransaction.IsSaved; } }

        public Guid GetRestorePoint()
        {
            if (CurrentTransaction == null) return Guid.Empty;

            var lastClosed = _session.LastOrDefault(t => t.IsFinished);
            if (lastClosed != null) return lastClosed.ID;

            return Guid.Empty;
        }

        public void RestoreToPoint(Guid id)
        {
            Transaction target = null;
            if (id != Guid.Empty)
            {
                target = _session.FirstOrDefault(t => t.ID == id);
                if (target != null)
                    throw new Exception("This restore point doesn't exist.");
            }
            while (CurrentTransaction != target)
                Undo();
        }

        public IEnumerable<string> UndoTransactions
        {
            get
            {
                for (int i = 0; i < _current + 1; i++)
                {
                    yield return _session[i].Name;
                }
            }
        }

        public IEnumerable<string> RedoTransactions
        {
            get
            {
                for (int i = _current + 1; i < _session.Count; i++)
                {
                    yield return _session[i].Name;
                }
            }
        }

        internal Session()
        {
        }


        internal void Add(Transaction txn)
        {
            if (CurrentTransaction != null && CurrentTransaction.State == Transaction.StateEnum.OPENED)
                throw new Exceptions.TransactionNotFinishedException(CurrentTransaction.Name);

            //remove all redo transactions ahead
            if (HasRedo)
                _session.RemoveRange(_current + 1, _session.Count - _current - 1);

            //purge all transactions with no actions
            _session.RemoveAll(t => t.IsFinished && !t.IsDirty);

            _session.Add(txn);
            _current = _session.Count - 1;
        }

        public  void Undo()
        {
            if (!HasUndo)
                return;

            _session[_current].Undo();
            _current--;
        }

        public  void Redo()
        {
            if (!HasRedo)
                return;

            _current++;
            _session[_current].Redo();
        }

        public  void DiscardHistory()
        {
            if (CurrentTransaction != null && CurrentTransaction.State == Transaction.StateEnum.OPENED)
                throw new Exceptions.TransactionNotFinishedException(CurrentTransaction.Name);

            if (!HasUndo)
                return;
            _session.Clear();
            _current = -1;
        }

        
    }
}
