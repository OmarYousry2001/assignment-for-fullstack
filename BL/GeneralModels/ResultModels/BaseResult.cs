

namespace Shared.GeneralModels.ResultModels
{
    public class BaseResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }

    public class BaseResult<T> : BaseResult
    {
        public T? Data { get; set; }
    }
}
