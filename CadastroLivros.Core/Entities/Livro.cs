namespace CadastroLivros.Core.Entities;

public class Livro
{
    public int CodL { get; set; }
    public required string Titulo { get; set; }
    public required string Editora { get; set; }
    public int Edicao { get; set; }
    public required string AnoPublicacao { get; set; }
}
