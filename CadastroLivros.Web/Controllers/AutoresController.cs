using CadastroLivros.Core.Repositories;
using CadastroLivros.Web.Models.Autores;
using CadastroLivros.Web.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace CadastroLivros.Web.Controllers;

public class AutoresController : Controller
{
    private readonly ILogger<AutoresController> _logger;
    private readonly AutorRepository _autorRepository;
    private readonly LivroRepository _livroRepository;

    public AutoresController(
        ILogger<AutoresController> logger,
        AutorRepository autorRepository,
        LivroRepository livroRepository
    )
    {
        _logger = logger;
        _livroRepository = livroRepository;
        _autorRepository = autorRepository;
    }

    [HttpGet("Autores/{id:int}")]
    public async Task<IActionResult> Index(int id)
    {
        var autor = await _autorRepository.PesquisarPorId(id);
        if (autor is null)
        {
            this.SetErrorResult("Autor não encontrado");
            return RedirectToAction("Index", "Home");
        }

        var model = new AutoresControllerViewModel
        {
            Autor = autor,
            Livros = await _livroRepository.PesquisarPorAutor(autor.CodAu)
        };

        return View(model);
    }
}
