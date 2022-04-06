using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;

namespace XIVComboExpandedPlugin.Combos
{
    internal static class SGE
    {
        public const byte JobID = 40;

        public const uint
            Dosis = 24283,
            Diagnosis = 24284,
            Kardia = 24285,
            Prognosis = 24286,
            Egeiro = 24287,
            Physis = 24288,
            Phlegma = 24289,
            Eukrasia = 24290,
            Soteria = 24294,
            Druochole = 24296,
            Dyskrasia = 24297,
            Kerachole = 24298,
            Ixochole = 24299,
            Zoe = 24300,
            Pepsis = 24301,
            Physis2 = 24302,
            Taurochole = 24303,
            Toxikon = 24304,
            Haima = 24305,
            Phlegma2 = 24307,
            Rhizomata = 24309,
            Holos = 24310,
            Panhaima = 24311,
            Phlegma3 = 24313,
            Krasis = 24317,
            Pneuma = 24318;

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
                Ixochole = 52,
                Taurochole = 62,
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
                Egeiro = 12,
                Phlegma = 26,
                Eukrasia = 30,
                Soteria = 35,
                Druochole = 45,
                Dyskrasia = 46,
                Kerachole = 50,
                Ixochole = 52,
                Physis2 = 60,
                Taurochole = 62,
                Toxikon = 66,
                Haima = 70,
                Phlegma2 = 72,
                Dosis2 = 72,
                Rhizomata = 74,
                Holos = 76,
                Panhaima = 80,
                Phlegma3 = 82,
                Dosis3 = 82,
                Krasis = 86,
                Pneuma = 90;
        }

        internal class SageDosis : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SgeAny;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == SGE.Dosis)
                {
                    if (IsEnabled(CustomComboPreset.SageDosisKardiaFeature))
                    {
                        if (!HasEffect(SGE.Buffs.Kardion))
                            return SGE.Kardia;
                    }
                }

                return actionID;
            }
        }

        internal class SageToxikon : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SgeAny;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == SGE.Toxikon)
                {
                    if (IsEnabled(CustomComboPreset.SageToxikonPhlegma))
                    {
                        var phlegma =
                            level >= SGE.Levels.Phlegma3 ? SGE.Phlegma3 :
                            level >= SGE.Levels.Phlegma2 ? SGE.Phlegma2 :
                            level >= SGE.Levels.Phlegma ? SGE.Phlegma : 0;

                        if (phlegma != 0 && HasCharges(phlegma))
                            return OriginalHook(SGE.Phlegma);
                    }
                }

                return actionID;
            }
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

                    return SGE.Kardia;
                }

                return actionID;
            }
        }

        internal class SageTaurochole : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SgeAny;

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

        internal class SageDruochole : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SgeAny;

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

                    if (IsEnabled(CustomComboPreset.SageDruocholeTaurocholeFeature))
                    {
                        if (level >= SGE.Levels.Taurochole && IsOffCooldown(SGE.Taurochole))
                            return SGE.Taurochole;
                    }
                }

                return actionID;
            }
        }

        internal class SageIxochole : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SgeAny;

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

        internal class SageKerachole : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SgeAny;

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

        internal class DsykrasiaToxikon : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SgeAny;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID == SGE.Dyskrasia)
                {
                    var gauge = GetJobGauge<SGEGauge>();

                    if (IsEnabled(CustomComboPreset.DsykrasiaToxikon))
                    {
                        if (level >= SGE.Levels.Toxikon && gauge.Addersting >= 1)
                            return OriginalHook(SGE.Toxikon);
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
                        Status? eukresiandosis1 = FindTargetEffect(SGE.Debuffs.EukrasianDosis1);
                        if (eukresiandosis1 is not null)
                        {
                            if (TargetHasEffect(SGE.Debuffs.EukrasianDosis1) && eukresiandosis1.RemainingTime >= 10)
                                return OriginalHook(SGE.Dosis);

                            if (TargetHasEffect(SGE.Debuffs.EukrasianDosis1) && eukresiandosis1.RemainingTime <= 10)
                            {
                                if (HasEffect(SGE.Buffs.Eukrasia) && eukresiandosis1.RemainingTime <= 10)
                                    return OriginalHook(SGE.Dosis);
                            }

                            return OriginalHook(SGE.Eukrasia);
                        }
                    }

                    if (level >= SGE.Levels.Dosis2)
                    {
                        Status? eukresiandosis2 = FindTargetEffect(SGE.Debuffs.EukrasianDosis2);
                        if (eukresiandosis2 is not null)
                        {
                            if (TargetHasEffect(SGE.Debuffs.EukrasianDosis2) && eukresiandosis2.RemainingTime >= 10)
                                return OriginalHook(SGE.Dosis);

                            if (TargetHasEffect(SGE.Debuffs.EukrasianDosis2) && eukresiandosis2.RemainingTime <= 10)
                            {
                                if (HasEffect(SGE.Buffs.Eukrasia) && eukresiandosis2.RemainingTime <= 10)
                                    return OriginalHook(SGE.Dosis);
                            }

                            return OriginalHook(SGE.Eukrasia);
                        }
                    }

                    if (level >= SGE.Levels.Dosis3)
                    {
                        Status? eukresiandosis3 = FindTargetEffect(SGE.Debuffs.EukrasianDosis3);
                        if (eukresiandosis3 is not null)
                        {
                            if (TargetHasEffect(SGE.Debuffs.EukrasianDosis3) && eukresiandosis3.RemainingTime >= 10)
                                return OriginalHook(SGE.Dosis);

                            if (TargetHasEffect(SGE.Debuffs.EukrasianDosis3) && eukresiandosis3.RemainingTime <= 10)
                            {
                                if (HasEffect(SGE.Buffs.Eukrasia) && eukresiandosis3.RemainingTime <= 10)
                                    return OriginalHook(SGE.Dosis);
                            }

                            return OriginalHook(SGE.Eukrasia);
                        }
                    }

                    if (HasEffect(SGE.Buffs.Eukrasia))
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

        internal class SagePhlegma : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SgeAny;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == SGE.Phlegma || actionID == SGE.Phlegma2 || actionID == SGE.Phlegma3)
                {
                    var gauge = GetJobGauge<SGEGauge>();

                    if (IsEnabled(CustomComboPreset.SagePhlegmaDyskrasia))
                    {
                        if (HasNoTarget())
                            return OriginalHook(SGE.Dyskrasia);
                    }

                    if (IsEnabled(CustomComboPreset.SagePhlegmaToxicon))
                    {
                        var phlegma =
                            level >= SGE.Levels.Phlegma3 ? SGE.Phlegma3 :
                            level >= SGE.Levels.Phlegma2 ? SGE.Phlegma2 :
                            level >= SGE.Levels.Phlegma ? SGE.Phlegma : 0;

                        if (phlegma != 0 && HasNoCharges(phlegma) && gauge.Addersting > 0)
                            return OriginalHook(SGE.Toxikon);
                    }

                    if (IsEnabled(CustomComboPreset.SagePhlegmaDyskrasia))
                    {
                        var phlegma =
                            level >= SGE.Levels.Phlegma3 ? SGE.Phlegma3 :
                            level >= SGE.Levels.Phlegma2 ? SGE.Phlegma2 :
                            level >= SGE.Levels.Phlegma ? SGE.Phlegma : 0;

                        if (phlegma != 0 && HasNoCharges(phlegma))
                            return OriginalHook(SGE.Dyskrasia);
                    }
                }

                return actionID;
            }
        }
    }
}