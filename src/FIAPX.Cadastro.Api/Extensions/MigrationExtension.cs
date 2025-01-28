using FIAPX.Cadastro.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FIAPX.Cadastro.Api.Extensions
{
    public static class MigrationExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();

            using FIAPXContext dbContext =
                scope.ServiceProvider.GetRequiredService<FIAPXContext>();

            dbContext.Database.Migrate();
        }
    }
}
