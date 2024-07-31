using CadastroLivros.Core.Entities;

namespace CadastroLivros.Web.Models.Home;

public class HomeControllerViewModel
{
    public List<Livro> Livros { get; set; } = [];
    public List<Autor> Autores { get; set; } = [];
    public List<Assunto> Assuntos { get; set; } = [];
}
