namespace Kemet.APIs.Errors
{
    public class ApiExceptionResponse : ApiResponse
    {
        private  string? Details { get; set; }

        public ApiExceptionResponse(int statusCode , string? message = null , string? details =null)
            : base(statusCode,message)
        {
           Details = details;
        }
    }
}
 