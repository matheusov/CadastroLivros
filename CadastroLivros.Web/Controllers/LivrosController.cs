using CadastroLivros.Application.Interfaces;
using CadastroLivros.Core.Entities;
using CadastroLivros.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using CadastroLivros.Web.Models.Livros;
using CadastroLivros.Web.Utilities;

namespace CadastroLivros.Web.Controllers;

public class LivrosController : Controller
{
    private readonly ILogger<LivrosController> _logger;
    private readonly ILivroRepository _livroRepository;
    private readonly IAutorRepository _autorRepository;
    private readonly IAssuntoRepository _assuntoRepository;
    private readonly ILivroValorRepository _livroValorRepository;

    public LivrosController(
        ILogger<LivrosController> logger,
        ILivroRepository livroRepository,
        IAutorRepository autorRepository,
        IAssuntoRepository assuntoRepository,
        ILivroValorRepository livroValorRepository
    )
    {
        _logger = logger;
        _livroRepository = livroRepository;
        _autorRepository = autorRepository;
        _assuntoRepository = assuntoRepository;
        _livroValorRepository = livroValorRepository;
    }

    [HttpGet("Livros/{id:int?}")]
    public async Task<IActionResult> Index(int? id)
    {
        var model = new LivrosControllerViewModel();

        var formasCompra = Enum.GetValues<FormaCompra>().Select(f => new FormaCompraViewModel { FormaCompra = f }).ToList();

        if (id is null or 0)
        {
            model.FormasCompra = formasCompra;
            return View(model);
        }

        var livro = await _livroRepository.PesquisarPorId(id.Value);
        if (livro is null)
        {
            this.SetErrorResult("Livro não encontrado");
            return RedirectToAction("Index");
        }

        model.CodL = livro.CodL;
        model.Titulo = livro.Titulo;
        model.Editora = livro.Editora;
        model.Edicao = livro.Edicao;
        model.AnoPublicacao = livro.AnoPublicacao;

        var autores = await _autorRepository.PesquisarPorLivro(livro.CodL);
        model.Autores = string.Join(", ", autores.Select(a => a.Nome));

        var assuntos = await _assuntoRepository.PesquisarPorLivro(livro.CodL);
        model.Assuntos = string.Join(", ", assuntos.Select(a => a.Descricao));

        var livroValores = await _livroValorRepository.PesquisarPorLivro(livro.CodL);
        foreach (var livroValor in livroValores)
        {
            var formaCompra = formasCompra.FirstOrDefault(f => f.FormaCompra == livroValor.IdFormaCompra);
            if (formaCompra is not null)
            {
                formaCompra.Valor = livroValor.Valor;
            }
        }

        model.FormasCompra = formasCompra;

        return View(model);
    }

    public async Task<IActionResult> Inserir(LivrosControllerViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        var livro = new Livro
        {
            Titulo = model.Titulo!,
            Editora = model.Editora!,
            Edicao = model.Edicao!.Value,
            AnoPublicacao = model.AnoPublicacao!
        };

        int codL = await _livroRepository.Inserir(livro);

        if (model.Autores is not null)
        {
            string[] autores = model.Autores.Split(",");

            var autoresDistintos = autores.Select(a => a.Trim()).Where(a => !string.IsNullOrWhiteSpace(a)).Distinct();

            foreach (string nomeAutor in autoresDistintos)
            {
                var autor = new Autor { Nome = nomeAutor.Trim() };

                var autorExistente = await _autorRepository.PesquisarPorNome(autor.Nome);
                int codAu = autorExistente?.CodAu ?? await _autorRepository.Inserir(autor);

                await _livroRepository.InserirAutorLivro(codL, codAu);
            }
        }

        if (model.Assuntos is not null)
        {
            string[] assuntos = model.Assuntos.Split(",");

            var assuntosDistintos = assuntos.Select(a => a.Trim()).Where(a => !string.IsNullOrWhiteSpace(a)).Distinct();

            foreach (string descricaoAssunto in assuntosDistintos)
            {
                var assunto = new Assunto { Descricao = descricaoAssunto };

                var assuntoExistente = await _assuntoRepository.PesquisarPorDescricao(assunto.Descricao);
                int codAs = assuntoExistente?.CodAs ?? await _assuntoRepository.Inserir(assunto);

                await _livroRepository.InserirAssuntoLivro(codL, codAs);
            }
        }

        foreach (var formaCompraViewModel in model.FormasCompra)
        {
            var livroValor = new LivroValor
            {
                CodL = codL,
                IdFormaCompra = formaCompraViewModel.FormaCompra,
                Valor = formaCompraViewModel.Valor
            };

            await _livroValorRepository.InserirOuAtualizar(livroValor);
        }

        this.SetSuccessResult("Livro inserido com sucesso");

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Editar(LivrosControllerViewModel model, int id)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        if (model.CodL is null)
        {
            return RedirectToAction("Index", "Home");
        }

        var livro = await _livroRepository.PesquisarPorId(model.CodL!.Value);
        if (livro is null)
        {
            return RedirectToAction("Index", "Home");
        }

        livro.Titulo = model.Titulo!;
        livro.Editora = model.Editora!;
        livro.Edicao = model.Edicao!.Value;
        livro.AnoPublicacao = model.AnoPublicacao!;

        await _livroRepository.Alterar(livro);

        var autoresInformados = model.Autores!.Split(",").Select(a => a.Trim()).ToList();
        var autores = await _autorRepository.PesquisarPorLivro(livro.CodL);

        var autoresParaInserir = autoresInformados.Where(ai => !autores.Select(a => a.Nome).Contains(ai)).ToList();
        var autoresParaExcluir = autores.Where(a => !autoresInformados.Contains(a.Nome)).ToList();

        foreach (string nomeAutor in autoresParaInserir)
        {
            var autor = new Autor { Nome = nomeAutor };
            int codAu = await _autorRepository.Inserir(autor);

            await _livroRepository.InserirAutorLivro(livro.CodL, codAu);
        }

        foreach (var autor in autoresParaExcluir)
        {
            await _livroRepository.ExcluirAutorLivro(livro.CodL, autor.CodAu);
        }

        var assuntosInformados = model.Assuntos!.Split(",").Select(a => a.Trim()).ToList();
        var assuntos = await _assuntoRepository.PesquisarPorLivro(livro.CodL);

        var assuntosParaInserir = assuntosInformados.Where(ai => !assuntos.Select(a => a.Descricao).Contains(ai)).ToList();
        var assuntosParaExcluir = assuntos.Where(a => !assuntosInformados.Contains(a.Descricao)).ToList();

        foreach (string descricaoAssunto in assuntosParaInserir)
        {
            var assunto = new Assunto { Descricao = descricaoAssunto };
            int codAs = await _assuntoRepository.Inserir(assunto);

            await _livroRepository.InserirAssuntoLivro(livro.CodL, codAs);
        }

        foreach (var assunto in assuntosParaExcluir)
        {
            await _livroRepository.ExcluirAssuntoLivro(livro.CodL, assunto.CodAs);
        }

        foreach (var pd in model.FormasCompra)
        {
            var livroValor = new LivroValor
            {
                CodL = livro.CodL,
                IdFormaCompra = pd.FormaCompra,
                Valor = pd.Valor
            };

            await _livroValorRepository.InserirOuAtualizar(livroValor);
        }

        this.SetSuccessResult("Livro alterado com sucesso");

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Excluir(int codL)
    {
        await _livroRepository.Excluir(codL);

        this.SetSuccessResult("Livro excluído com sucesso");

        return RedirectToAction("Index", "Home");
    }
}
