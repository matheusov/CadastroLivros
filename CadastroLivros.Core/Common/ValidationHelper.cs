using System.ComponentModel.DataAnnotations;
using CadastroLivros.Core.Result;

namespace CadastroLivros.Core.Common;

public static class ValidationHelper
{
    public static bool IsInvalid(IValidatableObject dto, out List<Error> validationErrors)
    {
        var context = new ValidationContext(dto, null, null);
        var results = new List<ValidationResult>();

        // utilizar Validate porque TryValidateObject não considera IValidatableObject
        var errors = dto.Validate(context).ToList();

        if (Validator.TryValidateObject(dto, context, results, true) && errors.Count == 0)
        {
            validationErrors = [];
            return false;
        }

        results.AddRange(errors.Where(error => results.All(validationFailure =>
            validationFailure.ErrorMessage != error.ErrorMessage
            || validationFailure.MemberNames.First() != error.MemberNames.First()
        )));

        validationErrors = results.ConvertAll(validationFailure =>
            Error.Validation(validationFailure.MemberNames.First(),
                validationFailure.ErrorMessage ?? "Valor inválido"));

        return true;
    }
}
