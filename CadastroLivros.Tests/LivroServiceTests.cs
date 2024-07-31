using Moq;
using CadastroLivros.Application.Services.Livros;
using CadastroLivros.Application.Interfaces;
using CadastroLivros.Core.Entities;
using CadastroLivros.Core.Result;
using Microsoft.Extensions.Logging.Abstractions;
using CadastroLivros.Core.Enums;

namespace CadastroLivros.Tests;

public class LivroServiceTests
{
    private readonly LivroService _livroService;
    private readonly Mock<ILivroRepository> _mockLivroRepository;
    private readonly Mock<IAutorRepository> _mockAutorRepository;
    private readonly Mock<IAssuntoRepository> _mockAssuntoRepository;
    private readonly Mock<ILivroValorRepository> _mockLivroValorRepository;

    public LivroServiceTests()
    {
        _mockLivroRepository = new Mock<ILivroRepository>();
        _mockAutorRepository = new Mock<IAutorRepository>();
        _mockAssuntoRepository = new Mock<IAssuntoRepository>();
        _mockLivroValorRepository = new Mock<ILivroValorRepository>();

        _livroService = new LivroService(
            new NullLogger<LivroService>(),
            _mockLivroRepository.Object,
            _mockAutorRepository.Object,
            _mockAssuntoRepository.Object,
            _mockLivroValorRepository.Object
        );
    }

    [Fact]
    public async Task PesquisarLivro_DeveRetornarLivroQuandoIdExistente()
    {
        // Arrange
        int idLivro = 1;
        var livroEsperado = new Livro { CodL = idLivro, Titulo = "Livro Teste", Editora = "Editora Teste", Edicao = 1, AnoPublicacao = "2024" };
        var autoresEsperados = new List<Autor> { new() { Nome = "Autor 1" }, new Autor { Nome = "Autor 2" } };
        var assuntosEsperados = new List<Assunto> { new() { Descricao = "Assunto 1" }, new Assunto { Descricao = "Assunto 2" } };
        var valoresEsperados = new List<LivroValor> { new() { IdFormaCompra = FormaCompra.Internet, Valor = 12.34m } };

        _mockLivroRepository.Setup(repo => repo.PesquisarPorId(idLivro)).ReturnsAsync(livroEsperado);
        _mockAutorRepository.Setup(repo => repo.PesquisarPorLivro(idLivro)).ReturnsAsync(autoresEsperados);
        _mockAssuntoRepository.Setup(repo => repo.PesquisarPorLivro(idLivro)).ReturnsAsync(assuntosEsperados);
        _mockLivroValorRepository.Setup(repo => repo.PesquisarPorLivro(idLivro)).ReturnsAsync(valoresEsperados);

        // Act
        var result = await _livroService.PesquisarLivro(idLivro);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(idLivro, result.Value.CodL);
        Assert.Equal("Livro Teste", result.Value.Titulo);
        Assert.Equal("Autor 1, Autor 2", result.Value.Autores);
        Assert.Equal("Assunto 1, Assunto 2", result.Value.Assuntos);
        Assert.Equal(4, result.Value.FormasCompra.Count);
        Assert.Equal(12.34m, result.Value.FormasCompra.First(x => x.FormaCompra == FormaCompra.Internet).Valor);
        Assert.Null(result.Value.FormasCompra.First(x => x.FormaCompra != FormaCompra.Internet).Valor);
    }

    [Fact]
    public async Task PesquisarLivro_DeveRetornarErroQuandoLivroNaoEncontrado()
    {
        // Arrange
        var idInexistente = 99;

        _mockLivroRepository.Setup(repo => repo.PesquisarPorId(idInexistente)).ReturnsAsync((Livro?)null);

        // Act
        var result = await _livroService.PesquisarLivro(idInexistente);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Errors.FirstOrDefault().Type);
    }

    [Fact]
    public async Task Inserir_DeveRetornarCodigoLivroQuandoInseridoComSucesso()
    {
        // Arrange
        var dto = new InserirLivroDto
        {
            Titulo = "Novo Livro",
            Editora = "Editora Exemplo",
            Edicao = 1,
            AnoPublicacao = "2024",
            Autores = "Autor 1, Autor 2",
            Assuntos = "Assunto 1, Assunto 2",
            FormasCompra = [new InserirLivroFormaCompraDto { FormaCompra = FormaCompra.Internet, Valor = 15.2m }]
        };

        _mockLivroRepository.Setup(repo => repo.Inserir(It.IsAny<Livro>())).ReturnsAsync(1);
        _mockAutorRepository.Setup(repo => repo.Inserir(It.IsAny<Autor>())).ReturnsAsync(1);
        _mockAssuntoRepository.Setup(repo => repo.Inserir(It.IsAny<Assunto>())).ReturnsAsync(1);

        // Act
        var result = await _livroService.Inserir(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value);
    }

    [Fact]
    public async Task Inserir_DeveRetornarErroDeValidacaoQuandoDtoInvalido()
    {
        // Arrange
        var dto = new InserirLivroDto
        {
            Titulo = "", // Título inválido
            Editora = null, // Editora inválida
            Edicao = 1,
            AnoPublicacao = "2024",
            Autores = "", // Autores inválidos
            Assuntos = ", , , ", // Assuntos inválidos
            FormasCompra = [new InserirLivroFormaCompraDto { FormaCompra = FormaCompra.Internet, Valor = -1m }] // Valor inválido
        };

        // Act
        var result = await _livroService.Inserir(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, erro => erro is { Type: ErrorType.Validation, Code: nameof(dto.Titulo) });
        Assert.Contains(result.Errors, erro => erro is { Type: ErrorType.Validation, Code: nameof(dto.Editora) });
        Assert.Contains(result.Errors, erro => erro is { Type: ErrorType.Validation, Code: nameof(dto.Autores) });
        Assert.Contains(result.Errors, erro => erro is { Type: ErrorType.Validation, Code: nameof(dto.Assuntos) });
    }

    [Fact]
    public async Task Alterar_DeveRetornarSucessoQuandoLivroAlterado()
    {
        // Arrange
        var dto = new AlterarLivroDto
        {
            CodL = 1,
            Titulo = "Livro Alterado",
            Editora = "Editora Alterada",
            Edicao = 2,
            AnoPublicacao = "2024",
            Autores = "Autor 1, Autor 3",
            Assuntos = "Assunto 1, Assunto 3",
            FormasCompra = [new AlterarLivroFormaCompraDto { FormaCompra = FormaCompra.Internet, Valor = 20.0m }]
        };

        var livroExistente = new Livro { CodL = 1, Titulo = "Livro Original", Editora = "Editora Original", Edicao = 1, AnoPublicacao = "2020" };

        _mockLivroRepository.Setup(repo => repo.PesquisarPorId(dto.CodL.Value)).ReturnsAsync(livroExistente);
        _mockAutorRepository.Setup(repo => repo.PesquisarPorLivro(dto.CodL.Value)).ReturnsAsync([new Autor { Nome = "Autor 1" }]);
        _mockAssuntoRepository.Setup(repo => repo.PesquisarPorLivro(dto.CodL.Value)).ReturnsAsync([new Assunto { Descricao = "Assunto 1" }]);

        // Act
        var result = await _livroService.Alterar(dto);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Alterar_DeveRetornarErroQuandoLivroNaoEncontrado()
    {
        // Arrange
        var dto = new AlterarLivroDto
        {
            CodL = 99, // ID inexistente
            Titulo = "Livro Alterado",
            Editora = "Editora Alterada",
            Edicao = 2,
            AnoPublicacao = "2024",
            Autores = "Autor 1, Autor",
            Assuntos = "Assunto 1, Assunto",
            FormasCompra = [new AlterarLivroFormaCompraDto { FormaCompra = FormaCompra.Internet, Valor = 20.0m }]
        };

        _mockLivroRepository.Setup(repo => repo.PesquisarPorId(dto.CodL.Value)).ReturnsAsync((Livro?)null);

        // Act
        var result = await _livroService.Alterar(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Errors.FirstOrDefault().Type);
    }
}
