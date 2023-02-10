namespace EasySave.src.Models.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class TooMuchSavesException : Exception
    {
        public TooMuchSavesException()
        {
        }

        public TooMuchSavesException(string message)
            : base(message)
        {
        }

        public TooMuchSavesException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected TooMuchSavesException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
