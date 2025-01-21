namespace FIAPX.Cadastro.Domain.Producer
{
    public interface IMessageBrokerProducer
    {
        Task SendMessageAsync<T>(T message);
    }
}
