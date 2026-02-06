#region Usings
using AsuntosPenalesAPI.ServiceRegistration;
using Sicoj.Utils.Policy;
using FluentValidation;
using Sicoj.Utils.Middleware;
using Sicoj.Utils;
using Microsoft.OpenApi.Models;
using Sicoj.Utils.Extentions;
#endregion

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();
#endregion

#region Inyeccion Generica
builder.Logging.ClearProviders();
builder.Logging.AddNLoggerService();
builder.Host.AddUseNLoggerService();
builder.Services.AddCatalogosEnpoints(builder.Configuration);
builder.Services.AddProxyEnpoints(builder.Configuration);
builder.Services.AddDatabaseCommandsService(builder.Configuration);
builder.Services.AddPostgreSQLService(builder.Configuration);
builder.Services.AddRedisService(builder.Configuration);
builder.Services.AddHttpServices();
builder.Services.AddDependency(builder.Configuration);

#endregion

#region Inyeccion de validadores
var assembly = typeof(Program).Assembly;
builder.Services.AddValidatorsFromAssembly(assembly);
#endregion

#region Inyeccion de servicios
builder.Services.AddGeneralServices();
#endregion

#region Add Controllers
builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = new CamelCaseNamingPolicy();
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
#endregion

#region Swagger
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Asuntos Penales API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});
#endregion

#region CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAny",
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        }
    );
});
#endregion

#region Ejecucion
var logger = builder.Logging.Services.GetServiceProvider<ILogger<Program>>();
try
{
    #region Builder
    var app = builder.Build();
    #endregion

    #region Argumentos PostgreSQL
    await app.ExecuteArgsDatabaseCommands(args);
    #endregion

    #region Redis
    app.ValidateRedisConnection();
    #endregion

    #region Swagger
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName.Equals("Local"))
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    #endregion

    #region Middleaware
    app.Services.GetRequiredService<DataService>().InitializeAsync().Wait();
    app.UseMiddleware<JwtMiddleware>();
    #endregion

    #region Redirection
    app.UseHttpsRedirection();
    #endregion

    #region CORS
    app.UseCors("AllowAny");
    #endregion

    #region Run
    app.UseAuthorization();

    app.MapControllers();

    string server = builder.Configuration.GetValue<string>("LaunchSettings:Host")!;
    string swagger = builder.Configuration.GetValue<string>("LaunchSettings:Swagger")!;
    logger.LogInformation($"SERVER ON: {server}");
    logger.LogInformation($"SWAGGER ON: {server}{swagger}");
    logger.LogInformation($"ENVIRONMENT: {builder.Environment.EnvironmentName}");
    app.Run();

    
    #endregion
}
catch (Exception _e)
{
    logger.LogCritical(_e, _e.ManageException());
}
#endregion