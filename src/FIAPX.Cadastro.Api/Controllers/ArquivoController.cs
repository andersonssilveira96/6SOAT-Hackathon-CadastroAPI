using Microsoft.AspNetCore.Mvc;

namespace FIAPX.Cadastro.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArquivoController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<ArquivoController> _logger;

        public ArquivoController(ILogger<ArquivoController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> UploadVideo(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Nenhum arquivo foi enviado ou o arquivo está vazio.");
            }

            try
            {
                //var filePath = Path.Combine(_uploadFolder, file.FileName);

                //using (var stream = new FileStream(filePath, FileMode.Create))
                //{
                //    await file.CopyToAsync(stream);
                //}

                return Ok(new { Message = "Upload realizado com sucesso!" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Erro no upload: {ex.Message}");
            }
        }
    }
}
