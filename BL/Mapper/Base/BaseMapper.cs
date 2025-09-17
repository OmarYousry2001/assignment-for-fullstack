using AutoMapper;
using AutoMapper.QueryableExtensions;
using BL.Contracts.IMapper;
using DAL.Models;

namespace BL.Mapper.Base
{
    public class BaseMapper : IBaseMapper
    {
        private readonly IMapper _mapper;

        public BaseMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        public TDestination MapModel<TSource, TDestination>(TSource source)
        {
            return _mapper.Map<TDestination>(source);
        }

        public IEnumerable<TDestination> MapList<TSource, TDestination>(IEnumerable<TSource> source)
        {
            return _mapper.Map<IEnumerable<TDestination>>(source);
        }
        public async Task<PaginatedResult<TDestination>> ProjectToPaginatedListAsync<TDestination>(IQueryable source,int pageNumber,int pageSize) where TDestination : class
        {
            return await source
                .ProjectTo<TDestination>(_mapper.ConfigurationProvider)
                .ToPaginatedListAsync(pageNumber, pageSize);
        }
    }
}
