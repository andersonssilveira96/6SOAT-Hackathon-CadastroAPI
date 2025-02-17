﻿using FIAPX.Cadastro.Domain.Entities;
using FIAPX.Cadastro.Domain.Interfaces.Repositories;
using FIAPX.Cadastro.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FIAPX.Cadastro.Infra.Data.Repositories
{
    public class ArquivoRepository : IArquivoRepository
    {
        private readonly FIAPXContext _context;
        public ArquivoRepository(FIAPXContext context)
        {
            _context = context;
        }

        public async Task<Arquivo> CreateFile(Arquivo arquivo)
        {
            if (arquivo is null)
            {
                throw new ArgumentNullException(nameof(arquivo));
            }

            _context.Arquivo.Add(arquivo);

            return arquivo;
        }

        public async Task<List<Arquivo>> GetAllByUserId(Guid userId) => await _context.Arquivo.Where(x => x.UserId == userId).ToListAsync();
        public async Task<Arquivo> GetById(Guid id) => await _context.Arquivo.FirstOrDefaultAsync(x => x.Id == id);       
        public async Task<Arquivo> Update(Arquivo arquivo)
        {
            var entry = _context.Entry(arquivo);

            _context.Arquivo.Update(entry.Entity);

            await _context.SaveChangesAsync();

            return arquivo;
        }
    }
}
