using CadastroLivros.Core.Entities;
using CadastroLivros.Core.Models;

namespace CadastroLivros.Web.Models.Autores;

public class AutoresControllerViewModel
{
    public required Autor Autor { get; set; }

    public List<LivroComAutorEAssunto> Livros { get; set; } = [];
}
