using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Runs;

namespace BetterSpire2;

public static class SettingsMenu
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Action _003C_003E9__9_0;

		public static Action<bool> _003C_003E9__9_1;

		public static Action<bool> _003C_003E9__9_2;

		public static Action<bool> _003C_003E9__9_3;

		public static Action _003C_003E9__9_4;

		public static Action<bool> _003C_003E9__9_5;

		public static Action<bool> _003C_003E9__9_6;

		public static Action _003C_003E9__9_7;

		public static Action<bool> _003C_003E9__9_8;

		public static Action<bool> _003C_003E9__9_9;

		public static Action<bool> _003C_003E9__9_10;

		public static Action<bool> _003C_003E9__9_11;

		public static Action<bool> _003C_003E9__9_12;

		public static Action<bool> _003C_003E9__9_13;

		public static Action<int> _003C_003E9__9_14;

		public static Action<int> _003C_003E9__9_15;

		public static Action _003C_003E9__9_16;

		public static Action<bool> _003C_003E9__9_17;

		public static Action<bool> _003C_003E9__9_18;

		public static Action<bool> _003C_003E9__9_19;

		public static Action<bool> _003C_003E9__9_20;

		public static Action<bool> _003C_003E9__9_21;

		public static OptionButton.ItemSelectedEventHandler _003C_003E9__19_0;

		public static Action _003C_003E9__27_0;

		internal void _003CShow_003Eb__9_0()
		{
			Hide();
		}

		internal void _003CShow_003Eb__9_1(bool v)
		{
			ModSettings.MultiHitTotals = v;
			ModSettings.Save();
			RefreshIntents();
		}

		internal void _003CShow_003Eb__9_2(bool v)
		{
			ModSettings.PlayerDamageTotal = v;
			ModSettings.Save();
			if (v)
			{
				DamageTracker.Recalculate();
			}
			else
			{
				DamageTracker.Hide();
			}
		}

		internal void _003CShow_003Eb__9_3(bool v)
		{
			ModSettings.ShowExpectedHp = v;
			ModSettings.Save();
			DamageTracker.Recalculate();
		}

		internal void _003CShow_003Eb__9_4()
		{
			DamageTracker.ResetDragOffset();
		}

		internal void _003CShow_003Eb__9_5(bool v)
		{
			ModSettings.ShowNativeHandBadge = v;
			ModSettings.Save();
			NativeHandBadge.RefreshAll();
		}

		internal void _003CShow_003Eb__9_6(bool v)
		{
			ModSettings.ShowPotionChance = v;
			ModSettings.Save();
			PotionChanceBadgePatch.RefreshVisibility();
		}

		internal void _003CShow_003Eb__9_7()
		{
			PotionChanceBadgePatch.ResetPosition();
		}

		internal void _003CShow_003Eb__9_8(bool v)
		{
			ModSettings.ShowTeammateHand = v;
			ModSettings.Save();
			if (!v)
			{
				DeckTracker.Hide();
			}
		}

		internal void _003CShow_003Eb__9_9(bool v)
		{
			ModSettings.AlwaysShowTeammateHand = v;
			ModSettings.Save();
		}

		internal void _003CShow_003Eb__9_10(bool v)
		{
			ModSettings.AutoShowTeammateHand = v;
			ModSettings.Save();
		}

		internal void _003CShow_003Eb__9_11(bool v)
		{
			ModSettings.HideOwnHand = v;
			ModSettings.Save();
			DeckTracker.RefreshIfVisible();
		}

		internal void _003CShow_003Eb__9_12(bool v)
		{
			ModSettings.CompactHandViewer = v;
			ModSettings.Save();
			DeckTracker.RefreshIfVisible();
		}

		internal void _003CShow_003Eb__9_13(bool v)
		{
			ModSettings.HandViewerPopout = v;
			ModSettings.Save();
			if (DeckTracker.IsVisible)
			{
				DeckTracker.Hide();
				DeckTracker.ShowIfInCombat();
			}
		}

		internal void _003CShow_003Eb__9_14(int v)
		{
			ModSettings.CardScalePercent = v;
			ModSettings.Save();
			DeckTracker.RefreshIfVisible();
		}

		internal void _003CShow_003Eb__9_15(int v)
		{
			ModSettings.HandViewerOpacityPercent = v;
			ModSettings.Save();
			DeckTracker.RefreshIfVisible();
		}

		internal void _003CShow_003Eb__9_16()
		{
			DeckTracker.ResetPosition();
		}

		internal void _003CShow_003Eb__9_17(bool v)
		{
			ModSettings.InstantFastMode = v;
			ModSettings.Save();
			if (!v)
			{
				InstantSpeedHelper.OnCombatEnd();
			}
			else if (CombatManager.Instance?.DebugOnlyGetState() != null)
			{
				InstantSpeedHelper.OnCombatStart();
			}
			ModLog.Info($"Instant Fast Mode (combat-only): {v}");
		}

		internal void _003CShow_003Eb__9_18(bool v)
		{
			ModSettings.AllowInstantInMultiplayer = v;
			ModSettings.Save();
			if (CombatManager.Instance?.DebugOnlyGetState() != null)
			{
				InstantSpeedHelper.OnCombatEnd();
				if (ModSettings.InstantFastMode)
				{
					InstantSpeedHelper.OnCombatStart();
				}
			}
			ModLog.Info($"Allow Instant in Multiplayer: {v}");
		}

		internal void _003CShow_003Eb__9_19(bool v)
		{
			ModSettings.SkipSplash = v;
			ModSettings.Save();
		}

		internal void _003CShow_003Eb__9_20(bool v)
		{
			ModSettings.ShowClock = v;
			ModSettings.Save();
			ClockDisplay.Toggle(v);
		}

		internal void _003CShow_003Eb__9_21(bool v)
		{
			ModSettings.Clock24Hour = v;
			ModSettings.Save();
		}

		internal void _003CAddLanguageRow_003Eb__19_0(long idx)
		{
			int num = (int)idx;
			if (num >= 0 && num < Localization.AvailableLanguages.Length)
			{
				ModSettings.LanguageOverride = Localization.AvailableLanguages[num].Item1;
				ModSettings.Save();
				Localization.Reload();
				Hide();
				Show();
			}
		}

		internal void _003CBuildPartySection_003Eb__27_0()
		{
			PartyManager.ClearAllDrawings();
		}
	}

	private static PanelContainer? _panel;

	private static CanvasLayer? _canvasLayer;

	private static bool _visible;

	private static Vector2? _savedPosition;

	private static bool _dragging;

	private static Vector2 _dragOffset;

	private static Button? _rebindButton;

	private static bool _awaitingRebind;

	private static bool _spLookupAttempted;

	private static PropertyInfo? _spProp;

	public static bool IsCapturingHotkey => _awaitingRebind && _rebindButton != null;

	public static void Toggle()
	{
		if (_visible)
		{
			Hide();
		}
		else
		{
			Show();
		}
	}

	public static void SetCanvasHidden(bool hidden)
	{
		if (_canvasLayer != null && GodotObject.IsInstanceValid(_canvasLayer))
		{
			_canvasLayer.Visible = !hidden;
		}
	}

	public static void HandleMouseInput(InputEvent @event)
	{
		if (!_visible || _panel == null || !GodotObject.IsInstanceValid(_panel))
		{
			return;
		}
		if (@event is InputEventMouseButton inputEventMouseButton && inputEventMouseButton.ButtonIndex == MouseButton.Left)
		{
			Vector2 position = inputEventMouseButton.Position;
			Rect2 globalRect = _panel.GetGlobalRect();
			if (inputEventMouseButton.Pressed)
			{
				if (globalRect.HasPoint(position))
				{
					_dragging = true;
					_dragOffset = position - _panel.Position;
				}
				else if (!DeckTracker.IsPointInPanel(position))
				{
					_dragging = false;
					Hide();
				}
			}
			else
			{
				_dragging = false;
			}
		}
		else if (@event is InputEventMouseMotion inputEventMouseMotion && _dragging)
		{
			_panel.Position = inputEventMouseMotion.Position - _dragOffset;
		}
	}

	private static void Show()
	{
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Expected O, but got Unknown
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Expected O, but got Unknown
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_057f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Expected O, but got Unknown
		//IL_077b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0780: Unknown result type (might be due to invalid IL or missing references)
		//IL_0786: Expected O, but got Unknown
		if (_visible)
		{
			Hide();
		}
		NGame instance = NGame.Instance;
		if (instance == null)
		{
			return;
		}
		_canvasLayer = new CanvasLayer();
		_canvasLayer.Layer = 10;
		CombatManager instance2 = CombatManager.Instance;
		if (instance2 != null && instance2.IsPaused)
		{
			_canvasLayer.Visible = false;
		}
		instance.AddChild(_canvasLayer, forceReadableName: false, Node.InternalMode.Disabled);
		_panel = new PanelContainer();
		_panel.ZIndex = 200;
		StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
		styleBoxFlat.BgColor = new Color(0.1f, 0.1f, 0.15f, 0.95f);
		styleBoxFlat.BorderColor = new Color(0.8f, 0.6f, 0.2f);
		styleBoxFlat.SetBorderWidthAll(2);
		styleBoxFlat.SetCornerRadiusAll(8);
		styleBoxFlat.SetContentMarginAll(16f);
		_panel.AddThemeStyleboxOverride("panel", styleBoxFlat);
		VBoxContainer vBoxContainer = new VBoxContainer();
		vBoxContainer.AddThemeConstantOverride("separation", 10);
		_panel.AddChild(vBoxContainer, forceReadableName: false, Node.InternalMode.Disabled);
		HBoxContainer hBoxContainer = new HBoxContainer();
		hBoxContainer.AddThemeConstantOverride("separation", 8);
		vBoxContainer.AddChild(hBoxContainer, forceReadableName: false, Node.InternalMode.Disabled);
		Label label = new Label();
		label.Text = Localization.Get("settings.title_lite");
		label.HorizontalAlignment = HorizontalAlignment.Center;
		label.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		label.MouseFilter = Control.MouseFilterEnum.Ignore;
		label.AddThemeColorOverride("font_color", new Color(0.9f, 0.7f, 0.2f));
		label.AddThemeFontSizeOverride("font_size", 20);
		hBoxContainer.AddChild(label, forceReadableName: false, Node.InternalMode.Disabled);
		Button button = new Button();
		button.Text = "X";
		button.CustomMinimumSize = new Vector2(28f, 28f);
		object obj = _003C_003Ec._003C_003E9__9_0;
		if (obj == null)
		{
			Action val = delegate
			{
				Hide();
			};
			_003C_003Ec._003C_003E9__9_0 = val;
			obj = (object)val;
		}
		button.Pressed += (Action)obj;
		StyleBoxFlat styleBoxFlat2 = new StyleBoxFlat();
		styleBoxFlat2.BgColor = new Color(0.3f, 0.1f, 0.1f);
		styleBoxFlat2.BorderColor = new Color(0.6f, 0.2f, 0.2f);
		styleBoxFlat2.SetBorderWidthAll(1);
		styleBoxFlat2.SetCornerRadiusAll(4);
		styleBoxFlat2.SetContentMarginAll(2f);
		button.AddThemeStyleboxOverride("normal", styleBoxFlat2);
		StyleBoxFlat styleBoxFlat3 = new StyleBoxFlat();
		styleBoxFlat3.BgColor = new Color(0.5f, 0.15f, 0.15f);
		styleBoxFlat3.BorderColor = new Color(0.8f, 0.3f, 0.3f);
		styleBoxFlat3.SetBorderWidthAll(1);
		styleBoxFlat3.SetCornerRadiusAll(4);
		styleBoxFlat3.SetContentMarginAll(2f);
		button.AddThemeStyleboxOverride("hover", styleBoxFlat3);
		button.AddThemeStyleboxOverride("pressed", styleBoxFlat3);
		button.AddThemeColorOverride("font_color", new Color(0.9f, 0.4f, 0.4f));
		button.AddThemeFontSizeOverride("font_size", 14);
		hBoxContainer.AddChild(button, forceReadableName: false, Node.InternalMode.Disabled);
		HBoxContainer hBoxContainer2 = new HBoxContainer();
		hBoxContainer2.AddThemeConstantOverride("separation", 24);
		vBoxContainer.AddChild(hBoxContainer2, forceReadableName: false, Node.InternalMode.Disabled);
		VBoxContainer vBoxContainer2 = new VBoxContainer();
		vBoxContainer2.AddThemeConstantOverride("separation", 10);
		vBoxContainer2.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		hBoxContainer2.AddChild(vBoxContainer2, forceReadableName: false, Node.InternalMode.Disabled);
		VBoxContainer vBoxContainer3 = new VBoxContainer();
		vBoxContainer3.AddThemeConstantOverride("separation", 10);
		vBoxContainer3.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		hBoxContainer2.AddChild(vBoxContainer3, forceReadableName: false, Node.InternalMode.Disabled);
		AddSectionHeader(vBoxContainer2, Localization.Get("section.combat_hud"));
		AddToggle(vBoxContainer2, Localization.Get("setting.multi_hit_totals"), ModSettings.MultiHitTotals, delegate(bool v)
		{
			ModSettings.MultiHitTotals = v;
			ModSettings.Save();
			RefreshIntents();
		});
		AddToggle(vBoxContainer2, Localization.Get("setting.player_damage_total"), ModSettings.PlayerDamageTotal, delegate(bool v)
		{
			ModSettings.PlayerDamageTotal = v;
			ModSettings.Save();
			if (v)
			{
				DamageTracker.Recalculate();
			}
			else
			{
				DamageTracker.Hide();
			}
		});
		AddToggle(vBoxContainer2, Localization.Get("setting.show_expected_hp"), ModSettings.ShowExpectedHp, delegate(bool v)
		{
			ModSettings.ShowExpectedHp = v;
			ModSettings.Save();
			DamageTracker.Recalculate();
		});
		string label2 = Localization.Get("setting.reset_damage_position");
		object obj2 = _003C_003Ec._003C_003E9__9_4;
		if (obj2 == null)
		{
			Action val2 = delegate
			{
				DamageTracker.ResetDragOffset();
			};
			_003C_003Ec._003C_003E9__9_4 = val2;
			obj2 = (object)val2;
		}
		AddButtonRow(vBoxContainer2, label2, (Action)obj2);
		AddToggle(vBoxContainer2, Localization.Get("setting.native_hand_badge"), ModSettings.ShowNativeHandBadge, delegate(bool v)
		{
			ModSettings.ShowNativeHandBadge = v;
			ModSettings.Save();
			NativeHandBadge.RefreshAll();
		});
		AddToggle(vBoxContainer2, Localization.Get("setting.show_potion_chance"), ModSettings.ShowPotionChance, delegate(bool v)
		{
			ModSettings.ShowPotionChance = v;
			ModSettings.Save();
			PotionChanceBadgePatch.RefreshVisibility();
		});
		string label3 = Localization.Get("setting.reset_potion_position");
		object obj3 = _003C_003Ec._003C_003E9__9_7;
		if (obj3 == null)
		{
			Action val3 = delegate
			{
				PotionChanceBadgePatch.ResetPosition();
			};
			_003C_003Ec._003C_003E9__9_7 = val3;
			obj3 = (object)val3;
		}
		AddButtonRow(vBoxContainer2, label3, (Action)obj3);
		AddSectionHeader(vBoxContainer2, Localization.Get("section.multiplayer"));
		AddToggle(vBoxContainer2, Localization.Get("setting.show_teammate_hand"), ModSettings.ShowTeammateHand, delegate(bool v)
		{
			ModSettings.ShowTeammateHand = v;
			ModSettings.Save();
			if (!v)
			{
				DeckTracker.Hide();
			}
		});
		AddToggle(vBoxContainer2, Localization.Get("setting.always_show"), ModSettings.AlwaysShowTeammateHand, delegate(bool v)
		{
			ModSettings.AlwaysShowTeammateHand = v;
			ModSettings.Save();
		});
		AddToggle(vBoxContainer2, Localization.Get("setting.auto_show"), ModSettings.AutoShowTeammateHand, delegate(bool v)
		{
			ModSettings.AutoShowTeammateHand = v;
			ModSettings.Save();
		});
		AddToggle(vBoxContainer2, Localization.Get("setting.hide_own_hand"), ModSettings.HideOwnHand, delegate(bool v)
		{
			ModSettings.HideOwnHand = v;
			ModSettings.Save();
			DeckTracker.RefreshIfVisible();
		});
		AddToggle(vBoxContainer2, Localization.Get("setting.compact_view"), ModSettings.CompactHandViewer, delegate(bool v)
		{
			ModSettings.CompactHandViewer = v;
			ModSettings.Save();
			DeckTracker.RefreshIfVisible();
		});
		AddToggle(vBoxContainer2, Localization.Get("setting.popout_window"), ModSettings.HandViewerPopout, delegate(bool v)
		{
			ModSettings.HandViewerPopout = v;
			ModSettings.Save();
			if (DeckTracker.IsVisible)
			{
				DeckTracker.Hide();
				DeckTracker.ShowIfInCombat();
			}
		});
		AddStepperRow(vBoxContainer2, Localization.Get("setting.card_size"), ModSettings.CardScalePercent, 50, 200, 10, delegate(int v)
		{
			ModSettings.CardScalePercent = v;
			ModSettings.Save();
			DeckTracker.RefreshIfVisible();
		});
		AddStepperRow(vBoxContainer2, Localization.Get("setting.opacity"), ModSettings.HandViewerOpacityPercent, 0, 100, 10, delegate(int v)
		{
			ModSettings.HandViewerOpacityPercent = v;
			ModSettings.Save();
			DeckTracker.RefreshIfVisible();
		});
		string label4 = Localization.Get("setting.reset_position");
		object obj4 = _003C_003Ec._003C_003E9__9_16;
		if (obj4 == null)
		{
			Action val4 = delegate
			{
				DeckTracker.ResetPosition();
			};
			_003C_003Ec._003C_003E9__9_16 = val4;
			obj4 = (object)val4;
		}
		AddButtonRow(vBoxContainer2, label4, (Action)obj4);
		AddSectionHeader(vBoxContainer3, Localization.Get("section.gameplay"));
		AddToggle(vBoxContainer3, Localization.Get("setting.instant_fast_mode"), ModSettings.InstantFastMode, delegate(bool v)
		{
			ModSettings.InstantFastMode = v;
			ModSettings.Save();
			if (!v)
			{
				InstantSpeedHelper.OnCombatEnd();
			}
			else if (CombatManager.Instance?.DebugOnlyGetState() != null)
			{
				InstantSpeedHelper.OnCombatStart();
			}
			ModLog.Info($"Instant Fast Mode (combat-only): {v}");
		});
		AddToggle(vBoxContainer3, Localization.Get("setting.allow_instant_mp"), ModSettings.AllowInstantInMultiplayer, delegate(bool v)
		{
			ModSettings.AllowInstantInMultiplayer = v;
			ModSettings.Save();
			if (CombatManager.Instance?.DebugOnlyGetState() != null)
			{
				InstantSpeedHelper.OnCombatEnd();
				if (ModSettings.InstantFastMode)
				{
					InstantSpeedHelper.OnCombatStart();
				}
			}
			ModLog.Info($"Allow Instant in Multiplayer: {v}");
		});
		AddToggle(vBoxContainer3, Localization.Get("setting.skip_splash"), ModSettings.SkipSplash, delegate(bool v)
		{
			ModSettings.SkipSplash = v;
			ModSettings.Save();
		});
		AddToggle(vBoxContainer3, Localization.Get("setting.show_clock"), ModSettings.ShowClock, delegate(bool v)
		{
			ModSettings.ShowClock = v;
			ModSettings.Save();
			ClockDisplay.Toggle(v);
		});
		AddToggle(vBoxContainer3, "  · " + Localization.Get("setting.clock_24h"), ModSettings.Clock24Hour, delegate(bool v)
		{
			ModSettings.Clock24Hour = v;
			ModSettings.Save();
		});
		AddSectionHeader(vBoxContainer3, Localization.Get("section.hotkey"));
		AddSettingsHotkeyRow(vBoxContainer3);
		AddSectionHeader(vBoxContainer3, Localization.Get("section.language"));
		AddLanguageRow(vBoxContainer3);
		BuildPartySection(vBoxContainer);
		Label label5 = new Label();
		label5.Text = Localization.Get("settings.hint");
		label5.HorizontalAlignment = HorizontalAlignment.Center;
		label5.MouseFilter = Control.MouseFilterEnum.Ignore;
		label5.AddThemeColorOverride("font_color", new Color(0.5f, 0.5f, 0.5f));
		label5.AddThemeFontSizeOverride("font_size", 14);
		vBoxContainer.AddChild(label5, forceReadableName: false, Node.InternalMode.Disabled);
		_canvasLayer.AddChild(_panel, forceReadableName: false, Node.InternalMode.Disabled);
		if (_savedPosition.HasValue)
		{
			_panel.Position = _savedPosition.Value;
		}
		else
		{
			Vector2 vector = instance.GetViewport()?.GetVisibleRect().Size ?? new Vector2(1920f, 1080f);
			_panel.CallDeferred("set_position", new Vector2(vector.X / 2f - 200f, vector.Y / 2f - 200f));
		}
		_visible = true;
	}

	private static void AddSectionHeader(VBoxContainer parent, string text)
	{
		VBoxContainer vBoxContainer = new VBoxContainer();
		vBoxContainer.AddThemeConstantOverride("separation", 2);
		Label label = new Label();
		label.Text = text;
		label.HorizontalAlignment = HorizontalAlignment.Left;
		label.MouseFilter = Control.MouseFilterEnum.Ignore;
		label.AddThemeColorOverride("font_color", new Color(0.7f, 0.55f, 0.2f));
		label.AddThemeFontSizeOverride("font_size", 15);
		vBoxContainer.AddChild(label, forceReadableName: false, Node.InternalMode.Disabled);
		HSeparator hSeparator = new HSeparator();
		hSeparator.AddThemeConstantOverride("separation", 0);
		StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
		styleBoxFlat.BgColor = new Color(0.5f, 0.4f, 0.15f, 0.6f);
		styleBoxFlat.ContentMarginTop = 1f;
		styleBoxFlat.ContentMarginBottom = 1f;
		hSeparator.AddThemeStyleboxOverride("separator", styleBoxFlat);
		vBoxContainer.AddChild(hSeparator, forceReadableName: false, Node.InternalMode.Disabled);
		parent.AddChild(vBoxContainer, forceReadableName: false, Node.InternalMode.Disabled);
	}

	private static void AddToggle(VBoxContainer parent, string label, bool initialValue, Action<bool> onToggle)
	{
		CheckButton checkButton = new CheckButton();
		checkButton.Text = label;
		checkButton.ButtonPressed = initialValue;
		checkButton.AddThemeColorOverride("font_color", new Color(0.9f, 0.9f, 0.9f));
		checkButton.AddThemeFontSizeOverride("font_size", 16);
		checkButton.Toggled += delegate(bool pressed)
		{
			onToggle.Invoke(pressed);
		};
		parent.AddChild(checkButton, forceReadableName: false, Node.InternalMode.Disabled);
	}

	private static void AddButtonRow(VBoxContainer parent, string label, Action onPressed)
	{
		Button button = new Button();
		button.Text = label;
		button.AddThemeFontSizeOverride("font_size", 14);
		button.Pressed += onPressed;
		parent.AddChild(button, forceReadableName: false, Node.InternalMode.Disabled);
	}

	private static void AddSettingsHotkeyRow(VBoxContainer parent)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		Button btn = new Button();
		btn.Text = ModSettings.FormatHotkey(ModSettings.SettingsHotkey);
		btn.AddThemeFontSizeOverride("font_size", 14);
		btn.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		btn.Pressed += (Action)delegate
		{
			_awaitingRebind = true;
			btn.Text = Localization.Get("hotkey.press_any_key");
		};
		_rebindButton = btn;
		parent.AddChild(btn, forceReadableName: false, Node.InternalMode.Disabled);
	}

	public static bool HandleHotkeyCapture(InputEventKey ev)
	{
		if (!IsCapturingHotkey)
		{
			return false;
		}
		if (!ev.Pressed || ev.IsEcho())
		{
			return true;
		}
		if (ev.Keycode == Key.Escape)
		{
			_awaitingRebind = false;
			if (_rebindButton != null)
			{
				_rebindButton.Text = ModSettings.FormatHotkey(ModSettings.SettingsHotkey);
			}
			return true;
		}
		if (ModSettings.IsAllowedHotkey((int)ev.Keycode))
		{
			ModSettings.SettingsHotkey = (int)ev.Keycode;
			ModSettings.Save();
			_awaitingRebind = false;
			if (_rebindButton != null)
			{
				_rebindButton.Text = ModSettings.FormatHotkey(ModSettings.SettingsHotkey);
			}
			return true;
		}
		if (_rebindButton != null)
		{
			_rebindButton.Text = Localization.Get("hotkey.fkeys_only");
		}
		return true;
	}

	private static void AddLanguageRow(VBoxContainer parent)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		OptionButton optionButton = new OptionButton();
		optionButton.AddThemeFontSizeOverride("font_size", 14);
		optionButton.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		int selected = 0;
		for (int i = 0; i < Localization.AvailableLanguages.Length; i++)
		{
			ValueTuple<string, string> val = Localization.AvailableLanguages[i];
			string item = val.Item1;
			string item2 = val.Item2;
			optionButton.AddItem(item2, i);
			if (item == ModSettings.LanguageOverride)
			{
				selected = i;
			}
		}
		optionButton.Selected = selected;
		optionButton.ItemSelected += delegate(long idx)
		{
			int num = (int)idx;
			if (num >= 0 && num < Localization.AvailableLanguages.Length)
			{
				ModSettings.LanguageOverride = Localization.AvailableLanguages[num].Item1;
				ModSettings.Save();
				Localization.Reload();
				Hide();
				Show();
			}
		};
		parent.AddChild(optionButton, forceReadableName: false, Node.InternalMode.Disabled);
	}

	private static void AddStepperRow(VBoxContainer parent, string label, int initialValue, int min, int max, int step, Action<int> onChange)
	{
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Expected O, but got Unknown
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Expected O, but got Unknown
		HBoxContainer hBoxContainer = new HBoxContainer();
		hBoxContainer.AddThemeConstantOverride("separation", 8);
		Label label2 = new Label();
		label2.Text = label;
		label2.AddThemeColorOverride("font_color", new Color(0.9f, 0.9f, 0.9f));
		label2.AddThemeFontSizeOverride("font_size", 16);
		hBoxContainer.AddChild(label2, forceReadableName: false, Node.InternalMode.Disabled);
		int current = initialValue;
		Button button = new Button();
		button.Text = "−";
		button.CustomMinimumSize = new Vector2(32f, 28f);
		hBoxContainer.AddChild(button, forceReadableName: false, Node.InternalMode.Disabled);
		Label valueLabel = new Label();
		valueLabel.Text = $"{initialValue}%";
		valueLabel.HorizontalAlignment = HorizontalAlignment.Center;
		valueLabel.CustomMinimumSize = new Vector2(50f, 0f);
		valueLabel.AddThemeColorOverride("font_color", new Color(0.9f, 0.7f, 0.2f));
		valueLabel.AddThemeFontSizeOverride("font_size", 16);
		hBoxContainer.AddChild(valueLabel, forceReadableName: false, Node.InternalMode.Disabled);
		Button button2 = new Button();
		button2.Text = "+";
		button2.CustomMinimumSize = new Vector2(32f, 28f);
		hBoxContainer.AddChild(button2, forceReadableName: false, Node.InternalMode.Disabled);
		button.Pressed += (Action)delegate
		{
			if (current > min)
			{
				current -= step;
				valueLabel.Text = $"{current}%";
				onChange.Invoke(current);
			}
		};
		button2.Pressed += (Action)delegate
		{
			if (current < max)
			{
				current += step;
				valueLabel.Text = $"{current}%";
				onChange.Invoke(current);
			}
		};
		parent.AddChild(hBoxContainer, forceReadableName: false, Node.InternalMode.Disabled);
	}

	public static bool IsPointInPanel(Vector2 point)
	{
		if (!_visible || _panel == null || !GodotObject.IsInstanceValid(_panel))
		{
			return false;
		}
		return _panel.GetGlobalRect().HasPoint(point);
	}

	public static void Hide()
	{
		if (_panel != null && GodotObject.IsInstanceValid(_panel))
		{
			_savedPosition = _panel.Position;
		}
		if (_canvasLayer != null && GodotObject.IsInstanceValid(_canvasLayer))
		{
			_canvasLayer.QueueFree();
		}
		_canvasLayer = null;
		_panel = null;
		_visible = false;
		_dragging = false;
		_rebindButton = null;
		_awaitingRebind = false;
	}

	private static void RefreshIntents()
	{
		try
		{
			NCombatRoom instance = NCombatRoom.Instance;
			if (instance == null)
			{
				return;
			}
			System.Collections.Generic.IEnumerator<Node> enumerator = instance.GetChildren().GetEnumerator();
			try
			{
				while (((System.Collections.IEnumerator)enumerator).MoveNext())
				{
					Node current = enumerator.Current;
					if (current is NCreature nCreature)
					{
						nCreature.RefreshIntents();
					}
				}
			}
			finally
			{
				((System.IDisposable)enumerator)?.Dispose();
			}
		}
		catch (System.Exception ex)
		{
			ModLog.Error("SettingsMenu.RefreshIntents", ex);
		}
	}

	[MethodImpl((MethodImplOptions)8)]
	private static bool IsSingleplayerOrFake(RunManager runManager)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (!_spLookupAttempted)
			{
				_spLookupAttempted = true;
				System.Type typeFromHandle = typeof(RunManager);
				BindingFlags val = (BindingFlags)52;
				_spProp = typeFromHandle.GetProperty("IsSingleplayerOrFakeMultiplayer", val) ?? typeFromHandle.GetProperty("IsSinglePlayerOrFakeMultiplayer", val);
			}
			PropertyInfo? spProp = _spProp;
			object obj = ((spProp != null) ? spProp.GetValue((object)runManager) : null);
			bool flag = default(bool);
			int num;
			if (obj is bool)
			{
				flag = (bool)obj;
				num = 1;
			}
			else
			{
				num = 0;
			}
			return (byte)((uint)num & (flag ? 1u : 0u)) != 0;
		}
		catch
		{
			return false;
		}
	}

	private static void BuildPartySection(VBoxContainer vbox)
	{
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Expected O, but got Unknown
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Expected O, but got Unknown
		try
		{
			RunManager instance = RunManager.Instance;
			if (instance == null || IsSingleplayerOrFake(instance))
			{
				return;
			}
			INetGameService netService = instance.NetService;
			if (netService == null)
			{
				return;
			}
			RunState value = Traverse.Create(instance).Property<RunState>("State").Value;
			if (value == null)
			{
				return;
			}
			System.Collections.Generic.IReadOnlyList<Player> players = value.Players;
			if (players == null || ((System.Collections.Generic.IReadOnlyCollection<Player>)players).Count <= 1)
			{
				return;
			}
			bool flag = netService.Type == NetGameType.Host;
			ulong netId = netService.NetId;
			AddSectionHeader(vbox, Localization.Get("section.party"));
			System.Collections.Generic.IEnumerator<Player> enumerator = ((System.Collections.Generic.IEnumerable<Player>)players).GetEnumerator();
			try
			{
				while (((System.Collections.IEnumerator)enumerator).MoveNext())
				{
					Player current = enumerator.Current;
					bool flag2 = current.NetId == netId;
					string playerName = PlatformUtil.GetPlayerName(PlatformType.Steam, current.NetId);
					string text = current.Character?.Title?.GetFormattedText();
					string text2 = (string.IsNullOrEmpty(playerName) ? (text ?? $"Player {current.NetId}") : ((!string.IsNullOrEmpty(text)) ? (playerName + " (" + text + ")") : playerName));
					HBoxContainer hBoxContainer = new HBoxContainer();
					hBoxContainer.AddThemeConstantOverride("separation", 8);
					Label label = new Label();
					label.Text = (flag2 ? (text2 + " (You)") : text2);
					label.AddThemeColorOverride("font_color", flag2 ? new Color(0.5f, 0.8f, 0.5f) : new Color(0.9f, 0.9f, 0.9f));
					label.AddThemeFontSizeOverride("font_size", 16);
					label.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
					hBoxContainer.AddChild(label, forceReadableName: false, Node.InternalMode.Disabled);
					if (!flag2)
					{
						Button button = new Button();
						bool flag3 = PartyManager.IsDrawingMuted(current.NetId);
						button.Text = Localization.Get(flag3 ? "party.show_drawings" : "party.hide_drawings");
						button.AddThemeFontSizeOverride("font_size", 14);
						button.CustomMinimumSize = new Vector2(130f, 0f);
						ulong capturedNetId = current.NetId;
						Button capturedBtn = button;
						button.Pressed += (Action)delegate
						{
							PartyManager.ToggleDrawingMute(capturedNetId);
							bool flag4 = PartyManager.IsDrawingMuted(capturedNetId);
							capturedBtn.Text = Localization.Get(flag4 ? "party.show_drawings" : "party.hide_drawings");
							if (flag4)
							{
								PartyManager.ClearDrawingsForPlayer(capturedNetId);
							}
						};
						hBoxContainer.AddChild(button, forceReadableName: false, Node.InternalMode.Disabled);
					}
					vbox.AddChild(hBoxContainer, forceReadableName: false, Node.InternalMode.Disabled);
				}
			}
			finally
			{
				((System.IDisposable)enumerator)?.Dispose();
			}
			Button button2 = new Button();
			button2.Text = Localization.Get("party.clear_drawings");
			button2.AddThemeFontSizeOverride("font_size", 14);
			button2.AddThemeColorOverride("font_color", new Color(1f, 0.6f, 0.2f));
			object obj = _003C_003Ec._003C_003E9__27_0;
			if (obj == null)
			{
				Action val = delegate
				{
					PartyManager.ClearAllDrawings();
				};
				_003C_003Ec._003C_003E9__27_0 = val;
				obj = (object)val;
			}
			button2.Pressed += (Action)obj;
			vbox.AddChild(button2, forceReadableName: false, Node.InternalMode.Disabled);
		}
		catch (System.Exception ex)
		{
			ModLog.Error("BuildPartySection", ex);
		}
	}
}
