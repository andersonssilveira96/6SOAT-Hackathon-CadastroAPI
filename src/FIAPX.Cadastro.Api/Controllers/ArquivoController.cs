using FIAPX.Cadastro.Application.DTOs;
using FIAPX.Cadastro.Application.UseCase;
using Microsoft.AspNetCore.Mvc;

namespace FIAPX.Cadastro.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArquivoController : ControllerBase
    {       
        private readonly ILogger<ArquivoController> _logger;
        private readonly IArquivoUseCase _arquivoUseCase;
        public ArquivoController(ILogger<ArquivoController> logger, IArquivoUseCase arquivoUseCase)
        {
            _logger = logger;
            _arquivoUseCase = arquivoUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> UploadVideo(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Nenhum arquivo foi enviado ou o arquivo está vazio.");
            }

            if (!file.ContentType.StartsWith("video/"))
            {
                return BadRequest("Apenas arquivos de vídeo são permitidos.");
            }

            if (file.Length > 524288000) // 500 MB
            {
                return BadRequest("O arquivo enviado é maior que o limite permitido (500 MB).");
            }

            var arquivo = new ArquivoDto
            {
                ContentType = file.ContentType,
                FileName = file.FileName              
            };

            using var stream = file.OpenReadStream();

            await _arquivoUseCase.CreateFile(arquivo, stream);

            return Ok(new { Message = "Upload realizado com sucesso!" });           
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var arquivos = await _arquivoUseCase.GetAll();

            return Ok(arquivos);
        }

        [HttpGet("download-zip/{key}")]        
        public async Task<IActionResult> DownloadZip(string key)
        {
            try
            {
                var fileStream = await _arquivoUseCase.DownloadZip(key);

                var fileName = Path.GetFileName(key);

                return File(fileStream, "application/zip", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao tentar baixar o arquivo: {ex.Message}");
            }
        }
    }
}
