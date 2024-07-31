using System.Diagnostics;
using CadastroLivros.Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using CadastroLivros.Web.Models;
using CadastroLivros.Web.Models.Home;

namespace CadastroLivros.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly LivroRepository _livroRepository;
    private readonly AutorRepository _autorRepository;
    private readonly AssuntoRepository _assuntoRepository;

    public HomeController(
        ILogger<HomeController> logger,
        LivroRepository livroRepository,
        AutorRepository autorRepository,
        AssuntoRepository assuntoRepository
    )
    {
        _logger = logger;
        _livroRepository = livroRepository;
        _autorRepository = autorRepository;
        _assuntoRepository = assuntoRepository;
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
