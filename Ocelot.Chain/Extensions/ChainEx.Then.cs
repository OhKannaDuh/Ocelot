using Ocelot.Chain.Steps;

namespace Ocelot.Chain.Extensions;

public static class ChainExThen
{
    public static IChain Then(this IChain chain, Func<IChainContext, ValueTask<StepResult>> action, string name)
    {
        return chain.Then(new ActionStep(action, name));
    }

    public static IChain Then(this IChain chain, Func<IChainContext, CancellationToken, ValueTask<StepResult>> action, string name)
    {
        return chain.Then(new ActionStep(ctx => action(ctx, ctx.CancellationToken), name));
    }

    public static IChain Then(this IChain chain, Func<IChainContext, Task<StepResult>> action, string name)
    {
        return chain.Then(new ActionStep(ctx => new ValueTask<StepResult>(action(ctx)), name));
    }

    public static IChain Then(this IChain chain, Func<IChainContext, StepResult> action, string name)
    {
        return chain.Then(new ActionStep(ctx => new ValueTask<StepResult>(action(ctx)), name));
    }

    public static IChain Then(this IChain chain, Func<ValueTask<StepResult>> action, string name)
    {
        return chain.Then(new ActionStep(_ => action(), name));
    }

    public static IChain Then(this IChain chain, Func<Task<StepResult>> action, string name)
    {
        return chain.Then(new ActionStep(_ => new ValueTask<StepResult>(action()), name));
    }

    public static IChain Then(this IChain chain, Func<StepResult> action, string name)
    {
        return chain.Then(new ActionStep(_ => new ValueTask<StepResult>(action()), name));
    }

    public static IChain Then(this IChain chain, Action<IChainContext> action, string name)
    {
        return chain.Then(new ActionStep(ctx =>
        {
            action(ctx);
            return new ValueTask<StepResult>(StepResult.Success());
        }, name));
    }

    public static IChain Then(this IChain chain, Action action, string name)
    {
        return chain.Then(new ActionStep(_ =>
        {
            action();
            return new ValueTask<StepResult>(StepResult.Success());
        }, name));
    }

    public static IChain Then(this IChain chain, Func<IChainContext, CancellationToken, Task> action, string name)
    {
        return chain.Then(new ActionStep(async ctx =>
        {
            await action(ctx, ctx.CancellationToken);
            return StepResult.Success();
        }, name));
    }

    public static IChain Then(this IChain chain, Func<Task> action, string name)
    {
        return chain.Then(new ActionStep(async _ =>
        {
            await action();
            return StepResult.Success();
        }, name));
    }

    public static IChain Then(this IChain chain, IChain inner)
    {
        return chain.Then(new ChainStep(inner));
    }
}
