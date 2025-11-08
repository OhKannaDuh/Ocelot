using ECommons;

namespace Ocelot.ECommons.Services;

public class ECommonsInitProvider : IECommonsInitProvider
{
    public Module[] GetModules()
    {
        return [Module.All];
    }
}
