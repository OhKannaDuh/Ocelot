using System;

namespace Ocelot.IPC;

public class UnableToLoadIpcProviderException : Exception
{
    public UnableToLoadIpcProviderException(string message) : base(message)
    {
    }
}
