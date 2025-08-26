using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using Ocelot.Gameplay.Targeting;

namespace Ocelot.Extensions;

public static class IEnumerableIBattleNpcEx
{
    public static IBattleNpc? Closest(this IEnumerable<IBattleNpc> enemies)
    {
        return enemies.FirstOrDefault(defaultValue: null);
    }

    public static IBattleNpc? Furthest(this IEnumerable<IBattleNpc> enemies)
    {
        return enemies.LastOrDefault(defaultValue: null);
    }

    public static IBattleNpc? Centroid(this IEnumerable<IBattleNpc> enemies)
    {
        var list = enemies.ToList();
        if (list.Count <= 0)
        {
            return null;
        }

        var sum = Vector3.Zero;
        foreach (var npc in list)
        {
            sum += npc.Position;
        }

        var centroid = sum / list.Count;

        return list
            .OrderBy(npc => Vector3.DistanceSquared(npc.Position, centroid))
            .FirstOrDefault(defaultValue: null);
    }

    public static IBattleNpc? Popular(this IEnumerable<IBattleNpc> enemies)
    {
        var enemyList = enemies as IList<IBattleNpc> ?? enemies.ToList();
        if (enemyList.Count == 0)
        {
            return null;
        }

        var playersWithTarget = TargetingHelper.Players.Where(p => p.TargetObject != null).ToList();
        if (playersWithTarget.Count == 0)
        {
            return enemyList.Closest();
        }

        var targetCounts = playersWithTarget
            .Select(p => p.TargetObjectId)
            .GroupBy(id => id)
            .ToDictionary(g => g.Key, g => g.Count());

        IBattleNpc? best = null;
        var bestCount = 0;

        foreach (var enemy in enemyList)
        {
            if (!targetCounts.TryGetValue(enemy.GameObjectId, out var count))
            {
                continue;
            }

            if (count > bestCount || count == bestCount && best is null)
            {
                best = enemy;
                bestCount = count;
            }
        }

        return best ?? enemyList.Closest();
    }

    public static unsafe IEnumerable<IBattleNpc> FilterByFate(this IEnumerable<IBattleNpc> enemies, uint fateId)
    {
        return enemies.Where(e =>
        {
            var battleChara = (BattleChara*)e.Address;

            return battleChara->FateId == fateId;
        });
    }
}
