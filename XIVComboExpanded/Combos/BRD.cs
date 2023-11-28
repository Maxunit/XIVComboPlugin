using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class BRD
{
    public const byte ClassID = 5;
    public const byte JobID = 23;

    public const uint
        HeavyShot = 97,
        StraightShot = 98,
        VenomousBite = 100,
        RagingStrikes = 101,
        QuickNock = 106,
        Barrage = 107,
        Bloodletter = 110,
        Windbite = 113,
        MagesBallad = 114,
        ArmysPaeon = 116,
        RainOfDeath = 117,
        BattleVoice = 118,
        EmpyrealArrow = 3558,
        WanderersMinuet = 3559,
        IronJaws = 3560,
        Sidewinder = 3562,
        PitchPerfect = 7404,
        CausticBite = 7406,
        Stormbite = 7407,
        RefulgentArrow = 7409,
        Peloton = 7557,
        BurstShot = 16495,
        ApexArrow = 16496,
        Shadowbite = 16494,
        Ladonsbite = 25783,
        BlastArrow = 25784,
        RadiantFinale = 25785;

    public static class Buffs
    {
        public const ushort
            StraightShotReady = 122,
            WanderersMinuet = 2009,
            BlastArrowReady = 2692,
            ShadowbiteReady = 3002;
    }

    public static class Debuffs
    {
        public const ushort
            VenomousBite = 124,
            Windbite = 129,
            CausticBite = 1200,
            Stormbite = 1201;
    }

    public static class Levels
    {
        public const byte
            StraightShot = 2,
            RagingStrikes = 4,
            VenomousBite = 6,
            Bloodletter = 12,
            MagesBallad = 30,
            Windbite = 30,
            Barrage = 38,
            ArmysPaeon = 40,
            RainOfDeath = 45,
            BattleVoice = 50,
            WanderersMinuet = 52,
            PitchPerfect = 52,
            EmpyrealArrow = 54,
            IronJaws = 56,
            Sidewinder = 60,
            BiteMastery = 64,
            RefulgentArrow = 70,
            Shadowbite = 72,
            BurstShot = 76,
            ApexArrow = 80,
            Ladonsbite = 82,
            BlastArrow = 86,
            RadiantFinale = 90;
    }
}

internal class BardHeavyShot : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BrdAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BRD.HeavyShot || actionID == BRD.BurstShot)
        {
            if (IsEnabled(CustomComboPreset.BasicBiteCombo))
            {
                var venomous = FindTargetEffect(BRD.Debuffs.VenomousBite);
                var windbite = FindTargetEffect(BRD.Debuffs.Windbite);

                if (CanUseAction(BRD.Windbite) && (HasTarget() || HasSoftTarget()) && (windbite is null || windbite.RemainingTime <= 15))
                    return BRD.Windbite;

                if (CanUseAction(BRD.VenomousBite) && (HasTarget() || HasSoftTarget()) && (venomous is null || venomous.RemainingTime <= 15))
                    return BRD.VenomousBite;
            }

            if (IsEnabled(CustomComboPreset.BardBloodletterUpgradeFeature))
            {
                if (CanUseAction(BRD.Bloodletter) && HasCharges(BRD.Bloodletter))
                    return BRD.Bloodletter;
            }

            if (IsEnabled(CustomComboPreset.BardApexFeature))
            {
                var gauge = GetJobGauge<BRDGauge>();

                if (CanUseAction(BRD.BlastArrow) && HasEffect(BRD.Buffs.BlastArrowReady))
                    return BRD.BlastArrow;

                if (CanUseAction(BRD.ApexArrow) && gauge.SoulVoice == 100)
                    return BRD.ApexArrow;
            }

            if (IsEnabled(CustomComboPreset.BardStraightShotUpgradeFeature))
            {
                if (CanUseAction(BRD.StraightShot) && HasEffect(BRD.Buffs.StraightShotReady))
                    // Refulgent Arrow
                    return OriginalHook(BRD.StraightShot);
            }
        }

        return actionID;
    }
}

internal class BardIronJaws : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BrdAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BRD.IronJaws)
        {
            if (IsEnabled(CustomComboPreset.BardPreIronJawsFeature))
            {
                if (!CanUseAction(BRD.Windbite))
                    return BRD.VenomousBite;

                if (!CanUseAction(BRD.IronJaws))
                {
                    var venomous = FindTargetEffect(BRD.Debuffs.VenomousBite);
                    var windbite = FindTargetEffect(BRD.Debuffs.Windbite);

                    if (venomous is null)
                        return BRD.VenomousBite;

                    if (windbite is null)
                        return BRD.Windbite;

                    if (venomous?.RemainingTime < windbite?.RemainingTime)
                        return BRD.VenomousBite;

                    return BRD.Windbite;
                }
            }

            if (IsEnabled(CustomComboPreset.BardIronJawsFeature))
            {
                if (level < BRD.Levels.BiteMastery)
                {
                    var venomous = TargetHasEffect(BRD.Debuffs.VenomousBite);
                    var windbite = TargetHasEffect(BRD.Debuffs.Windbite);

                    if (venomous && windbite)
                        return BRD.IronJaws;

                    if (windbite)
                        return BRD.VenomousBite;

                    return BRD.Windbite;
                }

                var caustic = TargetHasEffect(BRD.Debuffs.CausticBite);
                var stormbite = TargetHasEffect(BRD.Debuffs.Stormbite);

                if (caustic && stormbite)
                    return BRD.IronJaws;

                if (stormbite)
                    return BRD.CausticBite;

                return BRD.Stormbite;
            }
        }

        return actionID;
    }
}

internal class BardQuickNock : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BrdAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BRD.QuickNock || actionID == BRD.Ladonsbite)
        {
            if (IsEnabled(CustomComboPreset.BardApexFeature))
            {
                var gauge = GetJobGauge<BRDGauge>();

                if (CanUseAction(BRD.ApexArrow) && gauge.SoulVoice == 100)
                    return BRD.ApexArrow;

                if (CanUseAction(BRD.BlastArrow) && HasEffect(BRD.Buffs.BlastArrowReady))
                    return BRD.BlastArrow;
            }

            if (IsEnabled(CustomComboPreset.BardShadowbiteFeature))
            {
                if (CanUseAction(BRD.Shadowbite) && HasEffect(BRD.Buffs.ShadowbiteReady))
                {
                    if (IsEnabled(CustomComboPreset.BardShadowbiteBarrageFeature))
                    {
                        if (CanUseAction(BRD.Barrage) && IsOffCooldown(BRD.Barrage))
                            return BRD.Barrage;
                    }

                    return BRD.Shadowbite;
                }
            }
        }

        return actionID;
    }
}

internal class BardBloodletter : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BrdAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BRD.Bloodletter)
        {
            var gauge = GetJobGauge<BRDGauge>();

            if (IsEnabled(CustomComboPreset.BardExpiringPerfectBloodletterFeature))
            {
                if (CanUseAction(BRD.PitchPerfect) && gauge.Song == Song.WANDERER && gauge.Repertoire >= 1)
                {
                    if (gauge.SongTimer <= 2500)
                        return BRD.PitchPerfect;
                }
            }

            if (IsEnabled(CustomComboPreset.BardPerfectBloodletterFeature))
            {
                if (CanUseAction(BRD.PitchPerfect) && gauge.Song == Song.WANDERER && gauge.Repertoire == 3)
                    return BRD.PitchPerfect;
            }

            if (IsEnabled(CustomComboPreset.BardBloodletterFeature))
            {
                if (CanUseAction(BRD.Sidewinder))
                    return CalcBestAction(actionID, BRD.Bloodletter, BRD.EmpyrealArrow, BRD.Sidewinder);

                if (CanUseAction(BRD.EmpyrealArrow))
                    return CalcBestAction(actionID, BRD.Bloodletter, BRD.EmpyrealArrow);

                if (CanUseAction(BRD.Bloodletter))
                    return BRD.Bloodletter;
            }

            if (IsEnabled(CustomComboPreset.BardBloodRainFeature))
            {
                if (CanUseAction(BRD.RainOfDeath)
                    && !TargetHasEffect(BRD.Debuffs.CausticBite)
                    && !TargetHasEffect(BRD.Debuffs.Stormbite)
                    && !TargetHasEffect(BRD.Debuffs.Windbite)
                    && !TargetHasEffect(BRD.Debuffs.VenomousBite))
                {
                    return BRD.RainOfDeath;
                }
            }
        }

        return actionID;
    }
}

internal class BardRainOfDeath : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BrdAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BRD.RainOfDeath)
        {
            var gauge = GetJobGauge<BRDGauge>();

            if (IsEnabled(CustomComboPreset.BardExpiringPerfectRainOfDeathFeature))
            {
                if (CanUseAction(BRD.PitchPerfect) && gauge.Song == Song.WANDERER && gauge.Repertoire >= 1)
                {
                    if (gauge.SongTimer <= 2500)
                        return BRD.PitchPerfect;
                }
            }

            if (IsEnabled(CustomComboPreset.BardPerfectRainOfDeathFeature))
            {
                if (CanUseAction(BRD.PitchPerfect) && gauge.Song == Song.WANDERER && gauge.Repertoire == 3)
                    return BRD.PitchPerfect;
            }

            if (IsEnabled(CustomComboPreset.BardRainOfDeathFeature))
            {
                if (CanUseAction(BRD.Sidewinder))
                    return CalcBestAction(actionID, BRD.RainOfDeath, BRD.EmpyrealArrow, BRD.Sidewinder);

                if (CanUseAction(BRD.EmpyrealArrow))
                    return CalcBestAction(actionID, BRD.RainOfDeath, BRD.EmpyrealArrow);

                if (CanUseAction(BRD.RainOfDeath))
                    return BRD.RainOfDeath;
            }
        }

        return actionID;
    }
}

internal class BardSidewinder : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardSidewinderFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BRD.Sidewinder)
        {
            if (CanUseAction(BRD.Sidewinder))
                return CalcBestAction(actionID, BRD.EmpyrealArrow, BRD.Sidewinder);
        }

        return actionID;
    }
}

internal class BardEmpyrealArrow : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardEmpyrealArrowFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BRD.EmpyrealArrow)
        {
            if (CanUseAction(BRD.Sidewinder))
                return CalcBestAction(actionID, BRD.EmpyrealArrow, BRD.Sidewinder);
        }

        return actionID;
    }
}

internal class BardBarrage : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardBarrageFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BRD.Barrage)
        {
            if (CanUseAction(BRD.StraightShot) && HasEffect(BRD.Buffs.StraightShotReady) && !HasEffect(BRD.Buffs.ShadowbiteReady))
                // Refulgent Arrow
                return OriginalHook(BRD.StraightShot);
        }

        return actionID;
    }
}

internal class BardRadiantFinale : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BrdAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BRD.RadiantFinale)
        {
            if (IsEnabled(CustomComboPreset.BardRadiantStrikesFeature))
            {
                if (CanUseAction(BRD.RagingStrikes) && IsOffCooldown(BRD.RagingStrikes))
                    return BRD.RagingStrikes;
            }

            if (IsEnabled(CustomComboPreset.BardRadiantVoiceFeature))
            {
                if (CanUseAction(BRD.BattleVoice) && IsOffCooldown(BRD.BattleVoice))
                    return BRD.BattleVoice;
            }

            if (IsEnabled(CustomComboPreset.BardRadiantStrikesFeature))
            {
                if (!CanUseAction(BRD.RadiantFinale))
                    return BRD.RagingStrikes;
            }

            if (IsEnabled(CustomComboPreset.BardRadiantVoiceFeature))
            {
                if (!CanUseAction(BRD.RadiantFinale))
                    return BRD.BattleVoice;
            }
        }

        return actionID;
    }
}

internal class BardPeloton : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardWanderersPitchPerfectFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BRD.Peloton)
        {
            var gauge = GetJobGauge<BRDGauge>();

            if (CanUseAction(BRD.PitchPerfect) && gauge.Song == Song.WANDERER && HasTarget())
                return BRD.WanderersMinuet;

            return BRD.Peloton;
        }

        return actionID;
    }
}

internal class BardDoTCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardDoTCombo;

    protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
    {
        if (actionID == BRD.VenomousBite)
        {
            var venomous = FindTargetEffect(BRD.Debuffs.VenomousBite);
            var windbite = FindTargetEffect(BRD.Debuffs.Windbite);

            if (CanUseAction(BRD.Windbite) && (windbite is null || windbite.RemainingTime <= 15))
                return BRD.Windbite;

            if (CanUseAction(BRD.VenomousBite) && (venomous is null || venomous.RemainingTime <= 15))
                return BRD.VenomousBite;
        }

        return actionID;
    }
}

internal class BardMagesBallad : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardCyclingSongFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BRD.MagesBallad)
        {
            const ushort remaining = 40000;
            var gauge = GetJobGauge<BRDGauge>();

            if (CanUseAction(BRD.WanderersMinuet))
            {
                if (gauge.Song == Song.WANDERER && gauge.SongTimer >= remaining)
                    return BRD.WanderersMinuet;

                if (IsOffCooldown(BRD.WanderersMinuet))
                    return BRD.WanderersMinuet;
            }

            if (CanUseAction(BRD.MagesBallad))
            {
                if (gauge.Song == Song.MAGE && gauge.SongTimer >= remaining)
                    return BRD.MagesBallad;

                if (IsOffCooldown(BRD.MagesBallad))
                    return BRD.MagesBallad;
            }

            if (CanUseAction(BRD.ArmysPaeon))
            {
                if (gauge.Song == Song.ARMY && gauge.SongTimer >= remaining)
                    return BRD.ArmysPaeon;

                if (IsOffCooldown(BRD.ArmysPaeon))
                    return BRD.ArmysPaeon;
            }

            // Show the next expected song while on cooldown
            if (CanUseAction(BRD.WanderersMinuet))
                return BRD.WanderersMinuet;

            if (CanUseAction(BRD.MagesBallad))
                return BRD.MagesBallad;

            if (CanUseAction(BRD.ArmysPaeon))
                return BRD.ArmysPaeon;
        }

        return actionID;
    }
}