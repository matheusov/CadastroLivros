using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CadastroLivros.Core.Enums;

namespace CadastroLivros.Application.Services.Livros;

public class InserirLivroDto : IValidatableObject
{
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

    [Required(ErrorMessage = "Informe o(s) autor(es)")]
    public string? Autores { get; set; }

    [Required(ErrorMessage = "Informe o(s) assunto(s)")]
    public string? Assuntos { get; set; }

    public List<InserirLivroFormaCompraDto> FormasCompra { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        string[] autores = Autores!.Split(",");
        var autoresDistintos = autores.Select(a => a.Trim()).Where(a => !string.IsNullOrWhiteSpace(a)).Distinct().ToList();

        if (autoresDistintos.Count == 0)
        {
            yield return new ValidationResult("Informe o(s) autor(es)", new[] { nameof(Autores) });
        }

        if (autoresDistintos.Any(a => a.Length > 40))
        {
            yield return new ValidationResult("O nome do(s) autor(es) deve(m) ter no máximo 40 caracteres", new[] { nameof(Autores) });
        }

        string[] assuntos = Assuntos!.Split(",");
        var assuntosDistintos = assuntos.Select(a => a.Trim()).Where(a => !string.IsNullOrWhiteSpace(a)).Distinct().ToList();

        if (assuntosDistintos.Count == 0)
        {
            yield return new ValidationResult("Informe o(s) assunto(s)", new[] { nameof(Assuntos) });
        }

        if (assuntosDistintos.Any(a => a.Length > 40))
        {
            yield return new ValidationResult("A descrição do(s) assunto(s) deve(m) ter no máximo 40 caracteres", new[] { nameof(Assuntos) });
        }
    }
}

public class InserirLivroFormaCompraDto
{
    public FormaCompra FormaCompra { get; set; }

    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    [Range(0.00, 99999999.00, ErrorMessage = "Informe um valor válido")]
    public decimal? Valor { get; set; }
}
