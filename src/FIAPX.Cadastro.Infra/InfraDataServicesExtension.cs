using FIAPX.Cadastro.Domain.Interfaces.Repositories;
using FIAPX.Cadastro.Infra.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace FIAPX.Cadastro.Infra
{
    public static class InfraDataServicesExtensions
    {
        public static IServiceCollection AddInfraServices(this IServiceCollection services)
        {
            services.AddScoped<IArquivoRepository, ArquivoRepository>();
            return services;
        }
    }
}