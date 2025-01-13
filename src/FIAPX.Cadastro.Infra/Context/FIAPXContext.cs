using Microsoft.EntityFrameworkCore;

namespace FIAPX.Cadastro.Infra.Context
{
    public sealed class FIAPXContext : DbContext
    {
        public FIAPXContext(DbContextOptions<FIAPXContext> options)
            : base(options)
        {
        }
    }
}
