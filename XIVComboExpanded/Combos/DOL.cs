using Dalamud.Game.ClientState.Conditions;

namespace XIVComboExpandedPlugin.Combos;

internal static class DOL
{
    public const byte ClassID = 0;
    public const byte JobID = 51;

    public const uint
        Cast = 289,
        Hook = 296,
        CastLight = 2135,
        ThaliaksFavor = 26804,
        AgelessWords = 215,
        SolidReason = 232,
        Snagging = 4100,
        SurfaceSlap = 4595,
        Gig = 7632,
        VeteranTrade = 7906,
        NaturesBounty = 7909,
        Salvage = 7910,
        MinWiseToTheWorld = 26521,
        BtnWiseToTheWorld = 26522,
        ElectricCurrent = 26872,
        PioneersGift1 = 21178,
        PioneersGift2 = 25590,
        MountaineersGift1 = 21177,
        MountaineersGift2 = 25589,
        PrizeCatch = 26806;

    public static class Buffs
    {
        public const ushort
            EurekaMoment = 2765,
            GiftoftheLand = 2666,
            GiftoftheLand2 = 759,
            AnglersArt = 2778,
            PrizeCatch = 2780;
    }

    public static class Debuffs
    {
        public const ushort
            Placeholder = 0;
    }

    public static class Levels
    {
        public const byte
            Cast = 1,
            Hook = 1,
            PioneersGift1 = 15,
            MountaineersGift1 = 15,
            Snagging = 36,
            PioneersGift2 = 50,
            MountaineersGift2 = 50,
            Gig = 61,
            VeteranTrade = 63,
            Salvage = 67,
            NaturesBounty = 69,
            SurfaceSlap = 71,
            PrizeCatch = 81,
            WiseToTheWorld = 90;
    }
}

internal class MinerEurekaFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DolEurekaFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == DOL.SolidReason)
        {
            if (level >= DOL.Levels.WiseToTheWorld && HasEffect(DOL.Buffs.EurekaMoment))
                return DOL.MinWiseToTheWorld;
        }

        if (actionID == DOL.AgelessWords)
        {
            if (level >= DOL.Levels.WiseToTheWorld && HasEffect(DOL.Buffs.EurekaMoment))
                return DOL.BtnWiseToTheWorld;
        }

        return actionID;
    }
}

internal class FisherCast : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DolAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == DOL.Cast)
        {
            if (IsEnabled(CustomComboPreset.DolCastHookFeature))
            {
                if (HasCondition(ConditionFlag.Fishing))
                    return DOL.Hook;
            }

            if (IsEnabled(CustomComboPreset.DolCastGigFeature))
            {
                if (HasCondition(ConditionFlag.Diving))
                    return DOL.Gig;
            }
        }

        if (actionID == DOL.SurfaceSlap)
        {
            if (IsEnabled(CustomComboPreset.DolSurfaceTradeFeature))
            {
                if (HasCondition(ConditionFlag.Diving))
                    return DOL.VeteranTrade;
            }
        }

        if (actionID == DOL.PrizeCatch)
        {
            if (IsEnabled(CustomComboPreset.DolPrizeBountyFeature))
            {
                if (HasCondition(ConditionFlag.Diving))
                    return DOL.NaturesBounty;
            }
        }

        if (actionID == DOL.Snagging)
        {
            if (IsEnabled(CustomComboPreset.DolSnaggingSalvageFeature))
            {
                if (HasCondition(ConditionFlag.Diving))
                    return DOL.Salvage;
            }
        }

        if (actionID == DOL.CastLight)
        {
            if (IsEnabled(CustomComboPreset.DolCastLightElectricCurrentFeature))
            {
                if (HasCondition(ConditionFlag.Diving))
                    return DOL.ElectricCurrent;
            }
        }

        return actionID;
    }
}

internal class DolGiftoftheLand : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DolGiftoftheLand;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == DOL.PioneersGift1)
        {
            if (level >= DOL.Levels.PioneersGift2 && HasEffect(DOL.Buffs.GiftoftheLand))
                return DOL.PioneersGift2;

            return DOL.PioneersGift1;
        }

        if (actionID == DOL.MountaineersGift1)
        {
            if (level >= DOL.Levels.PioneersGift2 && HasEffect(DOL.Buffs.GiftoftheLand))
                return DOL.MountaineersGift2;

            return DOL.MountaineersGift1;
        }

        return actionID;
    }
}

internal class FisherPriceCatcher : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.Disabled;

    protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
    {
        if (actionID == DOL.Cast)
        {
            // Need to work something out
        }

        return actionID;
    }
}
