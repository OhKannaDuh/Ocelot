using Ocelot.UI.ComposableStrings;

namespace Ocelot.UI.Services;

public partial interface IUIService
{
    ComposableGroup Compose();

    ComposableGroupState Render(ComposableGroup left, ComposableGroup right);
}
