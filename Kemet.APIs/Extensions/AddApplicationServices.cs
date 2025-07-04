﻿using Kemet.APIs.Errors;
using Kemet.APIs.Helpers;
using Kemet.Core.Repositories.InterFaces;
using Kemet.Core.RepositoriesInterFaces;
using Kemet.Core.Services.Interfaces;
using Kemet.Core.Services.InterFaces;
using Kemet.Repository;
using Kemet.Repository.Repositories;
using Kemet.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using StackExchange.Redis;

namespace Kemet.APIs.Extensions
{
    public static class AddApplicationServices
    {
        public static IServiceCollection addApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfiles));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITokenServices, TokenServices>();
            services.AddScoped<IEmailSettings, EmailSettings>();
            services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));
            services.AddScoped<IinterestsRepository,InterestRepository>();
            services.AddScoped<ISearchInterface,SearchServices>();
            services.AddScoped<IHomeServices,HomeServices>();
            services.AddScoped<IBookingServices,BookingServices>();
            services.AddScoped<ITravelAgencyService,TravelAgencyService>();
            services.AddScoped<OtpExtensions>();
            services.AddScoped<IWishlistRepository, WishlistRepository>();
            services.AddHttpClient<IAiService, AiService>();
            services.AddScoped<IAiService, AiService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddHostedService<QueuedHostedService>();
            services.AddScoped<ICacheRepository, CacheRepository>();
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState
                        .Where(P => P.Value?.Errors.Count > 0)
                        .SelectMany(P => P.Value!.Errors)
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
