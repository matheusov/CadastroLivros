using CadastroLivros.Core.Enums;

namespace CadastroLivros.Core.Entities;

public class LivroValor
{
    public int CodL { get; set; }
    public FormaCompra IdFormaCompra { get; set; }
    public decimal? Valor { get; set; }
}
