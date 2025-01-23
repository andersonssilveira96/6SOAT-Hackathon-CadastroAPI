using FIAPX.Cadastro.Api.Middleware;
using FIAPX.Cadastro.Application;
using FIAPX.Cadastro.Infra.Data;
using FIAPX.Cadastro.Infra.MessageBroker;
using FIAPX.Cadastro.Infra.Data.Context;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using FIAPX.Cadastro.Domain.Consumer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<FormOptions>(options => { options.MultipartBodyLengthLimit = 5242880000; });

builder.WebHost.ConfigureKestrel(options => {  options.Limits.MaxRequestBodySize = 5242880000; });

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

// Invocar o serviço
using var scope = app.Services.CreateScope();
var messageConsumer = scope.ServiceProvider.GetRequiredService<IMessageBrokerConsumer>();
_ = Task.Run(() => messageConsumer.ReceiveMessageAsync());

app.UseMiddleware<UnitOfWorkMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
