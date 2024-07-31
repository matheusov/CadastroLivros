using System.Globalization;
using CadastroLivros.Application;
using CadastroLivros.Core;
using CadastroLivros.Infrastructure;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Iniciando a aplicação");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration));

    builder.Services.AddOptions<AppSettings>()
        .Bind(builder.Configuration)
        .ValidateDataAnnotations()
        .ValidateOnStart();

    var appSettings = builder.Configuration.Get<AppSettings>()!;

    builder.Services
        .AddApplication(appSettings)
        .AddInfrastructure(appSettings)
        .AddControllersWithViews();

    var app = builder.Build();

    var defaultCulture = new CultureInfo("pt-BR"); // Cultura brasileira
    CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
    CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "A aplicação parou inesperadamente");
}
finally
{
    Log.Information("Desligando");
    Log.CloseAndFlush();
}
