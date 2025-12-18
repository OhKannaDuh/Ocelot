using Dalamud.Interface.Textures.TextureWraps;

namespace Ocelot.Services.UI.ComposableStrings;

public partial class ComposableGroup
{
    public ComposableGroup Image(Image image)
    {
        composables.Add(image);
        return this;
    }

    public ComposableGroup Image(IDalamudTextureWrap image)
    {
        return Image(new Image(image));
    }

    public ComposableGroup Image(uint id)
    {
        return Image(new Image(textures, id));
    }
}
