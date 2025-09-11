namespace Ocelot.Intents;

[Intent]
public interface IToggleable
{
    bool IsEnabled { get; }

    void Toggle();

    void On();

    void Off();

    // Action OnEnabled { get; }

    // Action OnDisabled { get; }
}
