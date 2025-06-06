using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ECommons.DalamudServices;

namespace Ocelot.Chain
{
    public class WaitWhileLink : IChainlink
    {
        private readonly Func<bool> predicate;
        private readonly int timeout;
        private readonly int interval;
        private readonly bool framework;

        public WaitWhileLink(Func<bool> predicate, int timeout = 5000, int interval = 250, bool framework = false)
        {
            this.predicate = predicate;
            this.timeout = timeout;
            this.interval = interval;
            this.framework = framework;
        }

        public async Task RunAsync(ChainContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            while (!context.token.IsCancellationRequested)
            {
                if (stopwatch.ElapsedMilliseconds >= timeout)
                {
                    context.source.Cancel();
                    continue;
                }

                bool result = false;

                if (framework)
                {
                    var tcs = new TaskCompletionSource<bool>();
                    await Svc.Framework.RunOnFrameworkThread(() =>
                    {
                        try
                        {
                            tcs.SetResult(predicate());
                        }
                        catch (Exception ex)
                        {
                            tcs.SetException(ex);
                        }
                    });
                    result = await tcs.Task;
                }
                else
                {
                    result = predicate();
                }

                if (!result)
                {
                    break;
                }

                await Task.Delay(interval, context.token);
            }
        }
    }
}
