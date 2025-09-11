using Ocelot.States;

namespace Ocelot.Prowler.Handlers;

[State<ProwlState>(ProwlState.Cancelled)]
public class Cancelled : Handler;
