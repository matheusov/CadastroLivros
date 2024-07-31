namespace CadastroLivros.Core.Models;

public class DadosRelatorio
{
    public required List<DadosAutor> DadosAutor { get; set; }
}

public class DadosAutor
{
    public int CodAu { get; set; }
    public required string NomeAutor { get; set; }
    public List<DadosLivro> Livros { get; set; } = [];
}

public class DadosLivro
{
    public int CodL { get; set; }
    public required string TituloLivro { get; set; }
    public required string Editora { get; set; }
    public int Edicao { get; set; }
    public required string AnoPublicacao { get; set; }
    public string? Assuntos { get; set; }
}
