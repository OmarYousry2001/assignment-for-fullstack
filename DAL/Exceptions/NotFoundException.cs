using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Exceptions
{
    public class NotFoundException : DataAccessException
    {
        public NotFoundException(string message, ILogger logger) : base(message, logger) { }

        public NotFoundException(string message, Exception inner, ILogger logger) : base(message, inner, logger) { }
    }
}
