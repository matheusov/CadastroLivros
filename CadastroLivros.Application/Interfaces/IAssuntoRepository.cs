using CadastroLivros.Core.Entities;

namespace CadastroLivros.Application.Interfaces;

public interface IAssuntoRepository
{
    Task<List<Assunto>> Pesquisar();
    Task<List<Assunto>> PesquisarPorLivro(int codL);
    Task<Assunto?> PesquisarPorId(int id);
    Task<Assunto?> PesquisarPorDescricao(string descricao);
    Task<int> Inserir(Assunto autor);
    Task<int> Alterar(Assunto autor);
}
