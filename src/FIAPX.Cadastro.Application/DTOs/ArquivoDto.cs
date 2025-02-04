using FIAPX.Cadastro.Domain.Enum;

namespace FIAPX.Cadastro.Application.DTOs
{
    public class ArquivoDto
    {
        public Guid Id { get; set; }
        public required string FileName { get; set; }
        public required string ContentType { get; set; }
        public StatusEnum Status { get; set; }
        public Guid UserId { get; set; }
        public UsuarioDto User { get; set; }
    }
}
