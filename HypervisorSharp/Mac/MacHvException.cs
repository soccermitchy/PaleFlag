using System;
using System.Collections.Generic;
using System.Text;

namespace HypervisorSharp.Mac
{
    public class MacHvException : Exception
    {
        public MacHvException() : base()
        {
        }

        public MacHvException(string message) : base(message)
        {
        }

        public MacHvException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public override string ToString() => base.Message;
    }
}
