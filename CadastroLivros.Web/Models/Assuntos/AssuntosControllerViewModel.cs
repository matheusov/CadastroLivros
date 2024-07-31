using CadastroLivros.Core.Entities;
using CadastroLivros.Core.Models;

namespace CadastroLivros.Web.Models.Assuntos;

public class AssuntosControllerViewModel
{
    public required Assunto Assunto { get; set; }

    public List<LivroComAutorEAssunto> Livros { get; set; } = [];
}
