using Kemet.APIs.Errors;
using Kemet.APIs.Helpers;
using Kemet.Core.Services.Interfaces;
using Kemet.Core.Services.InterFaces;
using Kemet.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kemet.APIs.Extensions
{
    public static class AddApplicationServices
    {
        public static IServiceCollection addApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfiles));
            services.AddScoped<ITokenServices, TokenServices>();
            services.AddScoped<IEmailSettings, EmailSettings>();
            services.AddScoped<OtpExtensions>();
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count() > 0)
                                            .SelectMany(P => P.Value.Errors)
                                            .Select(E => E.ErrorMessage)
                                            .ToArray();
                    var validationErrorResponse = new ApiValidationErrorResponse()
                    {
                        Errors = errors

                    };
                    return new BadRequestObjectResult(validationErrorResponse);
                };
            });

            return services;
        }
    }
}
