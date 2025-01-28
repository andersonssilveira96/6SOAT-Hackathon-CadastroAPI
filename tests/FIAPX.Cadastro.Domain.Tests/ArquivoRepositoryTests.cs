using FIAPX.Cadastro.Domain.Entities;
using FIAPX.Cadastro.Domain.Enum;
using FIAPX.Cadastro.Infra.Data.Context;
using FIAPX.Cadastro.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FIAPX.Cadastro.Tests
{
    public class ArquivoRepositoryTests
    {
        private DbContextOptions<FIAPXContext> CreateInMemoryDatabaseOptions() => new DbContextOptionsBuilder<FIAPXContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        private FIAPXContext CreateContext(DbContextOptions<FIAPXContext> options)
        {
            var context = new FIAPXContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task CreateFile_ShouldAddArquivoToDatabase()
        {
            // Arrange
            var options = CreateInMemoryDatabaseOptions();
            await using var context = CreateContext(options);
            var repository = new ArquivoRepository(context);

            var arquivo = new Arquivo(Guid.NewGuid(), "Test File", "", StatusEnum.Cadastrado, Guid.Empty);

            // Act
            var result = await repository.CreateFile(arquivo);
            await context.SaveChangesAsync();

            // Assert
            var dbArquivo = await context.Arquivo.FirstOrDefaultAsync(a => a.Id == arquivo.Id);
            Assert.NotNull(dbArquivo);
            Assert.Equal(arquivo.FileName, dbArquivo.FileName);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllArquivos()
        {
            // Arrange
            var options = CreateInMemoryDatabaseOptions();
            await using var context = CreateContext(options);
            var repository = new ArquivoRepository(context);

            var arquivos = new List<Arquivo>
            {
                new Arquivo(Guid.NewGuid(), "Test File 1", "", StatusEnum.Cadastrado, Guid.Empty),
                new Arquivo(Guid.NewGuid(), "Test File 2", "", StatusEnum.Cadastrado, Guid.Empty)
            };

            await context.Arquivo.AddRangeAsync(arquivos);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAllByUserId(Guid.Empty);

            // Assert
            Assert.Equal(arquivos.Count, result.Count);
            Assert.All(result, r => Assert.Contains(arquivos, a => a.Id == r.Id));
        }

        [Fact]
        public async Task GetById_ShouldReturnCorrectArquivo()
        {
            // Arrange
            var options = CreateInMemoryDatabaseOptions();
            await using var context = CreateContext(options);
            var repository = new ArquivoRepository(context);

            var arquivo = new Arquivo(Guid.NewGuid(), "Test File", "", StatusEnum.Cadastrado, Guid.Empty);
            await context.Arquivo.AddAsync(arquivo);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetById(arquivo.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(arquivo.Id, result.Id);
            Assert.Equal(arquivo.FileName, result.FileName);
        }

        [Fact]
        public async Task Update_ShouldModifyArquivoInDatabase()
        {
            // Arrange
            var options = CreateInMemoryDatabaseOptions();
            await using var context = CreateContext(options);
            var repository = new ArquivoRepository(context);

            var arquivo = new Arquivo(Guid.NewGuid(), "Test File", "", StatusEnum.Cadastrado, Guid.Empty);
            await context.Arquivo.AddAsync(arquivo);
            await context.SaveChangesAsync();

            // Act
            arquivo.UpdateStatus(StatusEnum.Processado);
            var result = await repository.Update(arquivo);

            // Assert
            var dbArquivo = await context.Arquivo.FirstOrDefaultAsync(a => a.Id == arquivo.Id);
            Assert.NotNull(dbArquivo);
            Assert.Equal(StatusEnum.Processado, dbArquivo.Status);
        }
    }
}
