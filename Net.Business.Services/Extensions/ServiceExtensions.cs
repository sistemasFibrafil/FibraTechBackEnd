using Net.Data;
using Net.Connection;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Net.Business.Services
{
    public static class ServiceExtensions
    {

        public static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", builder => builder.WithOrigins("http://192.168.1.7:8080", "http://192.168.20.10", "http://192.168.1.13", "http://localhost:80", "http://localhost:4200").AllowAnyHeader().WithMethods("PUT", "PATCH", "GET"));
            });
        }

        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {
            });
        }

        public static void ConfigureSQLConnection(this IServiceCollection services)
        {
            services.AddScoped<IConnectionSQL, ConnectionSQL>();
        }

        public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        }

        public static void ConfigureHttpClientServiceLayer(this IServiceCollection services)
        {
            services.AddHttpClient("bypass-ssl-validation")
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (
                    httpRequestMessage, cert, certChain, policyErrors) =>
                {
                    return true;
                }
            });
        }
    }
}
