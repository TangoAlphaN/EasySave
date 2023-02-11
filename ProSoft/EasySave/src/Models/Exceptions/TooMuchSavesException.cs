namespace EasySave.src.Models.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception thrown when there is too much saves
    /// </summary>
    [Serializable]
    public class TooMuchSavesException : Exception
    {

        //All constructors are required by [Serializable]

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
