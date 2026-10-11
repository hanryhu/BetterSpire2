using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Runs;

namespace BetterSpire2;

[HarmonyPatch(typeof(NNormalMapPoint), "_Ready")]
internal static class MapPointRemovePriceTooltipPatch
{
	private const string TooltipName = "_bs2_remove_price_tooltip";

	private const string GoldIconPath = "res://images/packed/sprite_fonts/gold_icon.png";

	private static void Postfix(NNormalMapPoint __instance)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		try
		{
			MapPoint point = __instance.Point;
			if (point == null || point.PointType != MapPointType.Shop || __instance.GetNodeOrNull<Control>("_bs2_remove_price_tooltip") != null)
			{
				return;
			}
			PanelContainer tooltip = BuildTooltip();
			__instance.AddChild(tooltip, forceReadableName: false, Node.InternalMode.Disabled);
			tooltip.Visible = false;
			__instance.MouseEntered += (Action)delegate
			{
				if (GodotObject.IsInstanceValid(tooltip))
				{
					RefreshCost(tooltip);
					tooltip.Visible = true;
				}
			};
			__instance.MouseExited += (Action)delegate
			{
				if (GodotObject.IsInstanceValid(tooltip))
				{
					tooltip.Visible = false;
				}
			};
		}
		catch (System.Exception ex)
		{
			ModLog.Error("MapPointRemovePriceTooltipPatch._Ready", ex);
		}
	}

	private static PanelContainer BuildTooltip()
	{
		PanelContainer panelContainer = new PanelContainer();
		panelContainer.Name = "_bs2_remove_price_tooltip";
		panelContainer.MouseFilter = Control.MouseFilterEnum.Ignore;
		StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
		styleBoxFlat.BgColor = new Color(0.08f, 0.08f, 0.12f, 0.95f);
		styleBoxFlat.BorderColor = new Color(0.6f, 0.5f, 0.2f);
		styleBoxFlat.SetBorderWidthAll(1);
		styleBoxFlat.SetCornerRadiusAll(4);
		styleBoxFlat.SetContentMarginAll(6f);
		panelContainer.AddThemeStyleboxOverride("panel", styleBoxFlat);
		panelContainer.SetAnchorsPreset(Control.LayoutPreset.CenterTop);
		panelContainer.GrowHorizontal = Control.GrowDirection.Both;
		panelContainer.GrowVertical = Control.GrowDirection.Begin;
		panelContainer.OffsetTop = -12f;
		panelContainer.ZIndex = 20;
		HBoxContainer hBoxContainer = new HBoxContainer();
		hBoxContainer.AddThemeConstantOverride("separation", 4);
		hBoxContainer.MouseFilter = Control.MouseFilterEnum.Ignore;
		panelContainer.AddChild(hBoxContainer, forceReadableName: false, Node.InternalMode.Disabled);
		hBoxContainer.AddChild(BuildIcon("scissors", SafeLoadScissors(), 22), forceReadableName: false, Node.InternalMode.Disabled);
		Label label = new Label();
		label.Name = "cost";
		label.AddThemeFontSizeOverride("font_size", 18);
		label.AddThemeColorOverride("font_color", Colors.White);
		label.VerticalAlignment = VerticalAlignment.Center;
		label.MouseFilter = Control.MouseFilterEnum.Ignore;
		hBoxContainer.AddChild(label, forceReadableName: false, Node.InternalMode.Disabled);
		hBoxContainer.AddChild(BuildIcon("gold", SafeLoad("res://images/packed/sprite_fonts/gold_icon.png"), 20), forceReadableName: false, Node.InternalMode.Disabled);
		return panelContainer;
	}

	private static TextureRect BuildIcon(string name, Texture2D? tex, int size)
	{
		TextureRect textureRect = new TextureRect();
		textureRect.Name = name;
		textureRect.CustomMinimumSize = new Vector2(size, size);
		textureRect.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
		textureRect.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
		textureRect.MouseFilter = Control.MouseFilterEnum.Ignore;
		if (tex != null)
		{
			textureRect.Texture = tex;
		}
		return textureRect;
	}

	private static Texture2D? SafeLoadScissors()
	{
		try
		{
			return ModelDb.Relic<PreciseScissors>()?.Icon;
		}
		catch
		{
			return null;
		}
	}

	private static Texture2D? SafeLoad(string path)
	{
		try
		{
			return GD.Load<Texture2D>(path);
		}
		catch
		{
			return null;
		}
	}

	private static void RefreshCost(PanelContainer tooltip)
	{
		try
		{
			Label label = tooltip.GetNodeOrNull<Label>("HBoxContainer/cost") ?? FindLabelRecursive(tooltip);
			if (label != null)
			{
				int num = CalcCost();
				label.Text = ((num >= 0) ? $"- {num}" : "- ?");
			}
		}
		catch (System.Exception ex)
		{
			ModLog.Error("MapPointRemovePriceTooltipPatch.RefreshCost", ex);
		}
	}

	private static Label? FindLabelRecursive(Node node)
	{
		System.Collections.Generic.IEnumerator<Node> enumerator = node.GetChildren().GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				Node current = enumerator.Current;
				if (current is Label label && label.Name == "cost")
				{
					return label;
				}
				Label label2 = FindLabelRecursive(current);
				if (label2 != null)
				{
					return label2;
				}
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
		return null;
	}

	private static int CalcCost()
	{
		try
		{
			RunManager instance = RunManager.Instance;
			RunState runState = instance?.DebugOnlyGetState();
			if (runState == null)
			{
				return -1;
			}
			Player me = LocalContext.GetMe(runState);
			if (me == null)
			{
				return -1;
			}
			bool flag = instance.HasAscension(AscensionLevel.Inflation);
			int num = (flag ? 100 : 75);
			int num2 = (flag ? 50 : 25);
			decimal result = (decimal)(num + num2 * me.ExtraFields.CardShopRemovalsUsed);
			MerchantCardRemovalEntry entry = new MerchantCardRemovalEntry(me);
			result = Hook.ModifyMerchantPrice(runState, me, entry, result);
			return (int)result;
		}
		catch
		{
			return -1;
		}
	}
}
