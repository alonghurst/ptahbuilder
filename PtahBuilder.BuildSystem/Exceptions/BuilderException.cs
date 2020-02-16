using System;
using System.Collections.Generic;
using System.Text;

namespace PtahBuilder.BuildSystem.Exceptions
{
    public class BuilderException : Exception
    {
        public BuilderException(string message, Exception innerException)
        : base(message, innerException)
        {
        }
    }
}
