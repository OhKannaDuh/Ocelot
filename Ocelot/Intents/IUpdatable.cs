using Ocelot.Data;
using Ocelot.Modules;

namespace Ocelot.Intents;

[Intent]
public interface IUpdatable
{
    UpdateLimit UpdateLimit { get; }

    void PreUpdate(UpdateContext ctx);

    void Update(UpdateContext ctx);

    void PostUpdate(UpdateContext ctx);
}
