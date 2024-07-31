using System.Globalization;
using CadastroLivros.Application.Interfaces;
using CadastroLivros.Core;
using CadastroLivros.Core.Entities;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace CadastroLivros.Infrastructure.Repositories;

public class LivroValorRepository : ILivroValorRepository
{
    private readonly IOptionsMonitor<AppSettings> _configuration;

    public LivroValorRepository(IOptionsMonitor<AppSettings> configuration)
    {
        _configuration = configuration;
    }

    public async Task<List<LivroValor>> Pesquisar()
    {
        const string sql =
            """
            SELECT
              lv.CodL
              ,lv.IdFormaCompra
              ,CAST(lv.Valor AS DOUBLE) AS Valor
            FROM LivroValor lv
            ORDER BY lv.CodL, lv.IdFormaCompra
            """;

        await using var connection = new SqliteConnection(_configuration.CurrentValue.ConnectionStrings.DefaultConnection);
        var result = await connection.QueryAsync<LivroValor>(sql);
        return result.AsList();
    }

    public async Task<List<LivroValor>> PesquisarPorLivro(int codL)
    {
        const string sql =
            """
            SELECT
              lv.CodL
              ,lv.IdFormaCompra
              ,CAST(lv.Valor AS DOUBLE) AS Valor
            FROM LivroValor lv
            WHERE lv.CodL = @CodL
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@CodL", codL);

        await using var connection = new SqliteConnection(_configuration.CurrentValue.ConnectionStrings.DefaultConnection);
        var result = await connection.QueryAsync<LivroValor>(sql, parameters);
        return result.AsList();
    }

    public async Task<int> InserirOuAtualizar(LivroValor livroValor)
    {
        const string sql =
            """
            INSERT INTO LivroValor (
              CodL
              ,IdFormaCompra
              ,Valor
            )
            VALUES (
              @CodL
              ,@IdFormaCompra
              ,@Valor
            )
            ON CONFLICT (CodL, IdFormaCompra)
            DO UPDATE SET Valor = @Valor
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@CodL", livroValor.CodL);
        parameters.Add("@IdFormaCompra", (int)livroValor.IdFormaCompra);
        parameters.Add("@Valor", livroValor.Valor?.ToString(CultureInfo.InvariantCulture));

        await using var connection = new SqliteConnection(_configuration.CurrentValue.ConnectionStrings.DefaultConnection);
        return await connection.ExecuteAsync(sql, parameters);
    }

    public async Task<int> Excluir(LivroValor livroValor)
    {
        const string sql =
            """
            DELETE FROM LivroValor
            WHERE CodL = @CodL AND IdFormaCompra = @IdFormaCompra
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@CodL", livroValor.CodL);
        parameters.Add("@IdFormaCompra", livroValor.IdFormaCompra);

        await using var connection = new SqliteConnection(_configuration.CurrentValue.ConnectionStrings.DefaultConnection);
        return await connection.ExecuteAsync(sql, parameters);
    }
}
