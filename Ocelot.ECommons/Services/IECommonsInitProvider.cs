using ECommons;

namespace Ocelot.ECommons.Services;

public interface IECommonsInitProvider
{
    Module[] GetModules();
}
