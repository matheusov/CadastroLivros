namespace CadastroLivros.Core.Models;

public class LivroComAutorEAssunto
{
    public int CodL { get; set; }
    public required string Titulo { get; set; }
    public required string Editora { get; set; }
    public int Edicao { get; set; }
    public required string AnoPublicacao { get; set; }
    public string? Autores { get; set; }
    public string? Assuntos { get; set; }
}
