using CadastroLivros.Application.Services;
using CadastroLivros.Core;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;

namespace CadastroLivros.Application;

public static class DependencyInjectionRegister
{
    public static IServiceCollection AddApplication(this IServiceCollection services, AppSettings configuration)
    {
        services.AddScoped<RelatorioService>();

        QuestPDF.Settings.License = LicenseType.Community;
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            QuestPDF.Settings.EnableDebugging = true;

        return services;
    }
}
