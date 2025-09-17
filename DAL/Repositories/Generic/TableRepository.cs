using DAL.ApplicationContext;
using DAL.Contracts.Repositories.Generic;
using DAL.Exceptions;
using DAL.Models;
using Domains.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Linq.Expressions;

namespace DAL.Repositories.Generic
{
    public class TableRepository<T> : Repository<T>, ITableRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger _logger;
        protected DbSet<T> DbSet => _dbContext.Set<T>();

        public TableRepository(ApplicationDbContext dbContext, ILogger logger) : base(dbContext, logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext), "Database context cannot be null.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger instance cannot be null.");
        }

        /// <summary>
        /// Retrieves all active entities (CurrentState == 1).
        /// </summary>
        public override async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                return await DbSet.AsNoTracking()
                                  .Where(e => e.CurrentState == 1)
                                  .ToListAsync();
            }
            catch (Exception ex)
            {
                HandleException(nameof(GetAllAsync), $"Error occurred while retrieving all active entities of type {typeof(T).Name}.", ex);
                throw;
            }
        }
        /// <summary>
        /// Retrieves entities based on a predicate, filtering only active records.
        /// </summary>
        public override async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate = null)
        {
            try
            {
                IQueryable<T> query = DbSet.AsNoTracking().Where(e => e.CurrentState == 1);

                if (predicate != null)
                    query = query.Where(predicate);

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                HandleException(nameof(GetAsync), $"Error occurred while filtering active entities of type {typeof(T).Name}.", ex);
                throw;
            }
        }
        /// <summary>
        /// Retrieves paginated data.
        /// </summary>
        public override async Task<PaginatedDataModel<T>> GetPageAsync(
         int pageNumber,
         int pageSize,
         Expression<Func<T, bool>> filter = null,
         Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)
        {
            try
            {
                IQueryable<T> query = DbSet.AsNoTracking().Where(e => e.CurrentState == 1); // فلترة السجلات النشطة فقط

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                if (orderBy != null)
                {
                    query = orderBy(query);
                }

                int totalCount = await query.CountAsync(); // إجمالي عدد العناصر

                var data = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

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
        /// Finds an entity by its ID.
        /// </summary>
        /// <summary>
        /// Finds an entity by its ID asynchronously.
        /// </summary>
        public async Task<T> FindByIdAsync(Guid id)
        {
            try
            {
                var data = await DbSet.FindAsync(id);
                if (data == null)
                    throw new NotFoundException($"Entity of type {typeof(T).Name} with ID {id} not found.", _logger);

                return data;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                HandleException(nameof(FindByIdAsync), $"Error occurred while finding an entity of type {typeof(T).Name} with ID {id}.", ex);
                throw;
            }
        }
        /// <summary>
        /// Saves or updates an entity based on its key.
        /// </summary>
        public async Task<bool> SaveAsync(T model, Guid userId)
        {
            try
            {
                if (model.Id == Guid.Empty)
                    return await CreateAsync(model, userId);
                else
                    return await UpdateAsync(model, userId);
            }
            catch (Exception ex)
            {
                HandleException(nameof(SaveAsync), $"Error occurred while saving or updating an entity of type {typeof(T).Name}.", ex);
                throw;
            }
        }
        /// <summary>
        /// Asynchronously creates a new entity.
        /// </summary>
        public async Task<bool> CreateAsync(T model, Guid creatorId)
        {
            try
            {
                model.Id = Guid.NewGuid();
                model.CreatedDateUtc = DateTime.UtcNow;
                model.CreatedBy = creatorId;
                model.CurrentState = 1;

                await DbSet.AddAsync(model);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (DbUpdateException dbEx)
            {
                HandleException(nameof(CreateAsync), $"Conflict error while creating an entity of type {typeof(T).Name}.", dbEx);
                throw;
            }
            catch (Exception ex)
            {
                HandleException(nameof(CreateAsync), $"Error occurred while creating an entity of type {typeof(T).Name}.", ex);
                throw;
            }
        }
        /// <summary>
        /// Asynchronously updates an existing entity.
        /// </summary>
        public async Task<bool> UpdateAsync(T model, Guid updaterId)
        {
            try
            {
                var existingEntity = await DbSet.AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == model.Id);

                if (existingEntity == null)
                    throw new DataAccessException($"Entity with key {model.Id} not found.", _logger);

                model.UpdatedDateUtc = DateTime.UtcNow;
                model.UpdatedBy = updaterId;
                model.CreatedBy = existingEntity.CreatedBy;
                model.CreatedDateUtc = existingEntity.CreatedDateUtc;
                model.CurrentState = existingEntity.CurrentState;

                _dbContext.Entry(model).State = EntityState.Modified;

                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (DbUpdateConcurrencyException concurrencyEx)
            {
                HandleException(nameof(UpdateAsync), $"Concurrency error while updating an entity of type {typeof(T).Name}, ID {model.Id}.", concurrencyEx);
                throw;
            }
            catch (Exception ex)
            {
                HandleException(nameof(UpdateAsync), $"Error occurred while updating an entity of type {typeof(T).Name}, ID {model.Id}.", ex);
                throw;
            }
        }
        /// <summary>
        /// Saves or updates an entity based on its key and outputs the ID of the saved entity.
        /// </summary>
        public bool Save(T model, Guid userId, out Guid id)
        {
            try
            {
                if (model.Id == Guid.Empty)
                    return Create(model, userId, out id);
                else
                    return Update(model, userId, out id);
            }
            catch (Exception ex)
            {
                HandleException(nameof(Save), $"Error occurred while saving or updating an entity of type {typeof(T).Name}.", ex);
                throw;
            }
        }
        /// <summary>
        /// Creates a new entity.
        /// </summary>
        public bool Create(T model, Guid creatorId, out Guid id)
        {
            try
            {
                id = Guid.NewGuid();

                model.Id = id;
                model.CreatedDateUtc = DateTime.UtcNow;
                model.CreatedBy = creatorId;
                model.CurrentState = 1;

                DbSet.Add(model);
                return _dbContext.SaveChanges() > 0;
            }
            catch (DbUpdateException dbEx)
            {
                HandleException(nameof(Create), $"Conflict error while creating an entity of type {typeof(T).Name}.", dbEx);
                throw;
            }
            catch (Exception ex)
            {
                HandleException(nameof(Create), $"Error occurred while creating an entity of type {typeof(T).Name}.", ex);
                throw;
            }
        }
        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        public bool Update(T model, Guid updaterId, out Guid id)
        {
            try
            {
                var existingEntity = DbSet.AsNoTracking().FirstOrDefault(e => e.Id == model.Id);
                id = model.Id;

                if (existingEntity == null)
                    throw new DataAccessException($"Entity with key {id} not found.", _logger);

                model.UpdatedDateUtc = DateTime.UtcNow;
                model.UpdatedBy = updaterId;
                model.CreatedBy = existingEntity.CreatedBy;
                model.CurrentState = existingEntity.CurrentState;
                model.CreatedDateUtc = existingEntity.CreatedDateUtc;

                DbSet.Entry(model).State = EntityState.Modified;

                return _dbContext.SaveChanges() > 0;
            }
            catch (DbUpdateConcurrencyException concurrencyEx)
            {
                HandleException(nameof(Update), $"Concurrency error while updating an entity of type {typeof(T).Name}, ID {model.Id}.", concurrencyEx);
                throw;
            }
            catch (Exception ex)
            {
                HandleException(nameof(Update), $"Error occurred while updating an entity of type {typeof(T).Name}, ID {model.Id}.", ex);
                throw;
            }
        }
        /// <summary>
        /// Updates the CurrentState of an entity.
        /// </summary>
        /// <summary>
        /// Asynchronously updates the CurrentState of an entity.
        /// </summary>
        public async Task<bool> UpdateCurrentStateAsync(Guid entityId, Guid userId, int newValue = 0)
        {
            try
            {
                var entity = await DbSet.FindAsync(entityId);

                if (entity == null)
                    throw new NotFoundException($"Entity of type {typeof(T).Name} with ID {entityId} not found.", _logger);

                entity.CurrentState = newValue;
                entity.UpdatedBy = userId;

                DbSet.Update(entity);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (DbUpdateException dbEx)
            {
                HandleException(nameof(UpdateCurrentStateAsync), $"Database update error while updating CurrentState for entity type {typeof(T).Name}, ID {entityId}.", dbEx);
                throw;
            }
            catch (Exception ex)
            {
                HandleException(nameof(UpdateCurrentStateAsync), $"Error occurred while updating CurrentState for entity type {typeof(T).Name}, ID {entityId}.", ex);
                throw;
            }
        }
        /// <summary>
        /// Saves all pending changes to the database.
        /// </summary>
        public async Task<bool> SaveChangeAsync()
        {
            try
            {
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (DbUpdateConcurrencyException concurrencyEx)
            {
                HandleException(nameof(SaveChangeAsync), $"Concurrency error while saving changes for entity type {typeof(T).Name}.", concurrencyEx);
                throw;
            }
            catch (Exception ex)
            {
                HandleException(nameof(SaveChangeAsync), $"Error occurred while saving changes for entity type {typeof(T).Name}.", ex);
                throw;
            }
        }
        /// <summary>
        /// Adds multiple new entities to the database
        /// </summary>
        public async Task<bool> AddRangeAsync(IEnumerable<T> entities, Guid userId)
        {
            try
            {
                var utcNow = DateTime.UtcNow;

                foreach (var entity in entities)
                {
                    entity.Id = Guid.NewGuid();
                    entity.CreatedDateUtc = utcNow;
                    entity.CreatedBy = userId;
                    entity.CurrentState = 1;
                }

                await DbSet.AddRangeAsync(entities);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                HandleException(nameof(AddRangeAsync), $"Error occurred while adding multiple entities of type {typeof(T).Name}.", ex);
                throw;
            }
        }

        /// <summary>
        /// Updates multiple existing entities in the database
        /// </summary>
        public bool UpdateRange(IEnumerable<T> entities, Guid userId)
        {
            try
            {
                var utcNow = DateTime.UtcNow;
                var ids = entities.Select(e => e.Id).ToList();
                var existingEntities = DbSet.Where(e => ids.Contains(e.Id)).ToList();

                foreach (var entity in entities)
                {
                    var existingEntity = existingEntities.FirstOrDefault(e => e.Id == entity.Id);
                    if (existingEntity == null)
                        continue;

                    entity.CreatedBy = existingEntity.CreatedBy;
                    entity.CreatedDateUtc = existingEntity.CreatedDateUtc;
                    entity.UpdatedBy = userId;
                    entity.UpdatedDateUtc = utcNow;
                    entity.CurrentState = existingEntity.CurrentState;

                    DbSet.Entry(entity).State = EntityState.Modified;
                }

                return _dbContext.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                HandleException(nameof(UpdateRange), $"Error occurred while updating multiple entities of type {typeof(T).Name}.", ex);
                throw;
            }
        }
        /// <summary>
        /// Saves or updates multiple entities based on their keys and persists the changes to the database.
        /// </summary>
        public bool SaveRange(IEnumerable<T> entities, Guid userId)
        {
            try
            {
                if (!entities.Any())
                    return false;

                foreach (var entity in entities)   
                {
                    if (entity.Id == Guid.Empty)   
                    {
                  
                        Create(entity, userId, out Guid id);
                    }
                    else                         
                    {
                        Update(entity, userId , out Guid id);
                      
                    }
                }

                return _dbContext.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                HandleException(nameof(SaveRange),
                    $"Error occurred while SaveRange multiple entities of type {typeof(T).Name}.", ex);
                throw;
            }
        }
        /// <summary>
        /// Removes existing entities filtered by the given predicate and adds a new set of entities, replacing the old data.
        /// </summary>
        public bool ResetRange(IEnumerable<T> newEntities, Guid userId, Expression<Func<T, bool>> filterForOldEntities)
        {
            try
            {
                var oldEntities = DbSet.Where(filterForOldEntities).ToList();
                if (oldEntities.Any())
                {
                        DbSet.RemoveRange(oldEntities);
                }

                if(newEntities.Any())
                {
          
                    var utcNow = DateTime.UtcNow;
                    foreach (var entity in newEntities)
                    {
                        entity.Id = Guid.NewGuid();
                        entity.CreatedDateUtc = utcNow;
                        entity.CreatedBy = userId;
                        entity.CurrentState = 1;
                    }

                    DbSet.AddRange(newEntities);
                }
            
                var changes = _dbContext.SaveChanges() > 0;

                return changes;
            }
            catch (Exception ex)
            {
                HandleException(nameof(ResetRange), $"Error occurred while replacing entities of type {typeof(T).Name}.", ex);
                throw;
            }
        }
        /// <summary>
        /// Permanently deletes an entity from the database (Hard Delete).
        /// </summary>
        public bool Remove(Guid id)
        {
            try
            {
                var entity = DbSet.Find(id);
                if (entity == null)
                    throw new NotFoundException($"Entity of type {typeof(T).Name} with ID {id} not found.", _logger);

                DbSet.Remove(entity);
                return _dbContext.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                HandleException(nameof(Remove), $"Error occurred while hard-deleting entity of type {typeof(T).Name} with ID {id}.", ex);
                throw;
            }
        }
        /// <summary>
        /// Logs and rethrows exceptions with detailed information.
        /// </summary>
        private void HandleException(string methodName, string message, Exception ex)
        {
            // Log detailed message for analysis
            _logger.Error(ex, $"[{methodName}] {message}");

            // Throw exception with a general message for users
            throw new DataAccessException(message, ex, _logger);
        }

        public virtual async Task<T> AddAndReturnAsync(T model, Guid creatorId)
        {
            model.Id = Guid.NewGuid();
            model.CreatedDateUtc = DateTime.UtcNow;
            model.CreatedBy = creatorId;
            model.CurrentState = 1;

            await DbSet.AddAsync(model);
            await _dbContext.SaveChangesAsync();

            return model;
        }
        /// <summary>
        /// Permanently deletes a collection of entities from the database (Hard Delete).
        /// </summary>
        public async Task<bool> DeleteRangeAsync(IEnumerable<Guid> ids)
        {
            try
            {
                var entities = await DbSet.Where(e => ids.Contains(e.Id)).ToListAsync();
                if (!entities.Any())
                    throw new NotFoundException($"No entities of type {typeof(T).Name} found for the provided IDs.", _logger);

                DbSet.RemoveRange(entities);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                HandleException(nameof(DeleteRangeAsync), $"Error occurred while hard-deleting multiple entities of type {typeof(T).Name}.", ex);
                throw;
            }
        }


    }
}
