using System.Threading.Tasks;

namespace Ocelot.Prowler;

public interface IProwlerAction
{
    Task ExecuteAsync(ProwlerContext context);
}