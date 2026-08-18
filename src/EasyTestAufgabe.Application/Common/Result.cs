namespace EasyTestAufgabe.Application.Common;

/// <summary>
/// Ergebnis einer Operation ohne Rückgabewert (z.B. Update, Delete).
/// Vermeidet das Werfen von Exceptions für erwartbare Fehlerfälle
/// wie Validierungsfehler oder "nicht gefunden".
/// </summary>
public class Result
{
    public bool IsSuccess { get; }

    public string? Error { get; }

    protected Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);

    public static Result Failure(string error) => new(false, error);
}

/// <summary>
/// Ergebnis einer Operation mit Rückgabewert (z.B. GetById, Create).
/// </summary>
public class Result<T> : Result
{
    public T? Value { get; }

    protected Result(bool isSuccess, T? value, string? error) : base(isSuccess, error)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(true, value, null);

    public static new Result<T> Failure(string error) => new(false, default, error);
}