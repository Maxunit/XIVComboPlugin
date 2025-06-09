using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Hooking;

namespace PrincessRTFM.XIVComboVX.GameData;

internal class IconReplacer: IDisposable {

	private delegate ulong IsIconReplaceableDelegate(uint actionID);
	private delegate uint GetIconDelegate(IntPtr actionManager, uint actionID);
	private delegate IntPtr GetActionCooldownSlotDelegate(IntPtr actionManager, int cooldownGroup);

	private readonly Hook<IsIconReplaceableDelegate> isIconReplaceableHook;
	private readonly Hook<GetIconDelegate> getIconHook;

	private IntPtr actionManager = IntPtr.Zero;

	private readonly Dictionary<uint, List<CustomCombo>> comboActionMap = [];

	public IconReplacer() {
		Service.Log.Information("Loading registered combos");

		var allCombos = Assembly.GetAssembly(this.GetType())!.GetTypes()
			.Where(t => !t.IsAbstract && (t.BaseType == typeof(CustomCombo) || t.BaseType?.BaseType == typeof(CustomCombo)))
			.Select(Activator.CreateInstance)
			.Cast<CustomCombo>()
			.ToList();

		foreach (var combo in allCombos) {
			foreach (var actionId in combo.ActionIDs) {
				if (!this.comboActionMap.TryGetValue(actionId, out List<CustomCombo>? value)) {
					value = [];
					this.comboActionMap[actionId] = value;
				}

				value.Add(combo);
			}
		}

		Service.Log.Information($"Loaded {allCombos.Count} replacers for {this.comboActionMap.Count} unique actions");
#if DEBUG
		Service.Log.Information(string.Join(", ", allCombos.Select(combo => combo.GetType().Name)));
#endif

		this.getIconHook = Service.Interop.HookFromAddress<GetIconDelegate>(FFXIVClientStructs.FFXIV.Client.Game.ActionManager.Addresses.GetAdjustedActionId.Value, this.getIconDetour);
		this.isIconReplaceableHook = Service.Interop.HookFromAddress<IsIconReplaceableDelegate>(Service.Address.IsActionIdReplaceable, this.isIconReplaceableDetour);

		this.getIconHook.Enable();
		this.isIconReplaceableHook.Enable();
	}

	public void Dispose() {
		this.getIconHook?.Disable();
		this.isIconReplaceableHook?.Disable();

		this.getIconHook?.Dispose();
		this.isIconReplaceableHook?.Dispose();
	}

	private ulong isIconReplaceableDetour(uint actionID) => 1;

	/// <summary>
	/// Replace an ability with another ability
	/// actionID is the original ability to be "used"
	/// Return either actionID (itself) or a new Action table ID as the
	/// ability to take its place.
	/// I tend to make the "combo chain" button be the last move in the combo
	/// For example, Souleater combo on DRK happens by dragging Souleater
	/// onto your bar and mashing it.
	/// </summary>
	private unsafe uint getIconDetour(IntPtr actionManager, uint actionID) {
		try {
			this.actionManager = actionManager;
			IPlayerCharacter? player = Service.Client.LocalPlayer;

			if (player is null)
				return this.OriginalHook(actionID);

			if (this.comboActionMap.TryGetValue(actionID, out var relevantCombos)) {
				uint lastComboActionId = *(uint*)Service.Address.LastComboMove;
				float comboTime = *(float*)Service.Address.ComboTimer;
				byte level = player.Level;
				uint classJobID = player.ClassJob.RowId;

				foreach (CustomCombo combo in relevantCombos) {
					if (combo.TryInvoke(actionID, lastComboActionId, comboTime, level, classJobID, out uint newActionID))
						return newActionID;
				}
			}

			return this.OriginalHook(actionID);
		}
		catch (Exception ex) {
			Service.TickLogger.Error("Don't crash the game", ex);
			return this.getIconHook.Original(actionManager, actionID);
		}
	}


	public uint OriginalHook(uint actionID) => this.getIconHook.Original(this.actionManager, actionID);

}
