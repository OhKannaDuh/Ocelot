using Dalamud.Interface.Textures.TextureWraps;

namespace Ocelot.Services.UI.ComposableStrings;

public partial class ComposableGroup
{
    public Services.UI.ComposableStrings.ComposableGroup Image(Image image)
    {
        composables.Add(image);
        return this;
    }

    public Services.UI.ComposableStrings.ComposableGroup Image(IDalamudTextureWrap image)
    {
        return Image(new Image(image));
    }

    public Services.UI.ComposableStrings.ComposableGroup Image(uint id)
    {
        return Image(new Image(textures, id));
    }
}
