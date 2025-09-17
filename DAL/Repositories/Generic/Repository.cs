using DAL.ApplicationContext;
using DAL.Contracts.Repositories.Generic;
using DAL.Exceptions;
using DAL.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Linq.Expressions;

namespace DAL.Repositories.Generic
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger _logger;
        protected DbSet<T> DbSet => _dbContext.Set<T>();

        public Repository(ApplicationDbContext dbContext, ILogger logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext), "Database context cannot be null.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger instance cannot be null.");
        }
        /// <summary>
        /// Retrieves the table as a queryable with **no tracking**, 
        /// which improves performance for read-only operations.
        /// </summary>
        public IQueryable<T> GetTableNoTracking()
        {
            return DbSet.AsNoTracking().AsQueryable();
        }
        /// <summary>
        /// Retrieves the table as a queryable with **tracking enabled**, 
        /// which is useful when you intend to update the entities.
        /// </summary>
        public IQueryable<T> GetTableAsTracking()
        {
            return DbSet.AsQueryable();

        }
        /// <summary>
        /// Retrieves all entities.
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                return await DbSet.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                HandleException(nameof(GetAllAsync), "An error occurred while retrieving data.", $"Error occurred while retrieving all entities of type {typeof(T).Name}.", ex);
                throw;
            }
        }
        /// <summary>
        /// Retrieves entities based on a predicate.
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate = null)
        {
            try
            {
                if (predicate == null)
                    return await DbSet.AsNoTracking().ToListAsync();

                return await DbSet.Where(predicate).AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                HandleException(nameof(GetAsync), "An error occurred while filtering data.", $"Error occurred while filtering entities of type {typeof(T).Name}.", ex);
                throw; // Rethrow the exception after logging
            }
        }
        /// <summary>
        /// Retrieves entities with optional filter and includes.  
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>>? predicate = null,
            params Expression<Func<T, object>>[] includes)
        {
            try
            {
                IQueryable<T> query = DbSet.AsNoTracking();

                if (includes != null && includes.Length > 0)
                {
                    foreach (var include in includes)
                    {
                        query = query.Include(include);
                    }
                }

                if (predicate != null)
                {
                    query = query.Where(predicate);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                HandleException(nameof(GetAsync), "An error occurred while retrieving data.", $"Error occurred while retrieving entities of type {typeof(T).Name}.", ex);
                throw; // Rethrow the exception after logging
            }
        }
        /// <summary>
        /// Retrieves entities with optional filtering, ordering, and eager loading.
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetAsync(
       Expression<Func<T, bool>> predicate = null,
       Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
       int? take = null,
       string includeProperties = "",
       params Expression<Func<T, object>>[] thenIncludeProperties)
        {
            try
            {
                IQueryable<T> query = DbSet.AsNoTracking();

                if (predicate != null)
                {
                    query = query.Where(predicate);
                }

                if (!string.IsNullOrEmpty(includeProperties))
                {
                    foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includeProperty.Trim());
                    }
                }

                if (thenIncludeProperties != null && thenIncludeProperties.Length > 0)
                {
                    foreach (var thenIncludeProperty in thenIncludeProperties)
                    {
                        query = query.Include(thenIncludeProperty);
                    }
                }

                if (orderBy != null)
                {
                    query = orderBy(query);
                }

                if (take.HasValue && take.Value > 0)
                {
                    query = query.Take(take.Value);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                HandleException(nameof(GetAsync), "An error occurred while retrieving data with advanced options.", $"Error occurred while retrieving entities of type {typeof(T).Name} with advanced options.", ex);
                throw; // Rethrow the exception after logging
            }
        }
        /// <summary>
        /// Retrieves paginated data.
        /// </summary>
        public virtual async Task<PaginatedDataModel<T>> GetPageAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)
        {
            try
            {
                IQueryable<T> query = DbSet.AsNoTracking();

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                if (orderBy != null)
                {
                    query = orderBy(query);
                }

                int totalCount = await query.CountAsync();

                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

                var data = await query.ToListAsync();

                return new PaginatedDataModel<T>(data, totalCount);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(
                    $"Error occurred in {nameof(GetPageAsync)} method for entity type {typeof(T).Name}.",
                    ex,
                    _logger
                );
            }
        }
       



        /// <summary>
        /// Finds the first entity matching the predicate.
        /// </summary>
        public virtual async Task<T> FindAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await DbSet.AsNoTracking().FirstOrDefaultAsync(predicate);
            }
            catch (Exception ex)
            {
                HandleException(nameof(FindAsync), "An error occurred while finding an entity.", $"Error occurred while finding an entity of type {typeof(T).Name}.", ex);
                throw;
            }
        }
        /// <summary>
        /// Finds the first entity matching the predicate  with  related data included..
        /// </summary>
        public virtual async Task<T> FindAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes)
        {
            try
            {
                IQueryable<T> query = DbSet.AsNoTracking();

                if (includes != null && includes.Length > 0)
                {
                    foreach (var include in includes)
                    {
                        query = query.Include(include);
                    }
                }

                if (predicate != null)
                {
                    query = query.Where(predicate);
                }

                var sql = query.ToQueryString();
                Console.WriteLine(sql);
                return await query.FirstOrDefaultAsync();
                //return await query.SingleOrDefaultAsync();

               

            }
            catch (Exception ex)
            {
                HandleException(nameof(FindAsync), "An error occurred while finding an entity.", $"Error occurred while finding an entity of type {typeof(T).Name}.", ex);
                throw;
            }
        }
        /// <summary>
        /// Counts entities matching the predicate.
        /// </summary>
        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await DbSet.CountAsync(predicate);
            }
            catch (Exception ex)
            {
                HandleException(nameof(CountAsync), "An error occurred while counting entities.", $"Error occurred while counting entities of type {typeof(T).Name}.", ex);
                throw; // Rethrow the exception after logging
            }
        }
        /// <summary>
        /// Checks if an entity exists based on a key-value pair.
        /// </summary>
        public async Task<bool> IsExistsAsync<TValue>(string key, TValue value)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                    throw new ArgumentException("Key cannot be null or empty.", nameof(key));

                var parameter = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, key);
                var constant = Expression.Constant(value);
                var equality = Expression.Equal(property, constant);
                var lambda = Expression.Lambda<Func<T, bool>>(equality, parameter);

                return await DbSet.AnyAsync(lambda);
            }
            catch (Exception ex)
            {
                HandleException(nameof(IsExistsAsync), "An error occurred while checking existence.", $"Error occurred while checking existence for entity type {typeof(T).Name}.", ex);
                throw; // Rethrow the exception after logging
            }
        }
        /// <summary>
        /// Executes a stored procedure and maps the results to the entity class.
        /// </summary>
        public async Task<IEnumerable<T>> ExecuteStoredProcedureAsync(string storedProcedureName, params SqlParameter[] parameters)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(storedProcedureName))
                    throw new ArgumentException("Stored procedure name cannot be null or empty.", nameof(storedProcedureName));

                // Handle null parameters safely
                var safeParameters = parameters?.Select(param =>
                {
                    if (param.Value == null)
                    {
                        param.Value = DBNull.Value; // Convert null to DBNull.Value
                    }
                    return param;
                }).ToArray();

                return await DbSet
                    .FromSqlRaw(storedProcedureName, safeParameters)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 50000) // Custom error from stored procedure
                {
                    _logger.Error(sqlEx, nameof(ExecuteStoredProcedureAsync),
                        $"Custom error from stored procedure: {storedProcedureName}");
                    throw new DataAccessException(sqlEx.Message, sqlEx, _logger);
                }

                HandleException(nameof(ExecuteStoredProcedureAsync),
                    "An SQL error occurred while executing the stored procedure.",
                    $"SQL error occurred while executing stored procedure '[SensitiveData]'.", sqlEx);
                throw; // Rethrow the exception after logging
            }
            catch (Exception ex)
            {
                HandleException(nameof(ExecuteStoredProcedureAsync),
                    "An error occurred while executing the stored procedure.",
                    $"Error occurred while executing stored procedure '[SensitiveData]'.", ex);
                throw; // Rethrow the exception after logging
            }
        }

        /// <summary>
        /// Executes a custom SQL function and returns a single scalar value.
        /// </summary>
        public TResult ExecuteScalarSqlFunction<TResult>(string sqlFunctionQuery, params object[] parameters)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sqlFunctionQuery))
                    throw new ArgumentException("SQL function query cannot be null or empty.", nameof(sqlFunctionQuery));

                // Use ADO.NET to execute the scalar query
                return ExecuteScalar<TResult>(sqlFunctionQuery, parameters);
            }
            catch (Exception ex)
            {
                HandleException(nameof(ExecuteScalarSqlFunction), "An error occurred while executing the scalar SQL function.", $"Error occurred while executing scalar SQL function '[SensitiveData]'.", ex);
                throw; // Rethrow the exception after logging
            }
        }
        /// <summary>
        /// Executes a raw SQL query and returns a single scalar value.
        /// </summary>
        public TResult ExecuteScalarRawSql<TResult>(string sqlQuery, params object[] parameters)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sqlQuery))
                    throw new ArgumentException("SQL query cannot be null or empty.", nameof(sqlQuery));

                // Use ADO.NET to execute the scalar query
                return ExecuteScalar<TResult>(sqlQuery, parameters);
            }
            catch (Exception ex)
            {
                HandleException(nameof(ExecuteScalarRawSql), "An error occurred while executing the scalar raw SQL query.", $"Error occurred while executing scalar raw SQL query '[SensitiveData]'.", ex);
                throw; // Rethrow the exception after logging
            }
        }
        /// <summary>
        /// Internal method to execute a scalar query using ADO.NET.
        /// </summary>
        private TResult ExecuteScalar<TResult>(string sql, params object[] parameters)
        {
            using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                command.CommandType = System.Data.CommandType.Text;

                // Add parameters to the command
                if (parameters != null && parameters.Length > 0)
                {
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = $"@p{i}";
                        parameter.Value = parameters[i] ?? DBNull.Value;
                        command.Parameters.Add(parameter);
                    }
                }

                // Open the connection if it's closed
                if (command.Connection.State == System.Data.ConnectionState.Closed)
                {
                    command.Connection.Open();
                }

                // Execute the scalar query
                var result = command.ExecuteScalar();

                // Convert the result to the desired type
                return result == DBNull.Value ? default : (TResult)Convert.ChangeType(result, typeof(TResult));
            }
        }
        /// <summary>
        /// Logs and rethrows exceptions with detailed information.
        /// </summary>
        protected void HandleException(string methodName, string userMessage, string logMessage, Exception ex)
        {
            // Log detailed message for analysis
            _logger.Error(ex, $"[{methodName}] {logMessage}");

            // Throw exception with a general message for users
            throw new DataAccessException(userMessage, ex, _logger);
        }
        /// <summary>
        /// Validates pagination parameters.
        /// </summary>
        private void ValidatePaginationParameters(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0)
                throw new ArgumentException("Page number must be greater than zero.", nameof(pageNumber));

            if (pageSize <= 0)
                throw new ArgumentException("Page size must be greater than zero.", nameof(pageSize));
        }

       
    }
}