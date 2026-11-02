namespace Rest.API.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        /// <summary>Machine-readable error code (clients translate it). Null for most errors.</summary>
        public string? ErrorCode { get; set; }
        public IReadOnlyDictionary<string, object?>? ErrorParams { get; set; }

        public static ApiResponse<T> SuccessResponse( T? data, string message = "Success")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Errors = null
            };
        }

        public static ApiResponse<T> FailResponse(
            List<string> errors,
            string message = "Error",
            string? errorCode = null,
            IReadOnlyDictionary<string, object?> errorParams = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = errors,
                ErrorCode = errorCode,
                ErrorParams = errorParams
            };
        }
    }
}
