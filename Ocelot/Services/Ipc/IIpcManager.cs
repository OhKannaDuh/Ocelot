using System;

namespace Ocelot.Services.Ipc;

public interface IIpcManager
{
    void Refresh();

    bool IsInitialized(Type providerType);
}
