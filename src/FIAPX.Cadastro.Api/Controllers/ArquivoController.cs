using FIAPX.Cadastro.Application.DTOs;
using FIAPX.Cadastro.Application.UseCase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FIAPX.Cadastro.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]

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
        public async Task<IActionResult> UploadVideos(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest("Nenhum arquivo foi enviado.");
            }

            var tamanhoMaximo = 524288000; // 500 MB
            var arquivosEnviados = new List<object>();

            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest($"O arquivo '{file?.FileName}' está vazio.");
                }

                if (!file.ContentType.StartsWith("video/"))
                {
                    return BadRequest($"O arquivo '{file.FileName}' não é um vídeo válido.");
                }

                if (file.Length > tamanhoMaximo)
                {
                    return BadRequest($"O arquivo '{file.FileName}' excede o tamanho permitido de 500 MB.");
                }

                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
                var arquivo = new ArquivoDto
                {
                    Id = Guid.NewGuid(),
                    ContentType = file.ContentType,
                    FileName = file.FileName,
                    UserId = userId,
                    User = new UsuarioDto 
                    {
                        Id = userId,
                        Name = User.FindFirst("name")?.Value!,
                        Email = User.FindFirst(ClaimTypes.Email)?.Value!
                    }
                };

                using var stream = file.OpenReadStream();

                await _arquivoUseCase.CreateFile(arquivo, stream);

                arquivosEnviados.Add(new { file.FileName, Status = "Upload realizado com sucesso!" });
            }

            return Ok(new
            {
                Message = "Upload concluído.",
                Arquivos = arquivosEnviados
            });
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var arquivos = await _arquivoUseCase.GetAllByUserId(Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString()));

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
                return BadRequest($"{ex.Message}");
            }
        }
    }
}
