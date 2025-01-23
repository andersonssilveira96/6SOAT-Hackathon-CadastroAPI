using FIAPX.Cadastro.Application.DTOs;

namespace FIAPX.Cadastro.Application.UseCase
{
    public interface IArquivoUseCase
    {
        Task CreateFile(ArquivoDto arquivoDto, Stream stream);
        Task<List<ArquivoDto>> GetAll();
        Task<ArquivoDto> UpdateStatus(Guid id, int status);
        Task<Stream> DownloadZip(string key);
    }
}
