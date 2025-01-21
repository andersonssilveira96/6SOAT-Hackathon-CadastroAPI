namespace FIAPX.Cadastro.Domain.Consumer
{
    public interface IMessageBrokerConsumer
    {
        public Task ReceiveMessageAsync();
    }
}
