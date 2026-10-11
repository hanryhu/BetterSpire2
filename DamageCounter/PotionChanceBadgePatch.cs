using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Odds;
using MegaCrit.Sts2.Core.Runs;

namespace BetterSpire2;

internal static class PotionChanceBadgePatch
{
	private const string IconName = "_bs2_potion_chance_icon";

	private const float PanelW = 110f;

	private const float PanelH = 60f;

	private const float TopOffset = 6f;

	private const float IconPx = 30f;

	private const float RightShift = 240f;

	private const float DefaultGapFromRas = 4f;

	private const int FontSizeTitle = 26;

	private const int OutlineSizeLarge = 10;

	private const int ContentMarginH = 10;

	private const int ContentMarginV = 4;

	private const string BackdropTexture = "res://images/atlases/ui_atlas.sprites/top_bar/top_bar_char_backdrop.tres";

	private const int BackdropPatch = 32;

	private const string PotionIconPath = "res://images/atlases/potion_atlas.sprites/attack_potion.tres";

	private static readonly Color OutlineColor = new Color("18282F");

	private static Control? _widgetRoot;

	private static Label? _percentLabel;

	private static bool _dragging;

	private static Vector2 _dragOffset;

	[HarmonyPatch(typeof(NTopBar), "Initialize")]
	[HarmonyPostfix]
	private static void AfterTopBarInitialize(NTopBar __instance, IRunState runState)
	{
		try
		{
			if (((System.Collections.Generic.IReadOnlyCollection<Player>)runState.Players).Count != 0)
			{
				Player player = runState.Players[0];
				float currentValue = player.PlayerOdds.PotionReward.CurrentValue;
				Control control = BuildBadge(currentValue);
				__instance.AddChild(control, forceReadableName: false, Node.InternalMode.Disabled);
				_widgetRoot = control;
				if (LayoutStore.PotionPos.HasValue)
				{
					ApplyAbsolutePosition(control, LayoutStore.PotionPos.Value);
				}
				else
				{
					ApplyDefaultPosition(control, __instance);
				}
				control.Visible = ModSettings.ShowPotionChance;
			}
		}
		catch (System.Exception ex)
		{
			ModLog.Error("PotionChanceBadge.AfterTopBarInitialize", ex);
		}
	}

	[HarmonyPatch(typeof(PotionRewardOdds), "Roll")]
	[HarmonyPostfix]
	private static void AfterPotionRoll(PotionRewardOdds __instance)
	{
		try
		{
			if (_percentLabel != null && GodotObject.IsInstanceValid(_percentLabel))
			{
				UpdateLabel(__instance.CurrentValue);
			}
		}
		catch (System.Exception ex)
		{
			ModLog.Error("PotionChanceBadge.AfterPotionRoll", ex);
		}
	}

	[HarmonyPatch(typeof(NTopBar), "_ExitTree")]
	[HarmonyPostfix]
	private static void AfterTopBarExitTree()
	{
		_widgetRoot = null;
		_percentLabel = null;
		_dragging = false;
	}

	public static void HandleMouseInput(InputEvent @event)
	{
		try
		{
			if (!ModSettings.ShowPotionChance || _widgetRoot == null || !GodotObject.IsInstanceValid(_widgetRoot) || !_widgetRoot.Visible)
			{
				return;
			}
			if (@event is InputEventMouseButton inputEventMouseButton && inputEventMouseButton.ButtonIndex == MouseButton.Left)
			{
				if (inputEventMouseButton.Pressed && !_dragging)
				{
					if (new Rect2(_widgetRoot.GlobalPosition, _widgetRoot.Size).HasPoint(inputEventMouseButton.Position))
					{
						_dragging = true;
						_dragOffset = inputEventMouseButton.Position - _widgetRoot.GlobalPosition;
					}
				}
				else if (!inputEventMouseButton.Pressed && _dragging)
				{
					_dragging = false;
					LayoutStore.PotionPos = _widgetRoot.GlobalPosition;
					LayoutStore.Save();
				}
			}
			else if (@event is InputEventMouseMotion inputEventMouseMotion && _dragging)
			{
				ApplyAbsolutePosition(_widgetRoot, inputEventMouseMotion.Position - _dragOffset);
			}
		}
		catch (System.Exception ex)
		{
			ModLog.Error("PotionChanceBadge.HandleMouseInput", ex);
		}
	}

	private static void ApplyAbsolutePosition(Control root, Vector2 pos)
	{
		root.AnchorLeft = 0f;
		root.AnchorRight = 0f;
		root.AnchorTop = 0f;
		root.AnchorBottom = 0f;
		root.OffsetLeft = pos.X;
		root.OffsetTop = pos.Y;
		root.OffsetRight = pos.X + 110f;
		root.OffsetBottom = pos.Y + 60f;
	}

	public static void RefreshVisibility()
	{
		if (_widgetRoot != null && GodotObject.IsInstanceValid(_widgetRoot))
		{
			_widgetRoot.Visible = ModSettings.ShowPotionChance;
		}
	}

	private static void ApplyDefaultPosition(Control root, Node? topBar)
	{
		Control control = ((topBar != null) ? FindControlByName(topBar, "RightAlignedStuff") : null);
		float num = ((control != null) ? (0f - control.OffsetLeft + 4f - 240f) : (-40f));
		float anchorLeft = (root.AnchorRight = 1f);
		root.AnchorLeft = anchorLeft;
		anchorLeft = (root.AnchorBottom = 0f);
		root.AnchorTop = anchorLeft;
		root.OffsetRight = 0f - num;
		root.OffsetLeft = root.OffsetRight - 110f;
		root.OffsetTop = 6f;
		root.OffsetBottom = 66f;
	}

	public static void ResetPosition()
	{
		try
		{
			LayoutStore.PotionPos = null;
			LayoutStore.Save();
			if (_widgetRoot != null && GodotObject.IsInstanceValid(_widgetRoot))
			{
				ApplyDefaultPosition(_widgetRoot, _widgetRoot.GetParent());
			}
		}
		catch (System.Exception ex)
		{
			ModLog.Error("PotionChanceBadge.ResetPosition", ex);
		}
	}

	private static Control BuildBadge(float initValue)
	{
		Control control = new Control();
		control.Name = "_bs2_potion_chance_icon";
		control.CustomMinimumSize = new Vector2(110f, 60f);
		control.MouseFilter = Control.MouseFilterEnum.Ignore;
		Texture2D texture2D = TryLoadTexture("res://images/atlases/ui_atlas.sprites/top_bar/top_bar_char_backdrop.tres");
		if (texture2D != null)
		{
			NinePatchRect ninePatchRect = new NinePatchRect();
			ninePatchRect.Texture = texture2D;
			ninePatchRect.PatchMarginLeft = 32;
			ninePatchRect.PatchMarginRight = 32;
			ninePatchRect.PatchMarginTop = 32;
			ninePatchRect.PatchMarginBottom = 32;
			ninePatchRect.AnchorRight = 1f;
			ninePatchRect.AnchorBottom = 1f;
			ninePatchRect.MouseFilter = Control.MouseFilterEnum.Ignore;
			control.AddChild(ninePatchRect, forceReadableName: false, Node.InternalMode.Disabled);
		}
		MarginContainer marginContainer = new MarginContainer();
		marginContainer.AnchorRight = 1f;
		marginContainer.AnchorBottom = 1f;
		marginContainer.AddThemeConstantOverride("margin_left", 10);
		marginContainer.AddThemeConstantOverride("margin_right", 10);
		marginContainer.AddThemeConstantOverride("margin_top", 4);
		marginContainer.AddThemeConstantOverride("margin_bottom", 4);
		marginContainer.MouseFilter = Control.MouseFilterEnum.Ignore;
		control.AddChild(marginContainer, forceReadableName: false, Node.InternalMode.Disabled);
		HBoxContainer hBoxContainer = new HBoxContainer();
		hBoxContainer.Alignment = BoxContainer.AlignmentMode.Center;
		hBoxContainer.SizeFlagsHorizontal = Control.SizeFlags.Fill;
		hBoxContainer.SizeFlagsVertical = Control.SizeFlags.Fill;
		hBoxContainer.AddThemeConstantOverride("separation", 4);
		hBoxContainer.MouseFilter = Control.MouseFilterEnum.Ignore;
		marginContainer.AddChild(hBoxContainer, forceReadableName: false, Node.InternalMode.Disabled);
		Texture2D texture2D2 = TryLoadTexture("res://images/atlases/potion_atlas.sprites/attack_potion.tres");
		if (texture2D2 != null)
		{
			TextureRect textureRect = new TextureRect();
			textureRect.Texture = texture2D2;
			textureRect.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
			textureRect.CustomMinimumSize = new Vector2(30f, 30f);
			textureRect.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
			textureRect.SizeFlagsHorizontal = Control.SizeFlags.ShrinkBegin;
			textureRect.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
			textureRect.MouseFilter = Control.MouseFilterEnum.Ignore;
			hBoxContainer.AddChild(textureRect, forceReadableName: false, Node.InternalMode.Disabled);
		}
		Label label = new Label();
		label.HorizontalAlignment = HorizontalAlignment.Left;
		label.VerticalAlignment = VerticalAlignment.Center;
		label.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
		label.AutowrapMode = TextServer.AutowrapMode.Off;
		label.MouseFilter = Control.MouseFilterEnum.Ignore;
		label.AddThemeColorOverride("font_color", Colors.White);
		label.AddThemeColorOverride("font_outline_color", OutlineColor);
		label.AddThemeConstantOverride("outline_size", 10);
		label.AddThemeFontSizeOverride("font_size", 26);
		hBoxContainer.AddChild(label, forceReadableName: false, Node.InternalMode.Disabled);
		_percentLabel = label;
		UpdateLabel(initValue);
		return control;
	}

	private static void UpdateLabel(float currentValue)
	{
		if (_percentLabel != null && GodotObject.IsInstanceValid(_percentLabel))
		{
			int num = (int)Math.Round((double)(Math.Clamp(currentValue, 0f, 1f) * 100f));
			_percentLabel.Text = $"{num}%";
		}
	}

	private static Texture2D? TryLoadTexture(string path)
	{
		try
		{
			return ResourceLoader.Load<Texture2D>(path, "", ResourceLoader.CacheMode.Reuse);
		}
		catch (System.Exception ex)
		{
			ModLog.Error("PotionChanceBadge.TryLoadTexture(" + path + ")", ex);
			return null;
		}
	}

	private static Control? FindControlByName(Node root, string name)
	{
		System.Collections.Generic.IEnumerator<Node> enumerator = root.GetChildren().GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				Node current = enumerator.Current;
				if (current.Name == name && current is Control result)
				{
					return result;
				}
				Control control = FindControlByName(current, name);
				if (control != null)
				{
					return control;
				}
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
		return null;
	}
}
