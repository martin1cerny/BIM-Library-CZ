using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLData.Exceptions
{
    [Serializable]
    public class TransactionFinishedException : Exception
    {
        public TransactionFinishedException(string txnName) : base(String.Format("Transaction '{0}' has been finished before.", txnName)) { }
        public TransactionFinishedException(string txnName, Exception inner) : base(String.Format("Transaction '{0}' has been finished before.", txnName), inner) { }
        protected TransactionFinishedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class TransactionNotFinishedException : Exception
    {
        public TransactionNotFinishedException(string name) : base(String.Format("Transaction '{0}' is still open", name)) { }
        public TransactionNotFinishedException(string name, Exception inner) : base(String.Format("Transaction '{0}' is still open", name), inner) { }
        protected TransactionNotFinishedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class NoTransactionException : Exception
    {
        public NoTransactionException() { }
        public NoTransactionException(string message) : base(message) { }
        public NoTransactionException(string message, Exception inner) : base(message, inner) { }
        protected NoTransactionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class FieldNotFoundException : Exception
    {
        public FieldNotFoundException(string field) : base(String.Format("Field '{0}' doesn't exist.", field)) { }
        public FieldNotFoundException(string field, Exception inner) : base(String.Format("Field '{0}' doesn't exist.", field), inner) { }
        protected FieldNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
    [Serializable]
    public class ModelOriginException : Exception
    {
        public ModelOriginException() { }
        public ModelOriginException(string message) : base(message) { }
        public ModelOriginException(string message, Exception inner) : base(message, inner) { }
        protected ModelOriginException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
