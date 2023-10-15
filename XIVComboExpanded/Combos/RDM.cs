using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class RDM
{
    public const byte JobID = 35;

    public const uint
        Jolt = 7503,
        Riposte = 7504,
        Verthunder = 7505,
        Veraero = 7507,
        Scatter = 7509,
        Verfire = 7510,
        Verstone = 7511,
        Zwerchhau = 7512,
        Moulinet = 7513,
        Redoublement = 7516,
        Fleche = 7517,
        Acceleration = 7518,
        ContreSixte = 7519,
        Embolden = 7520,
        Manafication = 7521,
        Verraise = 7523,
        Jolt2 = 7524,
        Verflare = 7525,
        Verholy = 7526,
        EnchantedRiposte = 7527,
        EnchantedZwerchhau = 7528,
        EnchantedRedoublement = 7529,
        Verthunder2 = 16524,
        Veraero2 = 16525,
        Impact = 16526,
        Scorch = 16530,
        Verthunder3 = 25855,
        Veraero3 = 25856,
        Resolution = 25858;

    public static class Buffs
    {
        public const ushort
            Swiftcast = 167,
            VerfireReady = 1234,
            VerstoneReady = 1235,
            Acceleration = 1238,
            Dualcast = 1249,
            LostChainspell = 2560;
    }

    public static class Debuffs
    {
        public const ushort
            Placeholder = 0;
    }

    public static class Levels
    {
        public const byte
            Jolt = 2,
            Verthunder = 4,
            Veraero = 10,
            Scatter = 15,
            Verfire = 26,
            Verstone = 30,
            Zwerchhau = 35,
            Fleche = 45,
            Redoublement = 50,
            Acceleration = 50,
            Vercure = 54,
            ContreSixte = 56,
            Embolden = 58,
            Manafication = 60,
            Jolt2 = 62,
            Verraise = 64,
            Impact = 66,
            Verflare = 68,
            Verholy = 70,
            Scorch = 80,
            Veraero3 = 82,
            Verthunder3 = 82,
            Resolution = 90;
    }
}

internal class RedMageVeraeroVerthunder : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RdmAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == RDM.Veraero || actionID == RDM.Veraero3 || actionID == RDM.Verthunder || actionID == RDM.Verthunder3)
        {
            var gauge = GetJobGauge<RDMGauge>();

            if (IsEnabled(CustomComboPreset.RedMageVeraeroVerthunderCapstoneCombo))
            {
                if (lastComboMove == RDM.Scorch && CanUseAction(RDM.Resolution))
                    return RDM.Resolution;

                if ((lastComboMove == RDM.Verflare || lastComboMove == RDM.Verholy) && CanUseAction(RDM.Scorch))
                    return RDM.Scorch;

                if (gauge.ManaStacks == 3)
                {
                    if (actionID == RDM.Verthunder || actionID == RDM.Verthunder3)
                    {
                        if (CanUseAction(RDM.Verflare))
                            return RDM.Verflare;
                    }

                    if (actionID == RDM.Veraero || actionID == RDM.Veraero3)
                    {
                        if (CanUseAction(RDM.Verholy))
                            return RDM.Verholy;

                        // From 68-70
                        if (CanUseAction(RDM.Verflare))
                            return RDM.Verflare;
                    }
                }
            }
        }

        return actionID;
    }
}

internal class RedMageVeraeroVerthunder2 : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RdmAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == RDM.Veraero2 || actionID == RDM.Verthunder2)
        {
            var gauge = GetJobGauge<RDMGauge>();

            if (IsEnabled(CustomComboPreset.RedMageAoECapstoneCombo))
            {
                if (lastComboMove == RDM.Scorch && CanUseAction(RDM.Resolution))
                    return RDM.Resolution;

                if ((lastComboMove == RDM.Verflare || lastComboMove == RDM.Verholy) && CanUseAction(RDM.Scorch))
                    return RDM.Scorch;

                // While it transforms natively, we need to include this due to the RedMageAoeFeature below.
                if (gauge.ManaStacks == 3)
                {
                    if (actionID == RDM.Verthunder2)
                    {
                        if (CanUseAction(RDM.Verflare))
                            return RDM.Verflare;
                    }

                    if (actionID == RDM.Veraero2)
                    {
                        if (CanUseAction(RDM.Verholy))
                            return RDM.Verholy;

                        // From 68-70
                        if (CanUseAction(RDM.Verflare))
                            return RDM.Verflare;
                    }
                }
            }

            if (IsEnabled(CustomComboPreset.RedMageAoEFeature))
            {
                if (CanUseAction(RDM.Scatter) && (HasEffect(RDM.Buffs.Dualcast) || HasEffect(RDM.Buffs.Acceleration) || HasEffect(RDM.Buffs.Swiftcast) || HasEffect(RDM.Buffs.LostChainspell)))
                    return OriginalHook(RDM.Scatter);
            }
        }

        return actionID;
    }
}

internal class RedMageRedoublementMoulinet : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RdmAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == RDM.Redoublement || actionID == RDM.Moulinet)
        {
            var gauge = GetJobGauge<RDMGauge>();

            if (IsEnabled(CustomComboPreset.RedMageMeleeCapstoneCombo))
            {
                if (lastComboMove == RDM.Scorch && CanUseAction(RDM.Resolution))
                    return RDM.Resolution;

                if ((lastComboMove == RDM.Verflare || lastComboMove == RDM.Verholy) && CanUseAction(RDM.Scorch))
                    return RDM.Scorch;
            }

            if (IsEnabled(CustomComboPreset.RedMageMeleeManaStacksFeature))
            {
                if (gauge.ManaStacks == 3 && CanUseAction(RDM.Verflare))
                {
                    if (!CanUseAction(RDM.Verholy))
                        return RDM.Verflare;

                    if (gauge.BlackMana >= gauge.WhiteMana)
                    {
                        if (HasEffect(RDM.Buffs.VerstoneReady) && !HasEffect(RDM.Buffs.VerfireReady) && (gauge.BlackMana - gauge.WhiteMana <= 9))
                            return RDM.Verflare;

                        return RDM.Verholy;
                    }
                    else
                    {
                        if (HasEffect(RDM.Buffs.VerfireReady) && !HasEffect(RDM.Buffs.VerstoneReady) && (gauge.WhiteMana - gauge.BlackMana <= 9))
                            return RDM.Verholy;

                        return RDM.Verflare;
                    }
                }
            }
        }

        if (actionID == RDM.Redoublement)
        {
            if (IsEnabled(CustomComboPreset.RedMageMeleeCombo))
            {
                if (lastComboMove == RDM.Zwerchhau && CanUseAction(RDM.Redoublement))
                    // Enchanted
                    return OriginalHook(RDM.Redoublement);

                if ((lastComboMove == RDM.Riposte || lastComboMove == RDM.EnchantedRiposte) && CanUseAction(RDM.Zwerchhau))
                    // Enchanted
                    return OriginalHook(RDM.Zwerchhau);

                // Enchanted
                return OriginalHook(RDM.Riposte);
            }
        }

        return actionID;
    }
}

internal class RedMageVerstoneVerfire : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RdmAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == RDM.Verstone)
        {
            var gauge = GetJobGauge<RDMGauge>();

            if (IsEnabled(CustomComboPreset.RedMageVerprocCapstoneCombo))
            {
                if (lastComboMove == RDM.Scorch && CanUseAction(RDM.Resolution))
                    return RDM.Resolution;

                if ((lastComboMove == RDM.Verflare || lastComboMove == RDM.Verholy) && CanUseAction(RDM.Scorch))
                    return RDM.Scorch;
            }

            if (IsEnabled(CustomComboPreset.RedMageVerprocManaStacksFeature))
            {
                if (gauge.ManaStacks == 3)
                {
                    if (CanUseAction(RDM.Verholy))
                        return RDM.Verholy;

                    // From 68-70
                    if (CanUseAction(RDM.Verflare))
                        return RDM.Verflare;
                }
            }

            if (IsEnabled(CustomComboPreset.RedMageVerprocPlusFeature))
            {
                if (CanUseAction(RDM.Veraero) && (HasEffect(RDM.Buffs.Dualcast) || HasEffect(RDM.Buffs.Acceleration) || HasEffect(RDM.Buffs.Swiftcast) || HasEffect(RDM.Buffs.LostChainspell)))
                    // Veraero3
                    return OriginalHook(RDM.Veraero);
            }

            if (IsEnabled(CustomComboPreset.RedMageVerprocOpenerStoneFeature))
            {
                if (CanUseAction(RDM.Veraero) && !InCombat() && !HasEffect(RDM.Buffs.VerstoneReady))
                    // Veraero3
                    return OriginalHook(RDM.Veraero);
            }

            if (IsEnabled(CustomComboPreset.RedMageVerprocFeature))
            {
                if (HasEffect(RDM.Buffs.VerstoneReady))
                    return RDM.Verstone;

                // Jolt
                return OriginalHook(RDM.Jolt2);
            }
        }

        if (actionID == RDM.Verfire)
        {
            var gauge = GetJobGauge<RDMGauge>();

            if (IsEnabled(CustomComboPreset.RedMageVerprocCapstoneCombo))
            {
                if (lastComboMove == RDM.Scorch && CanUseAction(RDM.Resolution))
                    return RDM.Resolution;

                if ((lastComboMove == RDM.Verflare || lastComboMove == RDM.Verholy) && CanUseAction(RDM.Scorch))
                    return RDM.Scorch;
            }

            if (IsEnabled(CustomComboPreset.RedMageVerprocManaStacksFeature))
            {
                if (gauge.ManaStacks == 3)
                {
                    if (CanUseAction(RDM.Verflare))
                        return RDM.Verflare;
                }
            }

            if (IsEnabled(CustomComboPreset.RedMageVerprocPlusFeature))
            {
                if (CanUseAction(RDM.Verthunder) && (HasEffect(RDM.Buffs.Dualcast) || HasEffect(RDM.Buffs.Acceleration) || HasEffect(RDM.Buffs.Swiftcast) || HasEffect(RDM.Buffs.LostChainspell)))
                    // Verthunder3
                    return OriginalHook(RDM.Verthunder);
            }

            if (IsEnabled(CustomComboPreset.RedMageVerprocOpenerFireFeature))
            {
                if (CanUseAction(RDM.Verthunder) && !InCombat() && !HasEffect(RDM.Buffs.VerfireReady))
                    // Verthunder3
                    return OriginalHook(RDM.Verthunder);
            }

            if (IsEnabled(CustomComboPreset.RedMageVerprocFeature))
            {
                if (HasEffect(RDM.Buffs.VerfireReady))
                    return RDM.Verfire;

                // Jolt
                return OriginalHook(RDM.Jolt2);
            }
        }

        return actionID;
    }
}

internal class RedMageAcceleration : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageAccelerationSwiftcastFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == RDM.Acceleration)
        {
            if (CanUseAction(RDM.Acceleration))
            {
                if (IsEnabled(CustomComboPreset.RedMageAccelerationSwiftcastOption))
                {
                    if (IsOffCooldown(RDM.Acceleration) && IsOffCooldown(ADV.Swiftcast))
                        return ADV.Swiftcast;
                }

                if (IsOffCooldown(RDM.Acceleration))
                    return RDM.Acceleration;

                if (IsOffCooldown(ADV.Swiftcast))
                    return ADV.Swiftcast;

                return RDM.Acceleration;
            }

            if (CanUseAction(ADV.Swiftcast))
                return ADV.Swiftcast;
        }

        return actionID;
    }
}

internal class RedMageEmbolden : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageEmboldenFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == RDM.Embolden)
        {
            if (CanUseAction(RDM.Manafication) && IsOffCooldown(RDM.Manafication) && !IsOffCooldown(RDM.Embolden))
                return RDM.Manafication;
        }

        return actionID;
    }
}

internal class RedMageContreSixteFleche : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageContreFlecheFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == RDM.ContreSixte || actionID == RDM.Fleche)
        {
            if (CanUseAction(RDM.ContreSixte))
                return CalcBestAction(actionID, RDM.Fleche, RDM.ContreSixte);

            if (CanUseAction(RDM.Fleche))
                return RDM.Fleche;
        }

        return actionID;
    }
}

internal class RedMageJoltGaugeBalancer : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageJoltGaugeBalancer;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == RDM.Jolt || actionID == RDM.Jolt2)
        {
            var gauge = GetJobGauge<RDMGauge>();

            if (lastComboMove == RDM.Scorch && CanUseAction(RDM.Resolution))
                return RDM.Resolution;

            if ((lastComboMove == RDM.Verflare || lastComboMove == RDM.Verholy) && CanUseAction(RDM.Scorch))
                return RDM.Scorch;

            if (gauge.ManaStacks == 3)
            {
                if (CanUseAction(RDM.Verholy) && !CanUseAction(RDM.Verflare))
                    return RDM.Verholy;

                if (CanUseAction(RDM.Verholy) && CanUseAction(RDM.Verflare) && (gauge.WhiteMana >= gauge.BlackMana))
                    return RDM.Verholy;

                // From 68-70
                if (CanUseAction(RDM.Verflare) && (gauge.BlackMana >= gauge.WhiteMana))
                    return RDM.Verflare;
            }

            if (HasEffect(RDM.Buffs.VerstoneReady) && CanUseAction(RDM.Verstone))
                return RDM.Verstone;

            if (HasEffect(RDM.Buffs.VerfireReady) && CanUseAction(RDM.Verfire))
                return RDM.Verfire;

            if (CanUseAction(RDM.Veraero) && (HasEffect(RDM.Buffs.Dualcast) || HasEffect(RDM.Buffs.Acceleration) || HasEffect(RDM.Buffs.Swiftcast) || HasEffect(RDM.Buffs.LostChainspell)) && (gauge.BlackMana >= gauge.WhiteMana || gauge.BlackMana == gauge.WhiteMana))
                // Veraero3
                return OriginalHook(RDM.Veraero);

            if (CanUseAction(RDM.Verthunder) && (HasEffect(RDM.Buffs.Dualcast) || HasEffect(RDM.Buffs.Acceleration) || HasEffect(RDM.Buffs.Swiftcast) || HasEffect(RDM.Buffs.LostChainspell)) && (gauge.WhiteMana >= gauge.BlackMana))
                // Veraero3
                return OriginalHook(RDM.Verthunder);

            if (IsEnabled(CustomComboPreset.RedMageVerprocOpenerStoneFeature))
            {
                if (CanUseAction(RDM.Veraero) && !InCombat() && !HasEffect(RDM.Buffs.VerstoneReady))
                    // Veraero3
                    return OriginalHook(RDM.Veraero);
            }

            if (IsEnabled(CustomComboPreset.RedMageVerprocOpenerFireFeature))
            {
                if (CanUseAction(RDM.Verthunder) && !InCombat() && !HasEffect(RDM.Buffs.VerfireReady))
                    // Verthunder3
                    return OriginalHook(RDM.Verthunder);
            }

            // Jolt
            return OriginalHook(RDM.Jolt2);
        }

        return actionID;
    }
}