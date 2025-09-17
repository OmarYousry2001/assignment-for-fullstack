
namespace Shared.GeneralModels.SearchCriteriaModels
{
    public class BaseSearchCriteriaModel
    {
        /// <summary>
        /// Search term for unit name or description
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Page number (default: 1)
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Number of items per page (default: 10, max: 100)
        /// </summary>
        public int PageSize { get; set; } = 10;

        
    }
}
