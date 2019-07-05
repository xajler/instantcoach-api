using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.Swagger;
using Core;
using Core.Context;

namespace Api
{
    public static class SatrtupExtensions
    {
        // public static void UseMeasureRequestTime(this IApplicationBuilder app)
        // {
        //     app.Use(async (context, next) =>
        //     {
        //         var stopWatch = new Stopwatch();
        //         stopWatch.Start();
        //         await next.Invoke();
        //         stopWatch.Stop();
        //         WriteLine($"Request processing time: {stopWatch.ElapsedMilliseconds}ms");
        //     });
        // }
        public static void AddWebApiService(this IServiceCollection services)
        {
            services.AddMvc(options =>
                {
                    // TODO: Not sure to use it or not
                    // options.Filters.Add<OperationCancelledExceptionFilter>();
                })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.Formatting = Formatting.None;
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public static void AddConfigOptionsService(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddOptions<Config>()
                    .Configure(options => configuration.GetSection(Config.Name).Bind(options))
                    .ValidateDataAnnotations();
        }

        public static void AddHeaderApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(v =>
            {
                v.AssumeDefaultVersionWhenUnspecified = true;
                v.ReportApiVersions = true;
                v.ErrorResponses = new ApiVersioningErrorResponseProvider();
                v.ApiVersionReader = new HeaderApiVersionReader("X-Api-Version");
            });
        }

        public static void AddDbcontextService(this IServiceCollection services, string connectionString)
        {
            // WriteLine($"conn string: {connectionString}");
            services.AddDbContext<ICContext>(options =>
                  options.UseSqlServer(connectionString, providerOptions =>
                          {
                              providerOptions.CommandTimeout(180);
                              providerOptions.EnableRetryOnFailure(
                                maxRetryCount: 10,
                                maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorNumbersToAdd: null);
                          })
                         .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
        }

        public static void AddSwaggerService(this IServiceCollection services)
        {
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "InstantCoach API",
                    Description = "A sample ASP.NET Core web API for microservices",
                    Contact = new Contact
                    {
                        Name = "Kornelije Sajler",
                        Email = "ks@metaintellect.com",
                        Url = "https://git.430n.com/x430n/instantcoach"
                    },
                    License = new License
                    {
                        Name = "MIT",
                        Url = "https://git.430n.com/x430n/instantcoach/src/branch/master/LICENSE"
                    }
                });
            });
        }
    }
}