using BL.Contracts.IMapper;
using BL.Contracts.Services.Generic;
using BL.GenericResponse;
using DAL.Contracts.Repositories.Generic;
using Domains.Entities.Base;

namespace BL.Services.Generic
{
    public abstract class BaseService<T, D> : ResponseHandler, IBaseService<T, D>
     where T : BaseEntity

    {
        private readonly ITableRepository<T> _baseRepository;
        protected  IBaseMapper _mapper;
        public BaseService(ITableRepository<T> baseRepository, IBaseMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public virtual async Task<Response<IEnumerable<D>>> GetAllAsync()
        {
            var entitiesList = await _baseRepository.GetAllAsync();
            if (entitiesList == null) return NotFound<IEnumerable<D>>();
            var dtoList = _mapper.MapList<T, D>(entitiesList);
            return Success(dtoList);
        }

        public virtual async Task<Response<D>> FindByIdAsync(Guid Id)
        {
            var entity =  await _baseRepository.FindByIdAsync(Id);
            if (entity == null) return NotFound<D>();
            var dto = _mapper.MapModel<T, D>(entity);
            return Success(dto);
        }

        public virtual async Task<Response<bool>> SaveAsync(D dto, Guid userId)
        {
            var entity = _mapper.MapModel<D, T>(dto);
            var isSaved = await _baseRepository.SaveAsync(entity, userId);
            if (isSaved) return Success(true);
            else return BadRequest<bool>();

        }

        public virtual async Task<Response<bool>> CreateAsync(D dto, Guid creatorId)
        {
            var entity = _mapper.MapModel<D, T>(dto);
            var isCreated = await  _baseRepository.CreateAsync(entity, creatorId);
            if (isCreated)
                return Created(true);
            else
                return BadRequest<bool>();
        }

        public virtual async Task<Response<bool>> UpdateAsync(D dto, Guid updaterId)
        {
            var entity = _mapper.MapModel<D, T>(dto);
            var isUpdated  = await _baseRepository.UpdateAsync(entity, updaterId);
            if (isUpdated)
                return Success(true);
            else
                return NotFound<bool>();
        }

        public virtual async Task<Response<bool>> DeleteAsync(Guid id, Guid userId)
        {
            var isDeleted = await _baseRepository.UpdateCurrentStateAsync(id, userId);
            if (isDeleted) return Deleted<bool>();
            else return BadRequest<bool>();

        }

    }
}
