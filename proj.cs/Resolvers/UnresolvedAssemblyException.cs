using System;
using System.Runtime.Serialization;

namespace AtomPackageManager.Resolvers
{
    [Serializable]
    internal class UnresolvedAssemblyException : Exception
    {
        public UnresolvedAssemblyException()
        {
        }

        public UnresolvedAssemblyException(string message) : base(message)
        {
        }

        public UnresolvedAssemblyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnresolvedAssemblyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}