using System;

namespace Ocelot.Gameplay;

public interface IPlugin : IDisposable
{
    public string DisplayName { get; }

    public string InternalName { get; }

    public string Author { get; }

    public string[] Maintainers { get; }
}
