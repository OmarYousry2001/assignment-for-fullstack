using DAL.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DAL.Contracts.Repositories.Generic
{
    public interface IRepository<T> where T : class
    {
        public IQueryable<T> GetTableNoTracking();
        public IQueryable<T> GetTableAsTracking();
        public Task<IEnumerable<T>> GetAllAsync();
        public Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate = null);
        Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            int? take = null,
            string includeProperties = "",
            params Expression<Func<T, object>>[] thenIncludeProperties);
        public Task<PaginatedDataModel<T>> GetPageAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);
        public Task<T> FindAsync(Expression<Func<T, bool>> predicate);
        public Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        public Task<bool> IsExistsAsync<TValue>(string key, TValue value);
        public IEnumerable<T> ExecuteStoredProcedure(string storedProcedureName, params SqlParameter[] parameters);
        TResult ExecuteScalarSqlFunction<TResult>(string sqlFunctionQuery, params object[] parameters);
        TResult ExecuteScalarRawSql<TResult>(string sqlQuery, params object[] parameters);
        public Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? predicate = null, params Expression<Func<T, object>>[] includes);
        public Task<T> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

    }
}
