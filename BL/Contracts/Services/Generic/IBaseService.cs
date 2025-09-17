using BL.GenericResponse;

namespace BL.Contracts.Services.Generic
{
    public interface IBaseService<T, D>
    {
        public Task<Response<IEnumerable<D>>> GetAllAsync();
        public Task<Response<D>> FindByIdAsync(Guid Id);
        public Task<Response<bool>> SaveAsync(D dto, Guid userId);
        public Task<Response<bool>> CreateAsync(D dto, Guid creatorId);
        public Task<Response<bool>> UpdateAsync(D dto, Guid updaterId);
        public Task<Response<bool>> DeleteAsync(Guid id, Guid userId);

    }
}
