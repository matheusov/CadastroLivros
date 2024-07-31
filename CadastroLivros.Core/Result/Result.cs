using System.Diagnostics.CodeAnalysis;

namespace CadastroLivros.Core.Result;

public readonly record struct Result<TValue>
{
    private readonly TValue? _value = default;
    private readonly List<Error>? _errors = null;

    public bool IsError { get; }

    public bool IsSuccess => !IsError;

    public List<Error> Errors => IsError ? _errors! : new List<Error> { Error.NoErrors };

    public List<Error> ErrorsOrEmptyList => IsError ? _errors! : new List<Error>();

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Não é possível acessar o valor de um erro.");

    public TValue? ValueOrDefault => _value;

    private Result(Error error)
    {
        _errors = new List<Error> { error };
        IsError = true;
    }

    private Result(List<Error> errors)
    {
        _errors = errors;
        IsError = true;
    }

    private Result(TValue value)
    {
        _value = value;
        IsError = false;
    }

    public static implicit operator Result<TValue>(TValue value)
    {
        return new Result<TValue>(value);
    }

    public static implicit operator Result<TValue>(Error error)
    {
        return new Result<TValue>(error);
    }

    public static implicit operator Result<TValue>(List<Error> errors)
    {
        return new Result<TValue>(errors);
    }

    public static implicit operator Result<TValue>(Error[] errors)
    {
        return new Result<TValue>(errors.ToList());
    }

    public bool TryGetValue([NotNullWhen(true)] out TValue? value, out List<Error> errors)
    {
        value = ValueOrDefault;
        errors = ErrorsOrEmptyList;
        return IsSuccess;
    }

    public bool TryGetValue([NotNullWhen(true)] out TValue? value)
    {
        value = ValueOrDefault;
        return IsSuccess;
    }
}
