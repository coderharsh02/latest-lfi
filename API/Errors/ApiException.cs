namespace API.Errors
{
    public class ApiException : Exception
    {
        public ApiException(int statusCode, string message = null, string details = null)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }

        public int StatusCode { get; set; }
        override public string Message { get; } 
        public string Details { get; set; }
    }
}