using CadastroLivros.Core.Models;
using CadastroLivros.Core.Repositories;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;

namespace CadastroLivros.Core.Services;

public class RelatorioService
{
    private readonly ILogger<RelatorioService> _logger;
    private readonly LivroRepository _livroRepository;

    public RelatorioService(ILogger<RelatorioService> logger, LivroRepository livroRepository)
    {
        _logger = logger;
        _livroRepository = livroRepository;
    }

    public async Task<Stream> GerarRelatorio()
    {
        var model = await _livroRepository.PesquisarLivroAutorRelatorio();

        foreach (var item in model)
        {
            if (item.Valores is null)
                continue;

            string[] paresValores = item.Valores.Split(',');
            var valoresFormatados = paresValores.Select(x =>
            {
                string[] partes = x.Split(':');
                string modo = partes[0];
                string valor = double.Parse(partes[1].Replace('.', ',')).ToString("C");
                return $"{modo}: {valor}";
            });

            item.Valores = string.Join(", ", valoresFormatados);
        }

        var dadosAutores = model.GroupBy(x => x.CodAu).Select(x => new DadosAutor
        {
            CodAu = x.Key,
            NomeAutor = x.First().NomeAutor,
            Livros = x.Where(y => y.CodL is not null).Select(y => new DadosLivro
            {
                CodL = y.CodL!.Value,
                TituloLivro = y.TituloLivro!,
                Editora = y.Editora!,
                Edicao = y.Edicao!.Value,
                AnoPublicacao = y.AnoPublicacao!,
                Assuntos = y.Assuntos,
                Valores = y.Valores
            }).ToList()
        }).ToList();

        var dadosRelatorio = new DadosRelatorio { DadosAutor = dadosAutores };

        var relatorio = new RelatorioAutores(dadosRelatorio);
        var document = relatorio.GerarRelatorio();

        var stream = new MemoryStream();
        document.GeneratePdf(stream);
        stream.Position = 0;

        return stream;
    }
}
