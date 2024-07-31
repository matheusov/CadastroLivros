using CadastroLivros.Application.Interfaces;
using CadastroLivros.Core.Common;
using CadastroLivros.Core.Entities;
using CadastroLivros.Core.Enums;
using CadastroLivros.Core.Result;
using Microsoft.Extensions.Logging;

namespace CadastroLivros.Application.Services.Livros;

public class LivroService
{
    private readonly ILogger<LivroService> _logger;
    private readonly ILivroRepository _livroRepository;
    private readonly IAutorRepository _autorRepository;
    private readonly IAssuntoRepository _assuntoRepository;
    private readonly ILivroValorRepository _livroValorRepository;

    public LivroService(ILogger<LivroService> logger, ILivroRepository livroRepository, IAutorRepository autorRepository, IAssuntoRepository assuntoRepository, ILivroValorRepository livroValorRepository)
    {
        _logger = logger;
        _livroRepository = livroRepository;
        _autorRepository = autorRepository;
        _assuntoRepository = assuntoRepository;
        _livroValorRepository = livroValorRepository;
    }

    public async Task<Result<PesquisarLivroResponse>> PesquisarLivro(int id)
    {
        var model = new PesquisarLivroResponse();

        var formasCompra = Enum.GetValues<FormaCompra>().Select(f => new PesquisarLivroFormaCompraResponse { FormaCompra = f }).ToList();

        var livro = await _livroRepository.PesquisarPorId(id);
        if (livro is null)
        {
            return Error.NotFound("Livro não encontrado");
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

        return model;
    }

    public async Task<Result<int>> Inserir(InserirLivroDto dto)
    {
        if (ValidationHelper.IsInvalid(dto, out var validationErrors))
        {
            return validationErrors;
        }

        var livro = new Livro
        {
            Titulo = dto.Titulo!,
            Editora = dto.Editora!,
            Edicao = dto.Edicao!.Value,
            AnoPublicacao = dto.AnoPublicacao!
        };

        int codL = await _livroRepository.Inserir(livro);

        if (dto.Autores is not null)
        {
            var autoresDistintos = dto.Autores.Split(",").Select(a => a.Trim())
                .Where(a => !string.IsNullOrWhiteSpace(a)).Distinct();

            foreach (string nomeAutor in autoresDistintos)
            {
                var autor = new Autor { Nome = nomeAutor };

                var autorExistente = await _autorRepository.PesquisarPorNome(autor.Nome);
                int codAu = autorExistente?.CodAu ?? await _autorRepository.Inserir(autor);

                await _livroRepository.InserirAutorLivro(codL, codAu);
            }
        }

        if (dto.Assuntos is not null)
        {
            string[] assuntos = dto.Assuntos.Split(",");

            var assuntosDistintos = assuntos.Select(a => a.Trim()).Where(a => !string.IsNullOrWhiteSpace(a)).Distinct();

            foreach (string descricaoAssunto in assuntosDistintos)
            {
                var assunto = new Assunto { Descricao = descricaoAssunto };

                var assuntoExistente = await _assuntoRepository.PesquisarPorDescricao(assunto.Descricao);
                int codAs = assuntoExistente?.CodAs ?? await _assuntoRepository.Inserir(assunto);

                await _livroRepository.InserirAssuntoLivro(codL, codAs);
            }
        }

        foreach (var formaCompra in dto.FormasCompra)
        {
            var livroValor = new LivroValor
            {
                CodL = codL,
                IdFormaCompra = formaCompra.FormaCompra,
                Valor = formaCompra.Valor
            };

            await _livroValorRepository.InserirOuAtualizar(livroValor);
        }

        return codL;
    }

    public async Task<Result<Success>> Alterar(AlterarLivroDto dto)
    {
        if (ValidationHelper.IsInvalid(dto, out var validationErrors))
        {
            return validationErrors;
        }

        var livro = await _livroRepository.PesquisarPorId(dto.CodL!.Value);
        if (livro is null)
        {
            return Error.NotFound("Livro não encontrado");
        }

        livro.Titulo = dto.Titulo!;
        livro.Editora = dto.Editora!;
        livro.Edicao = dto.Edicao!.Value;
        livro.AnoPublicacao = dto.AnoPublicacao!;

        await _livroRepository.Alterar(livro);

        var autoresInformados = dto.Autores!.Split(",").Select(a => a.Trim())
            .Where(a => !string.IsNullOrWhiteSpace(a)).Distinct().ToList();

        var autores = await _autorRepository.PesquisarPorLivro(livro.CodL);

        var autoresParaInserir = autoresInformados.Where(ai => !autores.Select(a => a.Nome).Contains(ai)).ToList();
        var autoresParaExcluir = autores.Where(a => !autoresInformados.Contains(a.Nome)).ToList();

        foreach (string nomeAutor in autoresParaInserir)
        {
            var autor = new Autor { Nome = nomeAutor };

            var autorExistente = await _autorRepository.PesquisarPorNome(autor.Nome);
            int codAu = autorExistente?.CodAu ?? await _autorRepository.Inserir(autor);

            await _livroRepository.InserirAutorLivro(livro.CodL, codAu);
        }

        foreach (var autor in autoresParaExcluir)
        {
            await _livroRepository.ExcluirAutorLivro(livro.CodL, autor.CodAu);
        }

        var assuntosInformados = dto.Assuntos!.Split(",").Select(a => a.Trim())
            .Where(a => !string.IsNullOrWhiteSpace(a)).Distinct().ToList();

        var assuntos = await _assuntoRepository.PesquisarPorLivro(livro.CodL);

        var assuntosParaInserir = assuntosInformados.Where(ai => !assuntos.Select(a => a.Descricao).Contains(ai)).ToList();
        var assuntosParaExcluir = assuntos.Where(a => !assuntosInformados.Contains(a.Descricao)).ToList();

        foreach (string descricaoAssunto in assuntosParaInserir)
        {
            var assunto = new Assunto { Descricao = descricaoAssunto };

            var assuntoExistente = await _assuntoRepository.PesquisarPorDescricao(assunto.Descricao);
            int codAs = assuntoExistente?.CodAs ?? await _assuntoRepository.Inserir(assunto);

            await _livroRepository.InserirAssuntoLivro(livro.CodL, codAs);
        }

        foreach (var assunto in assuntosParaExcluir)
        {
            await _livroRepository.ExcluirAssuntoLivro(livro.CodL, assunto.CodAs);
        }

        foreach (var formaCompra in dto.FormasCompra)
        {
            var livroValor = new LivroValor
            {
                CodL = livro.CodL,
                IdFormaCompra = formaCompra.FormaCompra,
                Valor = formaCompra.Valor
            };

            await _livroValorRepository.InserirOuAtualizar(livroValor);
        }

        return Result.Success;
    }

    public async Task<Result<Success>> Excluir(int codL)
    {
        await _livroRepository.Excluir(codL);
        return Result.Success;
    }
}
