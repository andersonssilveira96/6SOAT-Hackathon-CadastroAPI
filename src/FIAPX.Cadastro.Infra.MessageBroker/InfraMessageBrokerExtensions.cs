using FIAPX.Cadastro.Domain.Consumer;
using FIAPX.Cadastro.Domain.Producer;
using Microsoft.Extensions.DependencyInjection;

namespace FIAPX.Cadastro.Infra.MessageBroker
{
    public static class InfraMessageBrokerExtensions
    {
        public static IServiceCollection AddInfraMessageBrokerServices(this IServiceCollection services)
        {
            services.AddScoped<IMessageBrokerConsumer, MessageBrokerConsumer>();
            services.AddScoped<IMessageBrokerProducer, MessageBrokerProducer>();
            return services;
        }
    }
}
