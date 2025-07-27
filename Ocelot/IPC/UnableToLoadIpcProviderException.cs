using System;

namespace Ocelot.IPC;

public class UnableToLoadIpcProviderException(string message) : Exception(message);
