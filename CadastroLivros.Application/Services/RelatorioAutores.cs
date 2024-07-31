using CadastroLivros.Core.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CadastroLivros.Application.Services;

public class RelatorioAutores
{
    private readonly DadosRelatorio _model;

    public RelatorioAutores(DadosRelatorio model)
    {
        _model = model;
    }

    public Document GerarRelatorio()
    {
        return Document.Create(container =>
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
                        columnAutor.Item().Column(columnLivro =>
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
