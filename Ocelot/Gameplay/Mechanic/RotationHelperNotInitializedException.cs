using System;

namespace Ocelot.Gameplay.Mechanic;

public class MechanicHelperNotInitializedException() : InvalidOperationException("Attempted to use MechanicHelper before it was initialized.");
