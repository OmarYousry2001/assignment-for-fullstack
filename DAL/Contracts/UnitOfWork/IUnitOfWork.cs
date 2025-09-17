using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Domains.Entities.Base;
using DAL.Contracts.Repositories.Generic;

namespace DAL.Contracts.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> Repository<T>() where T : class;
        ITableRepository<TD> TableRepository<TD>() where TD : BaseEntity;
        Task<IDbContextTransaction> BeginTransactionAsync();
         Task<int> Commit() ;
        void Rollback();
        Task DisposeAsync(); // Changed from Task ValueTask to Task for simplicity
        DbContext GetContext(); // Add this method
    }
}
