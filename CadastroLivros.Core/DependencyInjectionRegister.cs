using CadastroLivros.Core.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CadastroLivros.Core;

public static class DependencyInjectionRegister
{
    public static IServiceCollection AddCore(this IServiceCollection services, AppSettings configuration)
    {
        services.AddScoped<LivroRepository>();
        services.AddScoped<AutorRepository>();
        services.AddScoped<AssuntoRepository>();
        services.AddScoped<LivroValorRepository>();

        return services;
    }
}
