using CadastroLivros.Application.Interfaces;
using CadastroLivros.Core;
using CadastroLivros.Core.Entities;
using CadastroLivros.Core.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace CadastroLivros.Infrastructure.Repositories;

public class LivroRepository : ILivroRepository
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
              l.CodL
              ,l.Titulo
              ,l.Editora
              ,l.Edicao
              ,l.AnoPublicacao
            FROM Livro l
            ORDER BY l.CodL DESC
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
              l.CodL
              ,l.Titulo
              ,l.Editora
              ,l.Edicao
              ,l.AnoPublicacao
            FROM Livro l
            WHERE l.CodL = @CodL
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@CodL", codL);

        await using var connection = new SqliteConnection(_configuration.CurrentValue.ConnectionStrings.DefaultConnection);
        var result = await connection.QueryFirstOrDefaultAsync<Livro>(sql, parameters);
        return result;
    }

    public async Task<List<LivroComAutorEAssunto>> PesquisarPorAutor(int codAu)
    {
        const string sql =
            """
            WITH AutoresPorLivro AS (
              SELECT
                l.CodL
                ,GROUP_CONCAT(au.Nome, ', ') AS Autores
              FROM Livro l
              INNER JOIN Livro_Autor lau ON lau.Livro_CodL = l.CodL
              INNER JOIN Autor au ON au.CodAu = lau.Autor_CodAu
              GROUP BY l.CodL
            )
            , AssuntosPorLivro AS (
            SELECT
              l.CodL
              ,GROUP_CONCAT(DISTINCT ass.Descricao) AS Assuntos
              FROM Livro l
              INNER JOIN Livro_Assunto las ON las.Livro_CodL = l.CodL
              INNER JOIN Assunto ass ON ass.CodAs = las.Assunto_CodAs
              GROUP BY l.CodL
            )
            SELECT
              l.CodL
              ,l.Titulo
              ,l.Editora
              ,l.Edicao
              ,l.AnoPublicacao
              ,aupl.Autores
              ,aspl.Assuntos
            FROM Livro l
            LEFT JOIN AutoresPorLivro aupl ON aupl.CodL = l.CodL
            LEFT JOIN AssuntosPorLivro aspl ON aspl.CodL = l.CodL
            WHERE EXISTS (
              SELECT 1
              FROM Livro_Autor lau
              WHERE lau.Livro_CodL = l.CodL
              AND lau.Autor_CodAu = @CodAu
            )
            ORDER BY l.CodL
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@CodAu", codAu);

        await using var connection = new SqliteConnection(_configuration.CurrentValue.ConnectionStrings.DefaultConnection);
        var result = await connection.QueryAsync<LivroComAutorEAssunto>(sql, parameters);
        return result.AsList();
    }

    public async Task<List<LivroComAutorEAssunto>> PesquisarPorAssunto(int codAs)
    {
        const string sql =
            """
            WITH AutoresPorLivro AS (
              SELECT
                l.CodL
                ,GROUP_CONCAT(au.Nome, ', ') AS Autores
              FROM Livro l
              INNER JOIN Livro_Autor lau ON lau.Livro_CodL = l.CodL
              INNER JOIN Autor au ON au.CodAu = lau.Autor_CodAu
              GROUP BY l.CodL
            )
            , AssuntosPorLivro AS (
            SELECT
              l.CodL
              ,GROUP_CONCAT(DISTINCT ass.Descricao) AS Assuntos
              FROM Livro l
              INNER JOIN Livro_Assunto las ON las.Livro_CodL = l.CodL
              INNER JOIN Assunto ass ON ass.CodAs = las.Assunto_CodAs
              GROUP BY l.CodL
            )
            SELECT
              l.CodL
              ,l.Titulo
              ,l.Editora
              ,l.Edicao
              ,l.AnoPublicacao
              ,aupl.Autores
              ,aspl.Assuntos
            FROM Livro l
            LEFT JOIN AutoresPorLivro aupl ON aupl.CodL = l.CodL
            LEFT JOIN AssuntosPorLivro aspl ON aspl.CodL = l.CodL
            WHERE EXISTS (
              SELECT 1
              FROM Livro_Assunto las
              WHERE las.Livro_CodL = l.CodL
                AND las.Assunto_CodAs = @CodAs
            )
            ORDER BY l.CodL
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@CodAs", codAs);

        await using var connection = new SqliteConnection(_configuration.CurrentValue.ConnectionStrings.DefaultConnection);
        var result = await connection.QueryAsync<LivroComAutorEAssunto>(sql, parameters);
        return result.AsList();
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

    public async Task<List<LivroAutor>> PesquisarLivroAutorRelatorio()
    {
        const string sql =
            """
            SELECT
              la.CodAu
              ,la.NomeAutor
              ,la.CodL
              ,la.TituloLivro
              ,la.Editora
              ,la.Edicao
              ,la.AnoPublicacao
              ,la.Assuntos
              ,la.Valores
            FROM uvwLivroAutor la
            ORDER BY la.CodAu, la.CodL
            """;

        await using var connection = new SqliteConnection(_configuration.CurrentValue.ConnectionStrings.DefaultConnection);
        var result = await connection.QueryAsync<LivroAutor>(sql);
        return result.AsList();
    }
}
