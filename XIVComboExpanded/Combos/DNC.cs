using System;
using System.Linq;

using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class DNC
{
    public const byte JobID = 38;

    public const uint
        // Single Target
        Cascade = 15989,
        Fountain = 15990,
        ReverseCascade = 15991,
        Fountainfall = 15992,
        // AoE
        Windmill = 15993,
        Bladeshower = 15994,
        RisingWindmill = 15995,
        Bloodshower = 15996,
        // Dancing
        StandardStep = 15997,
        TechnicalStep = 15998,
        Tillana = 25790,
        LastDance = 36983,
        FinishingMove = 36984,
        // Fans
        FanDance1 = 16007,
        FanDance2 = 16008,
        FanDance3 = 16009,
        FanDance4 = 25791,
        // Other
        SaberDance = 16005,
        EnAvant = 16010,
        Devilment = 16011,
        Flourish = 16013,
        Improvisation = 16014,
        StarfallDance = 25792;

    public static class Buffs
    {
        public const ushort
            FlourishingSymmetry = 3017,
            FlourishingFlow = 3018,
            FlourishingFinish = 2698,
            FlourishingStarfall = 2700,
            SilkenSymmetry = 2693,
            SilkenFlow = 2694,
            StandardStep = 1818,
            TechnicalStep = 1819,
            ThreefoldFanDance = 1820,
            FourfoldFanDance = 2699,
            LastDanceReady = 3867,
            FinishingMoveReady = 3868;
    }

    public static class Debuffs
    {
        public const ushort
            Placeholder = 0;
    }

    public static class Levels
    {
        public const byte
            Cascade = 1,
            Fountain = 2,
            Windmill = 15,
            StandardStep = 15,
            ReverseCascade = 20,
            Bladeshower = 25,
            FanDance1 = 30,
            RisingWindmill = 35,
            Fountainfall = 40,
            Bloodshower = 45,
            FanDance2 = 50,
            FanDance3 = 66,
            TechnicalStep = 70,
            Flourish = 72,
            SaberDance = 76,
            Tillana = 82,
            FanDance4 = 86,
            StarfallDance = 90,
            LastDance = 92,
            FinishingMove = 96;
    }
}

internal class DancerDanceComboCompatibility : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerDanceComboCompatibility;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        var actionIDs = Service.Configuration.DancerDanceCompatActionIDs;

        if (actionIDs.Contains(actionID))
        {
            var gauge = GetJobGauge<DNCGauge>();

            if (CanUseAction(DNC.StandardStep) && gauge.IsDancing)
            {
                if (actionID == actionIDs[0] || (actionIDs[0] == 0 && actionID == DNC.Cascade))
                    return OriginalHook(DNC.Cascade);

                if (actionID == actionIDs[1] || (actionIDs[1] == 0 && actionID == DNC.Flourish))
                    return OriginalHook(DNC.Fountain);

                if (actionID == actionIDs[2] || (actionIDs[2] == 0 && actionID == DNC.FanDance1))
                    return OriginalHook(DNC.ReverseCascade);

                if (actionID == actionIDs[3] || (actionIDs[3] == 0 && actionID == DNC.FanDance2))
                    return OriginalHook(DNC.Fountainfall);
            }
        }

        return actionID;
    }
}

internal class DancerFanDance12 : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DncAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == DNC.FanDance1 || actionID == DNC.FanDance2)
        {
            var gauge = GetJobGauge<DNCGauge>();

            if (IsEnabled(CustomComboPreset.DancerFanDance3Feature))
            {
                if (IsEnabled(CustomComboPreset.DancerFanDance4Feature))
                {
                    if (gauge.Feathers == 4)
                    {
                        if (CanUseAction(DNC.FanDance3) && HasEffect(DNC.Buffs.ThreefoldFanDance))
                            return DNC.FanDance3;

                        return actionID;
                    }

                    if (CanUseAction(DNC.FanDance4) && HasEffect(DNC.Buffs.FourfoldFanDance))
                        return DNC.FanDance4;
                }

                if (CanUseAction(DNC.FanDance3) && HasEffect(DNC.Buffs.ThreefoldFanDance))
                    return DNC.FanDance3;
            }
        }

        return actionID;
    }
}

internal class DancerStandardStepTechnicalStep : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerDanceStepCombo;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == DNC.StandardStep)
        {
            var gauge = GetJobGauge<DNCGauge>();

            if (CanUseAction(DNC.StandardStep) && gauge.IsDancing && HasEffect(DNC.Buffs.StandardStep))
            {
                if (gauge.CompletedSteps < 2)
                    return gauge.NextStep;

                return OriginalHook(DNC.StandardStep);
            }

            return DNC.StandardStep;
        }

        if (actionID == DNC.TechnicalStep)
        {
            var gauge = GetJobGauge<DNCGauge>();

            if (CanUseAction(DNC.TechnicalStep) && gauge.IsDancing && HasEffect(DNC.Buffs.TechnicalStep))
            {
                if (gauge.CompletedSteps < 4)
                    return gauge.NextStep;
            }

            // Tillana
            return OriginalHook(DNC.TechnicalStep);
        }

        return actionID;
    }
}

internal class DancerFlourish : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DncAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == DNC.Flourish)
        {
            if (IsEnabled(CustomComboPreset.DancerFlourishFan3Feature))
            {
                if (CanUseAction(DNC.FanDance3) && HasEffect(DNC.Buffs.ThreefoldFanDance))
                    return DNC.FanDance3;
            }

            if (IsEnabled(CustomComboPreset.DancerFlourishFan4Feature))
            {
                if (CanUseAction(DNC.FanDance4) && HasEffect(DNC.Buffs.FourfoldFanDance))
                    return DNC.FanDance4;
            }
        }

        return actionID;
    }
}

internal class DancerCascadeFountain : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DncAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == DNC.Cascade)
        {
            var gauge = GetJobGauge<DNCGauge>();

            if (IsEnabled(CustomComboPreset.EvilDancerFanDanceCombo))
            {
                if (CanUseAction(DNC.FanDance3) && HasEffect(DNC.Buffs.ThreefoldFanDance))
                    return DNC.FanDance3;

                if (gauge.Feathers >= 1)
                {
                    if (CanUseAction(DNC.FanDance1))
                        return DNC.FanDance1;
                }

                if (gauge.Esprit >= 50)
                {
                    if (CanUseAction(DNC.SaberDance))
                        return DNC.SaberDance;
                }
            }

            if (IsEnabled(CustomComboPreset.DancerSingleTargetMultibutton))
            {
                if (CanUseAction(DNC.Fountainfall) && (HasEffect(DNC.Buffs.FlourishingFlow) || HasEffect(DNC.Buffs.SilkenFlow)))
                    return DNC.Fountainfall;

                if (CanUseAction(DNC.ReverseCascade) && (HasEffect(DNC.Buffs.FlourishingSymmetry) || HasEffect(DNC.Buffs.SilkenSymmetry)))
                    return DNC.ReverseCascade;

                if (lastComboMove == DNC.Cascade && CanUseAction(DNC.Fountain))
                    return DNC.Fountain;
            }

            if (IsEnabled(CustomComboPreset.DancerSingleTargetProcs))
            {
                if (CanUseAction(DNC.ReverseCascade) && (HasEffect(DNC.Buffs.FlourishingSymmetry) || HasEffect(DNC.Buffs.SilkenSymmetry)))
                    return DNC.ReverseCascade;
            }
        }

        if (actionID == DNC.Fountain)
        {
            if (IsEnabled(CustomComboPreset.DancerSingleTargetProcs))
            {
                if (CanUseAction(DNC.Fountainfall) && (HasEffect(DNC.Buffs.FlourishingFlow) || HasEffect(DNC.Buffs.SilkenFlow)))
                    return DNC.Fountainfall;
            }
        }

        return actionID;
    }
}

internal class DancerWindmillBladeshower : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DncAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == DNC.Windmill)
        {
            var gauge = GetJobGauge<DNCGauge>();

            if (IsEnabled(CustomComboPreset.EvilDancerFanDanceCombo))
            {
                if (CanUseAction(DNC.FanDance4) && HasEffect(DNC.Buffs.FourfoldFanDance))
                    return DNC.FanDance4;

                if (gauge.Feathers >= 1)
                {
                    if (CanUseAction(DNC.FanDance2))
                        return DNC.FanDance2;
                }
            }

            if (IsEnabled(CustomComboPreset.DancerAoeMultibutton))
            {
                if (CanUseAction(DNC.Bloodshower) && (HasEffect(DNC.Buffs.FlourishingFlow) || HasEffect(DNC.Buffs.SilkenFlow)))
                    return DNC.Bloodshower;

                if (CanUseAction(DNC.RisingWindmill) && (HasEffect(DNC.Buffs.FlourishingSymmetry) || HasEffect(DNC.Buffs.SilkenSymmetry)))
                    return DNC.RisingWindmill;

                if (lastComboMove == DNC.Windmill && CanUseAction(DNC.Bladeshower))
                    return DNC.Bladeshower;
            }

            if (IsEnabled(CustomComboPreset.DancerAoeProcs))
            {
                if (CanUseAction(DNC.RisingWindmill) && (HasEffect(DNC.Buffs.FlourishingSymmetry) || HasEffect(DNC.Buffs.SilkenSymmetry)))
                    return DNC.RisingWindmill;
            }
        }

        if (actionID == DNC.Bladeshower)
        {
            if (IsEnabled(CustomComboPreset.DancerAoeProcs))
            {
                if (CanUseAction(DNC.Bloodshower) && (HasEffect(DNC.Buffs.FlourishingFlow) || HasEffect(DNC.Buffs.SilkenFlow)))
                    return DNC.Bloodshower;
            }
        }

        return actionID;
    }
}

internal class DancerDevilment : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerDevilmentFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == DNC.Devilment)
        {
            if (CanUseAction(DNC.StarfallDance) && HasEffect(DNC.Buffs.FlourishingStarfall))
                return DNC.StarfallDance;
        }

        return actionID;
    }
}

internal class DancerLastDanceFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerLastDanceFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == DNC.StandardStep)
        {
            if (level >= DNC.Levels.LastDance)
            {
                if (IsEnabled(CustomComboPreset.DancerFinishingMovePriorityFeature) && HasEffect(DNC.Buffs.FinishingMoveReady) && level >= DNC.Levels.FinishingMove)
                {
                    return DNC.FinishingMove;
                }

                return DNC.LastDance;
            }
        }

        return actionID;
    }
}