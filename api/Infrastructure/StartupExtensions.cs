using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Core;
using Core.Context;

namespace Api
{
    public static class SatrtupExtensions
    {
        public static void UseSwaggerUIWAsHomeRoute(this IApplicationBuilder app,
            IApiVersionDescriptionProvider provider)
        {
            app.UseSwaggerUI(s =>
            {
                // In descaeding order, first show newer (greater) major versions
                // Add descending order minor version number, if needed!
                foreach (var description in provider.ApiVersionDescriptions
                                                    .OrderByDescending(x => x.ApiVersion.MajorVersion))
                {
                    var name = $"InstantCoach API v{description.GroupName.ToUpperInvariant()}";
                    s.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", name);
                }
                s.RoutePrefix = string.Empty;
            });
        }

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
                v.DefaultApiVersion = new ApiVersion(2, 0);
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
                s.OperationFilter<SwaggerDefaultValues>();
                s.IncludeXmlComments(XmlCommentsFilePath);
            });
        }

        private static string XmlCommentsFilePath
        {
            get
            {
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine(basePath, fileName);
            }
        }
    }
}