using FIAPX.Cadastro.Api.Middleware;
using FIAPX.Cadastro.Application;
using FIAPX.Cadastro.Infra.Data;
using FIAPX.Cadastro.Infra.MessageBroker;
using FIAPX.Cadastro.Infra.Data.Context;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<FormOptions>(options => { options.MultipartBodyLengthLimit = 524288000; });

builder.WebHost.ConfigureKestrel(options => {  options.Limits.MaxRequestBodySize = 524288000; });

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationService();
builder.Services.AddInfraDataServices();
builder.Services.AddInfraMessageBrokerServices();

builder.Services.AddTransient<UnitOfWorkMiddleware>();
builder.Services.AddDbContext<FIAPXContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<UnitOfWorkMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
