using System;

namespace Ocelot.Prowler;

public class AlreadyProwlingException() : Exception("Another instance of prowler is already running");
