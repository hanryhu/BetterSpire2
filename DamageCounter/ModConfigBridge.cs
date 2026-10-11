using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Nodes;

namespace BetterSpire2;

internal static class ModConfigBridge
{
	private static bool _detected;

	private static bool _available;

	private static System.Type? _apiType;

	private static System.Type? _entryType;

	private static System.Type? _configType;

	private static bool _registered;

	internal static bool IsAvailable
	{
		get
		{
			if (!_detected)
			{
				_detected = true;
				_apiType = System.Type.GetType("ModConfig.ModConfigApi, ModConfig");
				_entryType = System.Type.GetType("ModConfig.ConfigEntry, ModConfig");
				_configType = System.Type.GetType("ModConfig.ConfigType, ModConfig");
				_available = _apiType != (System.Type)null && _entryType != (System.Type)null && _configType != (System.Type)null;
			}
			return _available;
		}
	}

	internal static void TryRegister()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		if (_registered)
		{
			return;
		}
		if (!IsAvailable)
		{
			_registered = true;
			return;
		}
		try
		{
			SceneTree tree = NGame.Instance?.GetTree();
			int frames;
			if (tree != null)
			{
				frames = 0;
				tree.ProcessFrame += new Action(Handler);
			}
			void Handler()
			{
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Expected O, but got Unknown
				if (++frames >= 2)
				{
					tree.ProcessFrame -= new Action(Handler);
					Register();
				}
			}
		}
		catch (System.Exception ex)
		{
			ModLog.Error("ModConfigBridge.TryRegister", ex);
		}
	}

	private static void Register()
	{
		if (_registered)
		{
			return;
		}
		_registered = true;
		try
		{
			List<object> val = new List<object>();
			val.Add(MakeHeader(Localization.Get("section.combat_hud")));
			val.Add(MakeToggle("MultiHitTotals", Localization.Get("setting.multi_hit_totals"), ModSettings.MultiHitTotals, delegate(object v)
			{
				ModSettings.MultiHitTotals = (bool)v;
				ModSettings.Save();
			}));
			val.Add(MakeToggle("PlayerDamageTotal", Localization.Get("setting.player_damage_total"), ModSettings.PlayerDamageTotal, delegate(object v)
			{
				ModSettings.PlayerDamageTotal = (bool)v;
				ModSettings.Save();
			}));
			val.Add(MakeToggle("ShowExpectedHp", Localization.Get("setting.show_expected_hp"), ModSettings.ShowExpectedHp, delegate(object v)
			{
				ModSettings.ShowExpectedHp = (bool)v;
				ModSettings.Save();
			}));
			val.Add(MakeToggle("ShowNativeHandBadge", Localization.Get("setting.native_hand_badge"), ModSettings.ShowNativeHandBadge, delegate(object v)
			{
				ModSettings.ShowNativeHandBadge = (bool)v;
				ModSettings.Save();
				NativeHandBadge.RefreshAll();
			}));
			val.Add(MakeToggle("ShowPotionChance", Localization.Get("setting.show_potion_chance"), ModSettings.ShowPotionChance, delegate(object v)
			{
				ModSettings.ShowPotionChance = (bool)v;
				ModSettings.Save();
				PotionChanceBadgePatch.RefreshVisibility();
			}));
			val.Add(MakeHeader(Localization.Get("section.multiplayer")));
			val.Add(MakeToggle("ShowTeammateHand", Localization.Get("setting.show_teammate_hand"), ModSettings.ShowTeammateHand, delegate(object v)
			{
				ModSettings.ShowTeammateHand = (bool)v;
				ModSettings.Save();
				if (!(bool)v)
				{
					DeckTracker.Hide();
				}
			}));
			val.Add(MakeToggle("AlwaysShowTeammateHand", Localization.Get("setting.always_show"), ModSettings.AlwaysShowTeammateHand, delegate(object v)
			{
				ModSettings.AlwaysShowTeammateHand = (bool)v;
				ModSettings.Save();
			}));
			val.Add(MakeToggle("AutoShowTeammateHand", Localization.Get("setting.auto_show"), ModSettings.AutoShowTeammateHand, delegate(object v)
			{
				ModSettings.AutoShowTeammateHand = (bool)v;
				ModSettings.Save();
			}));
			val.Add(MakeToggle("HideOwnHand", Localization.Get("setting.hide_own_hand"), ModSettings.HideOwnHand, delegate(object v)
			{
				ModSettings.HideOwnHand = (bool)v;
				ModSettings.Save();
				DeckTracker.RefreshIfVisible();
			}));
			val.Add(MakeToggle("CompactHandViewer", Localization.Get("setting.compact_view"), ModSettings.CompactHandViewer, delegate(object v)
			{
				ModSettings.CompactHandViewer = (bool)v;
				ModSettings.Save();
				DeckTracker.RefreshIfVisible();
			}));
			val.Add(MakeToggle("HandViewerPopout", Localization.Get("setting.popout_window"), ModSettings.HandViewerPopout, delegate(object v)
			{
				ModSettings.HandViewerPopout = (bool)v;
				ModSettings.Save();
				if (DeckTracker.IsVisible)
				{
					DeckTracker.Hide();
					DeckTracker.ShowIfInCombat();
				}
			}));
			val.Add(MakeEntry("CardScalePercent", Localization.Get("setting.card_size") + " (%)", ConfigTypeValue("Slider"), (float)ModSettings.CardScalePercent, 50f, 200f, 10f, "F0", null, null, null, delegate(object v)
			{
				ModSettings.CardScalePercent = (int)(float)v;
				ModSettings.Save();
				DeckTracker.RefreshIfVisible();
			}));
			val.Add(MakeEntry("HandViewerOpacityPercent", Localization.Get("setting.opacity") + " (%)", ConfigTypeValue("Slider"), (float)ModSettings.HandViewerOpacityPercent, 0f, 100f, 10f, "F0", null, null, null, delegate(object v)
			{
				ModSettings.HandViewerOpacityPercent = (int)(float)v;
				ModSettings.Save();
				DeckTracker.RefreshIfVisible();
			}));
			val.Add(MakeHeader(Localization.Get("section.gameplay")));
			val.Add(MakeToggle("InstantFastMode", Localization.Get("setting.instant_fast_mode"), ModSettings.InstantFastMode, delegate(object v)
			{
				ModSettings.InstantFastMode = (bool)v;
				ModSettings.Save();
				if (!(bool)v)
				{
					InstantSpeedHelper.OnCombatEnd();
				}
				else if (CombatManager.Instance?.DebugOnlyGetState() != null)
				{
					InstantSpeedHelper.OnCombatStart();
				}
			}));
			val.Add(MakeToggle("AllowInstantInMultiplayer", Localization.Get("setting.allow_instant_mp"), ModSettings.AllowInstantInMultiplayer, delegate(object v)
			{
				ModSettings.AllowInstantInMultiplayer = (bool)v;
				ModSettings.Save();
				if (CombatManager.Instance?.DebugOnlyGetState() != null)
				{
					InstantSpeedHelper.OnCombatEnd();
					if (ModSettings.InstantFastMode)
					{
						InstantSpeedHelper.OnCombatStart();
					}
				}
			}));
			val.Add(MakeToggle("SkipSplash", Localization.Get("setting.skip_splash"), ModSettings.SkipSplash, delegate(object v)
			{
				ModSettings.SkipSplash = (bool)v;
				ModSettings.Save();
			}));
			val.Add(MakeToggle("ShowClock", Localization.Get("setting.show_clock"), ModSettings.ShowClock, delegate(object v)
			{
				ModSettings.ShowClock = (bool)v;
				ModSettings.Save();
				ClockDisplay.Toggle((bool)v);
			}));
			val.Add(MakeToggle("Clock24Hour", Localization.Get("setting.clock_24h"), ModSettings.Clock24Hour, delegate(object v)
			{
				ModSettings.Clock24Hour = (bool)v;
				ModSettings.Save();
			}));
			System.Array array = System.Array.CreateInstance(_entryType, val.Count);
			for (int num = 0; num < val.Count; num++)
			{
				array.SetValue(val[num], num);
			}
			string text = "BetterSpire2Lite";
			string text2 = "BetterSpire2 Lite";
			MethodInfo method = _apiType.GetMethod("Register", new System.Type[3]
			{
				typeof(string),
				typeof(string),
				((object)array).GetType()
			});
			if (method != null)
			{
				((MethodBase)method).Invoke((object)null, new object[3] { text, text2, array });
			}
			ModLog.Info($"ModConfig integration registered ({val.Count} entries)");
		}
		catch (System.Exception ex)
		{
			ModLog.Error("ModConfigBridge.Register", ex);
		}
	}

	private static object ConfigTypeValue(string name)
	{
		return System.Enum.Parse(_configType, name);
	}

	private static object MakeToggle(string key, string label, bool defaultValue, Action<object> onChanged)
	{
		return MakeEntry(key, label, ConfigTypeValue("Toggle"), defaultValue, 0f, 100f, 1f, "F0", null, null, null, onChanged);
	}

	private static object MakeHeader(string label)
	{
		return MakeEntry("", label, ConfigTypeValue("Header"));
	}

	private static object MakeEntry(string key, string label, object type, object? defaultValue = null, float min = 0f, float max = 100f, float step = 1f, string format = "F0", string[]? options = null, string? buttonText = null, Func<object, bool>? validator = null, Action<object>? onChanged = null)
	{
		object obj = Activator.CreateInstance(_entryType);
		SetProp(obj, "Key", key);
		SetProp(obj, "Label", label);
		SetProp(obj, "Type", type);
		if (defaultValue != null)
		{
			SetProp(obj, "DefaultValue", defaultValue);
		}
		SetProp(obj, "Min", min);
		SetProp(obj, "Max", max);
		SetProp(obj, "Step", step);
		SetProp(obj, "Format", format);
		if (options != null)
		{
			SetProp(obj, "Options", options);
		}
		if (buttonText != null)
		{
			SetProp(obj, "ButtonText", buttonText);
		}
		if (validator != null)
		{
			SetProp(obj, "Validator", validator);
		}
		if (onChanged != null)
		{
			SetProp(obj, "OnChanged", onChanged);
		}
		return obj;
	}

	private static void SetProp(object obj, string name, object value)
	{
		PropertyInfo property = obj.GetType().GetProperty(name);
		if (property != null)
		{
			property.SetValue(obj, value);
		}
	}
}
