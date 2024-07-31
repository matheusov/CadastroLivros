using CadastroLivros.Application.Interfaces;
using CadastroLivros.Core;
using CadastroLivros.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CadastroLivros.Infrastructure;

public static class DependencyInjectionRegister
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, AppSettings configuration)
    {
        services.AddScoped<ILivroRepository, LivroRepository>();
        services.AddScoped<IAutorRepository, AutorRepository>();
        services.AddScoped<IAssuntoRepository, AssuntoRepository>();
        services.AddScoped<ILivroValorRepository, LivroValorRepository>();

        return services;
    }
}
