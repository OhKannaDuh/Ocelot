using Ocelot.Lifecycle;
using Pictomancy;

namespace Ocelot.Pictomancy.Services;

public class PictomancyProvider : IPictomancyProvider, IOnPreRender, IOnPostRender
{
    private PctDrawList? current;

    private bool failed;

    public bool HasDrawList
    {
        get => current is not null;
    }

    public PctDrawList GetDrawList()
    {
        if (current is null)
        {
            throw new InvalidOperationException("Pictomancy draw list not available. It is only available during the render lifecycle hook.");
        }

        return current;
    }

    public void PreRender()
    {
        if (current is not null)
        {
            try
            {
                current.Dispose();
            }
            catch
            {
                /* ignore */
            }

            current = null;
        }

        if (failed)
        {
            return;
        }

        // After a failed start (e.g. a game patch) stop calling it; the host logs the error once.
        try
        {
            current = PctService.Draw();
        }
        catch
        {
            failed = true;
            throw;
        }
    }

    public void PostRender()
    {
        if (current is null)
        {
            return;
        }

        try
        {
            current.Dispose();
        }
        catch (InvalidOperationException)
        {
        }
        finally
        {
            current = null;
        }
    }
}
