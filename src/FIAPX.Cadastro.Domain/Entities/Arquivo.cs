using FIAPX.Cadastro.Domain.Enum;

namespace FIAPX.Cadastro.Domain.Entities
{
    public class Arquivo
    {
        public Arquivo()
        {
            
        }
        public Guid Id { get; private set; }
        public string FileName { get; private set; }
        public string ContentType { get; private set; }
        public StatusEnum Status { get; private set; }
        public long UserId { get; private set; }
        public void AtualizarStatus(StatusEnum status) => Status = status;
    }
}
