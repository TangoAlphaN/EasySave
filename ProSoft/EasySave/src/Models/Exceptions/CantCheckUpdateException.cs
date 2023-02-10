namespace EasySave.src.Models.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class CantCheckUpdateException : Exception
    {
        public CantCheckUpdateException()
        {
        }

        public CantCheckUpdateException(string message)
            : base(message)
        {
        }

        public CantCheckUpdateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CantCheckUpdateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
