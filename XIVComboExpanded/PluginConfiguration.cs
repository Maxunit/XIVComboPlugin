using System;
using System.Collections.Generic;
using System.Linq;

using Dalamud.Configuration;
using Dalamud.Utility;
using Newtonsoft.Json;
using XIVComboExpandedPlugin.Attributes;
using XIVComboExpandedPlugin.Combos;

namespace XIVComboExpandedPlugin;

/// <summary>
/// Plugin configuration.
/// </summary>
[Serializable]
public class PluginConfiguration : IPluginConfiguration
{
    private static readonly HashSet<CustomComboPreset> SecretCombos;
    private static readonly Dictionary<CustomComboPreset, CustomComboPreset[]> ConflictingCombos;
    private static readonly Dictionary<CustomComboPreset, CustomComboPreset?> ParentCombos;  // child: parent
    private static readonly HashSet<CustomComboPreset> EvilCombo; // Evil_Crab Combos

    static PluginConfiguration()
    {
        SecretCombos = Enum.GetValues<CustomComboPreset>()
            .Where(preset => preset.GetAttribute<SecretCustomComboAttribute>() != default)
            .ToHashSet();

        ConflictingCombos = Enum.GetValues<CustomComboPreset>()
            .Distinct() // Prevent ArgumentExceptions from adding the same int twice, should not be seen anymore
            .ToDictionary(
                preset => preset,
                preset => preset.GetAttribute<ConflictingCombosAttribute>()?.ConflictingPresets ?? Array.Empty<CustomComboPreset>());

        ParentCombos = Enum.GetValues<CustomComboPreset>()
            .Distinct() // Prevent ArgumentExceptions from adding the same int twice, should not be seen anymore
            .ToDictionary(
                preset => preset,
                preset => preset.GetAttribute<ParentComboAttribute>()?.ParentPreset);

        EvilCombo = Enum.GetValues<CustomComboPreset>()
            .Where(preset => preset.GetAttribute<EvilComboAttribute>() != default)
            .ToHashSet();
    }

    /// <summary>
    /// Gets or sets the configuration version.
    /// </summary>
    public int Version { get; set; } = 5;

    /// <summary>
    /// Gets or sets the collection of enabled combos.
    /// </summary>
    [JsonProperty("EnabledActionsV5")]
    public HashSet<CustomComboPreset> EnabledActions { get; set; } = new();

    /// <summary>
    /// Gets or sets the collection of enabled combos.
    /// </summary>
    [JsonProperty("EnabledActionsV4")]
    public HashSet<CustomComboPreset> EnabledActions4 { get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether to allow and display secret combos.
    /// </summary>
    [JsonProperty("Debug")]
    public bool EnableSecretCombos { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to hide the children of a feature if it is disabled.
    /// </summary>
    public bool HideChildren { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to allow and display combos added by Evil Crab.
    /// </summary>
    [JsonProperty("Evil")]
    public bool EnableEvilCombos { get; set; } = false;

    /// <summary>
    /// Gets or sets an array of 4 ability IDs to interact with the <see cref="CustomComboPreset.DancerDanceComboCompatibility"/> combo.
    /// </summary>
    public uint[] DancerDanceCompatActionIDs { get; set; } = new uint[]
    {
        DNC.Cascade,
        DNC.Flourish,
        DNC.FanDance1,
        DNC.FanDance2,
    };

    /// <summary>
    /// Gets or sets the offset of the melee range check. Default is 0.
    /// </summary>
    public double MeleeOffset { get; set; } = 0;

    /// <summary>
    /// Save the configuration to disk.
    /// </summary>
    public void Save()
        => Service.Interface.SavePluginConfig(this);

    /// <summary>
    /// Gets a value indicating whether a preset is enabled.
    /// </summary>
    /// <param name="preset">Preset to check.</param>
    /// <returns>The boolean representation.</returns>
    public bool IsEnabled(CustomComboPreset preset)
        => this.EnabledActions.Contains(preset) && (this.EnableSecretCombos || !this.IsSecret(preset));

    /// <summary>
    /// Gets a value indicating whether a preset is secret.
    /// </summary>
    /// <param name="preset">Preset to check.</param>
    /// <returns>The boolean representation.</returns>
    public bool IsSecret(CustomComboPreset preset)
        => SecretCombos.Contains(preset);

    /// <summary>
    /// Gets an array of conflicting combo presets.
    /// </summary>
    /// <param name="preset">Preset to check.</param>
    /// <returns>The conflicting presets.</returns>
    public CustomComboPreset[] GetConflicts(CustomComboPreset preset)
        => ConflictingCombos[preset];

    /// <summary>
    /// Gets the parent combo preset if it exists, or null.
    /// </summary>
    /// <param name="preset">Preset to check.</param>
    /// <returns>The parent preset.</returns>
    public CustomComboPreset? GetParent(CustomComboPreset preset)
        => ParentCombos[preset];

    /// <summary>
    /// Gets a value indicating whether a preset is evil.
    /// </summary>
    /// <param name="preset">Preset to check.</param>
    /// <returns>The boolean representation.</returns>
    public bool IsEvil(CustomComboPreset preset)
        => EvilCombo.Contains(preset);
}
