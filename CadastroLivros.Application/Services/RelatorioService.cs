using CadastroLivros.Application.Interfaces;
using CadastroLivros.Core.Models;
using CadastroLivros.Core.Result;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CadastroLivros.Application.Services;

public class RelatorioService
{
    private readonly ILogger<RelatorioService> _logger;
    private readonly ILivroRepository _livroRepository;
    private readonly DadosRelatorio _model;

    public RelatorioService(ILogger<RelatorioService> logger, ILivroRepository livroRepository)
    {
        _logger = logger;
        _livroRepository = livroRepository;
        _model = new DadosRelatorio();
    }

    public async Task<Result<StreamResponse>> GerarRelatorio()
    {
        var model = await _livroRepository.PesquisarLivroAutorRelatorio();

        // if (model.Count == 0)
        // {
        //     return Error.NotFound("GerarRelatorio.NotFound", "Nenhum dado encontrado");
        // }

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

        _model.DadosAutor = dadosAutores;

        try
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.MarginHorizontal(30);
                    page.MarginVertical(15);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().PaddingBottom(10).Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().Element(ComposeFooter);
                });
            });

            var stream = new MemoryStream();
            document.GeneratePdf(stream);
            stream.Position = 0;

            return new StreamResponse("relatorio.pdf", "application/pdf", stream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar relatório");
            return Error.Failure("GerarRelatorio.Failure", "Erro ao gerar relatório");
        }
    }

    private void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Text(text =>
            {
                text.Span("Livros agrupados por autor");
                text.DefaultTextStyle(Typography.TitleStyle);
            });
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Text(text =>
            {
                text.Span("Página ");
                text.CurrentPageNumber();
                text.Span("/");
                text.TotalPages();
            });

            row.RelativeItem().Text(text => { text.Span(DateTime.Now.ToString("dd/MM/yyyy")); });
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.Column(columnAutores =>
        {
            if (_model.DadosAutor.Count == 0)
            {
                columnAutores.Item().Text("Nenhum autor encontrado");
                return;
            }

            foreach (var autor in _model.DadosAutor)
            {
                columnAutores.Item().Column(columnAutor =>
                {
                    columnAutor.Item().PaddingBottom(10).Text(text =>
                    {
                        text.Span(autor.NomeAutor);
                        text.DefaultTextStyle(Typography.AutorHeader);
                    });

                    if (autor.Livros.Count == 0)
                    {
                        columnAutores.Item().Text("Nenhum livro encontrado");
                        return;
                    }

                    foreach (var livro in autor.Livros)
                    {
                        columnAutor.Item().ShowEntire().Column(columnLivro =>
                        {
                            columnLivro.Item().Text(text =>
                            {
                                text.Span("Código: ").Bold();
                                text.Span(livro.CodL.ToString());
                            });

                            columnLivro.Item().Text(text =>
                            {
                                text.Span("Título: ").Bold();
                                text.Span(livro.TituloLivro);
                            });

                            columnLivro.Item().Text(text =>
                            {
                                text.Span("Editora: ").Bold();
                                text.Span(livro.Editora);
                            });

                            columnLivro.Item().Text(text =>
                            {
                                text.Span("Edição: ").Bold();
                                text.Span(livro.Edicao.ToString());
                            });

                            columnLivro.Item().Text(text =>
                            {
                                text.Span("Ano da publicação: ").Bold();
                                text.Span(livro.AnoPublicacao);
                            });

                            columnLivro.Item().Text(text =>
                            {
                                text.Span("Assuntos: ").Bold();
                                text.Span(livro.Assuntos);
                            });

                            columnLivro.Item().Text(text =>
                            {
                                text.Span("Valores: ").Bold();
                                text.Span(livro.Valores);
                            });

                            columnLivro.Item().PaddingVertical(4).LineHorizontal(.5f);
                        });
                    }
                });
            }
        });
    }

    private static class Typography
    {
        public static TextStyle TitleStyle => TextStyle
            .Default
            .FontSize(18).Bold();

        public static TextStyle AutorHeader => TextStyle
            .Default
            .FontSize(15)
            .Bold();
    }
}
