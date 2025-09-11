using System;

namespace Ocelot.Modules;

public class UnableToLoadModuleException(string message) : Exception(message);
