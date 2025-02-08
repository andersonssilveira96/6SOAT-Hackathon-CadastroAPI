using FIAPX.Cadastro.Domain.Producer;
using Microsoft.AspNetCore.Http;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FIAPX.Cadastro.Infra.MessageBroker
{
    public class MessageBrokerProducer : IMessageBrokerProducer
    {
        private IHttpContextAccessor _httpContext;
        public MessageBrokerProducer(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }
        public async Task SendMessageAsync<T>(T message)
        {
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq-service"
            };

            var connection = await factory.CreateConnectionAsync();

            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync("arquivos-novos", exclusive: false);

            JsonSerializerOptions options = new()
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(message, options);
            var body = Encoding.UTF8.GetBytes(json);
            await channel.BasicPublishAsync(exchange: "", routingKey: "arquivos-novos", body: body);
        }
    }
}
