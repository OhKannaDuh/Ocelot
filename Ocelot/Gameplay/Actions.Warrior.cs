﻿using FFXIVClientStructs.FFXIV.Client.Game;

namespace Ocelot.Gameplay;

public partial class Actions
{
    public class Warrior
    {
        public static readonly Action Defiance = new(ActionType.Action, 48);

        public static readonly Action ReleaseDefiance = new(ActionType.Action, 32066);
    }
}
