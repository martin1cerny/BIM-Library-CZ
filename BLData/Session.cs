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

        internal  void Undo()
        {
            if (!HasUndo)
                return;

            _session[_current].Undo();
            _current--;
        }

        internal  void Redo()
        {
            if (!HasRedo)
                return;

            _current++;
            _session[_current].Redo();
        }

        public  void DiscardHistory()
        {
            if (!HasUndo)
                return;
            var actual = CurrentTransaction;
            _session.Clear();
            _session.Add(actual);
            _current = 0;
        }

        
    }
}
