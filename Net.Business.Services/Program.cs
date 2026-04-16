using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Net.Business.Services;
using Net.Business.Services.Validators.SAPBusinessOne;
using Net.Connection.ConnectionSAPBusinessOne;
using Net.CrossCotting;
using Net.Data;
using Net.Data.AppContext;

// ===============================
// BUILDER
// ===============================

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// ===============================
// SERVICES (ANTES ConfigureServices)
// ===============================

// IIS
builder.Services.ConfigureIISIntegration();

// Conexiones / servicios custom
builder.Services.ConfigureSQLConnection();
builder.Services.ConfigureHttpClientServiceLayer();

// HttpContext
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// ===============================
// AUTH JWT
// ===============================

builder.Services.Configure<ParametrosTokenConfig>(
    configuration.GetSection("ParametrosTokenConfig")
);

string semilla = configuration.GetValue<string>("ParametrosTokenConfig:Semilla");
string emisor = configuration.GetValue<string>("ParametrosTokenConfig:Emisor");
string destinatario = configuration.GetValue<string>("ParametrosTokenConfig:Destinatario");

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(semilla));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = emisor,
            ValidAudience = destinatario,
            IssuerSigningKey = key
        };
    });

// ===============================
// REPOSITORY
// ===============================

builder.Services.ConfigureRepositoryWrapper();
builder.Services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();

// ===============================
// CONTROLLERS
// ===============================

builder.Services.AddControllers();

// ===============================
// FLUENT VALIDATION (FORMA MODERNA)
// ===============================

builder.Services.AddValidatorsFromAssemblyContaining<OrdersCreateRequestDtoValidator>();

// ===============================
// AUTOMAPPER
// ===============================

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ===============================
// SWAGGER
// ===============================

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("ApiFibrafil", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API Fibrafil",
        Version = "1",
        Description = "BackEnd Fibrafil",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Email = "nflorespizango@gmail.com",
            Name = "Fibrafil",
            Url = new Uri("https://www.linkedin.com/company/grupo-fibrafil-per%C3%BA/?originalSubdomain=pe/")
        },
        License = new Microsoft.OpenApi.Models.OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://www.linkedin.com/in/nerio-flores-pizango/")
        }
    });

    var archivoXml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var rutaXml = Path.Combine(AppContext.BaseDirectory, archivoXml);
    options.IncludeXmlComments(rutaXml);

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Autenticación JWT (Bearer)",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Id = "Bearer",
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

// ===============================
// CORS
// ===============================

builder.Services.ConfigureCors(configuration);

// ===============================
// DB CONTEXTS
// ===============================

// Seguridad
var connSeg = Utilidades.GetCon(configuration, "EntornoConnection:Entorno");
builder.Services.AddDbContext<DataContextSeguridad>(opt =>
    opt.UseSqlServer(connSeg));

// SAP
var connSap = Utilidades.GetCon(configuration, "EntornoConnectionSap:Entorno");
builder.Services.AddDbContext<DataContextSAPBusinessOne>(opt =>
    opt.UseSqlServer(connSap));

// Profil
var connProfil = Utilidades.GetCon(configuration, "EntornoConnectionProfil:Entorno");
builder.Services.AddDbContext<DataContextProfil>(opt =>
    opt.UseSqlServer(connProfil));

// ===============================
// SAP DI API
// ===============================

var connSapDi = Utilidades.GetConSap(configuration, "EntornoConnectionSapDiApi:Entorno");

builder.Services.AddSingleton<IConnectionSAPBusinessOne, ConnectionSAPBusinessOne>();
builder.Services.AddSingleton(provider =>
    new CompanyProviderSAPBusinessOne(
        provider.GetRequiredService<IConnectionSAPBusinessOne>(),
        connSapDi
    )
);

// ===============================
// BUILD APP
// ===============================

var app = builder.Build();

// ===============================
// MIDDLEWARE (ANTES Configure)
// ===============================

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.ConfigureExceptionHandler();

// Swagger
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("swagger/ApiFibrafil/swagger.json", "API Fibrafil");
    options.RoutePrefix = "";
});

app.UseRouting();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();