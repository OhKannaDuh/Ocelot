using Dalamud.Plugin;
using WrathCombo.API;

namespace Ocelot.Rotation.Services.Wrath;

/// <summary>Lists Wrath's phantom-job option names without taking a lease (for config UIs).</summary>
public interface IWrathOccultOptionCatalog
{
    /// <summary>Option names for a phantom job, or null when Wrath is missing / too old.</summary>
    IReadOnlyList<string>? GetOptionNames(uint phantomJobId);

    /// <summary>Options BOCCHI always leaves off, whatever the user picks.</summary>
    IReadOnlySet<string> BuiltInOptionsLeftOff { get; }
}

public sealed class WrathOccultOptionCatalog(IDalamudPluginInterface pluginInterface) : IWrathOccultOptionCatalog
{
    public IReadOnlySet<string> BuiltInOptionsLeftOff => WrathJobRotation.BuiltInOccultOptionsLeftOff;

    public IReadOnlyList<string>? GetOptionNames(uint phantomJobId)
    {
        try
        {
            // Init only stores the interface; safe to repeat alongside WrathJobRotation.
            WrathIPCWrapper.Init(pluginInterface, WrathIPCWrapper.ErrorType.All);
            return WrathIPCWrapper.GetOccultOptionNames(phantomJobId);
        }
        catch
        {
            return null;
        }
    }
}
