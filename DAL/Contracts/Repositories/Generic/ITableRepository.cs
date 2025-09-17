using Domains.Entities.Base;
using System.Linq.Expressions;

namespace DAL.Contracts.Repositories.Generic
{
    public interface ITableRepository<T> : IRepository<T> where T : BaseEntity
    {

        Task<T> FindByIdAsync(Guid id);
        Task<bool> SaveAsync(T model, Guid userId);
        bool Save(T model, Guid userId, out Guid id);
        bool Create(T model, Guid creatorId, out Guid id);
        public  Task<bool> CreateAsync(T model, Guid creatorId);
        public  Task<bool> UpdateAsync(T model, Guid updaterId);
        bool Update(T model, Guid updaterId, out Guid id);
        public Task<bool> UpdateCurrentStateAsync(Guid entityId, Guid userId, int newValue = 0);
        public Task<bool> AddRangeAsync(IEnumerable<T> entities, Guid userId);
        public bool SaveRange(IEnumerable<T> entities, Guid userId);
        public bool ResetRange(IEnumerable<T> newEntities, Guid userId, Expression<Func<T, bool>> filterForOldEntities);
        public bool Remove(Guid id);
        public Task<bool> DeleteRangeAsync(IEnumerable<Guid> ids);
        Task<bool> SaveChangeAsync();
        public   Task<T> AddAndReturnAsync(T model, Guid creatorId);
    }
}
