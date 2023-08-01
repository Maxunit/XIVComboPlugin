using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class PLD
{
    public const byte ClassID = 1;
    public const byte JobID = 19;

    public const uint
        FastBlade = 9,
        RiotBlade = 15,
        ShieldBash = 16,
        FightOrFlight = 20,
        RageOfHalone = 21,
        Bulwark = 22,
        CircleOfScorn = 23,
        SpiritsWithin = 29,
        Sheltron = 3542,
        GoringBlade = 3538,
        RoyalAuthority = 3539,
        Clemency = 3541,
        TotalEclipse = 7381,
        Requiescat = 7383,
        HolySpirit = 7384,
        Prominence = 16457,
        HolyCircle = 16458,
        Confiteor = 16459,
        Atonement = 16460,
        HolySheltron = 25746,
        Expiacion = 25747,
        BladeOfFaith = 25748,
        BladeOfTruth = 25749,
        BladeOfValor = 25750,
        Intervene = 16461;

    public static class Buffs
    {
        public const ushort
            FightOrFlight = 76,
            Requiescat = 1368,
            SwordOath = 1902,
            DivineMight = 2673,
            ConfiteorReady = 3019;
    }

    public static class Debuffs
    {
        public const ushort
            Placeholder = 0;
    }

    public static class Levels
    {
        public const byte
            FightOrFlight = 2,
            RiotBlade = 4,
            SpiritsWithin = 30,
            Sheltron = 35,
            CircleOfScorn = 50,
            RageOfHalone = 26,
            Prominence = 40,
            Bulwark = 52,
            GoringBlade = 54,
            RoyalAuthority = 60,
            HolySpirit = 64,
            DivineMagicMastery = 64,
            Requiescat = 68,
            HolyCircle = 72,
            Intervene = 74,
            Atonement = 76,
            Confiteor = 80,
            HolySheltron = 82,
            Expiacion = 86,
            BladeOfFaith = 90,
            BladeOfTruth = 90,
            BladeOfValor = 90;
    }
}

internal abstract class PaladinCombo : CustomCombo
{
    protected bool HasMp(uint spell)
    {
        int cost;
        switch (spell)
        {
            case PLD.Clemency:
                cost = 4000;
                break;
            case PLD.HolySpirit:
            case PLD.HolyCircle:
            case PLD.Confiteor:
            case PLD.BladeOfFaith:
            case PLD.BladeOfTruth:
            case PLD.BladeOfValor:
                cost = 2000;
                break;
            default:
                cost = 0;
                break;
        }

        if (LocalPlayer?.Level >= PLD.Levels.DivineMagicMastery)
            cost /= 2;

        return LocalPlayer?.CurrentMp >= cost;
    }
}

internal class PaladinRoyalAuthority : PaladinCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PldAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == PLD.RageOfHalone || actionID == PLD.RoyalAuthority)
        {
            // During FoF, prioritize the higher-potency Divine Might cast over Atonement and the normal combo chain
            if (IsEnabled(CustomComboPreset.PaladinRoyalAuthorityFightOrFlightFeature))
            {
                if (HasEffect(PLD.Buffs.FightOrFlight) && HasEffect(PLD.Buffs.DivineMight) && HasMp(PLD.HolySpirit))
                    return PLD.HolySpirit;
            }

            if (IsEnabled(CustomComboPreset.PaladinRoyalAuthorityAtonementFeature))
            {
                if (level >= PLD.Levels.Atonement && lastComboMove != PLD.FastBlade && lastComboMove != PLD.RiotBlade && HasEffect(PLD.Buffs.SwordOath))
                    return PLD.Atonement;
            }

            if (IsEnabled(CustomComboPreset.PaladinRoyalAuthorityCombo))
            {
                if (comboTime > 0)
                {
                    if (lastComboMove == PLD.RiotBlade && level >= PLD.Levels.RageOfHalone)
                    {
                        if (IsEnabled(CustomComboPreset.PaladinRoyalAuthorityDivineMightFeature))
                        {
                            if (level >= PLD.Levels.HolySpirit && HasEffect(PLD.Buffs.DivineMight) && HasMp(PLD.HolySpirit))
                                return PLD.HolySpirit;
                        }

                        // Royal Authority
                        return OriginalHook(PLD.RageOfHalone);
                    }

                    if (lastComboMove == PLD.FastBlade && level >= PLD.Levels.RiotBlade)
                        return PLD.RiotBlade;
                }

                return PLD.FastBlade;
            }
        }

        return actionID;
    }
}

internal class PaladinProminence : PaladinCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinProminenceCombo;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == PLD.Prominence)
        {
            // During FoF, prioritize the higher-potency Divine Might cast over Atonement and the normal combo chain
            if (IsEnabled(CustomComboPreset.PaladinProminenceDivineMightFeature))
            {
                if (HasEffect(PLD.Buffs.FightOrFlight) && HasEffect(PLD.Buffs.DivineMight) && HasMp(PLD.HolyCircle))
                    return PLD.HolyCircle;
            }

            if (IsEnabled(CustomComboPreset.PaladinProminenceDivineMightFeature) && level >= PLD.Levels.Prominence)
            {
                if (level >= PLD.Levels.HolyCircle && HasEffect(PLD.Buffs.DivineMight) && LocalPlayer?.CurrentMp > 1000)
                    return OriginalHook(PLD.HolyCircle);
            }

            if (comboTime > 0)
            {
                if (lastComboMove == PLD.TotalEclipse && level >= PLD.Levels.Prominence)
                {
                    if (IsEnabled(CustomComboPreset.PaladinProminenceDivineMightFeature))
                    {
                        if (level >= PLD.Levels.HolyCircle && HasEffect(PLD.Buffs.DivineMight) && HasMp(PLD.HolyCircle))
                            return PLD.HolyCircle;
                    }

                    return PLD.Prominence;
                }
            }

            return PLD.TotalEclipse;
        }

        return actionID;
    }
}

internal class PaladinHolySpiritHolyCircle : PaladinCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinConfiteorFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == PLD.HolySpirit || actionID == PLD.HolyCircle)
        {
            if (IsEnabled(CustomComboPreset.PaladinConfiteorFeature))
            {
                if (level >= PLD.Levels.Confiteor && LocalPlayer?.CurrentMp > 1000)
                {
                    var original = OriginalHook(PLD.Confiteor);
                    if (original != PLD.Confiteor)
                        return original;

                    if (HasEffect(PLD.Buffs.ConfiteorReady))
                        return OriginalHook(PLD.Confiteor);
                }
            }
        }

        return actionID;
    }
}

internal class PaladinFightOrFlight : PaladinCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PldAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == PLD.FightOrFlight)
        {
            if (IsEnabled(CustomComboPreset.PaladinFightOrFlightGoringBladeFeature))
            {
                if (level >= PLD.Levels.GoringBlade && HasEffect(PLD.Buffs.FightOrFlight) && IsOffCooldown(PLD.GoringBlade))
                    return PLD.GoringBlade;
            }
        }

        return actionID;
    }
}

internal class PaladinRequiescat : PaladinCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PldAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == PLD.Requiescat)
        {
            if (IsEnabled(CustomComboPreset.PaladinRequiescatCombo))
            {
                // Prioritize Goring Blade over the Confiteor combo.  While Goring Blade deals less damage (700p) than
                // most of the Confiteor combo (900p -> 700p -> 800p -> 900p), Goring Blade uniquely requires melee
                // range to cast, while the entire Confiteor combo chain does not.  Since Requiescat also requires
                // melee range to cast, the most reliable time that the player will be in melee range during the Req
                // buff is immediately following the usage of Req.  This minimizes potential losses and potential
                // cooldown drift if the player is forced out of melee range during the Confiteor combo and is unable
                // to return to melee range by the time it is completed.
                //
                // Since Goring Blade, the entire Confiteor combo, *and* one additional GCD (typically Holy Spirit) fits
                // within even the shortest of party buffs (15s ones like Battle Litany), this should not result in a
                // net reduction in potency, and *may* in fact increase it if someone is slightly late in applying
                // their party buffs, as it shifts the high-potency Confiteor cast back into the party buff window by a
                // single GCD.
                if (IsEnabled(CustomComboPreset.PaladinRequiescatFightOrFlightFeature) && IsEnabled(CustomComboPreset.PaladinFightOrFlightGoringBladeFeature))
                {
                    if (level >= PLD.Levels.GoringBlade && IsOffCooldown(PLD.GoringBlade))
                    {
                        if (IsOnCooldown(PLD.FightOrFlight) && (level < PLD.Levels.Requiescat || IsOnCooldown(PLD.Requiescat)))
                            return PLD.GoringBlade;
                    }
                }

                if (level >= PLD.Levels.Confiteor && LocalPlayer?.CurrentMp > 1000)
                {
                    // Blade combo
                    var original = OriginalHook(PLD.Confiteor);
                    if (original != PLD.Confiteor)
                        return original;

                    if (HasEffect(PLD.Buffs.ConfiteorReady))
                        return PLD.Confiteor;
                }

                if (level >= PLD.Levels.Requiescat && HasEffect(PLD.Buffs.Requiescat))
                    return PLD.HolySpirit;
            }

            if (IsEnabled(CustomComboPreset.PaladinRequiescatFightOrFlightFeature))
            {
                // Prefer FoF if it is off cooldown, or if it will be ready sooner than Requiescat.  In practice, this
                // means that Req should only be returned if FoF is on cooldown and Req is not, ie. immediately after
                // FoF is cast.  This ensures that the button shows the action that will next be available for use in
                // that hotbar slot, rather than swapping to FoF at the last instant when FoF comes off cooldown a
                // a single weave slot earlier than Req.
                if (level >= PLD.Levels.FightOrFlight)
                    return CalcBestAction(PLD.FightOrFlight, PLD.FightOrFlight, PLD.Requiescat);
            }
        }

        return actionID;
    }
}

internal class PaladinSpiritsWithinCircleOfScorn : PaladinCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinScornfulSpiritsFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == PLD.SpiritsWithin || actionID == PLD.Expiacion || actionID == PLD.CircleOfScorn)
        {
            if (level >= PLD.Levels.Expiacion)
                return CalcBestAction(actionID, PLD.Expiacion, PLD.CircleOfScorn);

            if (level >= PLD.Levels.CircleOfScorn)
                return CalcBestAction(actionID, PLD.SpiritsWithin, PLD.CircleOfScorn);

            return PLD.SpiritsWithin;
        }

        return actionID;
    }
}

internal class PaladinShieldBash : PaladinCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AdvAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == PLD.ShieldBash)
        {
            if (IsEnabled(CustomComboPreset.PaladinShieldBashFeature) && level >= ADV.Levels.LowBlow && IsOffCooldown(ADV.LowBlow))
                return OriginalHook(ADV.LowBlow);
        }

        return actionID;
    }
}

internal class PaladinFastBladeSingleCombo : PaladinCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PldAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == PLD.FastBlade)
        {
            var gauge = GetJobGauge<PLDGauge>();

            if (IsEnabled(CustomComboPreset.FastBladeInterveneFeature) && level >= PLD.Levels.Intervene && !InMeleeRange && HasCharges(PLD.Intervene) && !HasEffect(PLD.Buffs.ConfiteorReady) && !HasEffect(PLD.Buffs.Requiescat) && !HasEffect(PLD.Buffs.DivineMight))
                return OriginalHook(PLD.Intervene);

            if (IsEnabled(CustomComboPreset.PaladinSheltronSpender) && InCombat())
            {
                if (level >= PLD.Levels.Sheltron && level <= PLD.Levels.HolySheltron && gauge.OathGauge >= 99)
                    return OriginalHook(PLD.Sheltron);

                if (level >= PLD.Levels.HolySheltron && gauge.OathGauge >= 99)
                    return OriginalHook(PLD.HolySheltron);
            }

            if (IsEnabled(CustomComboPreset.PaladinFastBladeSingleCombo))
            {
                if (level >= PLD.Levels.Confiteor && HasMp(PLD.Confiteor))
                {
                    var original = OriginalHook(PLD.Confiteor);
                    if (original != PLD.Confiteor)
                        return original;

                    if (HasEffect(PLD.Buffs.ConfiteorReady))
                        return OriginalHook(PLD.Confiteor);
                }

                if (level >= PLD.Levels.HolySpirit && HasEffect(PLD.Buffs.DivineMight) && HasMp(PLD.HolySpirit))
                    return OriginalHook(PLD.HolySpirit);

                if (IsEnabled(CustomComboPreset.PaladinRoyalAuthorityAtonementFeature))
                {
                    if (level >= PLD.Levels.Atonement && HasEffect(PLD.Buffs.SwordOath) && lastComboMove != PLD.FastBlade && lastComboMove != PLD.RiotBlade)
                        return PLD.Atonement;
                }

                if (comboTime > 0)
                {
                    if (lastComboMove == PLD.RiotBlade && level >= PLD.Levels.RageOfHalone)
                        return OriginalHook(PLD.RageOfHalone);

                    if (lastComboMove == PLD.FastBlade && level >= PLD.Levels.RiotBlade)
                        return PLD.RiotBlade;
                }

                return PLD.FastBlade;
            }
        }

        return actionID;
    }
}

internal class PaladinScornfulSpiritsExtended : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinScornfulSpiritsExtended;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == PLD.FightOrFlight)
        {
            if (level >= PLD.Levels.Expiacion && IsOnCooldown(PLD.FightOrFlight))
                return CalcBestAction(actionID, PLD.Expiacion, PLD.Requiescat, PLD.GoringBlade, PLD.CircleOfScorn, PLD.SpiritsWithin, PLD.FightOrFlight);

            if (level >= PLD.Levels.Requiescat && level <= PLD.Levels.Expiacion && IsOnCooldown(PLD.FightOrFlight))
                return CalcBestAction(actionID, PLD.Requiescat, PLD.GoringBlade, PLD.CircleOfScorn, PLD.SpiritsWithin, PLD.FightOrFlight);

            if (level >= PLD.Levels.GoringBlade && level <= PLD.Levels.Requiescat && IsOnCooldown(PLD.FightOrFlight))
                return CalcBestAction(actionID, PLD.GoringBlade, PLD.CircleOfScorn, PLD.SpiritsWithin, PLD.FightOrFlight);

            if (level >= PLD.Levels.CircleOfScorn && level <= PLD.Levels.GoringBlade && IsOnCooldown(PLD.FightOrFlight))
                return CalcBestAction(actionID, PLD.CircleOfScorn, PLD.SpiritsWithin, PLD.FightOrFlight);

            if (level >= PLD.Levels.SpiritsWithin && level <= PLD.Levels.CircleOfScorn && IsOnCooldown(PLD.FightOrFlight))
                return CalcBestAction(actionID, PLD.SpiritsWithin, PLD.FightOrFlight);
        }

        return actionID;
    }
}

internal class PaladinBulwarkSheltronFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinBulwarkSheltronFeature;

    protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
    {
        var gauge = GetJobGauge<PLDGauge>();

        if (actionID == PLD.Sheltron)
        {
            if (level >= PLD.Levels.Bulwark && (IsOffCooldown(PLD.Bulwark) && gauge.OathGauge <= 50))
                return PLD.Bulwark;
        }

        return actionID;
    }
}