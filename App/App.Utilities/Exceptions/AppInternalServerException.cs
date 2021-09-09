using System;
using System.Collections.Generic;
using System.Text;

namespace App.Utilities.Exceptions
{
    public class AppInternalServerException : Exception
    {
        public AppInternalServerException()
        {
        }

        public AppInternalServerException(string message)
            : base(message)
        {
        }

        public AppInternalServerException(string message, Exception inner)
            : base(message, inner)
        {
        }
        public AppInternalServerException(Exception ex):base(ex.Message)
        {
        }
    }
}
