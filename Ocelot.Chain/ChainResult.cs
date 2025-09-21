namespace Ocelot.Chain;

public record ChainResult
{
    public ChainState State { get; init; }

    public bool IsSuccess
    {
        get => State == ChainState.Ok;
    }

    public bool IsError
    {
        get => State == ChainState.Error;
    }

    public bool IsCanceled
    {
        get => State == ChainState.Canceled;
    }

    public string? ErrorMessage { get; init; }

    public Exception? Exception { get; init; }

    public static ChainResult Success()
    {
        return new ChainResult
        {
            State = ChainState.Ok,
        };
    }

    public static ChainResult Failure(string errorMessage)
    {
        return new ChainResult
        {
            State = ChainState.Error,
            ErrorMessage = errorMessage,
        };
    }

    public static ChainResult Failure(Exception exception)
    {
        return new ChainResult
        {
            State = ChainState.Error,
            Exception = exception,
            ErrorMessage = exception.Message,
        };
    }

    public static ChainResult Failure(StepResult stepResult)
    {
        return new ChainResult
        {
            State = ChainState.Error,
            Exception = stepResult.Exception,
            ErrorMessage = stepResult.ErrorMessage,
        };
    }

    public static ChainResult Canceled()
    {
        return new ChainResult
        {
            State = ChainState.Canceled,
            ErrorMessage = "Chain execution was canceled",
        };
    }
}
