using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ocelot.States.Flow;
using Ocelot.States.Score;

namespace Ocelot.States;

public static class StateMachineDiExtensions
{
    private static void AddFlowStateHandlers<TState>(this IServiceCollection services)
        where TState : struct, Enum
    {
        var candidates = Registry.GetTypesAssignableFrom<IFlowStateHandler<TState>>();

        foreach (var type in candidates)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient(typeof(IFlowStateHandler<TState>), type));
        }
    }

    public static void AddFlowStateMachine<TState>(this IServiceCollection services, TState initial, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TState : struct, Enum
    {
        services.AddFlowStateHandlers<TState>();

        services.Add(new ServiceDescriptor(
            typeof(IStateMachine<TState>),
            c =>
            {
                var handlers = c.GetRequiredService<IEnumerable<IFlowStateHandler<TState>>>();
                return new FlowStateMachine<TState>(initial, handlers);
            },
            lifetime)
        );

        if (lifetime == ServiceLifetime.Transient)
        {
            services.AddTransient<Func<IStateMachine<TState>>>(sp => sp.GetRequiredService<IStateMachine<TState>>);
        }
    }


    private static void AddScoreStateHandlers<TState, TScore>(this IServiceCollection services)
        where TState : struct, Enum
        where TScore : IComparable
    {
        var candidates = Registry.GetTypesAssignableFrom<IScoreStateHandler<TState, TScore>>();

        foreach (var type in candidates)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient(typeof(IScoreStateHandler<TState, TScore>), type));
        }
    }

    public static void AddScoreStateMachine<TState, TScore>(
        this IServiceCollection services,
        TState initial,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TState : struct, Enum
        where TScore : IComparable
    {
        services.AddScoreStateHandlers<TState, TScore>();

        services.Add(new ServiceDescriptor(
            typeof(IStateMachine<TState>),
            c =>
            {
                var handlers = c.GetRequiredService<IEnumerable<IScoreStateHandler<TState, TScore>>>();
                return new ScoreStateMachine<TState, TScore>(initial, handlers);
            },
            lifetime)
        );

        if (lifetime == ServiceLifetime.Transient)
        {
            services.AddTransient<Func<IStateMachine<TState>>>(sp => sp.GetRequiredService<IStateMachine<TState>>);
        }
    }
}
