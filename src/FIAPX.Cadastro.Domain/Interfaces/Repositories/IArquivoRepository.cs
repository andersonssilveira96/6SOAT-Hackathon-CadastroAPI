﻿using FIAPX.Cadastro.Domain.Entities;

namespace FIAPX.Cadastro.Domain.Interfaces.Repositories
{
    public interface IArquivoRepository
    {
        Task<Arquivo> CreateFile(Arquivo arquivoDto);
        Task<List<Arquivo>> GetAllByUserId(Guid userId);
        Task<Arquivo> GetById(Guid id);
        Task<Arquivo> Update(Arquivo arquivo);
    }
}
