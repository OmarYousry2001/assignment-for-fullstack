using Serilog;

namespace DAL.Exceptions
{
    public class DataAccessException : Exception
    {
        public DataAccessException(ILogger logger)
        {
            logger.Error(this, "An error occurred in the data access layer.");
        }

        public DataAccessException(string message, ILogger logger) : base(message)
        {
            logger.Error(this, $"An error occurred in the data access layer: {message}");
        }

        public DataAccessException(string message, Exception inner, ILogger logger) : base(message, inner)
        {
            // Log the error without exposing sensitive details
            logger.Error(inner, $"An error occurred in the data access layer: {message}");
        }
    }
}

