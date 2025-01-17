using FIAPX.Cadastro.Domain.Interfaces.Repositories;
using FIAPX.Cadastro.Infra.Context;

namespace FIAPX.Cadastro.Infra.Repositories
{
    public class ArquivoRepository : IArquivoRepository
    {
        private readonly FIAPXContext _context;
        public ArquivoRepository(FIAPXContext context)
        {
            _context = context;
        }
    }
}
