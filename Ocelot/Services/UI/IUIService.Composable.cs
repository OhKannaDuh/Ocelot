using Ocelot.Services.UI.ComposableStrings;
using ComposableGroup = Ocelot.Services.UI.ComposableStrings.ComposableGroup;

namespace Ocelot.Services.UI;

public partial interface IUIService
{
    ComposableGroup Compose();

    ComposableGroupState Render(ComposableGroup left, ComposableGroup right);
}
