using System;
using JetBrains.Annotations;

namespace Riql.Transpiler
{
    public class RiqlException : Exception
    {
        public RiqlException(string message, [CanBeNull] Exception innerException = null)
            : base(message, innerException)
        {
        }
    }
}