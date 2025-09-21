namespace Ocelot.Services.Gate;

public interface IGateService
{
    bool Milliseconds(object owner, string scope, long interval);

    bool UpdatesPerSecond(object owner, string scope, int ups);

    bool Seconds(object owner, string scope, int seconds);

    bool Minutes(object owner, string scope, int minutes);

    void Reset(object owner);

    void Reset(object owner, string scope);
}
