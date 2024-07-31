using CadastroLivros.Core.Entities;
using CadastroLivros.Core.Models;

namespace CadastroLivros.Application.Interfaces;

public interface ILivroRepository
{
    Task<List<Livro>> Pesquisar();
    Task<Livro?> PesquisarPorId(int codL);
    Task<List<LivroComAutorEAssunto>> PesquisarPorAutor(int codAu);
    Task<List<LivroComAutorEAssunto>> PesquisarPorAssunto(int codAs);
    Task<int> Inserir(Livro livro);
    Task<int> Alterar(Livro livro);
    Task<int> Excluir(int codL);
    Task InserirAutorLivro(int codL, int codAu);
    Task InserirAssuntoLivro(int codL, int codAs);
    Task ExcluirAutorLivro(int codL, int codAu);
    Task ExcluirAssuntoLivro(int codL, int codAs);
    Task<List<LivroAutor>> PesquisarLivroAutorRelatorio();
}
