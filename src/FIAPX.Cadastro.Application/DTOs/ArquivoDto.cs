using FIAPX.Cadastro.Domain.Enum;

namespace FIAPX.Cadastro.Application.DTOs
{
    public class ArquivoDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public StatusEnum Status { get; set; }
        public long UserId { get; set; }
    }
}
