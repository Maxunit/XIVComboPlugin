using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using ImGuiNET;
using XIVComboExpandedPlugin.Attributes;

namespace XIVComboExpandedPlugin.Interface;

class Version
{
    public string GetAssemblyVersion()
    {
        return GetType().Assembly.GetName().Version.ToString();
    }
}

/// <summary>
/// Plugin configuration window.
/// </summary>
internal class ConfigWindow : Window
{
    private readonly Dictionary<string, List<(CustomComboPreset Preset, CustomComboInfoAttribute Info)>> groupedPresets;
    private readonly Dictionary<CustomComboPreset, (CustomComboPreset Preset, CustomComboInfoAttribute Info)[]> presetChildren;
    private readonly Vector4 shadedColor = new(0.68f, 0.68f, 0.68f, 1.0f);

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigWindow"/> class.
    /// </summary>
    public ConfigWindow()
        : base("Custom Combo Setup - Version: " + $"{new Version().GetAssemblyVersion()}")
    {
        this.RespectCloseHotkey = true;

        this.groupedPresets = Enum
            .GetValues<CustomComboPreset>()
            .Where(preset => (int)preset > 100 && preset != CustomComboPreset.Disabled)
            .Select(preset => (Preset: preset, Info: preset.GetAttribute<CustomComboInfoAttribute>()))
            .Where(tpl => tpl.Info != null && Service.Configuration.GetParent(tpl.Preset) == null)
            .OrderBy(tpl => tpl.Info.JobName)
            .ThenBy(tpl => tpl.Info.Order)
            .GroupBy(tpl => tpl.Info.JobName)
            .ToDictionary(
                tpl => tpl.Key,
                tpl => tpl.ToList());

        var childCombos = Enum.GetValues<CustomComboPreset>().ToDictionary(
            tpl => tpl,
            tpl => new List<CustomComboPreset>());

        foreach (var preset in Enum.GetValues<CustomComboPreset>())
        {
            var parent = preset.GetAttribute<ParentComboAttribute>()?.ParentPreset;
            if (parent != null)
                childCombos[parent.Value].Add(preset);
        }

        this.presetChildren = childCombos.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value
                .Select(preset => (Preset: preset, Info: preset.GetAttribute<CustomComboInfoAttribute>()))
                .OrderBy(tpl => tpl.Info.Order).ToArray());

        this.SizeCondition = ImGuiCond.FirstUseEver;
        this.Size = new Vector2(740, 490);
    }

    /// <inheritdoc/>
    public override void Draw()
    {
        ImGui.Text("DAWNTRAIL EDITION");
        ImGui.Text("This version of XIVCombo has been updated for Dawntrail (7.0).");
        ImGui.Text("Because of the massive amount of changes, some combos have been removed, reworked, added.");
        ImGui.Text("New combos will be added at a later date.");
        ImGui.Text("If you encounter any problems, please open an issue on github.");
        ImGui.Separator();
        ImGui.Text("Welcome to Maxunit's XIVCombo Expanded Testbed.");
        ImGui.Text("This window allows you to enable and disable custom combos to your liking.");
        ImGui.Text("For features that replace buttons with cooldowns, it is recommended to");
        ImGui.Text("use something like Job Bars or XIVAuras.");

        var showSecrets = Service.Configuration.EnableSecretCombos;
        if (ImGui.Checkbox("Daemitus's Secrets", ref showSecrets))
        {
            Service.Configuration.EnableSecretCombos = showSecrets;
            Service.Configuration.Save();
        }

        if(ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.TextUnformatted("The secrets of the creator.");
            ImGui.EndTooltip();
        }

        var showEvil = Service.Configuration.EnableEvilCombos;
        if (ImGui.Checkbox("Evil-Crab's Secrets (and others based on it)", ref showEvil))
        {
            Service.Configuration.EnableEvilCombos = showEvil;
            Service.Configuration.Save();
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.TextUnformatted("Combos that make a lot of stuff easier...and could be questionable.");
            ImGui.EndTooltip();
        }

        var hideChildren = Service.Configuration.HideChildren;
        if (ImGui.Checkbox("Hide children of disabled combos and features", ref hideChildren))
        {
            Service.Configuration.HideChildren = hideChildren;
            Service.Configuration.Save();
        }

        float offset = (float)Service.Configuration.MeleeOffset;

        var inputChangedeth = false;
        // inputChangedeth |= ImGui.InputFloat("Melee Distance Offset", ref offset);
        inputChangedeth |= ImGui.SliderFloat("Melee Distance Offset", ref offset, 0.0f, 6.0f, "%.0f");

        if (inputChangedeth)
        {
            Service.Configuration.MeleeOffset = (double)offset;
            Service.Configuration.Save();
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.TextUnformatted("Configurable melee distance offset. For those who don't want to immediately use their ranged attack if the boss walks slighty out of range.");
            ImGui.TextUnformatted("0 if you want to snuggle. 3 if you want the maximum melee distance. 4 - 6 leaves (some) room on when switching to ranged.");
            ImGui.TextUnformatted("Jobs currently using it: Tanks (more planned?)");
            ImGui.EndTooltip();
        }

        ImGui.BeginChild("scrolling", new Vector2(0, -1), true);

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 5));

        var i = 1;

        foreach (var jobName in this.groupedPresets.Keys)
        {
            if (ImGui.CollapsingHeader(jobName))
            {
                foreach (var (preset, info) in this.groupedPresets[jobName])
                {
                    this.DrawPreset(preset, info, ref i);
                }
            }
            else
            {
                i += this.groupedPresets[jobName].Count;
            }
        }

        ImGui.PopStyleVar();

        ImGui.EndChild();
    }

    private void DrawPreset(CustomComboPreset preset, CustomComboInfoAttribute info, ref int i)
    {
        var enabled = Service.Configuration.IsEnabled(preset);
        var secret = Service.Configuration.IsSecret(preset);
        var showSecrets = Service.Configuration.EnableSecretCombos;
        var evil = Service.Configuration.IsEvil(preset);
        var showEvil = Service.Configuration.EnableEvilCombos;
        var conflicts = Service.Configuration.GetConflicts(preset);
        var parent = Service.Configuration.GetParent(preset);

        if (secret && !showSecrets)
            return;

        if (evil && !showEvil)
            return;

        ImGui.PushItemWidth(200);

        if (ImGui.Checkbox(info.FancyName, ref enabled))
        {
            if (enabled)
            {
                this.EnableParentPresets(preset);
                Service.Configuration.EnabledActions.Add(preset);
                foreach (var conflict in conflicts)
                {
                    Service.Configuration.EnabledActions.Remove(conflict);
                }
            }
            else
            {
                Service.Configuration.EnabledActions.Remove(preset);
            }

            Service.Configuration.Save();
        }

        if (secret)
        {
            ImGui.SameLine();
            ImGui.Text("  ");
            ImGui.SameLine();
            ImGui.PushFont(UiBuilder.IconFont);
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
            ImGui.Text(FontAwesomeIcon.Star.ToIconString());
            ImGui.PopStyleColor();
            ImGui.PopFont();

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted("Secret");
                ImGui.EndTooltip();
            }
        }

        if (evil)
        {
            ImGui.SameLine();
            ImGui.Text("  ");
            ImGui.SameLine();
            ImGui.PushFont(UiBuilder.IconFont);
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudRed);
            ImGui.Text(FontAwesomeIcon.FireAlt.ToIconString());
            ImGui.PopStyleColor();
            ImGui.PopFont();

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted("Evil");
                ImGui.EndTooltip();
            }
        }

        ImGui.PopItemWidth();

        ImGui.PushStyleColor(ImGuiCol.Text, this.shadedColor);
        ImGui.TextWrapped($"#{i}: {info.Description}");
        ImGui.PopStyleColor();
        ImGui.Spacing();

        if (conflicts.Length > 0)
        {
            var conflictText = conflicts.Select(conflict =>
            {
                if (!showSecrets && Service.Configuration.IsSecret(conflict))
                    return string.Empty;

                var conflictInfo = conflict.GetAttribute<CustomComboInfoAttribute>();
                return $"\n - {conflictInfo.FancyName}";
            }).Aggregate((t1, t2) => $"{t1}{t2}");

            if (conflictText.Length > 0)
            {
                ImGui.TextColored(this.shadedColor, $"Conflicts with: {conflictText}");
                ImGui.Spacing();
            }
        }

        if (preset == CustomComboPreset.DancerDanceComboCompatibility && enabled)
        {
            var actions = Service.Configuration.DancerDanceCompatActionIDs.Cast<int>().ToArray();

            var inputChanged = false;
            inputChanged |= ImGui.InputInt("Emboite (Red) ActionID", ref actions[0], 0);
            inputChanged |= ImGui.InputInt("Entrechat (Blue) ActionID", ref actions[1], 0);
            inputChanged |= ImGui.InputInt("Jete (Green) ActionID", ref actions[2], 0);
            inputChanged |= ImGui.InputInt("Pirouette (Yellow) ActionID", ref actions[3], 0);

            if (inputChanged)
            {
                Service.Configuration.DancerDanceCompatActionIDs = actions.Cast<uint>().ToArray();
                Service.Configuration.Save();
            }

            ImGui.Spacing();
        }

        i++;

        var hideChildren = Service.Configuration.HideChildren;
        if (enabled || !hideChildren)
        {
            var children = this.presetChildren[preset];
            if (children.Length > 0)
            {
                ImGui.Indent();

                foreach (var (childPreset, childInfo) in children)
                    this.DrawPreset(childPreset, childInfo, ref i);

                ImGui.Unindent();
            }
        }
    }

    /// <summary>
    /// Iterates up a preset's parent tree, enabling each of them.
    /// </summary>
    /// <param name="preset">Combo preset to enabled.</param>
    private void EnableParentPresets(CustomComboPreset preset)
    {
        var parentMaybe = Service.Configuration.GetParent(preset);
        while (parentMaybe != null)
        {
            var parent = parentMaybe.Value;

            if (!Service.Configuration.EnabledActions.Contains(parent))
            {
                Service.Configuration.EnabledActions.Add(parent);
                foreach (var conflict in Service.Configuration.GetConflicts(parent))
                {
                    Service.Configuration.EnabledActions.Remove(conflict);
                }
            }

            parentMaybe = Service.Configuration.GetParent(parent);
        }
    }
}
