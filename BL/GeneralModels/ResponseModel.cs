namespace Shared.GeneralModels
{
    public class ResponseModel<T>
    {
        private IEnumerable<string> errors;

        public bool Success { get; set; }
        public int StatusCode { get; set; } = 200;
        public T? Data { get; set; }
        public string Message { get; set; }
        public IEnumerable<string> Errors
        {
            get => errors;
            set
            {
                errors = value ?? new List<string>(); // Ensure errors is never null
                if (errors.Count() > 0)
                    Message = string.Join(", ", errors);
            }
        }

        public ResponseModel()
        {
            Success = true;
            Errors = new List<string>();
            Data = default;
            Message = string.Empty;
        }

        public void SetSuccessMessage(string message)
        {
            Success = true;
            Message = message;
        }

        public void SetErrorMessage(string message)
        {
            Success = false;
            Message = message;
        }
    }
}
