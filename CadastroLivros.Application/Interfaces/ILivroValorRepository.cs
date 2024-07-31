using CadastroLivros.Core.Entities;

namespace CadastroLivros.Application.Interfaces;

public interface ILivroValorRepository
{
    Task<List<LivroValor>> Pesquisar();
    Task<List<LivroValor>> PesquisarPorLivro(int codL);
    Task<int> InserirOuAtualizar(LivroValor livroValor);
    Task<int> Excluir(LivroValor livroValor);
}
