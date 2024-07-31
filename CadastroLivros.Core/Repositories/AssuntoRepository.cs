using CadastroLivros.Core.Entities;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace CadastroLivros.Core.Repositories;

public class AssuntoRepository
{
    private readonly IOptionsMonitor<AppSettings> _configuration;

    public AssuntoRepository(IOptionsMonitor<AppSettings> configuration)
    {
        _configuration = configuration;
    }

    public async Task<List<Assunto>> Pesquisar()
    {
        const string sql =
            """
            SELECT CodAs, Descricao
            FROM Assunto
            ORDER BY Descricao
            """;

        await using var connection = new SqliteConnection(_configuration.CurrentValue.ConnectionStrings.DefaultConnection);
        var result = await connection.QueryAsync<Assunto>(sql);
        return result.AsList();
    }

    public async Task<List<Assunto>> PesquisarPorLivro(int codL)
    {
        const string sql =
            """
            SELECT
              a.CodAs
              ,a.Descricao
            FROM Assunto a
            INNER JOIN Livro_Assunto la ON la.Assunto_CodAs = a.CodAs
            WHERE la.Livro_CodL = @CodL
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@CodL", codL);

        await using var connection = new SqliteConnection(_configuration.CurrentValue.ConnectionStrings.DefaultConnection);
        var result = await connection.QueryAsync<Assunto>(sql, parameters);
        return result.AsList();
    }

    public async Task<Assunto?> PesquisarPorId(int id)
    {
        const string sql =
            """
            SELECT
              a.CodAs
              ,a.Descricao
            FROM Assunto a
            WHERE a.CodAs = @CodAs
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@CodAs", id);

        await using var connection = new SqliteConnection(_configuration.CurrentValue.ConnectionStrings.DefaultConnection);
        var result = await connection.QueryFirstOrDefaultAsync<Assunto>(sql, parameters);
        return result;
    }

    public async Task<Assunto?> PesquisarPorDescricao(string descricao)
    {
        const string sql =
            """
            SELECT
              a.CodAs
              ,a.Descricao
            FROM Assunto a
            WHERE a.Descricao LIKE @Descricao
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@Descricao", descricao);

        await using var connection = new SqliteConnection(_configuration.CurrentValue.ConnectionStrings.DefaultConnection);
        var result = await connection.QueryFirstOrDefaultAsync<Assunto>(sql, parameters);
        return result;
    }

    public async Task<int> Inserir(Assunto autor)
    {
        const string sql =
            """
            INSERT INTO Assunto (Descricao)
            VALUES (@Descricao);

            SELECT LAST_INSERT_ROWID();
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@Descricao", autor.Descricao);

        await using var connection = new SqliteConnection(_configuration.CurrentValue.ConnectionStrings.DefaultConnection);
        return await connection.ExecuteScalarAsync<int>(sql, parameters);
    }

    public async Task<int> Alterar(Assunto autor)
    {
        const string sql =
            """
            UPDATE Assunto
            SET Descricao = @Descricao
            WHERE CodAs = @CodAs
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@CodAs", autor.CodAs);
        parameters.Add("@Descricao", autor.Descricao);

        await using var connection = new SqliteConnection(_configuration.CurrentValue.ConnectionStrings.DefaultConnection);
        return await connection.ExecuteAsync(sql, parameters);
    }
}
