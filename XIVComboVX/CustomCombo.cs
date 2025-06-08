using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using Dalamud.Utility;

using FFXIVClientStructs.FFXIV.Client.UI.Agent;

using PrincessRTFM.XIVComboVX.Attributes;
using PrincessRTFM.XIVComboVX.Combos;
using PrincessRTFM.XIVComboVX.GameData;

namespace PrincessRTFM.XIVComboVX;

internal abstract class CustomCombo {
	public const uint InvalidObjectID = 0xE000_0000;

	public abstract CustomComboPreset Preset { get; }
	public virtual uint[] ActionIDs { get; } = [];
	public readonly HashSet<uint> AffectedIDs;
	public readonly string ModuleName;

	public byte JobID { get; }
	public byte ClassID => this.JobID switch {
		>= 19 and <= 25 => (byte)(this.JobID - 18),
		27 or 28 => 26,
		30 => 29,
		_ => this.JobID,
	};

	protected CustomCombo() {
		CustomComboInfoAttribute presetInfo = this.Preset.GetAttribute<CustomComboInfoAttribute>()!;
		this.JobID = presetInfo.JobID;
		this.ModuleName = this.GetType().Name;
		this.AffectedIDs = new(this.ActionIDs);
	}

	public bool TryInvoke(uint actionID, uint lastComboActionId, float comboTime, byte level, uint classJobID, out uint newActionID) {
		newActionID = 0;

		if (!Service.Configuration.Active)
			return false;

		if (classJobID is >= 8 and <= 15)
			classJobID = DOH.JobID;

		if (classJobID is >= 16 and <= 18)
			classJobID = DOL.JobID;

		if (this.JobID > 0 && this.JobID != classJobID && this.ClassID != classJobID)
			return false;
		if (this.AffectedIDs.Count > 0 && !this.AffectedIDs.Contains(actionID))
			return false;
		if (!IsEnabled(this.Preset))
			return false;

		if (comboTime <= 0)
			lastComboActionId = 0;

		Service.TickLogger.Debug($"{this.ModuleName}.Invoke({actionID}, {lastComboActionId}, {comboTime}, {level})");
		try {
			uint resultingActionID = this.Invoke(actionID, lastComboActionId, comboTime, level);
			if (resultingActionID == 0 || actionID == resultingActionID) {
				Service.TickLogger.Debug("NO REPLACEMENT");
				return false;
			}

			Service.TickLogger.Debug($"Became #{resultingActionID}");
			newActionID = resultingActionID;
			return true;
		}
		catch (Exception ex) {
			Service.TickLogger.Error($"Error in {this.ModuleName}.Invoke({actionID}, {lastComboActionId}, {comboTime}, {level})", ex);
			return false;
		}
	}
	protected abstract uint Invoke(uint actionID, uint lastComboActionId, float comboTime, byte level);

	protected internal static bool IsEnabled(CustomComboPreset preset) {
		if ((int)preset < 0) {
			Service.TickLogger.Debug($"Aborting is-enabled check, {preset}#{(int)preset} is forcibly disabled");
			return false;
		}
		if ((int)preset < 100) {
			Service.TickLogger.Debug($"Bypassing is-enabled check for preset #{(int)preset}");
			return true;
		}
		bool enabled = Service.Configuration.IsEnabled(preset);
		Service.TickLogger.Debug($"Checking status of preset #{(int)preset} - {enabled}");
		return enabled;
	}

	#region Common calculations and shortcuts

	/// <summary>
	/// Selects the best action from a list based on cooldown, charges, and a preferred action.
	/// The priority is: 1. Ready actions (preference is tie-breaker), 2. Actions with more charges, 3. Action with the shortest cooldown.
	/// </summary>
	protected internal static uint PickByCooldown(uint preference, params uint[] actions) {
		if (actions is null || actions.Length == 0)
			return 0;

		var actionsWithCooldown = actions.Select(id => (ActionID: id, Data: GetCooldown(id)));

		// 1. Prioritize ready actions, using 'preference' as a tie-breaker
		var readyActions = actionsWithCooldown.Where(a => a.Data.Available && !a.Data.IsCooldown).ToList();
		if (readyActions.Count != 0) {
			uint bestAction = readyActions.FirstOrDefault(a => a.ActionID == preference, readyActions.First()).ActionID;
			Service.TickLogger.Debug($"Final selection (ready): {bestAction}");
			return bestAction;
		}

		// 2. If no actions are fully ready, check for actions with charges
		var actionsWithCharges = actionsWithCooldown.Where(a => a.Data.HasCharges && a.Data.RemainingCharges > 0).ToList();
		if (actionsWithCharges.Count != 0) {
			// Prioritize by most charges, then by shortest time to next charge
			uint bestAction = actionsWithCharges.OrderByDescending(a => a.Data.RemainingCharges)
									 .ThenBy(a => a.Data.ChargeCooldownRemaining)
									 .First().ActionID;
			Service.TickLogger.Debug($"Final selection (charges): {bestAction}");
			return bestAction;
		}

		// 3. If no actions are ready or have charges, pick the one with the shortest remaining cooldown
		uint finalAction = actionsWithCooldown.OrderBy(a => a.Data.CooldownRemaining).First().ActionID;
		Service.TickLogger.Debug($"Final selection (cooldown): {finalAction}");
		return finalAction;
	}

	/// <summary>
	/// A helper to check if an oGCD can be weaved without clipping the GCD.
	/// </summary>
	/// <param name="actionToWeave">The ID of the oGCD action to potentially use.</param>
	/// <param name="baseAction">The ID of the GCD action that is being used.</param>
	/// <param name="minWeaveTime">The minimum time remaining on the GCD cooldown to allow weaving.</param>
	/// <returns>True if the action can be weaved, otherwise false.</returns>
	protected static bool TryWeave(uint actionToWeave, uint baseAction, float minWeaveTime = 0.7f) {
		return CanWeave(baseAction, minWeaveTime) && CanUse(actionToWeave);
	}

	/// <summary>
	/// A helper to check if a DoT/debuff needs to be reapplied based on its remaining duration.
	/// </summary>
	/// <param name="debuffId">The status ID of the debuff.</param>
	/// <param name="threshold">The remaining duration below which the debuff should be refreshed.</param>
	/// <returns>True if the debuff should be reapplied, otherwise false.</returns>
	protected static bool NeedsDebuff(ushort debuffId, float threshold) {
		return HasTarget && TargetOwnEffectDuration(debuffId) < threshold;
	}

	protected static bool IsJob(params uint[] jobs) {
		IPlayerCharacter? p = LocalPlayer;
		if (p is null)
			return false;
		uint current = p.ClassJob.RowId;
		foreach (uint job in jobs) {
			if (current == job)
				return true;
		}
		return false;
	}

	protected internal static uint OriginalHook(uint actionID) => Service.IconReplacer.OriginalHook(actionID);

	protected static bool IsOriginal(uint actionID) => OriginalHook(actionID) == actionID;

	#endregion

	#region Player details/stats

	protected internal static IPlayerCharacter LocalPlayer
		=> Service.Client.LocalPlayer!;

	protected internal static bool HasCondition(ConditionFlag flag)
		=> Service.Conditions[flag];

	protected internal static bool InCombat
		=> Service.Conditions[ConditionFlag.InCombat];

	protected internal static bool HasPetPresent
		=> Service.BuddyList.PetBuddy is not null;

	protected static double PlayerHealthPercentage
		=> (double)LocalPlayer.CurrentHp / LocalPlayer.MaxHp * 100.0;

	protected internal static bool ShouldSwiftcast
		=> IsOffCooldown(Common.Swiftcast)
			&& !SelfHasEffect(Common.Buffs.LostChainspell)
			&& !SelfHasEffect(RDM.Buffs.Dualcast);
	protected internal static bool IsFastcasting
		=> SelfHasEffect(Common.Buffs.Swiftcast1)
			|| SelfHasEffect(Common.Buffs.Swiftcast2)
			|| SelfHasEffect(Common.Buffs.Swiftcast3)
			|| SelfHasEffect(RDM.Buffs.Dualcast)
			|| SelfHasEffect(Common.Buffs.LostChainspell);
	protected internal static bool IsHardcasting => !IsFastcasting;

	protected internal static T GetJobGauge<T>() where T : JobGaugeBase
		=> Service.DataCache.GetJobGauge<T>();

	protected internal static unsafe bool IsMoving
		=> AgentMap.Instance() is not null && AgentMap.Instance()->IsPlayerMoving;

	#endregion

	#region Target details/stats

	protected internal static IGameObject? CurrentTarget => Service.Targets.SoftTarget ?? Service.Targets.Target;

	protected static bool HasTarget => CurrentTarget is not null;
	protected internal static bool CanInterrupt => Service.DataCache.CanInterruptTarget;

	protected internal static double TargetDistance {
		get {
			if (LocalPlayer is null || CurrentTarget is null)
				return 0;

			IGameObject target = CurrentTarget;

			Vector2 tPos = new(target.Position.X, target.Position.Z);
			Vector2 sPos = new(LocalPlayer.Position.X, LocalPlayer.Position.Z);

			return Vector2.Distance(tPos, sPos) - target.HitboxRadius - LocalPlayer.HitboxRadius;
		}
	}
	protected internal static bool InMeleeRange => TargetDistance <= 3;

	protected static double TargetCurrentHp => CurrentTarget is IBattleChara npc ? npc.CurrentHp : 0;
	protected static double TargetMaxHp => CurrentTarget is IBattleChara npc ? npc.MaxHp : 0;
	protected static double TargetHealthPercentage => CurrentTarget is IBattleChara npc ? npc.CurrentHp / npc.MaxHp * 100 : 0;

	#endregion

	#region Cooldowns and charges

	protected internal static CooldownData GetCooldown(uint actionID)
		=> Service.DataCache.GetCooldown(actionID);

	protected internal static bool IsOnCooldown(uint actionID)
		=> GetCooldown(actionID).IsCooldown;

	protected internal static bool IsOffCooldown(uint actionID)
		=> !GetCooldown(actionID).IsCooldown;

	protected internal static bool HasCharges(uint actionID)
		=> GetCooldown(actionID).HasCharges;

	protected internal static bool CanUse(uint actionID)
		=> GetCooldown(actionID).Available;

	protected internal static bool CanWeave(uint actionID, double weaveTime = 0.7)
	   => GetCooldown(actionID).CooldownRemaining > weaveTime;
	protected internal static bool CanSpellWeave(uint actionID, double weaveTime = 0.5)
		=> GetCooldown(actionID).CooldownRemaining > weaveTime && !LocalPlayer.IsCasting;

	#endregion

	#region Effects

	protected internal static Status? SelfFindEffect(ushort effectId)
		=> FindEffect(effectId, LocalPlayer, null);
	protected internal static bool SelfHasEffect(ushort effectId)
		=> SelfFindEffect(effectId) is not null;
	protected internal static float SelfEffectDuration(ushort effectId)
		=> SelfFindEffect(effectId)?.RemainingTime ?? 0;
	protected internal static float SelfEffectStacks(ushort effectId)
		=> SelfFindEffect(effectId)?.Param ?? 0;

	protected internal static Status? TargetFindAnyEffect(ushort effectId)
		=> FindEffect(effectId, CurrentTarget, null);
	protected internal static bool TargetHasAnyEffect(ushort effectId)
		=> TargetFindAnyEffect(effectId) is not null;
	protected internal static float TargetAnyEffectDuration(ushort effectId)
		=> TargetFindAnyEffect(effectId)?.RemainingTime ?? 0;
	protected internal static float TargetAnyEffectStacks(ushort effectId)
		=> TargetFindAnyEffect(effectId)?.Param ?? 0;

	protected internal static Status? TargetFindOwnEffect(ushort effectId)
		=> FindEffect(effectId, CurrentTarget, LocalPlayer?.EntityId);
	protected internal static bool TargetHasOwnEffect(ushort effectId)
		=> TargetFindOwnEffect(effectId) is not null;
	protected internal static float TargetOwnEffectDuration(ushort effectId)
		=> TargetFindOwnEffect(effectId)?.RemainingTime ?? 0;
	protected internal static float TargetOwnEffectStacks(ushort effectId)
		=> TargetFindOwnEffect(effectId)?.Param ?? 0;

	protected internal static Status? FindEffect(ushort effectId, IGameObject? actor, uint? sourceId)
		=> Service.DataCache.GetStatus(effectId, actor, sourceId);

	#endregion

	#region Job-specific utilities

	protected internal static uint DancerDancing() {
		DNCGauge gauge = GetJobGauge<DNCGauge>();

		if (gauge.IsDancing) {
			bool fast = SelfHasEffect(DNC.Buffs.StandardStep);
			int max = fast ? 2 : 4;

			return gauge.CompletedSteps >= max
				? OriginalHook(fast ? DNC.StandardStep : DNC.TechnicalStep)
				: gauge.NextStep;
		}

		return 0;
	}

	#endregion
}
