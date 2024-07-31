using System.ComponentModel.DataAnnotations;

namespace CadastroLivros.Core.Enums;

public enum FormaCompra
{
    [Display(Name = "Balcão")]
    Balcao = 1,

    [Display(Name = "Self-service")]
    SelfService = 2,

    [Display(Name = "Internet")]
    Internet = 3,

    [Display(Name = "Evento")]
    Evento = 4
}
