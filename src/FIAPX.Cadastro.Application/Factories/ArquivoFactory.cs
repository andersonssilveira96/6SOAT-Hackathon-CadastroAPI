using FIAPX.Cadastro.Application.DTOs;
using FIAPX.Cadastro.Domain.Entities;

namespace FIAPX.Cadastro.Application.Factories
{
    public static class ArquivoFactory
    {
        public static Arquivo Create(ArquivoDto arquivoDto)
        {
            return new Arquivo(Guid.NewGuid(), arquivoDto.FileName, arquivoDto.ContentType, arquivoDto.Status, arquivoDto.UserId);
        }
    }
}
