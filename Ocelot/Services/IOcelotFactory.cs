namespace Ocelot.Services;

public interface IOcelotFactory
{
    object Create(ServiceContainer container);
}
