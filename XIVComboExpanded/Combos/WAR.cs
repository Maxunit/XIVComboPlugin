using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;

namespace XIVComboExpandedPlugin.Combos;

internal static class WAR
{
    public const byte ClassID = 3;
    public const byte JobID = 21;

    public const uint
        HeavySwing = 31,
        Maim = 37,
        Berserk = 38,
        ThrillOfBattle = 40,
        Overpower = 41,
        StormsPath = 42,
        StormsEye = 45,
        InnerBeast = 49,
        SteelCyclone = 51,
        Infuriate = 52,
        FellCleave = 3549,
        Decimate = 3550,
        Onslaught = 7386,
        RawIntuition = 3551,
        Equilibrium = 3552,
        InnerRelease = 7389,
        MythrilTempest = 16462,
        ChaoticCyclone = 16463,
        NascentFlash = 16464,
        InnerChaos = 16465,
        Bloodwhetting = 25751,
        PrimalRend = 25753,
        Upheaval = 7387,
        Orogeny = 25752;

    public static class Buffs
    {
        public const ushort
            Berserk = 86,
            InnerRelease = 1177,
            NascentChaos = 1897,
            PrimalRendReady = 2624,
            SurgingTempest = 2677;
    }

    public static class Debuffs
    {
        public const ushort
            Placeholder = 0;
    }

    public static class Levels
    {
        public const byte
            Maim = 4,
            Berserk = 6,
            StormsPath = 26,
            ThrillOfBattle = 30,
            InnerBeast = 35,
            MythrilTempest = 40,
            StormsEye = 50,
            Infuriate = 50,
            InnerBeastMastery = 54,
            FellCleave = 54,
            RawIntuition = 56,
            Equilibrium = 58,
            SteelCycloneMastery = 60,
            Decimate = 60,
            Onslaught = 62,
            Upheaval = 64,
            InnerRelease = 70,
            NascentFlash = 76,
            InnerChaos = 80,
            Bloodwhetting = 82,
            Orogeny = 86,
            PrimalRend = 90;
    }
}

internal class WarriorStormsPathCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorStormsPathCombo;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == WAR.StormsPath)
        {
            var gauge = GetJobGauge<WARGauge>().BeastGauge;
            if (IsEnabled(CustomComboPreset.HeavySwingOnslaughtFeature) && level >= WAR.Levels.Onslaught && HasTarget() && !InMeleeRange && HasCharges(GNB.RoughDivide))
                return OriginalHook(WAR.Onslaught);
            if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && gauge >= 90 && level >= WAR.Levels.InnerBeastMastery)
                return OriginalHook(WAR.FellCleave);
            if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && gauge >= 90 && level <= WAR.Levels.InnerBeastMastery)
                return OriginalHook(WAR.InnerBeast);
            if (IsEnabled(CustomComboPreset.WarriorInnerReleaseFeature) && HasEffect(WAR.Buffs.InnerRelease) && level >= WAR.Levels.FellCleave)
                return OriginalHook(WAR.FellCleave);
            if (comboTime > 0)
            {
                if (lastComboMove == WAR.Maim && level >= WAR.Levels.StormsPath)
                {
                    return WAR.StormsPath;
                }

                if (lastComboMove == WAR.HeavySwing && level >= WAR.Levels.Maim)
                {
                    return WAR.Maim;
                }
            }

            return WAR.HeavySwing;
        }

        return actionID;
    }
}

internal class WarriorStormsEyeCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorStormsEyeCombo;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == WAR.StormsEye)
        {
            var gauge = GetJobGauge<WARGauge>().BeastGauge;
            if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && gauge >= 90 && level >= WAR.Levels.InnerBeastMastery)
                return OriginalHook(WAR.FellCleave);
            if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && gauge >= 90 && level <= WAR.Levels.InnerBeastMastery)
                return OriginalHook(WAR.InnerBeast);
            if (IsEnabled(CustomComboPreset.WarriorInnerReleaseFeature) && HasEffect(WAR.Buffs.InnerRelease) && level >= WAR.Levels.FellCleave)
                return OriginalHook(WAR.FellCleave);
            if (comboTime > 0)
            {
                if (lastComboMove == WAR.Maim && level >= WAR.Levels.StormsEye)
                    return WAR.StormsEye;

                if (lastComboMove == WAR.HeavySwing && level >= WAR.Levels.Maim)
                    return WAR.Maim;
            }

            return WAR.HeavySwing;
        }

        return actionID;
    }
}

internal class WarriorMythrilTempestCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorMythrilTempestCombo;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == WAR.MythrilTempest)
        {
            var gauge = GetJobGauge<WARGauge>().BeastGauge;

            if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && gauge >= 90 && level >= WAR.Levels.SteelCycloneMastery)
                return OriginalHook(WAR.Decimate);
            if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && gauge >= 90 && level <= WAR.Levels.SteelCycloneMastery)
                return OriginalHook(WAR.SteelCyclone);
            if (IsEnabled(CustomComboPreset.WarriorInnerReleaseFeature) && HasEffect(WAR.Buffs.InnerRelease))
                return OriginalHook(WAR.Decimate);
            if (comboTime > 0)
            {
                if (lastComboMove == WAR.Overpower && level >= WAR.Levels.MythrilTempest)
                {
                    return WAR.MythrilTempest;
                }
            }

            return WAR.Overpower;
        }

        return actionID;
    }
}

internal class WarriorOverpowerCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorOverpowerCombo;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == WAR.Overpower)
        {
            var gauge = GetJobGauge<WARGauge>().BeastGauge;
            if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && gauge >= 90 && level >= WAR.Levels.SteelCycloneMastery)
                return OriginalHook(WAR.Decimate);
            if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && gauge >= 90 && level <= WAR.Levels.SteelCycloneMastery)
                return OriginalHook(WAR.SteelCyclone);
            if (IsEnabled(CustomComboPreset.WarriorInnerReleaseFeature) && HasEffect(WAR.Buffs.InnerRelease))
                return OriginalHook(WAR.Decimate);
            if (comboTime > 0)
            {
                if (lastComboMove == WAR.Overpower && level >= WAR.Levels.MythrilTempest)
                {
                    return WAR.MythrilTempest;
                }
            }

            return WAR.Overpower;
        }

        return actionID;
    }
}

internal class WarriorNascentFlashFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorNascentFlashFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == WAR.NascentFlash)
        {
            if (level >= WAR.Levels.NascentFlash)
                // Bloodwhetting
                return WAR.NascentFlash;

            return WAR.RawIntuition;
        }

        return actionID;
    }
}

internal class WarriorFellCleaveDecimate : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == WAR.InnerBeast || actionID == WAR.FellCleave || actionID == WAR.SteelCyclone || actionID == WAR.Decimate)
        {
            if (IsEnabled(CustomComboPreset.WarriorPrimalBeastFeature))
            {
                if (level >= WAR.Levels.PrimalRend && HasEffect(WAR.Buffs.PrimalRendReady))
                    return WAR.PrimalRend;
            }

            // if (IsEnabled(CustomComboPreset.WarriorInfuriateBeastFeature))
            // {
            //     var gauge = GetJobGauge<WARGauge>();

            // if (level >= WAR.Levels.Infuriate && gauge.BeastGauge < 50 && !HasEffect(WAR.Buffs.InnerRelease))
            //         return WAR.Infuriate;
            // }
        }

        return actionID;
    }
}

internal class WArriorPrimalReleaseFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorPrimalReleaseFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == WAR.Berserk || actionID == WAR.InnerRelease)
        {
            if (level >= WAR.Levels.PrimalRend && HasEffect(WAR.Buffs.PrimalRendReady))
                return WAR.PrimalRend;
        }

        return actionID;
    }
}

internal class UpheavalOrogenySpenderFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.UpheavalOrogenySpenderFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (IsEnabled(CustomComboPreset.UpheavalOrogenySpenderFeature) && HasEffect(WAR.Buffs.SurgingTempest))
        {
            if (actionID is WAR.HeavySwing or WAR.Maim)
            {
                if (level >= WAR.Levels.Upheaval)
                    return PickByCooldown(actionID, actionID, WAR.Upheaval);
                return OriginalHook(actionID);
            }

            if (actionID is WAR.Overpower or WAR.MythrilTempest)
            {
                if (level >= WAR.Levels.Orogeny)
                    return PickByCooldown(actionID, actionID, WAR.Orogeny);
                return OriginalHook(actionID);
            }
        }

        return actionID;
    }
}

internal class WarriorStormPathStormEye : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorStormPathStormEye;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == WAR.HeavySwing)
        {
            var gauge = GetJobGauge<WARGauge>().BeastGauge;
            Status? surgingtempesttime = FindEffect(WAR.Buffs.SurgingTempest);
            if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && gauge >= 90 && level >= WAR.Levels.InnerBeastMastery)
                return OriginalHook(WAR.FellCleave);
            if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && gauge >= 90 && level <= WAR.Levels.InnerBeastMastery)
                return OriginalHook(WAR.InnerBeast);
            if (IsEnabled(CustomComboPreset.WarriorInnerReleaseFeature) && HasEffect(WAR.Buffs.InnerRelease) && level >= WAR.Levels.FellCleave)
                return OriginalHook(WAR.FellCleave);
            if (IsEnabled(CustomComboPreset.WarriorStormPathStormEye) && level >= WAR.Levels.StormsEye && HasEffect(WAR.Buffs.SurgingTempest) && surgingtempesttime is not null)
            {
                if (comboTime > 0 && surgingtempesttime.RemainingTime >= 30 && lastComboMove == WAR.Maim && level >= WAR.StormsPath)
                    return WAR.StormsPath;

                if (comboTime > 0 && surgingtempesttime.RemainingTime <= 30 && lastComboMove == WAR.Maim && level >= WAR.StormsEye)
                    return WAR.StormsEye;
            }

            if (comboTime > 0)
            {
                if (lastComboMove == WAR.Maim && level >= WAR.StormsPath)
                {
                    return WAR.StormsPath;
                }

                if (lastComboMove == WAR.Maim && level >= WAR.Levels.StormsEye)
                {
                    return WAR.StormsEye;
                }

                if (lastComboMove == WAR.HeavySwing && level >= WAR.Levels.Maim)
                {
                    return WAR.Maim;
                }
            }

            return WAR.HeavySwing;
        }

        return actionID;
    }
}

internal class WarriorBloodwhetting : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == WAR.Bloodwhetting || actionID == WAR.RawIntuition)
        {
            if (IsEnabled(CustomComboPreset.WarriorHealthyBalancedDietFeature))
            {
                if (level >= WAR.Levels.Bloodwhetting)
                {
                    if (IsOffCooldown(WAR.Bloodwhetting))
                        return WAR.Bloodwhetting;
                }
                else if (level >= WAR.Levels.RawIntuition)
                {
                    if (IsOffCooldown(WAR.RawIntuition))
                        return WAR.RawIntuition;
                }

                if (level >= WAR.Levels.ThrillOfBattle && IsOffCooldown(WAR.ThrillOfBattle))
                    return WAR.ThrillOfBattle;

                if (level >= WAR.Levels.Equilibrium && IsOffCooldown(WAR.Equilibrium))
                    return WAR.Equilibrium;
            }
        }

        return actionID;
    }
}
