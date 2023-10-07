using Lumina.Data.Parsing;

namespace XIVComboExpandedPlugin.Combos;

internal static class ADV
{
    public const byte ClassID = 0;
    public const byte JobID = 0;

    public const uint
        // tanks
        LowBlow = 7540,
        Interject = 7538,
        Rampart = 7531,
        Reprisal = 7535,
        // healers
        LucidDreaming = 1204,
        // mages
        Swiftcast = 7561,
        // bluemage
        AngelWhisper = 18317,
        VariantRaise2 = 29734;

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
            LowBlow = 12,
            Interject = 18,
            Swiftcast = 18,
            Reprisal = 22,
            VariantRaise2 = 90;
    }
}

internal class SwiftRaiseFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset => CustomComboPreset.AdvSwiftcastFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if ((actionID == AST.Ascend && level >= AST.Levels.Ascend) ||
            (actionID == SCH.Resurection && level >= SCH.Levels.Resurection) ||
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

internal class VariantRaiseFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset => CustomComboPreset.AdvVariantRaiseFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if ((actionID == AST.Ascend && level >= AST.Levels.Ascend) ||
            (actionID == SCH.Resurection && level >= SCH.Levels.Resurection) ||
            (actionID == SGE.Egeiro && level >= SGE.Levels.Egeiro) ||
            (actionID == WHM.Raise && level >= WHM.Levels.Raise) ||
            (actionID == RDM.Verraise && level >= RDM.Levels.Verraise && !HasEffect(RDM.Buffs.Dualcast)) ||
            (actionID == BLU.AngelWhisper && level >= BLU.Levels.AngelWhisper))
        {
            // Per Splatoon:
            // 1069: solo
            // 1075: group
            // 1076: savage
            if (level >= ADV.Levels.VariantRaise2 && CurrentTerritory == 1075u)
                return ADV.VariantRaise2;
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

internal class StunInterruptCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset => CustomComboPreset.AdvAny;

    protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
    {
        if ((actionID == ADV.LowBlow) || (actionID == PLD.ShieldBash))
        {
            if (IsEnabled(CustomComboPreset.StunInterruptCombo) && level >= ADV.Levels.Interject && IsOffCooldown(ADV.Interject) && CanInterrupt)
            {
                return ADV.Interject;
            }

            return OriginalHook(actionID);
        }

        return actionID;
    }
}