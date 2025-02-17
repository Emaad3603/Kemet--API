using Kemet.APIs.Extensions;
using Kemet.APIs.Helpers;
using Kemet.APIs.Middlewares;
using Kemet.Core.Entities;
using Kemet.Core.Entities.Identity;
using Kemet.Core.Repositories.InterFaces;
using Kemet.Core.RepositoriesInterFaces;
using Kemet.Core.Services.Interfaces;
using Kemet.Core.Services.InterFaces;
using Kemet.Repository.Data;
using Kemet.Repository.Data.DataSeed;
using Kemet.Repository.Data.DataSeed.Identity;
using Kemet.Repository.Repositories;
using Kemet.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Kemet.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            #region ConfigureServices

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new DateOnlyConverter());
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            });
            // Bind Appsettings from configuration
            var appSettings = new Appsettings();
            builder.Configuration.GetSection("Appsettings").Bind(appSettings);

            // Register Appsettings for DI (for other parts of the app)
            builder.Services.Configure<Appsettings>(builder.Configuration.GetSection("Appsettings"));
            

            //  var app = builder.Build();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<AppDbContext>(option =>
            {
                option.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    x => x.UseNetTopologySuite()
                );
            });

            //allow the dependancy injection for  generic repository

            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            //==================================================================================
            
            builder.Services.AddAutoMapper(M => M.AddProfile(new MappingProfiles()));
           
            builder.Services.addApplicationServices();

            var PPuploadsFolder = builder.Configuration.GetValue<string>("PPUploadsFolder");
            var BGuploadsFolder = builder.Configuration.GetValue<string>("BGUploadsFolder");

            // Register services
            builder.Services.AddScoped<IProfileService>(provider => new ProfileService(
                provider.GetRequiredService<UserManager<AppUser>>(),
                PPuploadsFolder, 
                BGuploadsFolder,
                provider.GetRequiredService<AppDbContext>()
                ));

            //=================================================
           builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            //=================================================
            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                // Allow spaces in the username and no special char except "._-   " 
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._-   ";
                options.User.RequireUniqueEmail = true; //  emails  unique
                options.Tokens.ProviderMap.Add("Default", new TokenProviderDescriptor(typeof(DataProtectorTokenProvider<AppUser>)));

            }
            ).AddEntityFrameworkStores<AppDbContext>()
             .AddDefaultTokenProviders() // This registers the default token providers (including the DataProtectorTokenProvider)
             .AddTokenProvider<DataProtectorTokenProvider<AppUser>>("Default"); // Register the DataProtectorTokenProvider explicitly;

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                                        .AddJwtBearer(options =>
                                        {
                                            options.TokenValidationParameters = new TokenValidationParameters()
                                            {
                                                ValidateIssuer = true,
                                                ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                                                ValidateAudience = true,
                                                ValidAudience = builder.Configuration["JWT:ValidAudience"],
                                                ValidateLifetime = true,
                                                ValidateIssuerSigningKey = true,
                                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))

                                            };
                                        });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", config =>
                {
                    config.AllowAnyHeader();
                    config.AllowAnyMethod();
                    config.AllowAnyOrigin();
                });
            });

            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
            builder.Services.AddTransient<IEmailSettings, EmailSettings>(); 
            #endregion

            #region Services 

            var app = builder.Build();


            using var scope = app.Services.CreateScope();

            var services = scope.ServiceProvider;

            var _context = services.GetRequiredService<AppDbContext>();

            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();




            var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            try
            {
                await _context.Database.MigrateAsync();

                var _userManager = services.GetRequiredService<UserManager<AppUser>>();

                await IdentityDbContextSeed.SeedUserAsync(_userManager, roleManager);
                await CategoryDataSeed.SeedCategoriesAsync(_context);

                await DataSeeding.SeedDataAsync(_context);

                await HomeDbContextSeed.SeedAsync(_context);

            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "an Error has been accured during applying the migration");

            }

            #endregion
            #region Configure
            // Configure the HTTP request pipeline.

            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseCors("MyPolicy");

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            #endregion
            app.Run();
        }
    }
}
