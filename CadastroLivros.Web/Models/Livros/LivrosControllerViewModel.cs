using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CadastroLivros.Web.Models.Livros;

public class LivrosControllerViewModel
{
    [Range(1, int.MaxValue)]
    public int? CodL { get; set; }

    [Required, StringLength(40)]
    [DisplayName("Título")]
    public string? Titulo { get; set; }

    [Required, StringLength(40)]
    public string? Editora { get; set; }

    [Required, Range(1, int.MaxValue)]
    [DisplayName("Edição")]
    public int? Edicao { get; set; }

    [DisplayName("Ano de publicação")]
    [Required, Range(0000, 9999)]
    public string? AnoPublicacao { get; set; }

    [Required, StringLength(40)]
    public string? Autores { get; set; }

    [Required, StringLength(40)]
    public string? Assuntos { get; set; }
}
