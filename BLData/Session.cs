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
        private  List<Transaction> _session;
        private  int _current;
        internal  Transaction CurrentTransaction { get { return _current > -1 ? _session[_current] : null; } }

        public  bool HasUndo { get { return _current > 0; } }
        public  bool HasRedo { get { return _current < _session.Count - 1; } }
        public bool CanUndo { get {return HasUndo && CurrentTransaction.State != Transaction.StateEnum.OPENED;}}

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
            _session = new List<Transaction>();
            _current = -1;
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
