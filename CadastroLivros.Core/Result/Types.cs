namespace CadastroLivros.Core.Result;

public readonly record struct Success;
public readonly record struct Created;
public readonly record struct Updated;
public readonly record struct Deleted;

public static class Result
{
    public static Success Success => default;
    public static Created Created => default;
    public static Updated Updated => default;
    public static Deleted Deleted => default;
}

public enum ErrorType
{
    Failure,
    Unexpected,
    Validation,
    Conflict,
    NotFound,
    Unauthorized,
    Forbidden
}
