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

You'll want to make a plugin that extends [`Ocelot.OcelotPlugin`](https://github.com/OhKannaDuh/Ocelot/blob/master/Ocelot/OcelotPlugin.cs), which youll need to provide with a [`Dalamud.Plugin.IDalamudPluginInterface`](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/IDalamudPluginInterface.cs). Luckily if you request this in your plugins constructor, Dalamud will supply it for you.

```cs
using Dalamud.Plugin;
using Ocelot;
using Ocelot.States;

namespace MyAwesomePlugin;

public sealed class Plugin(IDalamudPluginInterface plugin) : OcelotPlugin(plugin)
...
```

`OcelotPlugin` is where our DI container gets set up. Your plugin _should_ implement `void Boostrap(IServiceCollection services)` to register your own types and load any Ocelot modules you need (more on modules in the wiki).

```cs
using Microsoft.Extensions.DependencyInjection;
...
protected override void Boostrap(IServiceCollection services)
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
