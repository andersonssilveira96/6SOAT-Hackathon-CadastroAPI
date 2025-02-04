using FIAPX.Cadastro.Application.DTOs;

namespace FIAPX.Cadastro.Application.Factories
{
    public static class UsuarioFactory
    {
        public static UsuarioDto Create(string user)
        {
            return new UsuarioDto();
        }
    }
}
