using System;
using System.Collections.Generic;
using System.Text;

namespace App.Utilities.Exceptions
{
    public class AppNotFoundException : Exception
    {
        public AppNotFoundException()
        {
        }

        public AppNotFoundException(string message)
            : base(message)
        {
        }

        public AppNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
