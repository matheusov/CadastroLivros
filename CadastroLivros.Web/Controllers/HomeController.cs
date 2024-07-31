using System.Diagnostics;
using CadastroLivros.Application.Interfaces;
using CadastroLivros.Application.Services;
using Microsoft.AspNetCore.Mvc;
using CadastroLivros.Web.Models;
using CadastroLivros.Web.Models.Home;
using CadastroLivros.Web.Utilities;

namespace CadastroLivros.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ILivroRepository _livroRepository;
    private readonly IAutorRepository _autorRepository;
    private readonly IAssuntoRepository _assuntoRepository;
    private readonly RelatorioService _relatorioService;

    public HomeController(
        ILogger<HomeController> logger,
        ILivroRepository livroRepository,
        IAutorRepository autorRepository,
        IAssuntoRepository assuntoRepository,
        RelatorioService relatorioService
    )
    {
        _logger = logger;
        _livroRepository = livroRepository;
        _autorRepository = autorRepository;
        _assuntoRepository = assuntoRepository;
        _relatorioService = relatorioService;
    }

    public async Task<IActionResult> Index()
    {
        var model = new HomeControllerViewModel
        {
            Livros = await _livroRepository.Pesquisar(),
            Autores = await _autorRepository.Pesquisar(),
            Assuntos = await _assuntoRepository.Pesquisar()
        };

        return View(model);
    }

    public async Task<IActionResult> BaixarRelatorio()
    {
        var result = await _relatorioService.GerarRelatorio();
        if (!result.TryGetValue(out var streamResponse, out var errors))
        {
            this.SetErrorResult("Erro ao gerar relatório: " + string.Join(", ", errors.Select(e => e.Description)));
            return RedirectToAction("Index");
        }

        return File(streamResponse.Stream, streamResponse.ContentType, streamResponse.NomeArquivo);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
