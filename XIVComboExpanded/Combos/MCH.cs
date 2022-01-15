using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos
{
    internal static class MCH
    {
        public const byte JobID = 31;

        public const uint
            // Single target
            CleanShot = 2873,
            HeatedCleanShot = 7413,
            SplitShot = 2866,
            HeatedSplitShot = 7411,
            SlugShot = 2868,
            HeatedSlugshot = 7412,
            // Charges
            GaussRound = 2874,
            Ricochet = 2890,
            // AoE
            SpreadShot = 2870,
            AutoCrossbow = 16497,
            Scattergun = 25786,
            // Rook
            RookAutoturret = 2864,
            RookOverdrive = 7415,
            AutomatonQueen = 16501,
            QueenOverdrive = 16502,
            // Other
            Reassemble = 2876,
            Hypercharge = 17209,
            HeatBlast = 7410,
            HotShot = 2872,
            Wildfire = 2878,
            BarrelStabilizer = 7414,
            Drill = 16498,
            AirAnchor = 16500,
            Chainsaw = 25788;

        public static class Buffs
        {
            public const ushort
                Reassembled = 851,
                Placeholder = 0;
        }

        public static class Debuffs
        {
            public const ushort
                Placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                SlugShot = 2,
                Reassemble = 10,
                GaussRound = 15,
                CleanShot = 26,
                Hypercharge = 30,
                HeatBlast = 35,
                RookAutoturret = 40,
                RookOverdrive = 40,
                Wildfire = 45,
                Ricochet = 50,
                AutoCrossbow = 52,
                HeatedSplitShot = 54,
                Drill = 58,
                HeatedSlugshot = 60,
                HeatedCleanShot = 64,
                BarrelStabilizer = 66,
                ChargedActionMastery = 74,
                AirAnchor = 76,
                QueenOverdrive = 80,
                EnchancedReassemble = 84,
                Chainsaw = 90;
        }
    }

    internal class MachinistMainCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MachinistMainCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MCH.CleanShot || actionID == MCH.HeatedCleanShot)
            {
                if (comboTime > 0)
                {
                    if (IsEnabled(CustomComboPreset.MachinistAutoweaveFeature))
                    {
                        CooldownData gaussCd = GetCooldown(MCH.GaussRound);
                        var maxCharges = level >= MCH.Levels.ChargedActionMastery ? 3 : 2;
                        float gaussCharges = gaussCd.IsCooldown ? gaussCd.CooldownElapsed / (gaussCd.CooldownTotal / maxCharges) : maxCharges;
                        float ricochetCharges = 0;
                        var weaveAction = MCH.GaussRound;
                        if (level >= MCH.Levels.Ricochet)
                        {
                            CooldownData ricochetCd = GetCooldown(MCH.Ricochet);
                            ricochetCharges = ricochetCd.IsCooldown ? ricochetCd.CooldownElapsed / (ricochetCd.CooldownTotal / maxCharges) : maxCharges;
                            if (gaussCharges >= 1 || ricochetCharges >= 1)
                                weaveAction = CalcBestAction(actionID, MCH.GaussRound, MCH.Ricochet);
                        }

                        CooldownData globalCd = GetCooldown(MCH.SplitShot);
                        bool canWeave = (globalCd.CooldownElapsed / globalCd.CooldownTotal) < 0.7;
                        if (canWeave && (gaussCharges >= 1 || ricochetCharges >= 1))
                            return weaveAction;
                    }

                    if (lastComboMove == MCH.SlugShot && level >= MCH.Levels.CleanShot)
                    {
                        if (IsEnabled(CustomComboPreset.MachinistRookFeature) && level >= MCH.Levels.RookAutoturret)
                        {
                            var gauge = GetJobGauge<MCHGauge>();

                            if (gauge.Battery == 100)
                                return OriginalHook(MCH.AutomatonQueen);
                        }

                        // Heated
                        return OriginalHook(MCH.CleanShot);
                    }

                    if (lastComboMove == MCH.SplitShot && level >= MCH.Levels.SlugShot)
                        // Heated
                        return OriginalHook(MCH.SlugShot);
                }

                // Heated
                return OriginalHook(MCH.SplitShot);
            }

            return actionID;
        }
    }

    internal class MachinistGaussRoundRicochetFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MachinistGaussRoundRicochetFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MCH.GaussRound || actionID == MCH.Ricochet)
            {
                if (level >= MCH.Levels.Ricochet)
                    return CalcBestAction(actionID, MCH.GaussRound, MCH.Ricochet);

                return MCH.GaussRound;
            }

            return actionID;
        }
    }

    internal class MachinistOverheatFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MachinistOverheatFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MCH.HeatBlast || actionID == MCH.AutoCrossbow)
            {
                var gauge = GetJobGauge<MCHGauge>();

                if (IsEnabled(CustomComboPreset.MachinistBarrelStabilizerFeature))
                {
                    CooldownData barrelStabilizerCd = GetCooldown(MCH.BarrelStabilizer);
                    if (level >= MCH.Levels.BarrelStabilizer && !barrelStabilizerCd.IsCooldown && !gauge.IsOverheated && gauge.Heat < 50)
                        return MCH.BarrelStabilizer;
                }

                if (IsEnabled(CustomComboPreset.MachinistWildfireFeature))
                {
                    CooldownData wildfireCd = GetCooldown(MCH.Wildfire);
                    if (level >= MCH.Levels.Wildfire && !wildfireCd.IsCooldown && !gauge.IsOverheated && gauge.Heat >= 50)
                        return MCH.Wildfire;
                }

                if (level >= MCH.Levels.Hypercharge && !gauge.IsOverheated)
                    return MCH.Hypercharge;

                if (IsEnabled(CustomComboPreset.MachinistAutoweaveFeature) && actionID == MCH.HeatBlast)
                {
                    CooldownData gaussCd = GetCooldown(MCH.GaussRound);
                    var maxCharges = level >= MCH.Levels.ChargedActionMastery ? 3 : 2;
                    float gaussCharges = gaussCd.IsCooldown ? gaussCd.CooldownElapsed / (gaussCd.CooldownTotal / maxCharges) : maxCharges;
                    float ricochetCharges = 0;
                    var weaveAction = MCH.GaussRound;
                    if (level >= MCH.Levels.Ricochet)
                    {
                        CooldownData ricochetCd = GetCooldown(MCH.Ricochet);
                        ricochetCharges = ricochetCd.IsCooldown ? ricochetCd.CooldownElapsed / (ricochetCd.CooldownTotal / maxCharges) : maxCharges;
                        if (gaussCharges >= 1 || ricochetCharges >= 1)
                            weaveAction = CalcBestAction(actionID, MCH.GaussRound, MCH.Ricochet);
                    }

                    CooldownData heatBlastCd = GetCooldown(MCH.HeatBlast);
                    bool canWeave = (heatBlastCd.CooldownElapsed / heatBlastCd.CooldownTotal) < 0.5;
                    if (canWeave && (gaussCharges >= 1 || ricochetCharges >= 1))
                        return weaveAction;
                }

                if (level < MCH.Levels.AutoCrossbow)
                    return MCH.HeatBlast;
            }

            return actionID;
        }
    }

    internal class MachinistSpreadShotFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MachinistSpreadShotFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MCH.SpreadShot || actionID == MCH.Scattergun)
            {
                var gauge = GetJobGauge<MCHGauge>();

                if (level >= MCH.Levels.AutoCrossbow && gauge.IsOverheated)
                    return MCH.AutoCrossbow;
            }

            return actionID;
        }
    }

    internal class MachinistOverdriveFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MachinistOverdriveFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MCH.RookAutoturret)
            {
                var gauge = GetJobGauge<MCHGauge>();

                if (level >= MCH.Levels.RookOverdrive && gauge.IsRobotActive)
                    // Queen Overdrive
                    return OriginalHook(MCH.RookOverdrive);
            }

            return actionID;
        }
    }

    internal class MachinistDrillAirAnchorChainsawFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MachinistHotShotDrillChainsawFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MCH.HotShot || actionID == MCH.AirAnchor || actionID == MCH.Drill || actionID == MCH.Chainsaw)
            {
                uint bestAction = MCH.HotShot;
                if (level >= MCH.Levels.Drill)
                    bestAction = CalcBestAction(actionID, MCH.Drill, MCH.HotShot);

                if (level >= MCH.Levels.AirAnchor)
                    bestAction = CalcBestAction(actionID, MCH.AirAnchor, MCH.Drill);

                if (level >= MCH.Levels.Chainsaw)
                    bestAction = CalcBestAction(actionID, MCH.Chainsaw, MCH.AirAnchor, MCH.Drill);

                CooldownData bestActionCd = GetCooldown(bestAction);
                if (IsEnabled(CustomComboPreset.MachinistReassembleFeature) && level >= MCH.Levels.Reassemble && !HasEffect(MCH.Buffs.Reassembled) && !bestActionCd.IsCooldown)
                {
                    CooldownData reassembleCd = GetCooldown(MCH.Reassemble);

                    var maxCharges = level >= MCH.Levels.EnchancedReassemble ? 2 : 1;
                    float reassembleCharges = reassembleCd.IsCooldown ? reassembleCd.CooldownElapsed / (reassembleCd.CooldownTotal / maxCharges) : maxCharges;

                    if (reassembleCharges >= 1)
                        bestAction = MCH.Reassemble;
                }

                if (IsEnabled(CustomComboPreset.MachinistRookFeature) && level >= MCH.Levels.RookAutoturret && (bestAction == MCH.HotShot || bestAction == MCH.AirAnchor))
                {
                    var gauge = GetJobGauge<MCHGauge>();

                    if (gauge.Battery == 100)
                        bestAction = OriginalHook(MCH.AutomatonQueen);
                }

                return bestAction;
            }

            return actionID;
        }
    }

    internal class MachinistAirAnchorChainsawFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MachinistHotShotChainsawFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MCH.HotShot || actionID == MCH.AirAnchor || actionID == MCH.Chainsaw)
            {
                if (level >= MCH.Levels.Chainsaw)
                    return CalcBestAction(actionID, MCH.Chainsaw, MCH.AirAnchor);

                if (level >= MCH.Levels.AirAnchor)
                    return MCH.AirAnchor;

                return MCH.HotShot;
            }

            return actionID;
        }
    }
}
