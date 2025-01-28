using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using AutoMapper;
using FIAPX.Cadastro.Application.DTOs;
using FIAPX.Cadastro.Application.Factories;
using FIAPX.Cadastro.Domain.Enum;
using FIAPX.Cadastro.Domain.Interfaces.Repositories;
using FIAPX.Cadastro.Domain.Producer;

namespace FIAPX.Cadastro.Application.UseCase
{
    public class ArquivoUseCase : IArquivoUseCase
    {
        private readonly IArquivoRepository _arquivoRepository;
        private readonly IMapper _mapper;
        private readonly IMessageBrokerProducer _messageBrokerProducer;
        private readonly string _s3BucketName = "fiapxarquivosbucket";
        private readonly IAmazonS3 _s3Client;
        public ArquivoUseCase(IArquivoRepository arquivoRepository, IMapper mapper, IMessageBrokerProducer messageBrokerProducer, IAmazonS3 s3Client)
        {
            _arquivoRepository = arquivoRepository;
            _mapper = mapper;
            _messageBrokerProducer = messageBrokerProducer;
            _s3Client = s3Client;
        }
        public async Task CreateFile(ArquivoDto arquivoDto, Stream stream)
        {
            try
            {
                var arquivo = ArquivoFactory.Create(arquivoDto);

                await _arquivoRepository.CreateFile(arquivo);
                
                await UploadFileAsync(_s3Client, stream, _s3BucketName, arquivo.Id.ToString(), arquivo.ContentType);

                await _messageBrokerProducer.SendMessageAsync(arquivo);                                         

            }          
            catch (Exception e)
            {
                Console.WriteLine($"Erro geral: {e.Message}");              
            }
        }

        private static async Task UploadFileAsync(IAmazonS3 s3Client, Stream fileStream, string bucketName, string keyName, string contentType)
        {            
            var uploadRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = $"{keyName}/{keyName}{GetVideoExtensionFromContentType(contentType)}",
                InputStream = fileStream,
                ContentType = contentType
            };

            var response = await s3Client.PutObjectAsync(uploadRequest);
        }
        private static string GetVideoExtensionFromContentType(string contentType)
        {
            var videoMimeMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "video/mp4", ".mp4" },
                { "video/x-msvideo", ".avi" },
                { "video/x-matroska", ".mkv" },
                { "video/webm", ".webm" },
                { "video/ogg", ".ogv" },
                { "video/mpeg", ".mpeg" },
                { "video/quicktime", ".mov" },
                { "video/x-flv", ".flv" },
                { "video/3gpp", ".3gp" },
                { "video/3gpp2", ".3g2" }
            };

            return videoMimeMapping.TryGetValue(contentType, out var extension) ? extension : string.Empty;
        }
        public async Task<List<ArquivoDto>> GetAllByUserId(Guid userId)
        {
            var listaPedidos = await _arquivoRepository.GetAllByUserId(userId);
            
            return _mapper.Map<List<ArquivoDto>>(listaPedidos);
        }

        public async Task<ArquivoDto> UpdateStatus(Guid id, int status)
        {
            var arquivo = await _arquivoRepository.GetById(id);

            if (arquivo is null)
                throw new Exception($"PedidoId {id} inválido");

            if (!Enum.IsDefined(typeof(StatusEnum), status))
                throw new Exception($"Status {status} inválido");

            arquivo.UpdateStatus((StatusEnum)status);

            return _mapper.Map<ArquivoDto>(await _arquivoRepository.Update(arquivo));
        }

        public async Task<Stream> DownloadZip(string key)
        {            
            if (!Guid.TryParse(key, out Guid id))
                throw new Exception("ID inválido.");

            var arquivo = await _arquivoRepository.GetById(id);
            if (arquivo is null)
                throw new Exception("Arquivo não encontrado.");

            var s3Object = await _s3Client.GetObjectAsync(_s3BucketName, $"{key}/snapshots.zip");

            return s3Object.ResponseStream;          
        }
    }
}
