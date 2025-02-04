using AutoMapper;
using FIAPX.Cadastro.Application.UseCase;
using FIAPX.Cadastro.Domain.Interfaces.Repositories;
using FIAPX.Cadastro.Domain.Producer;
using Moq;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using FIAPX.Cadastro.Domain.Enum;
using FIAPX.Cadastro.Domain.Entities;
using FIAPX.Cadastro.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace FIAPX.Cadastro.Tests
{
    public class ArquivoUseCaseTests
    {
        private readonly Mock<IArquivoRepository> _arquivoRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMessageBrokerProducer> _messageBrokerProducerMock;
        private readonly Mock<IAmazonS3> _s3ClientMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
        private readonly Mock<ArquivoUseCase> _arquivoUseCaseMock;
        private readonly ArquivoUseCase _arquivoUseCase;
        private readonly MemoryStream _memoryStream = new();
        public ArquivoUseCaseTests()
        {
            _arquivoRepositoryMock = new Mock<IArquivoRepository>();
            _mapperMock = new Mock<IMapper>();
            _messageBrokerProducerMock = new Mock<IMessageBrokerProducer>();
            _s3ClientMock = new Mock<IAmazonS3>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();

            var getObjectResponse = new GetObjectResponse
            {
                ResponseStream = _memoryStream,
                HttpStatusCode = System.Net.HttpStatusCode.OK
            };

            _s3ClientMock
                .Setup(client => client.GetObjectAsync(It.IsAny<string>(), It.IsAny<string>(), default))
                .ReturnsAsync(getObjectResponse);

            _arquivoUseCaseMock = new Mock<ArquivoUseCase>(
                _arquivoRepositoryMock.Object,
                _mapperMock.Object,
                _messageBrokerProducerMock.Object,
                _s3ClientMock.Object,
                _httpContextAccessor.Object
            );

            _arquivoUseCase = _arquivoUseCaseMock.Object;
        }

        [Fact]
        public async Task CreateFile_ShouldCallRepositoryAndUpload_WhenValidInput()
        {
            // Arrange
            var arquivoDto = new ArquivoDto { ContentType = "video/mp4", FileName = "teste.mp4" };
            var stream = new MemoryStream();
            var arquivo = new Arquivo(Guid.NewGuid(), "teste.mp4", "video/mp4", StatusEnum.Cadastrado, Guid.Empty);

            _arquivoRepositoryMock.Setup(repo => repo.CreateFile(It.IsAny<Arquivo>())).Returns(Task.FromResult(arquivo));
            _s3ClientMock
                .Setup(client => client.PutObjectAsync(It.IsAny<PutObjectRequest>(), default))
                .ReturnsAsync(new PutObjectResponse());
            _messageBrokerProducerMock.Setup(producer => producer.SendMessageAsync(It.IsAny<Arquivo>())).Returns(Task.CompletedTask);

            // Act
            await _arquivoUseCase.CreateFile(arquivoDto, stream);
         
            // Assert
            _arquivoRepositoryMock.Verify(repo => repo.CreateFile(It.IsAny<Arquivo>()), Times.Once);
        }

        [Fact]
        public async Task CreateFile_ShouldGiveAnException_WithInvalidInput()
        {
            // Arrange
            var arquivoDto = new ArquivoDto { ContentType = "video/mp25", FileName = "teste.mp4" };
            var stream = new MemoryStream();
            var arquivo = new Arquivo(Guid.NewGuid(), "teste.mp25", "video/mp25", StatusEnum.Cadastrado, Guid.Empty);

            _arquivoRepositoryMock.Setup(repo => repo.CreateFile(It.IsAny<Arquivo>())).Returns(Task.FromResult(arquivo));
            _s3ClientMock
                .Setup(client => client.PutObjectAsync(It.IsAny<PutObjectRequest>(), default))
                .ReturnsAsync(new PutObjectResponse());
            _messageBrokerProducerMock.Setup(producer => producer.SendMessageAsync(It.IsAny<Arquivo>())).Returns(Task.CompletedTask);

            await Assert.ThrowsAsync<Exception>(() => _arquivoUseCase.CreateFile(arquivoDto, stream));
        }

        [Fact]
        public async Task GetAll_ShouldReturnMappedList()
        {
            // Arrange
            var arquivoList = new List<Arquivo> { new Arquivo(Guid.NewGuid(), string.Empty, string.Empty, StatusEnum.Cadastrado, Guid.Empty) };
            var arquivoDtoList = new List<ArquivoDto> { new() { FileName = "arquivo1", ContentType = "" } };

            _arquivoRepositoryMock.Setup(repo => repo.GetAllByUserId(Guid.Empty)).ReturnsAsync(arquivoList);
            _mapperMock.Setup(mapper => mapper.Map<List<ArquivoDto>>(arquivoList)).Returns(arquivoDtoList);

            // Act
            var result = await _arquivoUseCase.GetAllByUserId(Guid.Empty);

            // Assert
            Assert.Equal(arquivoDtoList, result);
            _arquivoRepositoryMock.Verify(repo => repo.GetAllByUserId(Guid.Empty), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<List<ArquivoDto>>(arquivoList), Times.Once);
        }

        [Fact]
        public async Task UpdateStatus_ShouldUpdateStatusAndReturnDto_WhenValidInput()
        {
            // Arrange
            var id = Guid.NewGuid();
            var arquivo = new Arquivo(id, "", "", StatusEnum.Cadastrado, Guid.Empty);
            var arquivoDto = new ArquivoDto { Id = id, FileName = "arquivo1", ContentType = "" };

            _arquivoRepositoryMock.Setup(repo => repo.GetById(id)).ReturnsAsync(arquivo);
            _arquivoRepositoryMock.Setup(repo => repo.Update(It.IsAny<Arquivo>())).ReturnsAsync(arquivo);
            _mapperMock.Setup(mapper => mapper.Map<ArquivoDto>(arquivo)).Returns(arquivoDto);

            // Act
            var result = await _arquivoUseCase.UpdateStatus(id, (int)StatusEnum.Processado);

            // Assert
            Assert.Equal(arquivoDto, result);
            _arquivoRepositoryMock.Verify(repo => repo.GetById(id), Times.Once);
            _arquivoRepositoryMock.Verify(repo => repo.Update(It.IsAny<Arquivo>()), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<ArquivoDto>(arquivo), Times.Once);
        }

        [Fact]
        public async Task CreateFile_ShouldThrowException_WhenInvalidFile()
        {
            // Arrange
            var id = Guid.NewGuid();

            _arquivoRepositoryMock.Setup(repo => repo.GetById(id)).ThrowsAsync(new Exception("Erro ao buscar o arquivo"));

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _arquivoUseCase.CreateFile((ArquivoDto)null, new MemoryStream()));
        }

        [Fact]
        public async Task UpdateStatus_ShouldThrowException_WhenInvalidStatus()
        {
            // Arrange
            var id = Guid.NewGuid();
            var arquivo = new Arquivo(id, "", "", StatusEnum.Cadastrado, Guid.Empty);

            _arquivoRepositoryMock.Setup(repo => repo.GetById(id)).ReturnsAsync(arquivo);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _arquivoUseCase.UpdateStatus(id, 999));
            _arquivoRepositoryMock.Verify(repo => repo.GetById(id), Times.Once);
        }

        [Fact]
        public async Task UpdateStatus_ShouldThrowException_WhenInvalidArquivoId()
        {
            // Arrange
            var id = Guid.NewGuid();

            _arquivoRepositoryMock.Setup(repo => repo.GetById(id)).ReturnsAsync((Arquivo)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _arquivoUseCase.UpdateStatus(id, 999));
            _arquivoRepositoryMock.Verify(repo => repo.GetById(id), Times.Once);
        }

        [Fact]
        public async Task DownloadZip_ShouldReturnStream_WhenValidKey()
        {
            // Arrange
            var id = Guid.NewGuid();
            var arquivo = new Arquivo(id, "", "", StatusEnum.Cadastrado, Guid.Empty);
        
            _arquivoRepositoryMock.Setup(repo => repo.GetById(id)).ReturnsAsync(arquivo);    

            // Act
            var result = await _arquivoUseCase.DownloadZip(id.ToString());

            // Assert
            Assert.Equal(_memoryStream, result);
            _arquivoRepositoryMock.Verify(repo => repo.GetById(id), Times.Once);
            _s3ClientMock.Verify(client => client.GetObjectAsync(It.IsAny<string>(), It.IsAny<string>(), default), Times.Once);
        }



        [Fact]
        public async Task DownloadZip_ShouldThrowException_WhenInvalidKey()
        {
            // Arrange
            var invalidKey = "invalid-guid";

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _arquivoUseCase.DownloadZip(invalidKey));
        }

        [Fact]
        public async Task DownloadZip_ShouldReturnException_WhenNullFile()
        {
            // Arrange
            var id = Guid.NewGuid();

            _arquivoRepositoryMock.Setup(repo => repo.GetById(id)).ReturnsAsync((Arquivo)null);

            // Assert & Act
            await Assert.ThrowsAsync<Exception>(() => _arquivoUseCase.DownloadZip(id.ToString()));
        }
    }
}