using FIAPX.Cadastro.Application.DTOs;
using FIAPX.Cadastro.Application.UseCase;
using FIAPX.Cadastro.Domain.Consumer;
using FIAPX.Cadastro.Domain.Enum;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace FIAPX.Cadastro.Infra.MessageBroker
{
    public class MessageBrokerConsumer : IMessageBrokerConsumer
    {
        private readonly IArquivoUseCase _arquivoUseCase;
        private IConnection _connection;
        private IChannel _channel;

        public MessageBrokerConsumer(IArquivoUseCase arquivoUseCase)
        {
            _arquivoUseCase = arquivoUseCase;
        }

        public async Task ReceiveMessageAsync()
        {
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq-service"
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync("arquivos-atualizados", exclusive: false);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, eventArgs) => {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"Order message received: {message}");

                var arquivo = JsonSerializer.Deserialize<ArquivoDto>(message)!;

                Console.WriteLine($"Pedido: { JsonSerializer.Serialize(message) }");

                await _arquivoUseCase.UpdateStatus(arquivo.Id, (int)arquivo.Status);
            };

            await _channel.BasicConsumeAsync(queue: "arquivos-atualizados", autoAck: false, consumer: consumer);
        }

        public void Dispose()
        {
            _channel?.CloseAsync();
            _connection?.CloseAsync();
        }
    }
}
