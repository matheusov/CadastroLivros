using CadastroLivros.Core.Enums;

namespace CadastroLivros.Application.Services.Livros;

public class PesquisarLivroResponse
{
    public int? CodL { get; set; }
    public string? Titulo { get; set; }
    public string? Editora { get; set; }
    public int? Edicao { get; set; }
    public string? AnoPublicacao { get; set; }
    public string? Autores { get; set; }
    public string? Assuntos { get; set; }
    public List<PesquisarLivroFormaCompraResponse> FormasCompra { get; set; } = [];
}

public class PesquisarLivroFormaCompraResponse
{
    public FormaCompra FormaCompra { get; set; }

    public decimal? Valor { get; set; }
}
