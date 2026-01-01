namespace Ocelot.Ipc.WrathCombo;

public static class WrathBailMessages
{
    public const string LiveDisabled =
        "IPC services are currently disabled.";

    public const string InvalidLease =
        "Invalid lease.";

    public const string BlacklistedLease =
        "Blacklisted lease.";

    public const string NotEnoughConfigurations =
        "Not enough configurations available.";
}

public enum WrathComboStateKeys
{
    Enabled,

    AutoMode,
}

public enum WrathComboTargetTypeKeys
{
    SingleTarget,

    MultiTarget,

    HealST,

    HealMT,

    Other,
}

public enum WrathComboSimplicityLevelKeys
{
    Simple,

    Advanced,

    Other,
}

public enum WrathCancellationReason
{
    WrathUserManuallyCancelled = 0,

    LeaseePluginDisabled = 1,

    WrathPluginDisabled = 2,

    LeaseeReleased = 3,

    AllServicesSuspended = 4,

    JobChanged = 5,
}

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
