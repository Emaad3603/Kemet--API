
namespace Kemet.APIs.Errors
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }

        public string Message { get; set; }


        public ApiResponse( int statusCode , string? message = null) 
        { 
            StatusCode = statusCode ;
            Message = message ?? GetDefaultMessageForStatuesCode(statusCode);
        }

        private string? GetDefaultMessageForStatuesCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "a bad request you have made",
                401 => "Authorized , you are not ",
                404 => "Resource is not found",
                500 => "Server error",
                _ => null
            };
        }
    }
}
