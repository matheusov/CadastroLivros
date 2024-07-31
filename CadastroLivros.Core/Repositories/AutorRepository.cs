using CadastroLivros.Core.Entities;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace CadastroLivros.Core.Repositories;

public class AutorRepository
{
    private readonly IOptionsMonitor<AppSettings> _configuration;

    public AutorRepository(IOptionsMonitor<AppSettings> configuration)
    {
        _configuration = configuration;
    }

    public async Task<List<Autor>> Pesquisar()
    {
        const string sql =
            """
            SELECT a.CodAu, a.Nome
            FROM Autor a
            ORDER BY a.Nome
            """;

        await using var connection = new SqliteConnection(_configuration.CurrentValue.ConnectionStrings.DefaultConnection);
        var result = await connection.QueryAsync<Autor>(sql);
        return result.AsList();
    }

    public async Task<List<Autor>> PesquisarPorLivro(int codL)
    {
        const string sql =
            """
            SELECT
              a.CodAu
              ,a.Nome
            FROM Autor a
            INNER JOIN Livro_Autor la ON la.Autor_CodAu = a.CodAu
            WHERE la.Livro_CodL = @CodL
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@CodL", codL);

        await using var connection = new SqliteConnection(_configuration.CurrentValue.ConnectionStrings.DefaultConnection);
        var result = await connection.QueryAsync<Autor>(sql, parameters);
        return result.AsList();
    }

    public async Task<Autor?> PesquisarPorId(int id)
    {
        const string sql =
            """
            SELECT
              a.CodAu
              ,a.Nome
            FROM Autor a
            WHERE a.CodAu = @CodAu
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@CodAu", id);

        await using var connection = new SqliteConnection(_configuration.CurrentValue.ConnectionStrings.DefaultConnection);
        var result = await connection.QueryFirstOrDefaultAsync<Autor>(sql, parameters);
        return result;
    }

    public async Task<Autor?> PesquisarPorNome(string nome)
    {
        const string sql =
            """
            SELECT
              a.CodAu
              ,a.Nome
            FROM Autor a
            WHERE a.Nome LIKE @Nome
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@Nome", nome);

        await using var connection = new SqliteConnection(_configuration.CurrentValue.ConnectionStrings.DefaultConnection);
        var result = await connection.QueryFirstOrDefaultAsync<Autor>(sql, parameters);
        return result;
    }

    public async Task<int> Inserir(Autor autor)
    {
        const string sql =
            """
            INSERT INTO Autor (Nome)
            VALUES (@Nome);

            SELECT LAST_INSERT_ROWID();
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@Nome", autor.Nome);

        await using var connection = new SqliteConnection(_configuration.CurrentValue.ConnectionStrings.DefaultConnection);
        return await connection.ExecuteScalarAsync<int>(sql, parameters);
    }

    public async Task<int> Alterar(Autor autor)
    {
        const string sql =
            """
            UPDATE Autor
            SET Nome = @Nome
            WHERE CodAu = @CodAu
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@CodAu", autor.CodAu);
        parameters.Add("@Nome", autor.Nome);

        await using var connection = new SqliteConnection(_configuration.CurrentValue.ConnectionStrings.DefaultConnection);
        return await connection.ExecuteAsync(sql, parameters);
    }
}
