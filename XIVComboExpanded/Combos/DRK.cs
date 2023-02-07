using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class DRK
{
    public const byte JobID = 32;

    public const uint
        HardSlash = 3617,
        Unleash = 3621,
        SyphonStrike = 3623,
        Souleater = 3632,
        BloodWeapon = 3625,
        SaltedEarth = 3639,
        AbyssalDrain = 3641,
        CarveAndSpit = 3643,
        Quietus = 7391,
        Bloodspiller = 7392,
        FloodOfDarkness = 16466,
        FloodOfShadow = 16469,
        EdgeOfDarkness = 16467,
        EdgeOfShadow = 16470,
        StalwartSoul = 16468,
        LivingShadow = 16472,
        SaltAndDarkness = 25755,
        Shadowbringer = 25757,
        Plunge = 3640;

    public static class Buffs
    {
        public const ushort
            BloodWeapon = 742,
            Darkside = 751,
            Delirium = 1972;
    }

    public static class Debuffs
    {
        public const ushort
            Placeholder = 0;
    }

    public static class Levels
    {
        public const byte
            SyphonStrike = 2,
            Souleater = 26,
            FloodOfDarkness = 30,
            BloodWeapon = 35,
            EdgeOfDarkness = 40,
            StalwartSoul = 40,
            SaltedEarth = 52,
            Plunge = 54,
            AbyssalDrain = 56,
            CarveAndSpit = 60,
            Bloodspiller = 62,
            Quietus = 64,
            Delirium = 68,
            EdgeOfShadow = 74,
            FloodOfShadow = 74,
            LivingShadow = 80,
            SaltAndDarkness = 86,
            Shadowbringer = 90;
    }
}

internal class DarkSouleater : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DrkAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == DRK.HardSlash)
        {
            var gauge = GetJobGauge<DRKGauge>();

            if (IsEnabled(CustomComboPreset.DarkEdgeSpender))
            {
                if (level >= DRK.Levels.EdgeOfShadow && gauge.DarksideTimeRemaining < 20000 && LocalPlayer?.CurrentMp > 3000)
                    return DRK.EdgeOfShadow;
                if (level >= DRK.Levels.EdgeOfDarkness && level <= DRK.Levels.EdgeOfShadow && gauge.DarksideTimeRemaining < 20000 && LocalPlayer?.CurrentMp > 3000)
                    return DRK.EdgeOfDarkness;
            }

            if (IsEnabled(CustomComboPreset.DarkBloodWeaponSpender))
            {
                if (level >= DRK.Levels.CarveAndSpit && IsOnCooldown(DRK.BloodWeapon) && IsOffCooldown(DRK.CarveAndSpit))
                    return DRK.CarveAndSpit;
            }

            if (IsEnabled(CustomComboPreset.DarkDeliriumFeature))
            {
                if (level >= DRK.Levels.Bloodspiller && level >= DRK.Levels.Delirium && HasEffect(DRK.Buffs.Delirium))
                    return DRK.Bloodspiller;
            }

            if (IsEnabled(CustomComboPreset.DarkSouleaterCombo))
            {
                if (IsEnabled(CustomComboPreset.DarkSouleaterOvercapFeature))
                {
                    if (level >= DRK.Levels.Bloodspiller && gauge.Blood > 90 && HasEffect(DRK.Buffs.BloodWeapon))
                        return DRK.Bloodspiller;
                }

                if (IsEnabled(CustomComboPreset.DarkSouleaterOvercapFeature))
                {
                    if (level >= DRK.Levels.Bloodspiller && (gauge.Blood > 80 || (gauge.Blood > 70 && HasEffect(DRK.Buffs.BloodWeapon))))
                        return DRK.Bloodspiller;
                }

                if (comboTime > 0)
                {
                    if (lastComboMove == DRK.SyphonStrike && level >= DRK.Levels.Souleater)
                        return DRK.Souleater;

                    if (lastComboMove == DRK.HardSlash && level >= DRK.Levels.SyphonStrike)
                        return DRK.SyphonStrike;
                }

                return DRK.HardSlash;
            }
        }

        return actionID;
    }
}

internal class DarkStalwartSoul : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DrkAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == DRK.Unleash)
        {
            var gauge = GetJobGauge<DRKGauge>();

            if (IsEnabled(CustomComboPreset.DarkFloodSpender))
            {
                if (level >= DRK.Levels.FloodOfShadow && gauge.DarksideTimeRemaining < 20000 && LocalPlayer?.CurrentMp > 3000)
                    return DRK.FloodOfShadow;
                if (level >= DRK.Levels.FloodOfDarkness && level <= DRK.Levels.FloodOfShadow && gauge.DarksideTimeRemaining < 20000 && LocalPlayer?.CurrentMp > 3000)
                    return DRK.FloodOfDarkness;
            }

            if (IsEnabled(CustomComboPreset.DarkBloodWeaponSpender))
            {
                if (level >= DRK.Levels.AbyssalDrain && IsOnCooldown(DRK.BloodWeapon) && IsOffCooldown(DRK.AbyssalDrain))
                    return DRK.AbyssalDrain;
            }

            if (IsEnabled(CustomComboPreset.DarkDeliriumFeature))
            {
                if (level >= DRK.Levels.Quietus && level >= DRK.Levels.Delirium && HasEffect(DRK.Buffs.Delirium))
                    return DRK.Quietus;
            }

            if (IsEnabled(CustomComboPreset.DarkStalwartSoulCombo))
            {
                if (IsEnabled(CustomComboPreset.DarkStalwartSoulOvercapFeature))
                {
                    if (level >= DRK.Levels.Quietus && gauge.Blood > 90 && HasEffect(DRK.Buffs.BloodWeapon))
                        return DRK.Quietus;
                }

                if (IsEnabled(CustomComboPreset.DarkStalwartSoulOvercapFeature))
                {
                    if (level >= DRK.Levels.Quietus && (gauge.Blood > 80 || (gauge.Blood > 70 && HasEffect(DRK.Buffs.BloodWeapon))))
                        return DRK.Quietus;
                }

                if (comboTime > 0)
                {
                    if (lastComboMove == DRK.Unleash && level >= DRK.Levels.StalwartSoul)
                    {
                        return DRK.StalwartSoul;
                    }
                }

                return DRK.Unleash;
            }
        }

        return actionID;
    }
}

internal class DarkCarveAndSpitAbyssalDrain : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DrkAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == DRK.CarveAndSpit || actionID == DRK.AbyssalDrain)
        {
            if (IsEnabled(CustomComboPreset.DarkBloodWeaponFeature))
            {
                if (actionID == DRK.AbyssalDrain && level < DRK.Levels.AbyssalDrain)
                    return DRK.BloodWeapon;

                if (actionID == DRK.CarveAndSpit && level < DRK.Levels.CarveAndSpit)
                    return DRK.BloodWeapon;

                if (level >= DRK.Levels.BloodWeapon && IsOffCooldown(DRK.BloodWeapon))
                    return DRK.BloodWeapon;
            }
        }

        return actionID;
    }
}

internal class DarkQuietusBloodspiller : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DrkAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == DRK.Quietus || actionID == DRK.Bloodspiller)
        {
            var gauge = GetJobGauge<DRKGauge>();

            if (IsEnabled(CustomComboPreset.DarkLivingShadowFeature))
            {
                if (level >= DRK.Levels.LivingShadow && gauge.Blood >= 50 && IsOffCooldown(DRK.LivingShadow))
                    return DRK.LivingShadow;
            }
        }

        return actionID;
    }
}

internal class DarkLivingShadow : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DrkAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == DRK.LivingShadow)
        {
            var gauge = GetJobGauge<DRKGauge>();

            if (IsEnabled(CustomComboPreset.DarkLivingShadowbringerFeature))
            {
                if (level >= DRK.Levels.Shadowbringer && gauge.ShadowTimeRemaining > 0 && HasCharges(DRK.Shadowbringer))
                    return DRK.Shadowbringer;
            }

            if (IsEnabled(CustomComboPreset.DarkLivingShadowbringerHpFeature))
            {
                if (level >= DRK.Levels.Shadowbringer && HasCharges(DRK.Shadowbringer) && IsOnCooldown(DRK.LivingShadow))
                    return DRK.Shadowbringer;
            }
        }

        return actionID;
    }
}