using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using Dalamud.Utility;
using XIVComboExpandedPlugin.Attributes;

namespace XIVComboExpandedPlugin.Combos;

/// <summary>
/// Base class for each combo.
/// </summary>
internal abstract partial class CustomCombo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomCombo"/> class.
    /// </summary>
    protected CustomCombo()
    {
        var presetInfo = this.Preset.GetAttribute<CustomComboInfoAttribute>();
        this.JobID = presetInfo.JobID;
        this.ClassID = this.JobID switch
        {
            ADV.JobID => ADV.ClassID,
            BLM.JobID => BLM.ClassID,
            BRD.JobID => BRD.ClassID,
            DRG.JobID => DRG.ClassID,
            MNK.JobID => MNK.ClassID,
            NIN.JobID => NIN.ClassID,
            PLD.JobID => PLD.ClassID,
            SCH.JobID => SCH.ClassID,
            SMN.JobID => SMN.ClassID,
            WAR.JobID => WAR.ClassID,
            WHM.JobID => WHM.ClassID,
            _ => 0xFF,
        };
    }

    /// <summary>
    /// Gets the preset associated with this combo.
    /// </summary>
    protected internal abstract CustomComboPreset Preset { get; }

    /// <summary>
    /// Gets the class ID associated with this combo.
    /// </summary>
    protected byte ClassID { get; }

    /// <summary>
    /// Gets the job ID associated with this combo.
    /// </summary>
    protected byte JobID { get; }

    /// <summary>
    /// Performs various checks then attempts to invoke the combo.
    /// </summary>
    /// <param name="actionID">Starting action ID.</param>
    /// <param name="level">Player level.</param>
    /// <param name="lastComboMove">Last combo action ID.</param>
    /// <param name="comboTime">Combo timer.</param>
    /// <param name="newActionID">Replacement action ID.</param>
    /// <returns>True if the action has changed, otherwise false.</returns>
    public bool TryInvoke(uint actionID, byte level, uint lastComboMove, float comboTime, out uint newActionID)
    {
        newActionID = 0;

        if (!IsEnabled(this.Preset))
            return false;

        var classJobID = LocalPlayer!.ClassJob.Id;

        if (classJobID >= 8 && classJobID <= 15)
            classJobID = DOH.JobID;

        if (classJobID >= 16 && classJobID <= 18)
            classJobID = DOL.JobID;

        if (this.JobID != ADV.JobID && this.ClassID != ADV.ClassID &&
            this.JobID != classJobID && this.ClassID != classJobID)
            return false;

        var resultingActionID = this.Invoke(actionID, lastComboMove, comboTime, level);

        if (resultingActionID == 0 || actionID == resultingActionID)
            return false;

        newActionID = resultingActionID;
        return true;
    }

    /// <summary>
    /// Calculate the best action to use, based on cooldown remaining.
    /// If there is a tie, the original is used.
    /// </summary>
    /// <param name="original">The original action.</param>
    /// <param name="actions">Action data.</param>
    /// <returns>The appropriate action to use.</returns>
    protected static uint CalcBestAction(uint original, params uint[] actions)
    {
        static (uint ActionID, CooldownData Data) Compare(
            uint original,
            (uint ActionID, CooldownData Data) a1,
            (uint ActionID, CooldownData Data) a2)
        {
            // Neither, return the first parameter
            if (!a1.Data.IsCooldown && !a2.Data.IsCooldown)
            {
                return original == a1.ActionID ? a1 :
                       original == a2.ActionID ? a2 :
                       a1;
            }

            // Both, return soonest available
            if (a1.Data.IsCooldown && a2.Data.IsCooldown)
            {
                if (a1.Data.HasCharges && a2.Data.HasCharges)
                {
                    if (a1.Data.RemainingCharges == a2.Data.RemainingCharges)
                    {
                        return a1.Data.ChargeCooldownRemaining < a2.Data.ChargeCooldownRemaining
                            ? a1 : a2;
                    }

                    return a1.Data.RemainingCharges > a2.Data.RemainingCharges
                        ? a1 : a2;
                }
                else if (a1.Data.HasCharges)
                {
                    if (a1.Data.RemainingCharges > 0)
                        return a1;

                    return a1.Data.ChargeCooldownRemaining < a2.Data.CooldownRemaining
                        ? a1 : a2;
                }
                else if (a2.Data.HasCharges)
                {
                    if (a2.Data.RemainingCharges > 0)
                        return a2;

                    return a2.Data.ChargeCooldownRemaining < a1.Data.CooldownRemaining
                        ? a2 : a1;
                }
                else
                {
                    return a1.Data.CooldownRemaining < a2.Data.CooldownRemaining
                        ? a1 : a2;
                }
            }

            // One or the other
            return a1.Data.IsCooldown ? a2 : a1;
        }

        static (uint ActionID, CooldownData Data) Selector(uint actionID)
            => (actionID, GetCooldown(actionID));

        return actions
            .Select(Selector)
            .Aggregate((a1, a2) => Compare(original, a1, a2))
            .ActionID;
    }

    /// <summary>
    /// Invokes the combo.
    /// </summary>
    /// <param name="actionID">Starting action ID.</param>
    /// <param name="lastComboActionID">Last combo action.</param>
    /// <param name="comboTime">Current combo time.</param>
    /// <param name="level">Current player level.</param>
    /// <returns>The replacement action ID.</returns>
    protected abstract uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level);
}

/// <summary>
/// Passthrough methods and properties to IconReplacer. Shortens what it takes to call each method.
/// </summary>
internal abstract partial class CustomCombo
{
    protected internal static bool InMeleeRange
            => TargetDistance <= Service.Configuration.MeleeOffset;

    protected internal static bool InSoftMeleeRange
            => SoftTargetDistance <= Service.Configuration.MeleeOffset;

    protected internal static double SoftTargetDistance
    {
        get
        {
            if (LocalPlayer is null || SoftTarget is null)
                return 0;

            IGameObject? target = SoftTarget;

            Vector2 tPos = new(target.Position.X, target.Position.Z);
            Vector2 sPos = new(LocalPlayer.Position.X, LocalPlayer.Position.Z);

            return Vector2.Distance(tPos, sPos) - target.HitboxRadius - LocalPlayer.HitboxRadius;
        }
    }

    // Took this code from PrincessRTFM.
    // All credits goes to them, not me!
    // This seems to be more accurate and faster than the previous GetTargetDistance and InMeleeRange.
    protected internal static double TargetDistance
    {
        get
        {
            if (LocalPlayer is null || CurrentTarget is null)
                return 0;

            IGameObject? target = CurrentTarget;

            Vector2 tPos = new(target.Position.X, target.Position.Z);
            Vector2 sPos = new(LocalPlayer.Position.X, LocalPlayer.Position.Z);

            return Vector2.Distance(tPos, sPos) - target.HitboxRadius - LocalPlayer.HitboxRadius;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the target can be interrupted or not.
    /// </summary>
    /// <returns>A value indicating if the target can be interrupted by the player.</returns>
    protected static bool CanInterrupt
        => Service.ComboCache.CanInterruptTarget;

    /// <summary>
    /// Gets the current target or null.
    /// </summary>
    protected static IGameObject? CurrentTarget
        => Service.TargetManager.Target;

    /// <summary>
    /// Gets the current territory type.
    /// 1069: solo.
    /// 1075: group.
    /// 1076: savage.
    /// </summary>
    protected static ushort CurrentTerritory
        => Service.ClientState.TerritoryType;

    /// <summary>
    /// Gets the player or null.
    /// </summary>
    protected static IPlayerCharacter? LocalPlayer
        => Service.ClientState.LocalPlayer;
    /// <summary>
    /// Gets the current soft target or null.
    /// </summary>
    protected static IGameObject? SoftTarget
        => Service.TargetManager.SoftTarget;
    /// <summary>
    /// Gets bool determining if action is greyed out or not.
    /// </summary>
    /// <param name="actionID">Action ID.</param>
    /// <returns>A bool value of whether the action can be used or not.</returns>
    protected static bool CanUseAction(uint actionID) => Service.IconReplacer.CanUseAction(actionID);

    /// <summary>
    /// Finds an effect on the player.
    /// The effect must be owned by the player or unowned.
    /// </summary>
    /// <param name="effectID">Status effect ID.</param>
    /// <returns>Status object or null.</returns>
    protected static Status? FindEffect(ushort effectID)
        => FindEffect(effectID, LocalPlayer, LocalPlayer?.EntityId);

    /// <summary>
    /// Finds an effect on the given object.
    /// </summary>
    /// <param name="effectID">Status effect ID.</param>
    /// <param name="obj">Object to look for effects on.</param>
    /// <param name="sourceID">Source object ID.</param>
    /// <returns>Status object or null.</returns>
    protected static Status? FindEffect(ushort effectID, IGameObject? obj, uint? sourceID)
        => Service.ComboCache.GetStatus(effectID, obj, sourceID);

    /// <summary>
    /// Finds an effect on the player.
    /// The effect may be owned by anyone or unowned.
    /// </summary>
    /// <param name="effectID">Status effect ID.</param>
    /// <returns>Status object or null.</returns>
    protected static Status? FindEffectAny(ushort effectID)
        => FindEffect(effectID, LocalPlayer, null);

    /// <summary>
    /// Finds an effect on the current target.
    /// The effect must be owned by the player or unowned.
    /// </summary>
    /// <param name="effectID">Status effect ID.</param>
    /// <returns>Status object or null.</returns>
    protected static Status? FindTargetEffect(ushort effectID)
        => FindEffect(effectID, CurrentTarget, LocalPlayer?.EntityId);

    /// <summary>
    /// Finds an effect on the current target.
    /// The effect may be owned by anyone or unowned.
    /// </summary>
    /// <param name="effectID">Status effect ID.</param>
    /// <returns>Status object or null.</returns>
    protected static Status? FindTargetEffectAny(ushort effectID)
        => FindEffect(effectID, CurrentTarget, null);

    /// <summary>
    /// Checks to see if the GCD would not currently clip if you used a cooldown.
    /// </summary>
    /// <returns>A bool indicating if the GCD is greater-than-or-equal-to 0.5s or not.</returns>
    protected static bool GCDClipCheck() => GetCooldown(PLD.RageOfHalone).CooldownRemaining >= 0.5;

    /// <summary>
    /// Gets the cooldown data for an action.
    /// </summary>
    /// <param name="actionID">Action ID to check.</param>
    /// <returns>Cooldown data.</returns>
    protected static CooldownData GetCooldown(uint actionID)
        => Service.ComboCache.GetCooldown(actionID);

    /// <summary>
    /// Get a job gauge.
    /// </summary>
    /// <typeparam name="T">Type of job gauge.</typeparam>
    /// <returns>The job gauge.</returns>
    protected static T GetJobGauge<T>() where T : JobGaugeBase
        => Service.ComboCache.GetJobGauge<T>();

    /// <summary>
    /// Get the maximum number of charges for an action.
    /// </summary>
    /// <param name="actionID">Action ID to check.</param>
    /// <returns>Number of charges.</returns>
    protected static ushort GetMaxCharges(uint actionID)
        => GetCooldown(actionID).MaxCharges;

    /// <summary>
    /// Get the current number of charges remaining for an action.
    /// </summary>
    /// <param name="actionID">Action ID to check.</param>
    /// <returns>Number of charges.</returns>
    protected static ushort GetRemainingCharges(uint actionID)
        => GetCooldown(actionID).RemainingCharges;

    /// <summary>
    /// Gets the distance from the target.
    /// </summary>
    /// <returns>Double representing the distance from the target.</returns>
    protected static double GetTargetDistance()
    {
        if (CurrentTarget is null)
            return 0;

        if (CurrentTarget is not IBattleChara chara)
            return 0;

        double distanceX = chara.YalmDistanceX;
        double distanceY = chara.YalmDistanceZ;

        return Math.Sqrt(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2));
    }

    /// <summary>
    /// Gets a value indicating whether an action has any available charges.
    /// </summary>
    /// <param name="actionID">Action ID to check.</param>
    /// <returns>True or false.</returns>
    protected static bool HasCharges(uint actionID)
        => GetCooldown(actionID).RemainingCharges > 0;

    /// <summary>
    /// Find if the player has a certain condition.
    /// </summary>
    /// <param name="flag">Condition flag.</param>
    /// <returns>A value indicating whether the player is in the condition.</returns>
    protected static bool HasCondition(ConditionFlag flag)
        => Service.Condition[flag];

    /// <summary>
    /// Find if an effect on the player exists.
    /// The effect may be owned by the player or unowned.
    /// </summary>
    /// <param name="effectID">Status effect ID.</param>
    /// <returns>A value indicating if the effect exists.</returns>
    protected static bool HasEffect(ushort effectID)
        => FindEffect(effectID) is not null;

    /// <summary>
    /// Find if an effect on the player exists.
    /// The effect may be owned by anyone or unowned.
    /// </summary>
    /// <param name="effectID">Status effect ID.</param>
    /// <returns>A value indicating if the effect exists.</returns>
    protected static bool HasEffectAny(ushort effectID)
        => FindEffectAny(effectID) is not null;

    /// <summary>
    /// Gets a value indicating whether an action has no available charges.
    /// </summary>
    /// <param name="actionID">Action ID to check.</param>
    /// <returns>True or false.</returns>
    protected static bool HasNoCharges(uint actionID)
        => GetCooldown(actionID).RemainingCharges == 0;

    /// <summary>
    /// Find if the player has no target.
    /// </summary>
    /// <returns>A value indicating whether the player has a target.</returns>
    protected static bool HasNoTarget()
        => CurrentTarget is null;

    /// <summary>
    /// Find if the player has a pet present.
    /// </summary>
    /// <returns>A value indicating whether the player has a pet present.</returns>
    protected static bool HasPetPresent()
        => Service.BuddyList.PetBuddy != null;

    /// <summary>
    /// Gets a value indicating whether the target is a soft target or not.
    /// </summary>
    /// <returns>A value indicating if the target is a soft target or not.</returns>
    protected static bool HasSoftTarget()
        => SoftTarget is not null;

    /// <summary>
    /// Find if the player has a target.
    /// </summary>
    /// <returns>A value indicating whether the player has a target.</returns>
    protected static bool HasTarget()
        => CurrentTarget is not null;

    /// <summary>
    /// Find if the player is in combat.
    /// </summary>
    /// <returns>A value indicating whether the player is in combat.</returns>
    protected static bool InCombat()
        => Service.Condition[ConditionFlag.InCombat];

    /// <summary>
    /// Determine if the given preset is enabled.
    /// </summary>
    /// <param name="preset">Preset to check.</param>
    /// <returns>A value indicating whether the preset is enabled.</returns>
    protected static bool IsEnabled(CustomComboPreset preset)
        => (int)preset < 100 || Service.Configuration.IsEnabled(preset);

    /// <summary>
    /// Determine if the given preset is not enabled.
    /// </summary>
    /// <param name="preset">Preset to check.</param>
    /// <returns>A value indicating whether the preset is not enabled.</returns>
    protected static bool IsNotEnabled(CustomComboPreset preset)
        => !IsEnabled(preset);

    /// <summary>
    /// Gets a value indicating whether an action is off cooldown.
    /// </summary>
    /// <param name="actionID">Action ID to check.</param>
    /// <returns>True or false.</returns>
    protected static bool IsOffCooldown(uint actionID)
        => !GetCooldown(actionID).IsCooldown;

    /// <summary>
    /// Gets a value indicating whether an action is on cooldown.
    /// </summary>
    /// <param name="actionID">Action ID to check.</param>
    /// <returns>True or false.</returns>
    protected static bool IsOnCooldown(uint actionID)
        => GetCooldown(actionID).IsCooldown;

    /// <summary>
    /// Compare the original hook to the given action ID.
    /// </summary>
    /// <param name="actionID">Action ID.</param>
    /// <returns>A value indicating whether the action would be modified.</returns>
    protected static bool IsOriginal(uint actionID)
        => Service.IconReplacer.OriginalHook(actionID) == actionID;

    /// <summary>
    /// Calls the original hook.
    /// </summary>
    /// <param name="actionID">Action ID.</param>
    /// <returns>The result from the hook.</returns>
    protected static uint OriginalHook(uint actionID)
        => Service.IconReplacer.OriginalHook(actionID);
    /// <summary>
    /// Find if the player is not in combat.
    /// </summary>
    /// <returns>A value indicating whether the player is not in combat.</returns>
    protected static bool OutOfCombat()
        => !InCombat();
    protected static uint PickByCooldown(uint original, params uint[] actions)
    {
        static (uint ActionID, CooldownData Data) Compare(uint original, (uint ActionID, CooldownData Data) a1, (uint ActionID, CooldownData Data) a2)
        {
            // Neither on cooldown, return the original (or the last one provided)
            if (!a1.Data.IsCooldown && !a2.Data.IsCooldown)
                return original == a1.ActionID ? a1 : a2;

            // Both on cooldown, return soonest available
            if (a1.Data.IsCooldown && a2.Data.IsCooldown)
                return a1.Data.CooldownRemaining < a2.Data.CooldownRemaining ? a1 : a2;

            // Return whatever's not on cooldown
            return a1.Data.IsCooldown ? a2 : a1;
        }

        static (uint ActionID, CooldownData Data) Selector(uint actionID) => (actionID, GetCooldown(actionID));

        return actions
            .Select(Selector)
            .Aggregate((a1, a2) => Compare(original, a1, a2))
            .ActionID;
    }

    /// <summary>
    /// Find if an effect on the target exists.
    /// The effect must be owned by the player or unowned.
    /// </summary>
    /// <param name="effectID">Status effect ID.</param>
    /// <returns>A value indicating if the effect exists.</returns>
    protected static bool TargetHasEffect(ushort effectID)
        => FindTargetEffect(effectID) is not null;

    /// <summary>
    /// Find if an effect on the target exists.
    /// The effect may be owned by anyone or unowned.
    /// </summary>
    /// <param name="effectID">Status effect ID.</param>
    /// <returns>A value indicating if the effect exists.</returns>
    protected static bool TargetHasEffectAny(ushort effectID)
        => FindTargetEffectAny(effectID) is not null;

    /// <summary>
    /// Find if the current target is an enemy.
    /// </summary>
    /// <returns>A value indicating whether the target is an enemy.</returns>
    protected static bool TargetIsEnemy()
        => HasTarget() && CurrentTarget?.ObjectKind == ObjectKind.BattleNpc && CurrentTarget?.SubKind == 5;
    /*
    /// <summary>
    /// Gets a value indicating whether you are in melee range from the current target.
    /// </summary>
    /// <returns>Bool indicating whether you are in melee range.</returns>
    protected static bool InMeleeRange()
    {
        var distance = GetTargetDistance();

        if (distance == 0)
            return true;

        if (distance > 3)
            return false;

        return true;
    }

    #region SGE settings

    /// <summary>
    /// Gets a value indicating whether you are in melee range from the current target or not and if Icarus should be used.
    /// This is a bit misleading, since I butchered some code. Eh...whatever.
    /// </summary>
    /// <returns>Bool indicating whether you are in melee range.</returns>
    protected static bool InIcarusRange()
    {
        var distance = GetTargetDistance();

        if (distance < 8)
            return true;

        if (distance > 8)
            return false;

        return true;
    }
    #endregion
    */
}