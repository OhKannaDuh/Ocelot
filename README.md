# Ocelot

Ocelot is an opinionated Dalamud plugin library.

Nuget Package: https://www.nuget.org/packages/FFXIVOcelot

## Version

| Library Version Range | SDK API Version | Notes                                        |
| --------------------- | --------------- | -------------------------------------------- |
| 0.x.x                 | API 12          |                                              |
| 1.x.x                 | API 13          |                                              |
| 2.x.x                 | API 13          | Major internal changes to support DI pattern |

## Getting Started with Ocelot DI

You'll want to make a plugin that extends [`Ocelot.OcelotPlugin`](https://github.com/OhKannaDuh/Ocelot/blob/master/Ocelot/OcelotPlugin.cs), which implements [`IAsyncDalamudPlugin`](https://dalamud.dev/api/Dalamud.Plugin/Interfaces/IAsyncDalamudPlugin). Request [`IDalamudPluginInterface`](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/IDalamudPluginInterface.cs) and `IPluginLog` in your plugin constructor — Dalamud supplies them; Ocelot finishes setup in `LoadAsync`.

```cs
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Ocelot;

namespace MyAwesomePlugin;

public sealed class Plugin(IDalamudPluginInterface plugin, IPluginLog logger) : OcelotPlugin(plugin, logger)
...
```

`OcelotPlugin` builds the DI container and starts lifecycle hosts in `LoadAsync` (not the constructor), so boot does not hitch on heavy init. Your plugin _should_ override `void Bootstrap(IServiceCollection services)` to register types and load Ocelot modules (more on modules in the wiki).

```cs
using Microsoft.Extensions.DependencyInjection;
...
protected override void Bootstrap(IServiceCollection services)
{
    services.AddSingleton<MyAwesomeService>();
    services.AddSingleton<ILoggerService, MyCustomLogger>();
}
```

[Read up on .NET dependency injection for more info on this.](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)

## Ocelot Service Attribute

You can also decorate your classes with any number of `Ocelot.Services.OcelotServiceAttribute` for auto enrolment.

The attribute has two initable params.

- `System.Type? Service`
  - Can be used to register this class as a specific type/interface rather than the actual implementations type
  - Null will use the implementations type
- `Microsoft.Extensions.DependencyInjection.ServiceLifetime Lifetime`
  - Default: `ServiceLifetime.Singleton`

### Service type explanation

```cs
[OcelotService(Service = typeof(ILoggerService))]
public class MyCustomLogger : ILoggerService
```

This would achieve the same thing as:

```cs
services.AddSingleton<ILoggerService, MyCustomLogger>();
```

Where as:

```cs
[OcelotService]
public class MyCustomLogger : ILoggerService
```

would be the same thing as:

```cs
services.AddSingleton<MyCustomLogger>();
```
