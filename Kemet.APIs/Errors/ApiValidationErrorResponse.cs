﻿namespace Kemet.APIs.Errors
{
    public class ApiValidationErrorResponse : ApiResponse
    {
        public IEnumerable<string>  Errors { get; set; }

        public ApiValidationErrorResponse() : base(400)
        {
            
        }
    }
}
