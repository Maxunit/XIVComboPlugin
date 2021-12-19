using Dalamud.Game.ClientState.Conditions;

namespace XIVComboVX.Combos {
	internal static class NIN {
		public const byte JobID = 30;

		public const uint
			SpinningEdge = 2240,
			GustSlash = 2242,
			Hide = 2245,
			Assassinate = 8814,
			Mug = 2248,
			DeathBlossom = 2254,
			AeolianEdge = 2255,
			TrickAttack = 2258,
			Ninjutsu = 2260,
			Chi = 2261,
			JinNormal = 2263,
			Kassatsu = 2264,
			ArmorCrush = 3563,
			DreamWithinADream = 3566,
			TenChiJin = 7403,
			HakkeMujinsatsu = 16488,
			Meisui = 16489,
			Jin = 18807,
			Bunshin = 16493,
			Huraijin = 25876,
			PhantomKamaitachi = 25774,
			ForkedRaiju = 25777,
			FleetingRaiju = 25778;

		public static class Buffs {
			public const ushort
				Mudra = 496,
				Kassatsu = 497,
				Suiton = 507,
				Hidden = 614,
				Bunshin = 1954,
				ForkedRaijuReady = 2690,
				FleetingRaijuReady = 2691;
		}

		public static class Debuffs {
			// public const ushort placeholder = 0;
		}

		public static class Levels {
			public const byte
				GustSlash = 4,
				Hide = 10,
				Mug = 15,
				AeolianEdge = 26,
				Ninjitsu = 30,
				Suiton = 45,
				HakkeMujinsatsu = 52,
				ArmorCrush = 54,
				Huraijin = 60,
				TenChiJin = 70,
				Meisui = 72,
				EnhancedKassatsu = 76,
				Bunshin = 80,
				PhantomKamaitachi = 82,
				ForkedRaiju = 90;
		}
	}

	internal class NinjaArmorCrushCombo: CustomCombo {
		protected internal override CustomComboPreset Preset => CustomComboPreset.NinAny;
		protected internal override uint[] ActionIDs { get; } = new[] { NIN.ArmorCrush };

		protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) {
			if (actionID is NIN.ArmorCrush) {

				if (level >= NIN.Levels.Ninjitsu && IsEnabled(CustomComboPreset.NinjaGCDNinjutsuFeature) && SelfHasEffect(NIN.Buffs.Mudra))
					return OriginalHook(NIN.Ninjutsu);

				if (IsEnabled(CustomComboPreset.NinjaArmorCrushRaijuFeature)) {

					if (level >= NIN.Levels.ForkedRaiju && SelfHasEffect(NIN.Buffs.FleetingRaijuReady))
						return NIN.FleetingRaiju;

					if (level >= NIN.Levels.ForkedRaiju && SelfHasEffect(NIN.Buffs.ForkedRaijuReady))
						return NIN.ForkedRaiju;

				}

				if (IsEnabled(CustomComboPreset.NinjaArmorCrushCombo))
					return SimpleChainCombo(level, lastComboMove, comboTime, (1, NIN.SpinningEdge),
						(NIN.Levels.GustSlash, NIN.GustSlash),
						(NIN.Levels.ArmorCrush, NIN.ArmorCrush)
					);
			}

			return actionID;
		}
	}

	internal class NinjaAeolianEdgeCombo: CustomCombo {
		protected internal override CustomComboPreset Preset => CustomComboPreset.NinAny;
		protected internal override uint[] ActionIDs { get; } = new[] { NIN.AeolianEdge };

		protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) {
			if (actionID is NIN.AeolianEdge) {
				if (level >= NIN.Levels.Ninjitsu && IsEnabled(CustomComboPreset.NinjaGCDNinjutsuFeature) && SelfHasEffect(NIN.Buffs.Mudra))
					return OriginalHook(NIN.Ninjutsu);

				if (IsEnabled(CustomComboPreset.NinjaArmorCrushRaijuFeature)) {

					if (level >= NIN.Levels.ForkedRaiju && SelfHasEffect(NIN.Buffs.FleetingRaijuReady))
						return NIN.FleetingRaiju;

					if (level >= NIN.Levels.ForkedRaiju && SelfHasEffect(NIN.Buffs.ForkedRaijuReady))
						return NIN.ForkedRaiju;

				}

				if (IsEnabled(CustomComboPreset.NinjaAeolianEdgeCombo))
					return SimpleChainCombo(level, lastComboMove, comboTime, (1, NIN.SpinningEdge),
						(NIN.Levels.GustSlash, NIN.GustSlash),
						(NIN.Levels.AeolianEdge, NIN.AeolianEdge)
					);
			}

			return actionID;
		}
	}

	internal class NinjaHakkeMujinsatsuCombo: CustomCombo {
		protected internal override CustomComboPreset Preset => CustomComboPreset.NinjaHakkeMujinsatsuCombo;
		protected internal override uint[] ActionIDs { get; } = new[] { NIN.HakkeMujinsatsu };

		protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) {
			if (actionID is NIN.HakkeMujinsatsu) {
				if (IsEnabled(CustomComboPreset.NinjaGCDNinjutsuFeature) && OriginalHook(NIN.JinNormal) == OriginalHook(NIN.Jin))
					return OriginalHook(NIN.Ninjutsu);

				if (level >= NIN.Levels.HakkeMujinsatsu && comboTime > 0 && lastComboMove == NIN.DeathBlossom)
					return NIN.HakkeMujinsatsu;

				return NIN.DeathBlossom;
			}

			return actionID;
		}
	}

	internal class NinjaKassatsuTrickFeature: CustomCombo {
		protected internal override CustomComboPreset Preset => CustomComboPreset.NinjaKassatsuTrickFeature;
		protected internal override uint[] ActionIDs { get; } = new[] { NIN.Kassatsu };

		protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) {
			if (actionID is NIN.Kassatsu
				&& (
					(level >= NIN.Levels.Hide && SelfHasEffect(NIN.Buffs.Hidden))
					|| (level >= NIN.Levels.Suiton && SelfHasEffect(NIN.Buffs.Suiton))
				)
			)
				return NIN.TrickAttack;

			return actionID;
		}
	}

	internal class NinjaHideMugFeature: CustomCombo {
		protected internal override CustomComboPreset Preset => CustomComboPreset.NinjaHideMugFeature;
		protected internal override uint[] ActionIDs { get; } = new[] { NIN.Hide };

		protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) {
			if (actionID is NIN.Hide) {

				if (
					(level >= NIN.Levels.Hide && SelfHasEffect(NIN.Buffs.Hidden))
					|| (level >= NIN.Levels.Suiton && SelfHasEffect(NIN.Buffs.Suiton))
				)
					return NIN.TrickAttack;

				if (HasCondition(ConditionFlag.InCombat))
					return NIN.Mug;
			}

			return actionID;
		}
	}

	internal class NinjaKassatsuChiJinFeature: CustomCombo {
		protected internal override CustomComboPreset Preset => CustomComboPreset.NinjaKassatsuChiJinFeature;
		protected internal override uint[] ActionIDs { get; } = new[] { NIN.Chi };

		protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) {
			if (actionID is NIN.Chi && level >= NIN.Levels.EnhancedKassatsu && SelfHasEffect(NIN.Buffs.Kassatsu))
				return NIN.Jin;

			return actionID;
		}
	}

	internal class NinjaTCJMeisuiFeature: CustomCombo {
		protected internal override CustomComboPreset Preset => CustomComboPreset.NinjaTCJMeisuiFeature;
		protected internal override uint[] ActionIDs { get; } = new[] { NIN.TenChiJin };

		protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) {
			if (actionID is NIN.TenChiJin && level >= NIN.Levels.Meisui && SelfHasEffect(NIN.Buffs.Suiton))
				return NIN.Meisui;

			return actionID;
		}
	}

	internal class NinjaBunshinKamaitachiFeature: CustomCombo {
		protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaBunshinKamaitachiFeature;
		protected internal override uint[] ActionIDs { get; } = new[] { NIN.Bunshin };

		protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) {
			if (actionID is NIN.Bunshin && level >= NIN.Levels.PhantomKamaitachi && SelfHasEffect(NIN.Buffs.Bunshin))
				return NIN.PhantomKamaitachi;

			return actionID;
		}
	}

	internal class NinjaHuraijinRaijuFeature: CustomCombo {
		protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaHuraijinRaijuFeature;
		protected internal override uint[] ActionIDs { get; } = new[] { NIN.Huraijin };

		protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) {
			if (actionID is NIN.Huraijin && level >= NIN.Levels.ForkedRaiju) {

				if (SelfHasEffect(NIN.Buffs.FleetingRaijuReady))
					return NIN.FleetingRaiju;

				if (SelfHasEffect(NIN.Buffs.ForkedRaijuReady))
					return NIN.ForkedRaiju;
			}

			return actionID;
		}
	}
}
