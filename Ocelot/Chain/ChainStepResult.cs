using System;
using System.Collections.Generic;
using Ocelot.Chain.Steps;

namespace Ocelot.Chain;

public readonly record struct ChainStepResult(
    ChainStepStatus Status,
    IReadOnlyList<IChainStep>? Insert = null,
    Exception? Error = null
);
