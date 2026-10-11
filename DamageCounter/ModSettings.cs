using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Godot;

namespace BetterSpire2;

public static class ModSettings
{
	public static bool MultiHitTotals = true;

	public static bool PlayerDamageTotal = true;

	public static bool ShowExpectedHp = true;

	public static bool ShowNativeHandBadge = true;

	public static bool SkipSplash = true;

	public static bool ShowTeammateHand = true;

	public static bool AlwaysShowTeammateHand = false;

	public static bool AutoShowTeammateHand = false;

	public static bool HideOwnHand = false;

	public static bool InstantFastMode = false;

	public static bool AllowInstantInMultiplayer = false;

	public static bool CompactHandViewer = false;

	public static bool HandViewerPopout = false;

	public static int CardScalePercent = 100;

	public static int HandViewerOpacityPercent = 0;

	public static string LanguageOverride = "";

	public static bool ShowClock = false;

	public static bool Clock24Hour = false;

	public static bool ShowPotionChance = true;

	public static int SettingsHotkey = 4194332;

	private static string SettingsPath => Path.Combine(OS.GetUserDataDir(), "betterspire2_settings.json");

	private static bool TryGetBool(Dictionary<string, JsonElement> dict, string key, out bool value)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Invalid comparison between Unknown and I4
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		value = false;
		JsonElement val = default(JsonElement);
		if (!dict.TryGetValue(key, out val) || ((int)val.ValueKind != 5 && (int)val.ValueKind != 6))
		{
			return false;
		}
		value = val.GetBoolean();
		return true;
	}

	private static bool TryGetInt(Dictionary<string, JsonElement> dict, string key, out int value)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Invalid comparison between Unknown and I4
		value = 0;
		JsonElement val = default(JsonElement);
		if (!dict.TryGetValue(key, out val) || (int)val.ValueKind != 4)
		{
			return false;
		}
		value = val.GetInt32();
		return true;
	}

	private static bool TryGetString(Dictionary<string, JsonElement> dict, string key, out string value)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Invalid comparison between Unknown and I4
		value = "";
		JsonElement val = default(JsonElement);
		if (!dict.TryGetValue(key, out val) || (int)val.ValueKind != 3)
		{
			return false;
		}
		value = val.GetString() ?? "";
		return true;
	}

	public static bool IsAllowedHotkey(int keycode)
	{
		return keycode >= 4194332 && keycode <= 4194343;
	}

	public static string FormatHotkey(int keycode)
	{
		try
		{
			return ((object)(Key)keycode/*cast due to .constrained prefix*/).ToString();
		}
		catch
		{
			return "F1";
		}
	}

	public static void Load()
	{
		try
		{
			if (!File.Exists(SettingsPath))
			{
				return;
			}
			string text = File.ReadAllText(SettingsPath);
			Dictionary<string, JsonElement> val = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(text, (JsonSerializerOptions)null);
			if (val != null)
			{
				if (TryGetBool(val, "MultiHitTotals", out var value))
				{
					MultiHitTotals = value;
				}
				if (TryGetBool(val, "PlayerDamageTotal", out value))
				{
					PlayerDamageTotal = value;
				}
				if (TryGetBool(val, "ShowExpectedHp", out value))
				{
					ShowExpectedHp = value;
				}
				if (TryGetBool(val, "ShowNativeHandBadge", out value))
				{
					ShowNativeHandBadge = value;
				}
				if (TryGetBool(val, "SkipSplash", out value))
				{
					SkipSplash = value;
				}
				if (TryGetBool(val, "ShowTeammateHand", out value))
				{
					ShowTeammateHand = value;
				}
				if (TryGetBool(val, "AlwaysShowTeammateHand", out value))
				{
					AlwaysShowTeammateHand = value;
				}
				if (TryGetBool(val, "AutoShowTeammateHand", out value))
				{
					AutoShowTeammateHand = value;
				}
				if (TryGetBool(val, "HideOwnHand", out value))
				{
					HideOwnHand = value;
				}
				if (TryGetBool(val, "InstantFastMode", out value))
				{
					InstantFastMode = value;
				}
				if (TryGetBool(val, "AllowInstantInMultiplayer", out value))
				{
					AllowInstantInMultiplayer = value;
				}
				if (TryGetBool(val, "CompactHandViewer", out value))
				{
					CompactHandViewer = value;
				}
				if (TryGetBool(val, "HandViewerPopout", out value))
				{
					HandViewerPopout = value;
				}
				if (TryGetBool(val, "ShowClock", out value))
				{
					ShowClock = value;
				}
				if (TryGetBool(val, "Clock24Hour", out value))
				{
					Clock24Hour = value;
				}
				if (TryGetBool(val, "ShowPotionChance", out value))
				{
					ShowPotionChance = value;
				}
				if (TryGetInt(val, "CardScalePercent", out var value2))
				{
					CardScalePercent = Math.Clamp(value2, 50, 200);
				}
				if (TryGetInt(val, "HandViewerOpacityPercent", out value2))
				{
					HandViewerOpacityPercent = Math.Clamp(value2, 0, 100);
				}
				if (TryGetInt(val, "SettingsHotkey", out value2) && IsAllowedHotkey(value2))
				{
					SettingsHotkey = value2;
				}
				if (TryGetString(val, "LanguageOverride", out string value3))
				{
					LanguageOverride = value3;
				}
			}
		}
		catch (System.Exception ex)
		{
			ModLog.Error("ModSettings.Load", ex);
		}
	}

	public static void Save()
	{
		try
		{
			Dictionary<string, object> val = new Dictionary<string, object>
			{
				["MultiHitTotals"] = MultiHitTotals,
				["PlayerDamageTotal"] = PlayerDamageTotal,
				["ShowExpectedHp"] = ShowExpectedHp,
				["ShowNativeHandBadge"] = ShowNativeHandBadge,
				["SkipSplash"] = SkipSplash,
				["ShowTeammateHand"] = ShowTeammateHand,
				["AlwaysShowTeammateHand"] = AlwaysShowTeammateHand,
				["AutoShowTeammateHand"] = AutoShowTeammateHand,
				["HideOwnHand"] = HideOwnHand,
				["InstantFastMode"] = InstantFastMode,
				["AllowInstantInMultiplayer"] = AllowInstantInMultiplayer,
				["CompactHandViewer"] = CompactHandViewer,
				["HandViewerPopout"] = HandViewerPopout,
				["CardScalePercent"] = CardScalePercent,
				["HandViewerOpacityPercent"] = HandViewerOpacityPercent,
				["LanguageOverride"] = LanguageOverride,
				["ShowClock"] = ShowClock,
				["Clock24Hour"] = Clock24Hour,
				["ShowPotionChance"] = ShowPotionChance,
				["SettingsHotkey"] = SettingsHotkey
			};
			string text = JsonSerializer.Serialize<Dictionary<string, object>>(val, (JsonSerializerOptions)null);
			File.WriteAllText(SettingsPath, text);
		}
		catch (System.Exception ex)
		{
			ModLog.Error("ModSettings.Save", ex);
		}
	}
}
