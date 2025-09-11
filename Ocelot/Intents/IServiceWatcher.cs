using Ocelot.Services;

namespace Ocelot.Intents;

[Intent]
public interface IServiceWatcher
{
    void OnServicesChanged(object? sender, ServiceChangedEventContext context);
}
