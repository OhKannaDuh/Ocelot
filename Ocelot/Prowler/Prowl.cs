using System.Numerics;
using ECommons.GameHelpers;

namespace Ocelot.Prowler;

public class Prowl(ProwlOptions options, Vector3 start, Vector3 destination)
{
    public readonly ProwlContext Context = new(start, destination, options);

    public ProwlOptions Options {
        get => Context.Options;
    }

    public Prowl(ProwlOptions options, Vector3 destination) : this(options, Player.Position, destination) { }
}
