namespace DAL.Models
{
    public class PaginatedDataModel<T>
    {
        public IEnumerable<T> Items { get; }
        public int TotalRecords { get; }

        public PaginatedDataModel(IEnumerable<T> items, int totalRecords)
        {
            Items = items.ToList();
            TotalRecords = totalRecords;
        }
    }
}
