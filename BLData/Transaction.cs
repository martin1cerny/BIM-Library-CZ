using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLData.Exceptions;

namespace BLData
{
    public class Transaction : IDisposable
    {
        private StateEnum _state;
        internal StateEnum State { get { return _state; } }
        public bool IsFinished { get { return _state != StateEnum.OPENED; } }
        public bool IsDirty { get { return _undoActions.Count > 0; } }
        public bool IsSaved { get; set; }

        private List<Action> _undoActions;
        private List<Action> _redoActions;

        private string _name;
        public string Name
        {
            get { return _name; }
        }

        internal Transaction(string name)
        {
            _name = name;
            _state = StateEnum.OPENED;
            _undoActions = new List<Action>();
            _redoActions = new List<Action>();
        }

        //action which would reverse the change
        internal void AddAction(Action undoAction, Action redoAction)
        {
            if (_state != StateEnum.OPENED)
                throw new TransactionFinishedException(_name);

            _undoActions.Add(undoAction);
            _redoActions.Add(redoAction);
        }

        public void Commit()
        {
            if (_state != StateEnum.OPENED)
                throw new TransactionFinishedException(_name);

            //do nothing. Changes are done by default;
            _state = StateEnum.FINISHED;
        }

        public void RollBack()
        {
            if (_state != StateEnum.OPENED)
                throw new TransactionFinishedException(_name);
            foreach (var action in _undoActions)
            {
                action();
            }
            _state = StateEnum.FINISHED;
            //remove all actions
            _undoActions = new List<Action>();
            _redoActions = new List<Action>();
        }

        internal void Undo()
        {
            if (_state != StateEnum.FINISHED && _state != StateEnum.REDONE) 
                throw new TransactionNotFinishedException(_name);
            foreach (var action in _undoActions)
            {
                action();
            }
            _state = StateEnum.UNDONE;
        }

        internal void Redo()
        {
            if (_state != StateEnum.FINISHED && _state != StateEnum.UNDONE) 
                throw new TransactionNotFinishedException(_name);
            foreach (var action in _redoActions)
            {
                action();
            }
            _state = StateEnum.REDONE;
        }

        public void Dispose()
        {
            if (_state == StateEnum.OPENED)
                RollBack();
        }

        internal enum StateEnum
        {
            FINISHED,
            OPENED,
            UNDONE,
            REDONE
        }

    }
}
