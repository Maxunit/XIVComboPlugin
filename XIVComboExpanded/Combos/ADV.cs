namespace XIVComboExpandedPlugin.Combos;

internal static class ADV
{
    public const byte ClassID = 0;
    public const byte JobID = 0;

    public const uint
        LucidDreaming = 1204,
        Swiftcast = 7561,
        AngelWhisper = 18317,
        Rampart = 7531,
        Reprisal = 7535;

    public static class Buffs
    {
        public const ushort
            Medicated = 49;
    }

    public static class Debuffs
    {
        public const ushort
            Placeholder = 0;
    }

    public static class Levels
    {
        public const byte
            Rampart = 8,
            Swiftcast = 18,
            Reprisal = 22;
    }
}

internal class SwiftRaiseFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset => CustomComboPreset.AllSwiftcastFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if ((actionID == AST.Ascend && level >= AST.Levels.Ascend) ||
            (actionID == SCH.Ressurection && level >= SCH.Levels.Ressurection) ||
            (actionID == SGE.Egeiro && level >= SGE.Levels.Egeiro) ||
            (actionID == WHM.Raise && level >= WHM.Levels.Raise) ||
            (actionID == RDM.Verraise && level >= RDM.Levels.Verraise && !HasEffect(RDM.Buffs.Dualcast)) ||
            (actionID == BLU.AngelWhisper && level >= BLU.Levels.AngelWhisper))
        {
            if (level >= ADV.Levels.Swiftcast && IsOffCooldown(ADV.Swiftcast))
                return ADV.Swiftcast;
        }

        return actionID;
    }
}

internal class RampartReprisalCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset => CustomComboPreset.RampartReprisalCombo;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == ADV.Rampart)
        {
            if (level >= ADV.Levels.Reprisal && IsOffCooldown(ADV.Reprisal))
            {
                return ADV.Reprisal;
            }

            return ADV.Rampart;
        }

        return actionID;
    }
}