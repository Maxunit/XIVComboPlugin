using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class WAR
{
    public const byte ClassID = 3;
    public const uint
        HeavySwing = 31,
        Maim = 37,
        Berserk = 38,
        ThrillOfBattle = 40,
        Overpower = 41,
        StormsPath = 42,
        StormsEye = 45,
        Tomahawk = 46,
        InnerBeast = 49,
        SteelCyclone = 51,
        Infuriate = 52,
        FellCleave = 3549,
        Decimate = 3550,
        Onslaught = 7386,
        Upheaval = 7387,
        RawIntuition = 3551,
        Equilibrium = 3552,
        InnerRelease = 7389,
        MythrilTempest = 16462,
        ChaoticCyclone = 16463,
        NascentFlash = 16464,
        InnerChaos = 16465,
        Bloodwhetting = 25751,
        Orogeny = 25752,
        PrimalRend = 25753,
        PrimalWrath = 36924,
        PrimalRuination = 36925;

    public const byte JobID = 21;

    public static class Buffs
    {
        public const ushort
            Berserk = 86,
            InnerRelease = 1177,
            NascentChaos = 1897,
            PrimalRendReady = 2624,
            SurgingTempest = 2677,
            Wrathful = 3901,
            PrimalRuinationReady = 3834;
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
            Tomahawk = 15,
            StormsPath = 26,
            ThrillOfBattle = 30,
            InnerBeast = 35,
            MythrilTempest = 40,
            SteelCyclone = 45,
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
            PrimalRend = 90,
            PrimalWrath = 96,
            PrimalRuination = 100;
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
                if (CanUseAction(WAR.PrimalRend) && HasEffect(WAR.Buffs.PrimalRendReady))
                    return WAR.PrimalRend;
            }
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

            if (IsEnabled(CustomComboPreset.UpheavalOrogenySpenderFeature) && CanUseAction(WAR.Orogeny) && IsOffCooldown(WAR.Orogeny) && HasEffect(WAR.Buffs.SurgingTempest))
                return OriginalHook(WAR.Orogeny);
            if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && gauge >= 90 && CanUseAction(WAR.Decimate))
                return OriginalHook(WAR.Decimate);
            if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && gauge >= 90 && CanUseAction(WAR.SteelCyclone))
                return OriginalHook(WAR.SteelCyclone);
            if (IsEnabled(CustomComboPreset.WarriorInnerReleaseFeature) && HasEffect(WAR.Buffs.InnerRelease))
                return OriginalHook(WAR.Decimate);
            if (comboTime > 0)
            {
                if (lastComboMove == WAR.Overpower && CanUseAction(WAR.MythrilTempest))
                    return WAR.MythrilTempest;
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
            if (CanUseAction(WAR.NascentFlash))
                // Bloodwhetting
                return WAR.NascentFlash;

            return WAR.RawIntuition;
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
            if (IsEnabled(CustomComboPreset.UpheavalOrogenySpenderFeature) && CanUseAction(WAR.Orogeny) && IsOffCooldown(WAR.Orogeny) && HasEffect(WAR.Buffs.SurgingTempest))
                return OriginalHook(WAR.Orogeny);
            if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && gauge >= 90 && CanUseAction(WAR.Decimate))
                return OriginalHook(WAR.Decimate);
            if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && gauge >= 90 && CanUseAction(WAR.SteelCyclone))
                return OriginalHook(WAR.SteelCyclone);
            if (IsEnabled(CustomComboPreset.WarriorInnerReleaseFeature) && HasEffect(WAR.Buffs.InnerRelease))
                return OriginalHook(WAR.Decimate);
            if (comboTime > 0)
            {
                if (lastComboMove == WAR.Overpower && CanUseAction(WAR.MythrilTempest))
                    return WAR.MythrilTempest;
            }

            return WAR.Overpower;
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
            if (level >= WAR.Levels.PrimalWrath && HasEffect(WAR.Buffs.Wrathful))
                return WAR.PrimalWrath;

            if (level >= WAR.Levels.PrimalRuination && HasEffect(WAR.Buffs.PrimalRuinationReady))
                return WAR.PrimalRuination;

            if (CanUseAction(WAR.PrimalRend) && HasEffect(WAR.Buffs.PrimalRendReady))
                return WAR.PrimalRend;
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
            var gauge = GetJobGauge<WARGauge>();

            if (IsEnabled(CustomComboPreset.HeavySwingOnslaughtFeature))
            {
                if (CanUseAction(WAR.Onslaught) && (!InMeleeRange || !InSoftMeleeRange) && HasCharges(WAR.Onslaught))
                    return OriginalHook(WAR.Onslaught);
            }

            if (IsEnabled(CustomComboPreset.HeavySwingTomahawkFeature))
            {
                if (CanUseAction(WAR.Tomahawk) && (!InMeleeRange || !InSoftMeleeRange))
                    return OriginalHook(WAR.Tomahawk);
            }

            if (IsEnabled(CustomComboPreset.UpheavalOrogenySpenderFeature) && CanUseAction(WAR.Upheaval) && IsOffCooldown(WAR.Upheaval) && HasEffect(WAR.Buffs.SurgingTempest))
                return OriginalHook(WAR.Upheaval);

            if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature))
            {
                if (CanUseAction(WAR.FellCleave) && (HasEffect(WAR.Buffs.InnerRelease) || gauge.BeastGauge >= 90))
                    return WAR.FellCleave;
            }

            if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature))
            {
                if (CanUseAction(WAR.InnerBeast) && gauge.BeastGauge >= 90)
                    // Fell Cleave
                    return OriginalHook(WAR.InnerBeast);
            }

            if (comboTime > 0)
            {
                if (lastComboMove == WAR.Maim)
                {
                    if (CanUseAction(WAR.StormsEye))
                    {
                        var surgingTempest = FindEffect(WAR.Buffs.SurgingTempest);
                        if (surgingTempest is null)
                            return WAR.StormsEye;

                        // Medicated + Opener
                        if (HasEffect(ADV.Buffs.Medicated) && surgingTempest.RemainingTime >= 10)
                            return WAR.StormsPath;

                        if (surgingTempest.RemainingTime <= 30)
                            return WAR.StormsEye;
                    }

                    if (CanUseAction(WAR.StormsPath))
                        return WAR.StormsPath;

                    return WAR.HeavySwing;
                }

                if (lastComboMove == WAR.HeavySwing && CanUseAction(WAR.Maim))
                    return WAR.Maim;
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
            var gauge = GetJobGauge<WARGauge>();

            if (IsEnabled(CustomComboPreset.HeavySwingOnslaughtFeature))
            {
                if (CanUseAction(WAR.Onslaught) && (!InMeleeRange || !InSoftMeleeRange) && HasCharges(WAR.Onslaught))
                    return OriginalHook(WAR.Onslaught);
            }

            if (IsEnabled(CustomComboPreset.HeavySwingTomahawkFeature))
            {
                if (CanUseAction(WAR.Tomahawk) && (!InMeleeRange || !InSoftMeleeRange))
                    return OriginalHook(WAR.Tomahawk);
            }

            if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature))
            {
                if (CanUseAction(WAR.FellCleave) && (HasEffect(WAR.Buffs.InnerRelease) || gauge.BeastGauge >= 90))
                    return WAR.FellCleave;
            }

            if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature))
            {
                if (CanUseAction(WAR.InnerBeast) && gauge.BeastGauge >= 90)
                    // Fell Cleave
                    return OriginalHook(WAR.InnerBeast);
            }

            if (comboTime > 0)
            {
                if (lastComboMove == WAR.Maim && CanUseAction(WAR.StormsEye))
                    return WAR.StormsEye;

                if (lastComboMove == WAR.HeavySwing && CanUseAction(WAR.Maim))
                    return WAR.Maim;
            }

            return WAR.HeavySwing;
        }

        return actionID;
    }
}

internal class WarriorStormsPathCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorStormsPathCombo;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == WAR.StormsPath)
        {
            var gauge = GetJobGauge<WARGauge>();

            if (IsEnabled(CustomComboPreset.HeavySwingOnslaughtFeature))
            {
                if (CanUseAction(WAR.Onslaught) && (!InMeleeRange || !InSoftMeleeRange) && HasCharges(WAR.Onslaught))
                    return OriginalHook(WAR.Onslaught);
            }

            if (IsEnabled(CustomComboPreset.HeavySwingTomahawkFeature))
            {
                if (CanUseAction(WAR.Tomahawk) && (!InMeleeRange || !InSoftMeleeRange))
                    return OriginalHook(WAR.Tomahawk);
            }

            if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature))
            {
                if (CanUseAction(WAR.FellCleave) && (HasEffect(WAR.Buffs.InnerRelease) || gauge.BeastGauge >= 90))
                    return WAR.FellCleave;
            }

            if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature))
            {
                if (CanUseAction(WAR.InnerBeast) && gauge.BeastGauge >= 90)
                    // Fell Cleave
                    return OriginalHook(WAR.InnerBeast);
            }

            if (comboTime > 0)
            {
                if (lastComboMove == WAR.Maim && CanUseAction(WAR.StormsPath))
                    return WAR.StormsPath;

                if (lastComboMove == WAR.HeavySwing && CanUseAction(WAR.Maim))
                    return WAR.Maim;
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
        if (actionID == WAR.ThrillOfBattle)
        {
            if (IsEnabled(CustomComboPreset.WarriorHealthyBalancedDietFeature))
            {
                if (CanUseAction(WAR.Bloodwhetting))
                {
                    if (IsOffCooldown(WAR.Bloodwhetting))
                        return WAR.Bloodwhetting;
                }
                else if (CanUseAction(WAR.RawIntuition))
                {
                    if (IsOffCooldown(WAR.RawIntuition))
                        return WAR.RawIntuition;
                }

                if (CanUseAction(WAR.ThrillOfBattle) && IsOffCooldown(WAR.ThrillOfBattle))
                    return WAR.ThrillOfBattle;

                if (CanUseAction(WAR.Equilibrium) && IsOffCooldown(WAR.Equilibrium))
                    return WAR.Equilibrium;
            }
        }

        return actionID;
    }
}
