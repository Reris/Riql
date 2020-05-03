using System;

namespace Riql.Transpiler
{
    public class PropertyNotFoundException : RiqlException
    {
        public PropertyNotFoundException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}