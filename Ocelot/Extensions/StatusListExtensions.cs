using Dalamud.Game.ClientState.Statuses;

namespace Ocelot.Extensions;

public static class StatusListExtensions
{
    extension(StatusList self)
    {
        public bool HasAny(params uint[] ids)
        {
            foreach (var s in self)
            {
                if (s.StatusId == 0)
                {
                    continue;
                }

                if (ids.Contains(s.StatusId))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Has(uint id)
        {
            return self.HasAny(id);
        }

        public bool TryGet(uint id, out IStatus status)
        {
            foreach (var s in self)
            {
                if (s.StatusId == id)
                {
                    status = s;
                    return true;
                }
            }

            status = null!;
            return false;
        }
    }
}
