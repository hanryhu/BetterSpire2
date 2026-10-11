using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Saves;

namespace BetterSpire2;

internal static class NativeHandBadge
{
	private const string BadgeNodeName = "_bs2_x2_badge";

	private static readonly ConditionalWeakTable<NCard, Action> _handlers = new ConditionalWeakTable<NCard, Action>();

	public static void UpdateBadge(NCard nCard)
	{
		try
		{
			if (nCard == null || !GodotObject.IsInstanceValid(nCard))
			{
				return;
			}
			Control body = nCard.Body;
			if (body == null || !GodotObject.IsInstanceValid(body))
			{
				return;
			}
			CardModel model = nCard.Model;
			bool flag = CombatManager.Instance?.IsPaused ?? false;
			int num = ((!ModSettings.ShowNativeHandBadge || flag || model == null || nCard.DisplayingPile != PileType.Hand) ? 1 : PlayCountHelper.GetEffectivePlayCount(model));
			Label label = body.GetNodeOrNull<Label>("_bs2_x2_badge");
			if (num <= 1)
			{
				if (label != null)
				{
					label.Visible = false;
				}
				return;
			}
			if (label == null)
			{
				label = CreateBadge(body);
			}
			label.Text = $"x{num}";
			label.Visible = true;
			SaveManager instance = SaveManager.Instance;
			bool flag2 = instance != null && instance.PrefsSave?.ShowCardIndices == true;
			label.OffsetTop = (flag2 ? (-380) : (-300));
		}
		catch (System.Exception ex)
		{
			ModLog.Error("NativeHandBadge.UpdateBadge", ex);
		}
	}

	private static Label CreateBadge(Control parent)
	{
		Label label = new Label();
		label.Name = "_bs2_x2_badge";
		label.MouseFilter = Control.MouseFilterEnum.Ignore;
		label.AddThemeColorOverride("font_color", Colors.White);
		label.AddThemeColorOverride("font_outline_color", new Color(0f, 0f, 0f, 0.95f));
		label.AddThemeConstantOverride("outline_size", 8);
		label.AddThemeFontSizeOverride("font_size", 72);
		label.HorizontalAlignment = HorizontalAlignment.Center;
		label.SetAnchorsPreset(Control.LayoutPreset.CenterTop);
		label.GrowHorizontal = Control.GrowDirection.Both;
		label.GrowVertical = Control.GrowDirection.Begin;
		label.OffsetTop = -300f;
		label.ZIndex = 10;
		parent.AddChild(label, forceReadableName: false, Node.InternalMode.Disabled);
		return label;
	}

	public static void RefreshAll()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			System.Collections.Generic.IEnumerator<KeyValuePair<NCard, Action>> enumerator = ((System.Collections.Generic.IEnumerable<KeyValuePair<NCard, Action>>)_handlers).GetEnumerator();
			try
			{
				while (((System.Collections.IEnumerator)enumerator).MoveNext())
				{
					UpdateBadge(enumerator.Current.Key);
				}
			}
			finally
			{
				((System.IDisposable)enumerator)?.Dispose();
			}
		}
		catch (System.Exception ex)
		{
			ModLog.Error("NativeHandBadge.RefreshAll", ex);
		}
	}

	public static void OnEnterTree(NCard nCard)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		try
		{
			if (nCard == null)
			{
				return;
			}
			CombatHistory combatHistory = CombatManager.Instance?.History;
			Action val = default(Action);
			if (combatHistory != null && !_handlers.TryGetValue(nCard, out val))
			{
				Action val2 = (Action)delegate
				{
					UpdateBadge(nCard);
				};
				_handlers.Add(nCard, val2);
				combatHistory.Changed += val2;
			}
		}
		catch (System.Exception ex)
		{
			ModLog.Error("NativeHandBadge.OnEnterTree", ex);
		}
	}

	public static void OnExitTree(NCard nCard)
	{
		try
		{
			Action value = default(Action);
			if (nCard != null && _handlers.TryGetValue(nCard, out value))
			{
				CombatHistory combatHistory = CombatManager.Instance?.History;
				if (combatHistory != null)
				{
					combatHistory.Changed -= value;
				}
				_handlers.Remove(nCard);
			}
		}
		catch (System.Exception ex)
		{
			ModLog.Error("NativeHandBadge.OnExitTree", ex);
		}
	}
}
