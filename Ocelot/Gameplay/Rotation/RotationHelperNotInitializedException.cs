using System;

namespace Ocelot.Gameplay.Rotation;

public class RotationHelperNotInitializedException() : InvalidOperationException("Attempted to use RotationHelper before it was initialized.");
