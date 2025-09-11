using System;

namespace Ocelot.Services;

public class ServiceNotFoundException(Type service) : Exception($"Service for type {service.FullName} could not be found");
