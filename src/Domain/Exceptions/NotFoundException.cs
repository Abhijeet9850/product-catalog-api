using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string name, object key)
            : base($"{name} with identifier ({key}) was not found.")
        {
        }
    }

    public class ValidationAppException : Exception
    {
        public ValidationAppException(string message) : base(message)
        {
        }
    }
}
