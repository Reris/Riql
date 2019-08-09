using System;
using JetBrains.Annotations;

namespace Riql.Transpiler
{
    public class PropertyNotFoundException : RiqlException
    {
        public PropertyNotFoundException(string message, [CanBeNull] Exception innerException = null)
            : base(message, innerException)
        {
        }
    }
}