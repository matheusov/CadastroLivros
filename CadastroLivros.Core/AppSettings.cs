using System.ComponentModel.DataAnnotations;

namespace CadastroLivros.Core;

public class AppSettings
{
    [Required]
    public required ConnectionStringsSettings ConnectionStrings { get; init; }

    public class ConnectionStringsSettings
    {
        [Required]
        public required string DefaultConnection { get; init; }
    }
}
