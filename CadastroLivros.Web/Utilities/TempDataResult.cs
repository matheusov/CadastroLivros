using Microsoft.AspNetCore.Mvc;

namespace CadastroLivros.Web.Utilities;

public static class TempDataResult
{
    public static void SetResult(this Controller controller, bool success, string message)
    {
        if (success)
            SetSuccessResult(controller, message);
        else
            SetErrorResult(controller, message);
    }

    public static void SetSuccessResult(this Controller controller, string message, bool exibirAlerta = true)
    {
        controller.TempData["sucesso"] = true;
        controller.TempData["mensagem"] = message;
        controller.TempData["exibirAlerta"] = exibirAlerta;
    }

    public static void SetErrorResult(this Controller controller, string message, bool exibirAlerta = true)
    {
        controller.TempData["sucesso"] = false;
        controller.TempData["mensagem"] = message;
        controller.TempData["exibirAlerta"] = exibirAlerta;
    }
}
