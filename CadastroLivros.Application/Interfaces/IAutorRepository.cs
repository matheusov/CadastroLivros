using CadastroLivros.Core.Entities;

namespace CadastroLivros.Application.Interfaces;

public interface IAutorRepository
{
    Task<List<Autor>> Pesquisar();
    Task<List<Autor>> PesquisarPorLivro(int codL);
    Task<Autor?> PesquisarPorId(int id);
    Task<Autor?> PesquisarPorNome(string nome);
    Task<int> Inserir(Autor autor);
    Task<int> Alterar(Autor autor);
}
