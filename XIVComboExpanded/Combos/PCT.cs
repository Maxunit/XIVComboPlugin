using System;

using Dalamud.Game.ClientState.JobGauge.Types;
using FFXIVClientStructs.FFXIV.Client.Game.Gauge;

namespace XIVComboExpandedPlugin.Combos;

internal static class PCT
{
    public const byte JobID = 42;

    public const uint
        FireRed = 34650,
        AeroGreen = 34651,
        WaterBlue = 34652,
        BlizzardCyan = 34653,
        EarthYellow = 34654,
        ThunderMagenta = 34655,
        ExtraFireRed = 34656,
        ExtraAeroGreen = 34657,
        ExtraWaterBlue = 34658,
        ExtraBlizzardCyan = 34659,
        ExtraEarthYellow = 34660,
        ExtraThunderMagenta = 34661,
        MiracleWhite = 34662,
        CometBlack = 34663,
        PomMotif = 34664,
        WingMotif = 34665,
        ClawMotif = 34666,
        MawMotif = 34667,
        HammerMotif = 34668,
        StarrySkyMotif = 34669,
        PomMuse = 34670,
        WingedMuse = 34671,
        ClawedMuse = 34672,
        FangedMuse = 34673,
        StrikingMuse = 34674,
        StarryMuse = 34675,
        MogOftheAges = 34676,
        Retribution = 34677,
        HammerStamp = 34678,
        HammerBrush = 34679,
        PolishingHammer = 34680,
        StarPrism1 = 34681,
        StarPrism2 = 34682,
        SubstractivePalette = 34683,
        Smudge = 34684,
        TemperaCoat = 34685,
        TemperaGrassa = 34686,
        RainbowDrip = 34688,
        CreatureMotif = 34689,
        WeaponMotif = 34690,
        LandscapeMotif = 34691,
        AnimalMotif2 = 35347,
        WeaponMotif2 = 35348,
        LandscapeMotif2 = 35349;

    public static class Buffs
    {
        public const ushort
            SubstractivePalette = 3674,
            Chroma2Ready = 3675,
            Chroma3Ready = 3676,
            RainbowReady = 3679,
            HammerReady = 3680,
            StarPrismReady = 3681,
            Installation = 3688,
            ArtisticInstallation = 3689,
            SubstractivePaletteReady = 3690,
            InvertedColors = 3691;
    }

    public static class Debuffs
    {
        public const ushort
            Placeholder = 0;
    }

    public static class Levels
    {
        public const byte
            FireRed = 1,
            AeroGreen = 5,
            TemperaCoat = 10,
            WaterBlue = 15,
            Smudge = 20,
            ExtraFireRed = 25,
            CreatureMotif = 30,
            PomMotif = 30,
            WingMotif = 30,
            PomMuse = 30,
            WingedMuse = 30,
            MogOftheAges = 30,
            ExtraAeroGreen = 35,
            ExtraWaterBlue = 45,
            HammerMotif = 50,
            HammerStamp = 50,
            WeaponMotif = 50,
            StrikingMuse = 50,
            SubstractivePalette = 60,
            BlizzardCyan = 60,
            EarthYellow = 60,
            ThunderMagenta = 60,
            ExtraBlizzardCyan = 60,
            ExtraEarthYellow = 60,
            ExtraThunderMagenta = 60,
            StarrySkyMotif = 70,
            LandscapeMotif = 70,
            MiracleWhite = 80,
            HammerBrush = 86,
            PolishingHammer = 86,
            TemperaGrassa = 88,
            CometBlack = 90,
            RainbowDrip = 92,
            ClawMotif = 96,
            MawMotif = 96,
            ClawedMuse = 96,
            FangedMuse = 96,
            StarryMuse = 70,
            Retribution = 96,
            StarPrism1 = 100,
            StarPrism2 = 100;
    }

    internal class PictomancerHolyCometCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PictomancerHolyCometCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == PCT.MiracleWhite && HasEffect(PCT.Buffs.InvertedColors))
                return PCT.CometBlack;

            return actionID;
        }
    }

    /* //internal class PictomancerHolyAutoCombo : CustomCombo
    //{
    //    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PictomancerHolyAutoCombo;

    //    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    //    {
    //        var gauge = GetJobGauge<PCTGauge>();
    //        if (actionID == PCT.FireRed || actionID == PCT.ExtraFireRed)
    //        {
    //            if (gauge.Paint.Count() == 5)
    //            {
    //                {
    //                    if (gauge.Paint.Black > 0) return PCT.CometBlack;
    //                    else return PCT.MiracleWhite;
    //                }
    //            }
    //        }

    //        return actionID;
    //    }
    //} */

    internal class PictomancerSubtractiveSTCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PictomancerSubtractiveSTCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (!HasEffect(PCT.Buffs.SubstractivePalette))
            {
                if (actionID == PCT.BlizzardCyan)
                {
                    if (HasEffect(PCT.Buffs.Chroma2Ready))
                    {
                        return PCT.AeroGreen;
                    }
                    else if (HasEffect(PCT.Buffs.Chroma3Ready))
                    {
                        return PCT.WaterBlue;
                    }

                    return PCT.FireRed;
                }
            }

            return actionID;
        }
    }

    internal class PictomancerSubtractiveAoECombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PictomancerSubtractiveAoECombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (!HasEffect(PCT.Buffs.SubstractivePalette))
            {
                if (actionID == PCT.ExtraBlizzardCyan)
                {
                    if (HasEffect(PCT.Buffs.Chroma2Ready))
                    {
                        return PCT.ExtraAeroGreen;
                    }
                    else if (HasEffect(PCT.Buffs.Chroma3Ready))
                    {
                        return PCT.ExtraWaterBlue;
                    }

                    return PCT.ExtraFireRed;
                }
            }

            return actionID;
        }
    }

    /* //internal class PictomancerSubtractiveAutoCombo : CustomCombo
    //{
    //    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PictomancerSubtractiveAutoCombo;

    //    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    //    {
    //        var gauge = GetJobGauge<PCTGauge>();
    //        if (actionID == PCT.FireRed || actionID == PCT.ExtraFireRed)
    //        {
    //            if (HasEffect(PCT.Buffs.Chroma3Ready) && !(HasEffect(PCT.Buffs.SubstractivePalette)) && gauge.PalleteGauge == 100)
    //            {
    //                return PCT.SubstractivePalette;
    //            }
    //        }

    //        return actionID;
    //    }
    //}

    //internal class PictomancerCreatureMotifCombo : CustomCombo
    //{
    //    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PictomancerCreatureMotifCombo;

    //    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    //    {
    //        return actionID;
    //    }
    //}

    //internal class PictomancerCreatureMogCombo : CustomCombo
    //{
    //    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PictomancerCreatureMogCombo;

    //    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    //    {
    //        return actionID;
    //    }
    //}

    //internal class PictomancerWeaponMotifCombo : CustomCombo
    //{
    //    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PictomancerWeaponMotifCombo;

    //    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    //    {
    //        return actionID;
    //    }
    //} */

    internal class PictomancerWeaponHammerCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PictomancerWeaponHammerCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == PCT.WeaponMotif || actionID == PCT.WeaponMotif2)
            {
                if (HasEffect(PCT.Buffs.HammerReady))
                {
                    return OriginalHook(PCT.HammerStamp);
                }
            }

            return actionID;
        }
    }

    /* //internal class PictomancerLandscapeMotifCombo : CustomCombo
    //{
    //    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PictomancerLandscapeMotifCombo;

    //    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    //    {
    //        return actionID;
    //    }
    //}

    //internal class PictomancerLandscapePrismCombo : CustomCombo
    //{
    //    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PictomancerLandscapePrismCombo;

    //    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    //    {
    //        return actionID;
    //    }
    //} */
}