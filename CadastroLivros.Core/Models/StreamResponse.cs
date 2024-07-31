namespace CadastroLivros.Core.Models;

public record StreamResponse
(
    string NomeArquivo,
    string ContentType,
    Stream Stream
);
