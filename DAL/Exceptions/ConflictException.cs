using Serilog;

namespace DAL.Exceptions
{
    public class ConflictException : DataAccessException
    {
        public ConflictException(ILogger logger) : base(logger) { }

        public ConflictException(string message, ILogger logger) : base(message, logger) { }

        public ConflictException(string message, Exception inner, ILogger logger) : base(message, inner, logger) { }
    }
}
