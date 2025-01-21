using FIAPX.Cadastro.Domain.Interfaces.Repositories;
using FIAPX.Cadastro.Infra.Data.Repositories;
using FIAPX.Cadastro.Infra.Data.UoW;
using Microsoft.Extensions.DependencyInjection;

namespace FIAPX.Cadastro.Infra.Data
{
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