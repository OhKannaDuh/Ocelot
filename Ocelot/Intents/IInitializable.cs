namespace Ocelot.Intents;

[Intent]
public interface IInitializable
{
    void PreInitialize();

    void Initialize();

    void PostInitialize();
}
