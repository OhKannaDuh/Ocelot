namespace Ocelot.Chain;

public record StepResult
{
    public bool IsSuccess { get; init; }

    public bool IsCanceled { get; init; }

    public bool ShouldBreak { get; init; } = false;

    public string? ErrorMessage { get; init; }

    public Exception? Exception { get; init; }


    public static StepResult Success()
    {
        return new StepResult
        {
            IsSuccess = true,
        };
    }

    public static StepResult Canceled() => new()
    {
        IsSuccess = false,
        IsCanceled = true,
        ShouldBreak = true,
        ErrorMessage = "Canceled",
    };

    public static StepResult Break()
    {
        return new StepResult
        {
            IsSuccess = true,
            ShouldBreak = true,
        };
    }

    public static StepResult Failure(string errorMessage)
    {
        return new StepResult
        {
            IsSuccess = false,
            ShouldBreak = true,
            ErrorMessage = errorMessage,
        };
    }

    public static StepResult Failure(Exception exception)
    {
        return new StepResult
        {
            IsSuccess = false,
            ShouldBreak = true,
            Exception = exception,
            ErrorMessage = exception.Message,
        };
    }
}
