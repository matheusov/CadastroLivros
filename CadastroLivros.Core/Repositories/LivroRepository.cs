using CadastroLivros.Core.Entities;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace CadastroLivros.Core.Repositories;

public class LivroRepository
{
    private readonly IOptionsMonitor<AppSettings> _configuration;

    public LivroRepository(IOptionsMonitor<AppSettings> configuration)
    {
        _configuration = configuration;
    }

    public async Task<List<Livro>> Pesquisar()
    {
        const string sql =
            """
            SELECT
              CodL
              ,Titulo
              ,Editora
              ,Edicao
              ,AnoPublicacao
            FROM Livro
            ORDER BY CodL DESC
            """;

        await using var connection = new SqliteConnection(_configuration.CurrentValue.ConnectionStrings.DefaultConnection);
        var result = await connection.QueryAsync<Livro>(sql);
        return result.AsList();
    }

    public async Task<Livro?> PesquisarPorId(int codL)
    {
        const string sql =
            """
            SELECT
              CodL
              ,Titulo
              ,Editora
              ,Edicao
              ,AnoPublicacao
            FROM Livro
            WHERE CodL = @CodL
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@CodL", codL);

        await using var connection = new SqliteConnection(_configuration.CurrentValue.ConnectionStrings.DefaultConnection);
        var result = await connection.QueryFirstOrDefaultAsync<Livro>(sql, parameters);
        return result;
    }

    public async Task<int> Inserir(Livro livro)
    {
        const string sql =
            """
            INSERT INTO Livro (
              Titulo
              ,Editora
              ,Edicao
              ,AnoPublicacao
            )
            VALUES (
              @Titulo
              ,@Editora
              ,@Edicao
              ,@AnoPublicacao
            );

            SELECT LAST_INSERT_ROWID();
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@Titulo", livro.Titulo);
        parameters.Add("@Editora", livro.Editora);
        parameters.Add("@Edicao", livro.Edicao);
        parameters.Add("@AnoPublicacao", livro.AnoPublicacao);

        await using var connection = new SqliteConnection(_configuration.CurrentValue.ConnectionStrings.DefaultConnection);
        return await connection.ExecuteScalarAsync<int>(sql, parameters);
    }

    public async Task<int> Alterar(Livro livro)
    {
        const string sql =
            """
            UPDATE Livro
            SET
              Titulo = @Titulo
              ,Editora = @Editora
              ,Edicao = @Edicao
              ,AnoPublicacao = @AnoPublicacao
            WHERE CodL = @CodL
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@CodL", livro.CodL);
        parameters.Add("@Titulo", livro.Titulo);
        parameters.Add("@Editora", livro.Editora);
        parameters.Add("@Edicao", livro.Edicao);
        parameters.Add("@AnoPublicacao", livro.AnoPublicacao);

        await using var connection = new SqliteConnection(_configuration.CurrentValue.ConnectionStrings.DefaultConnection);
        return await connection.ExecuteAsync(sql, parameters);
    }

    public async Task<int> Excluir(int codL)
    {
        const string sql =
            """
            DELETE FROM Livro
            WHERE CodL = @CodL
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@CodL", codL);

        await using var connection = new SqliteConnection(_configuration.CurrentValue.ConnectionStrings.DefaultConnection);
        return await connection.ExecuteAsync(sql, parameters);
    }

    public async Task InserirAutorLivro(int codL, int codAu)
    {
        const string sql =
            """
            INSERT INTO Livro_Autor (Livro_CodL, Autor_CodAu)
            VALUES (@CodL, @CodAu)
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@CodL", codL);
        parameters.Add("@CodAu", codAu);

        await using var connection = new SqliteConnection(_configuration.CurrentValue.ConnectionStrings.DefaultConnection);
        await connection.ExecuteAsync(sql, parameters);
    }

    public async Task InserirAssuntoLivro(int codL, int codAs)
    {
        const string sql =
            """
            INSERT INTO Livro_Assunto (Livro_CodL, Assunto_CodAs)
            VALUES (@CodL, @CodAs)
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@CodL", codL);
        parameters.Add("@CodAs", codAs);

        await using var connection = new SqliteConnection(_configuration.CurrentValue.ConnectionStrings.DefaultConnection);
        await connection.ExecuteAsync(sql, parameters);
    }

    public async Task ExcluirAutorLivro(int codL, int codAu)
    {
        const string sql =
            """
            DELETE FROM Livro_Autor
            WHERE Livro_CodL = @CodL
              AND Autor_CodAu = @CodAu
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@CodL", codL);
        parameters.Add("@CodAu", codAu);

        await using var connection = new SqliteConnection(_configuration.CurrentValue.ConnectionStrings.DefaultConnection);
        await connection.ExecuteAsync(sql, parameters);
    }

    public async Task ExcluirAssuntoLivro(int codL, int codAs)
    {
        const string sql =
            """
            DELETE FROM Livro_Assunto
            WHERE Livro_CodL = @CodL
              AND Assunto_CodAs = @CodAs
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@CodL", codL);
        parameters.Add("@CodAs", codAs);

        await using var connection = new SqliteConnection(_configuration.CurrentValue.ConnectionStrings.DefaultConnection);
        await connection.ExecuteAsync(sql, parameters);
    }
}
