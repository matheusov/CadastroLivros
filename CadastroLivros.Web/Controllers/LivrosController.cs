using CadastroLivros.Application.Services.Livros;
using CadastroLivros.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using CadastroLivros.Web.Models.Livros;
using CadastroLivros.Web.Utilities;

namespace CadastroLivros.Web.Controllers;

public class LivrosController : Controller
{
    private readonly ILogger<LivrosController> _logger;
    private readonly LivroService _livroService;

    public LivrosController(
        ILogger<LivrosController> logger,
        LivroService livroService
     )
    {
        _logger = logger;
        _livroService = livroService;
    }

    [HttpGet("Livros/{id:int?}")]
    public async Task<IActionResult> Index(int? id)
    {
        LivrosControllerViewModel model;

        if (id is null or 0)
        {
            model = new LivrosControllerViewModel();
            var formasCompra = Enum.GetValues<FormaCompra>().Select(f => new FormaCompraViewModel { FormaCompra = f }).ToList();
            model.FormasCompra = formasCompra;
            return View(model);
        }

        var result = await _livroService.PesquisarLivro(id.Value);
        if (!result.TryGetValue(out var livro, out var errors))
        {
            this.SetErrorResult("Erro ao pesquisar livro: " + string.Join(", ", errors.Select(e => e.Description)));
            return RedirectToAction("Index");
        }

        model = new LivrosControllerViewModel
        {
            CodL = livro.CodL,
            Titulo = livro.Titulo,
            Editora = livro.Editora,
            Edicao = livro.Edicao,
            AnoPublicacao = livro.AnoPublicacao,
            Autores = livro.Autores,
            Assuntos = livro.Assuntos,
            FormasCompra = livro.FormasCompra.Select(f =>
                new FormaCompraViewModel { FormaCompra = f.FormaCompra, Valor = f.Valor }).ToList()
        };

        return View(model);
    }

    public async Task<IActionResult> Inserir(LivrosControllerViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        var inserirLivroDto = new InserirLivroDto
        {
            Titulo = model.Titulo,
            Editora = model.Editora,
            Edicao = model.Edicao,
            AnoPublicacao = model.AnoPublicacao,
            Autores = model.Autores,
            Assuntos = model.Assuntos,
            FormasCompra = model.FormasCompra.Select(f =>
                new InserirLivroFormaCompraDto { FormaCompra = f.FormaCompra, Valor = f.Valor }).ToList()
        };

        var result = await _livroService.Inserir(inserirLivroDto);

        if (result.IsSuccess)
        {
            this.SetSuccessResult("Livro inserido com sucesso!");
            return RedirectToAction("Index", "Home");
        }

        this.SetErrorResult("Erro ao inserir livro: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        return View("Index", model);
    }

    public async Task<IActionResult> Alterar(LivrosControllerViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        var alterarLivroDto = new AlterarLivroDto
        {
            CodL = model.CodL,
            Titulo = model.Titulo,
            Editora = model.Editora,
            Edicao = model.Edicao,
            AnoPublicacao = model.AnoPublicacao,
            Autores = model.Autores,
            Assuntos = model.Assuntos,
            FormasCompra = model.FormasCompra.Select(f =>
                new AlterarLivroFormaCompraDto { FormaCompra = f.FormaCompra, Valor = f.Valor }).ToList()
        };

        var result = await _livroService.Alterar(alterarLivroDto);

        if (result.IsSuccess)
        {
            this.SetSuccessResult("Livro alterado com sucesso!");
            return RedirectToAction("Index", "Home");
        }

        this.SetErrorResult("Erro ao alterar livro: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        return View("Index", model);
    }

    public async Task<IActionResult> Excluir(int codL)
    {
        var result = await _livroService.Excluir(codL);

        this.SetSuccessResult(result.IsSuccess
            ? "Livro excluído com sucesso!"
            : "Erro ao excluir livro");

        return RedirectToAction("Index", "Home");
    }
}
