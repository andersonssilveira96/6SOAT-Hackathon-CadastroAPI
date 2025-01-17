using AutoMapper;
using FIAPX.Cadastro.Application.DTOs;
using FIAPX.Cadastro.Application.UseCase;
using FIAPX.Cadastro.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace FIAPX.Cadastro.Application
{
    public static class ServiceApplicationExtensions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            services.AddScoped<IArquivoUseCase, ArquivoUseCase>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ArquivoDto, Arquivo>().ReverseMap();            
            });

            IMapper mapper = config.CreateMapper();

            services.AddSingleton(mapper);

            return services;
        }
    }
}

