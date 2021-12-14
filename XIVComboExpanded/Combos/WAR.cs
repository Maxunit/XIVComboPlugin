using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;

namespace XIVComboExpandedPlugin.Combos
{
    internal static class WAR
    {
        public const byte ClassID = 3;
        public const byte JobID = 21;

        public const uint
            HeavySwing = 31,
            Maim = 37,
            Berserk = 38,
            Overpower = 41,
            StormsPath = 42,
            StormsEye = 45,
            InnerBeast = 49,
            SteelCyclone = 51,
            Infuriate = 52,
            FellCleave = 3549,
            Decimate = 3550,
            RawIntuition = 3551,
            InnerRelease = 7389,
            MythrilTempest = 16462,
            ChaoticCyclone = 16463,
            NascentFlash = 16464,
            InnerChaos = 16465,
            PrimalRend = 25753,
            Upheaval = 7387,
            Orogeny = 25752;

        public static class Buffs
        {
            public const ushort
                InnerRelease = 1177,
                NascentChaos = 1897,
                PrimalRendReady = 2624,
                SurgingTempest = 2677;
        }

        public static class Debuffs
        {
            public const ushort
                Placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                Maim = 4,
                StormsPath = 26,
                MythrilTempest = 40,
                StormsEye = 50,
                InnerBeastMastery = 54,
                FellCleave = 54,
                Decimate = 60,
                Upheaval = 64,
                MythrilTempestTrait = 74,
                NascentFlash = 76,
                InnerChaos = 80,
                Orogeny = 86,
                PrimalRend = 90;
        }
    }

    internal class WarriorStormsPathCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorStormsPathCombo;

        protected internal override uint[] ActionIDs { get; } = new[] { WAR.StormsPath };

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == WAR.StormsPath)
            {
                byte gauge = GetJobGauge<WARGauge>().BeastGauge;
                if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && gauge >= 90 && level >= WAR.Levels.InnerBeastMastery)
                    return OriginalHook(WAR.FellCleave);
                if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && gauge >= 90 && level <= WAR.Levels.InnerBeastMastery)
                    return OriginalHook(WAR.InnerBeast);
                if (IsEnabled(CustomComboPreset.WarriorInnerReleaseFeature) && HasEffect(WAR.Buffs.InnerRelease) && level >= WAR.Levels.FellCleave)
                    return OriginalHook(WAR.FellCleave);
                if (comboTime > 0)
                {
                    if (lastComboMove == WAR.Maim && level >= WAR.Levels.StormsPath)
                    {
                        return WAR.StormsPath;
                    }

                    if (lastComboMove == WAR.HeavySwing && level >= WAR.Levels.Maim)
                    {
                        return WAR.Maim;
                    }
                }

                return WAR.HeavySwing;
            }

            return actionID;
        }
    }

    internal class WarriorStormsEyeCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorStormsEyeCombo;

        protected internal override uint[] ActionIDs { get; } = new[] { WAR.StormsEye };

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == WAR.StormsEye)
            {
                byte gauge = GetJobGauge<WARGauge>().BeastGauge;
                if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && gauge >= 90 && level >= WAR.Levels.InnerBeastMastery)
                    return OriginalHook(WAR.FellCleave);
                if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && gauge >= 90 && level <= WAR.Levels.InnerBeastMastery)
                    return OriginalHook(WAR.InnerBeast);
                if (IsEnabled(CustomComboPreset.WarriorInnerReleaseFeature) && HasEffect(WAR.Buffs.InnerRelease) && level >= WAR.Levels.FellCleave)
                    return OriginalHook(WAR.FellCleave);
                if (comboTime > 0)
                {
                    if (lastComboMove == WAR.Maim && level >= WAR.Levels.StormsEye)
                    {
                        return WAR.StormsEye;
                    }

                    if (lastComboMove == WAR.HeavySwing && level >= WAR.Levels.Maim)
                        return WAR.Maim;
                }

                return WAR.HeavySwing;
            }

            return actionID;
        }
    }

    internal class WarriorMythrilTempestCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorMythrilTempestCombo;

        protected internal override uint[] ActionIDs { get; } = new[] { WAR.MythrilTempest };

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == WAR.MythrilTempest)
            {
                if (IsEnabled(CustomComboPreset.WarriorInnerReleaseFeature) && HasEffect(WAR.Buffs.InnerRelease))
                    return OriginalHook(WAR.Decimate);
                if (comboTime > 0)
                {
                    if (lastComboMove == WAR.Overpower && level >= WAR.Levels.MythrilTempest)
                    {
                        byte gauge = GetJobGauge<WARGauge>().BeastGauge;
                        if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && gauge >= 90 && level >= WAR.Levels.MythrilTempestTrait)
                            return OriginalHook(WAR.Decimate);
                        if (IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && gauge >= 90 && level <= WAR.Levels.MythrilTempestTrait)
                            return OriginalHook(WAR.SteelCyclone);
                        return WAR.MythrilTempest;
                    }
                }

                return WAR.Overpower;
            }

            return actionID;
        }
    }

    internal class WarriorNascentFlashFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorNascentFlashFeature;

        protected internal override uint[] ActionIDs { get; } = new[] { WAR.NascentFlash };

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == WAR.NascentFlash)
            {
                if (level >= WAR.Levels.NascentFlash)
                    // Bloodwhetting
                    return OriginalHook(WAR.NascentFlash);

                return WAR.RawIntuition;
            }

            return actionID;
        }
    }

    internal class WArriorPrimalBeastFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorPrimalBeastFeature;

        protected internal override uint[] ActionIDs { get; } = new[] { WAR.InnerBeast, WAR.FellCleave, WAR.SteelCyclone, WAR.Decimate };

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == WAR.InnerBeast || actionID == WAR.FellCleave || actionID == WAR.SteelCyclone || actionID == WAR.Decimate)
            {
                if (level >= WAR.Levels.PrimalRend && HasEffect(WAR.Buffs.PrimalRendReady))
                    return WAR.PrimalRend;
            }

            return actionID;
        }
    }

    internal class WArriorPrimalReleaseFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorPrimalReleaseFeature;

        protected internal override uint[] ActionIDs { get; } = new[] { WAR.Berserk, WAR.InnerRelease };

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == WAR.Berserk || actionID == WAR.InnerRelease)
            {
                if (level >= WAR.Levels.PrimalRend && HasEffect(WAR.Buffs.PrimalRendReady))
                    return WAR.PrimalRend;
            }

            return actionID;
        }
    }

    internal class UpheavalOrogenySpenderFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.UpheavalOrogenySpenderFeature;

        protected internal override uint[] ActionIDs { get; } = new[] { WAR.StormsPath, WAR.StormsEye, WAR.MythrilTempest };

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            byte gauge = GetJobGauge<WARGauge>().BeastGauge;
            if (IsEnabled(CustomComboPreset.UpheavalOrogenySpenderFeature) && HasEffect(WAR.Buffs.SurgingTempest))
            {
                if (actionID is WAR.StormsPath or WAR.StormsEye)
                {
                    if (level >= WAR.Levels.Upheaval)
                        return PickByCooldown(actionID, actionID, WAR.Upheaval);

                    return WAR.StormsPath;
                }

                if (actionID is WAR.MythrilTempest)
                {
                    if (level >= WAR.Levels.Orogeny)
                        return PickByCooldown(actionID, actionID, WAR.Orogeny);

                    return WAR.MythrilTempest;
                }
            }

            return actionID;
        }
    }

    internal class WarriorStormPathStormEye : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorStormPathStormEye;

        protected internal override uint[] ActionIDs { get; } = new[] { WAR.StormsPath, WAR.StormsEye };

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is WAR.StormsPath or WAR.StormsEye)
            {
                if (level >= WAR.Levels.StormsEye)
                {
                    if (IsEnabled(CustomComboPreset.WarriorStormPathStormEye))
                    {
                        Status? surgingtempesttime = FindEffectAny(WAR.Buffs.SurgingTempest);
                        if (HasEffect(WAR.Buffs.SurgingTempest) && surgingtempesttime is not null)
                            {
                            if (surgingtempesttime.RemainingTime >= 30)
                            {
                                if (IsEnabled(CustomComboPreset.WarriorStormPathStormEye) && HasEffect(WAR.Buffs.SurgingTempest))
                                    return WAR.StormsPath;
                            }

                            if (surgingtempesttime.RemainingTime <= 30)
                            {
                                if (IsEnabled(CustomComboPreset.WarriorStormPathStormEye) && HasEffect(WAR.Buffs.SurgingTempest))
                                    return WAR.StormsEye;
                            }
                        }

                        return WAR.StormsEye;
                    }
                }

                if (level <= WAR.Levels.StormsEye)
                {
                    if (IsEnabled(CustomComboPreset.WarriorStormPathStormEye))
                        return WAR.StormsPath;
                }
            }

            return actionID;
        }
    }
}
