using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CadastroLivros.Core.Enums;

namespace CadastroLivros.Web.Models.Livros;

public class LivrosControllerViewModel
{
    [Range(1, int.MaxValue)]
    public int? CodL { get; set; }

    [Required(ErrorMessage = "Informe o título do livro")]
    [StringLength(40, ErrorMessage = "O título do livro deve ter no máximo 40 caracteres")]
    [DisplayName("Título")]
    public string? Titulo { get; set; }

    [Required(ErrorMessage = "Informe a editora")]
    [StringLength(40, ErrorMessage = "A editora deve ter no máximo 40 caracteres")]
    public string? Editora { get; set; }

    [Required(ErrorMessage = "Informe a edição")]
    [Range(1, int.MaxValue, ErrorMessage = "Informe uma edição válida")]
    [DisplayName("Edição")]
    public int? Edicao { get; set; }

    [DisplayName("Ano de publicação")]
    [Required(ErrorMessage = "Informe o ano de publicação")]
    [Range(0000, 9999, ErrorMessage = "Informe um ano válido")]
    public string? AnoPublicacao { get; set; }

    [Required(ErrorMessage = "Informe o(s) autore(s)")]
    [StringLength(40)]
    public string? Autores { get; set; }

    [Required(ErrorMessage = "Informe o(s) assunto(s)")]
    [StringLength(40)]
    public string? Assuntos { get; set; }

    public List<FormaCompraViewModel> FormasCompra { get; set; } = [];
}

public class FormaCompraViewModel
{
    public FormaCompra FormaCompra { get; set; }

    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    [Range(0.00, 99999999.00, ErrorMessage = "Informe um valor válido")]
    public decimal? Valor { get; set; }
}
