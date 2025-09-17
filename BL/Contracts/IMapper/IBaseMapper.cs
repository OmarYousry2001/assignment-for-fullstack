
using DAL.Models;

namespace BL.Contracts.IMapper
{
    public interface IBaseMapper
    {
        TDestination MapModel<TSource, TDestination>(TSource source);
        IEnumerable<TDestination> MapList<TSource, TDestination>(IEnumerable<TSource> source);
        Task<PaginatedResult<TDestination>> ProjectToPaginatedListAsync<TDestination>(
        IQueryable source,
        int pageNumber,
        int pageSize
        ) where TDestination : class;
    }
}
