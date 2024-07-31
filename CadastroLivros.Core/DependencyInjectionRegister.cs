using CadastroLivros.Core.Repositories;
using CadastroLivros.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;

namespace CadastroLivros.Core;

public static class DependencyInjectionRegister
{
    public static IServiceCollection AddCore(this IServiceCollection services, AppSettings configuration)
    {
        services.AddScoped<LivroRepository>();
        services.AddScoped<AutorRepository>();
        services.AddScoped<AssuntoRepository>();
        services.AddScoped<LivroValorRepository>();

        services.AddScoped<RelatorioService>();

        QuestPDF.Settings.License = LicenseType.Community;
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            QuestPDF.Settings.EnableDebugging = true;

        return services;
    }
}
