using CadastroLivros.Application.Interfaces;
using CadastroLivros.Web.Models.Assuntos;
using CadastroLivros.Web.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace CadastroLivros.Web.Controllers;

public class AssuntosController : Controller
{
    private readonly ILogger<AssuntosController> _logger;
    private readonly IAssuntoRepository _assuntoRepository;
    private readonly ILivroRepository _livroRepository;

    public AssuntosController(
        ILogger<AssuntosController> logger,
        IAssuntoRepository assuntoRepository,
        ILivroRepository livroRepository
    )
    {
        _logger = logger;
        _livroRepository = livroRepository;
        _assuntoRepository = assuntoRepository;
    }

    [HttpGet("Assuntos/{id:int}")]
    public async Task<IActionResult> Index(int id)
    {
        var assunto = await _assuntoRepository.PesquisarPorId(id);
        if (assunto is null)
        {
            this.SetErrorResult("Assunto não encontrado");
            return RedirectToAction("Index", "Home");
        }

        var model = new AssuntosControllerViewModel
        {
            Assunto = assunto,
            Livros = await _livroRepository.PesquisarPorAssunto(assunto.CodAs)
        };

        return View(model);
    }
}
