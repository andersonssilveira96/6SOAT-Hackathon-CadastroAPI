using FIAPX.Cadastro.Api.Middleware;
using FIAPX.Cadastro.Application;
using FIAPX.Cadastro.Infra.Data;
using FIAPX.Cadastro.Infra.MessageBroker;
using FIAPX.Cadastro.Infra.Data.Context;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using FIAPX.Cadastro.Domain.Consumer;
using FIAPX.Cadastro.Api.Extensions;
using FIAPX.Cadastro.Api.Helper;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<FormOptions>(options => { options.MultipartBodyLengthLimit = 5242880000; });

builder.WebHost.ConfigureKestrel(options => {  options.Limits.MaxRequestBodySize = 5242880000; });

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FIAPX Cadastro API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                new string[] {}
        }
    });
});

builder.Services.AddApplicationService();
builder.Services.AddInfraDataServices();
builder.Services.AddInfraMessageBrokerServices();

builder.Services.AddTransient<UnitOfWorkMiddleware>();
builder.Services.AddDbContext<FIAPXContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));

builder.Services.Configure<CognitoAuthConfig>(builder.Configuration.GetSection("CognitoConfig"));
builder.Services.AddAuthenticationConfig();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Invocar o serviço
using var scope = app.Services.CreateScope();
var messageConsumer = scope.ServiceProvider.GetRequiredService<IMessageBrokerConsumer>();
_ = Task.Run(() => messageConsumer.ReceiveMessageAsync());

app.UseMiddleware<UnitOfWorkMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
