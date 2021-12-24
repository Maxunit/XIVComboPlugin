using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;

namespace XIVComboExpandedPlugin.Combos
{
    internal static class SGE
    {
        public const byte JobID = 40;

        public const uint
            Diagnosis = 24284,
            Kardia = 24285,
            Soteria = 24294,
            Druochole = 24296,
            Kerachole = 24298,
            Ixochole = 24299,
            Taurochole = 24303,
            Prognosis = 24286,
            Holos = 24310,
            Egeiro = 24287,
            Dosis = 24283,
            Rhizomata = 24309,
            Eukrasia = 24290;

        public static class Buffs
        {
            public const ushort
                Kardia = 2604,
                Kardion = 2604,
                Eukrasia = 2606,
                EukrasianPrognosis = 2609,
                EukrasianDiagnosis = 2607;
        }

        public static class Debuffs
        {
            public const ushort
                Taurochole = 62,
                Ixochole = 52,
                Dosis2 = 72,
                Rhizomata = 74,
                Holos = 76,
                Dosis3 = 82,
                Pneuma = 90,
                EukrasianDosis1 = 2614,
                EukrasianDosis2 = 2615,
                EukrasianDosis3 = 2616;
        }

        public static class Levels
        {
            public const ushort
                Dosis = 1,
                Prognosis = 10,
                Soteria = 35,
                Druochole = 45,
                Kerachole = 50,
                Taurochole = 62,
                Ixochole = 52,
                Dosis2 = 72,
                Holos = 76,
                Rhizomata = 74,
                Dosis3 = 82,
                Eukrasia = 30;
        }

        internal abstract class SageCustomCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SgeAny;
        }

        internal class SageKardiaFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SageKardiaFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == SGE.Kardia)
                {
                    if (HasEffect(SGE.Buffs.Kardia) && IsOffCooldown(SGE.Soteria))
                        return SGE.Soteria;
                }

                return actionID;
            }
        }

        internal class SageTaurochole : SageCustomCombo
        {
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == SGE.Taurochole)
                {
                    var gauge = GetJobGauge<SGEGauge>();

                    if (IsEnabled(CustomComboPreset.SageTaurocholeRhizomataFeature))
                    {
                        if (level >= SGE.Levels.Rhizomata && gauge.Addersgall == 0)
                            return SGE.Rhizomata;
                    }

                    if (IsEnabled(CustomComboPreset.SageTaurocholeDruocholeFeature))
                    {
                        if (level >= SGE.Levels.Taurochole && IsOffCooldown(SGE.Taurochole))
                            return SGE.Taurochole;

                        return SGE.Druochole;
                    }
                }

                return actionID;
            }
        }

        internal class SageDruochole : SageCustomCombo
        {
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == SGE.Druochole)
                {
                    var gauge = GetJobGauge<SGEGauge>();

                    if (IsEnabled(CustomComboPreset.SageDruocholeRhizomataFeature))
                    {
                        if (level >= SGE.Levels.Rhizomata && gauge.Addersgall == 0)
                            return SGE.Rhizomata;
                    }
                }

                return actionID;
            }
        }

        internal class SageIxochole : SageCustomCombo
        {
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == SGE.Ixochole)
                {
                    var gauge = GetJobGauge<SGEGauge>();

                    if (IsEnabled(CustomComboPreset.SageIxocholeRhizomataFeature))
                    {
                        if (level >= SGE.Levels.Rhizomata && gauge.Addersgall == 0)
                            return SGE.Rhizomata;
                    }
                }

                return actionID;
            }
        }

        internal class SageKerachole : SageCustomCombo
        {
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == SGE.Kerachole)
                {
                    var gauge = GetJobGauge<SGEGauge>();

                    if (IsEnabled(CustomComboPreset.SageKeracholaRhizomataFeature))
                    {
                        if (level >= SGE.Levels.Rhizomata && gauge.Addersgall == 0)
                            return SGE.Rhizomata;
                    }
                }

                return actionID;
            }
        }

        internal class SageSmartEukrasiaDosis : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SageSmartEukrasiaDosis;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID == SGE.Dosis)
                {
                    if (level >= SGE.Levels.Dosis)
                    {
                        Status? eukresiandosis1 = FindTargetEffectAny(SGE.Debuffs.EukrasianDosis1);
                        if (eukresiandosis1 is not null)
                        {
                            if (TargetHasEffectAny(SGE.Debuffs.EukrasianDosis1) && eukresiandosis1.RemainingTime >= 10)
                                return OriginalHook(SGE.Dosis);

                            if (TargetHasEffectAny(SGE.Debuffs.EukrasianDosis1) && eukresiandosis1.RemainingTime <= 10)
                            {
                                if (HasEffectAny(SGE.Buffs.Eukrasia) && eukresiandosis1.RemainingTime <= 10)
                                    return OriginalHook(SGE.Dosis);
                            }

                            return OriginalHook(SGE.Eukrasia);
                        }
                    }

                    if (level >= SGE.Levels.Dosis2)
                    {
                        Status? eukresiandosis2 = FindTargetEffectAny(SGE.Debuffs.EukrasianDosis2);
                        if (eukresiandosis2 is not null)
                        {
                            if (TargetHasEffectAny(SGE.Debuffs.EukrasianDosis2) && eukresiandosis2.RemainingTime >= 10)
                                return OriginalHook(SGE.Dosis);

                            if (TargetHasEffectAny(SGE.Debuffs.EukrasianDosis2) && eukresiandosis2.RemainingTime <= 10)
                            {
                                if (HasEffectAny(SGE.Buffs.Eukrasia) && eukresiandosis2.RemainingTime <= 10)
                                    return OriginalHook(SGE.Dosis);
                            }

                            return OriginalHook(SGE.Eukrasia);
                        }
                    }

                    if (level >= SGE.Levels.Dosis3)
                    {
                        Status? eukresiandosis3 = FindTargetEffectAny(SGE.Debuffs.EukrasianDosis3);
                        if (eukresiandosis3 is not null)
                        {
                            if (TargetHasEffectAny(SGE.Debuffs.EukrasianDosis3) && eukresiandosis3.RemainingTime >= 10)
                                return OriginalHook(SGE.Dosis);

                            if (TargetHasEffectAny(SGE.Debuffs.EukrasianDosis3) && eukresiandosis3.RemainingTime <= 10)
                            {
                                if (HasEffectAny(SGE.Buffs.Eukrasia) && eukresiandosis3.RemainingTime <= 10)
                                    return OriginalHook(SGE.Dosis);
                            }

                            return OriginalHook(SGE.Eukrasia);
                        }
                    }

                    if (HasEffectAny(SGE.Buffs.Eukrasia))
                        return OriginalHook(SGE.Dosis);

                    return OriginalHook(SGE.Eukrasia);
                }

                return actionID;
            }
        }

        internal class SageSmartEukrasia : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.Disabled; // SageSmartEukrasia

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                float cooldown = 1;

                // Dosis
                if (actionID == SGE.Dosis && level >= SGE.Levels.Eukrasia)
                {
                    if (level >= SGE.Levels.Dosis3)
                        cooldown = GetCooldown(SGE.Debuffs.EukrasianDosis3).CooldownRemaining;

                    if (level >= SGE.Levels.Dosis2 && level < SGE.Levels.Dosis3)
                        cooldown = GetCooldown(SGE.Debuffs.EukrasianDosis2).CooldownRemaining;

                    if (level >= SGE.Levels.Dosis && level < SGE.Levels.Dosis2)
                        cooldown = GetCooldown(SGE.Debuffs.EukrasianDosis1).CooldownRemaining;
                }

                // Diagnosis
                if (actionID == SGE.Diagnosis && level >= SGE.Levels.Eukrasia)
                {
                    cooldown = GetCooldown(SGE.Buffs.EukrasianDiagnosis).CooldownRemaining;
                }

                // Prognosis
                if (actionID == SGE.Prognosis && level >= SGE.Levels.Eukrasia)
                {
                    cooldown = GetCooldown(SGE.Buffs.EukrasianPrognosis).CooldownRemaining;
                }

                if (cooldown == 0 && level >= SGE.Levels.Eukrasia)
                    return SGE.Eukrasia;

                return actionID;
            }
        }
    }
}