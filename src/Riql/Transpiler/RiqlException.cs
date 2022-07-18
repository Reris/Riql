using System;

namespace Riql.Transpiler;

public class RiqlException : Exception
{
    public RiqlException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}