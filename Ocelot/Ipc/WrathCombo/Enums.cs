namespace Ocelot.Ipc.WrathCombo;

public enum WrathSetResult
{
    IGNORED = -1,

    // Success Statuses
    Okay = 0,

    OkayWorking = 1,

    // Error Statuses
    IPCDisabled = 10,

    InvalidLease = 11,

    BlacklistedLease = 12,

    Duplicate = 13,

    PlayerNotAvailable = 14,

    InvalidConfiguration = 15,

    InvalidValue = 16,
}

public enum WrathAutoRotationConfigOption
{
    InCombatOnly = 0,

    DPSRotationMode = 1,

    HealerRotationMode = 2,

    FATEPriority = 3,

    QuestPriority = 4,

    SingleTargetHPP = 5,

    AoETargetHPP = 6,

    SingleTargetRegenHPP = 7,

    ManageKardia = 8,

    AutoRez = 9,

    AutoRezDPSJobs = 10,

    AutoCleanse = 11,

    IncludeNPCs = 12,

    OnlyAttackInCombat = 13,

    OrbwalkerIntegration = 14,

    AutoRezOutOfParty = 15,

    DPSAoETargets = 16,

    SingleTargetExcogHPP = 17,
}
