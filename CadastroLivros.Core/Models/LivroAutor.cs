namespace CadastroLivros.Core.Models;

public class LivroAutor
{
    public int CodAu { get; set; }
    public required string NomeAutor { get; set; }
    public int? CodL { get; set; }
    public string? TituloLivro { get; set; }
    public string? Editora { get; set; }
    public int? Edicao { get; set; }
    public string? AnoPublicacao { get; set; }
    public string? Assuntos { get; set; }
}
