namespace Ocelot.Services.WindowManager;

public interface IMainRenderer
{
    void Render();

    string? ConfigGroup => null;
}

public class NullMainRenderer : IMainRenderer
{
    public void Render()
    {
    }
}

public interface IConfigRenderer
{
    void Render();

    void SelectGroup(string groupKey)
    {
    }
}

public class NullConfigRenderer : IConfigRenderer
{
    public void Render()
    {
    }
}
