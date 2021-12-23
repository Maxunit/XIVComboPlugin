using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;

namespace XIVComboExpandedPlugin.Combos
{
    internal static class SGE
    {
        public const byte JobID = 40;

        public const uint
            Diagnosis = 24284,
            Prognosis = 24286,
            Holos = 24310,
            Ixochole = 24299,
            Egeiro = 24287,
            Kardia = 24285,
            Soteria = 24294,
            Dosis = 24283,
            Eukrasia = 24290;

        public static class Buffs
        {
            public const ushort
                Kardia = 2604,
                Eukrasia = 2606,
                EukrasianPrognosis = 2609,
                EukrasianDiagnosis = 2607;
        }

        public static class Debuffs
        {
            public const ushort
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
                Rizomata = 74,
                Dosis3 = 82,
                Eukrasia = 30;
        }

        internal class SageKardiaFeature : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SageKardiaFeature;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == SGE.Kardia)
                {
                    if (HasEffect(SGE.Buffs.Kardia) && GetCooldown(SGE.Soteria).CooldownRemaining == 0)
                        return SGE.Soteria;
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