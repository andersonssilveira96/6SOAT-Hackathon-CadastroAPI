using FIAPX.Cadastro.Domain.Interfaces.Repositories;
using FIAPX.Cadastro.Infra.Data.Repositories;
using FIAPX.Cadastro.Infra.Data.UoW;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace FIAPX.Cadastro.Infra.Data
{
    [ExcludeFromCodeCoverage]
    public static class InfraDataServicesExtensions
    {
        public static IServiceCollection AddInfraDataServices(this IServiceCollection services)
        {
            services.AddScoped<IArquivoRepository, ArquivoRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}