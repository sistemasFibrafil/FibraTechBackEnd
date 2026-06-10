using Net.Data;
using Net.Connection;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Net.Business.Logic.Services.Common;
using Net.Business.Logic.Interfaces.Common;
using Microsoft.Extensions.DependencyInjection;
using Net.Business.Logic.Services.SAPBusinessOne.Draft;
using Net.Business.Logic.Services.SAPBusinessOne.Sales;
using Net.Business.Logic.Interfaces.SAPBusinessOne.Draft;
using Net.Business.Logic.Interfaces.SAPBusinessOne.Sales;
using Net.Business.Logic.Services.SAPBusinessOne.Inventory;
using Net.Business.Logic.Services.SAPBusinessOne.Purchasing;
using Net.Business.Logic.Interfaces.SAPBusinessOne.Inventory;
using Net.Business.Logic.Interfaces.SAPBusinessOne.Purchasing;
using Net.Business.Logic.Services.SAPBusinessOne.BusinessPartners;
using Net.Business.Logic.Interfaces.SAPBusinessOne.BusinessPartners;
using Net.Business.Logic.Services.SAPBusinessOne.Inventory.InventoryTransactions;
using Net.Business.Logic.Interfaces.SAPBusinessOne.Inventory.InventoryTransactions;
using Net.Business.Logic.Services.SAPBusinessOne.Administration.SystemInitialization;
using Net.Business.Logic.Interfaces.SAPBusinessOne.Administration.SystemInitialization;
namespace Net.Business.Services
{
    public static class ServiceExtensions
    {

        public static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", builder => builder.WithOrigins("http://192.168.20.8:8080", "http://192.168.1.7:8080", "http://localhost:80", "http://localhost:4200").AllowAnyHeader().WithMethods("PUT", "PATCH", "GET"));
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

        public static void ConfigureBusinessLogicServices(this IServiceCollection services)
        {
            services.AddScoped<IDocumentSeriesConfigurationService, DocumentSeriesConfigurationService>();

            services.AddScoped<IItemsService, ItemsService>();
            services.AddScoped<IStockTransfersService, StockTransfersService>();
            services.AddScoped<IInventoryTransferRequestService, InventoryTransferRequestService>();

            services.AddScoped<IPurchaseRequestService, PurchaseRequestService>();

            services.AddScoped<IOrdersService, OrdersService>();
            services.AddScoped<IInvoicesService, InvoicesService>();
            services.AddScoped<IDeliveryNotesService, DeliveryNotesService>();
            
            services.AddScoped<IDraftService, DraftService>();

            services.AddScoped<IUbigeoService, UbigeoService>();
            services.AddScoped<IDriversService, DriversService>();
            services.AddScoped<IVehiclesService, VehiclesService>();

            services.AddScoped<IFileService, FileService>();
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
