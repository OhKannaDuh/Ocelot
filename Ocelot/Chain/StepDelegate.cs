using System.Threading;
using System.Threading.Tasks;
using Ocelot.Chain.Steps;

namespace Ocelot.Chain;

public delegate ValueTask<ChainStepResult> StepDelegate(IChainStep step, ChainContext context, CancellationToken token);
