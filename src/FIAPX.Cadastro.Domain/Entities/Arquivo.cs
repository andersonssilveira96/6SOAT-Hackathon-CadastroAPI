using FIAPX.Cadastro.Domain.Enum;

namespace FIAPX.Cadastro.Domain.Entities
{
    public class Arquivo
    {      
        public Arquivo(Guid id, string fileName, string contentType, StatusEnum status, Guid userId)
        {
            Id = id;
            FileName = fileName;    
            ContentType = contentType;
            Status = status;
            UserId = userId;
        }
        public Guid Id { get; private set; }
        public string FileName { get; private set; }
        public string ContentType { get; private set; }
        public StatusEnum Status { get; private set; }
        public Guid UserId { get; private set; }
        public void UpdateStatus(StatusEnum status) => Status = status;
    }
}
