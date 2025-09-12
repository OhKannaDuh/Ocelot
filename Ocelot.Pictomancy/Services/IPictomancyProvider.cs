using Pictomancy;

namespace Ocelot.Pictomancy.Services;

public interface IPictomancyProvider
{
    PctDrawList GetDrawList();

    bool HasDrawList { get; }
}
