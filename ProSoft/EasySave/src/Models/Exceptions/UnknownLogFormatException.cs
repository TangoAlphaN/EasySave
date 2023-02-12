namespace EasySave.src.Models.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception thrown when the update can't be checked
    /// </summary>
    [Serializable]
    public class UnknownLogFormatException : Exception
    {

        //All constructors are required by [Serializable]

        public UnknownLogFormatException()
        {
        }

        public UnknownLogFormatException(string message)
            : base(message)
        {
        }

        public UnknownLogFormatException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected UnknownLogFormatException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
