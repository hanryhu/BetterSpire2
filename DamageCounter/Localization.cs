using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using MegaCrit.Sts2.Core.Localization;

namespace BetterSpire2;

public static class Localization
{
	private static Dictionary<string, string> _current = new Dictionary<string, string>();

	private static Dictionary<string, string> _english = new Dictionary<string, string>();

	private static bool _initialized;

	private static bool _subscribed;

	private static string _lastResolvedLang = "";

	[CompilerGenerated]
	private static Action? m_LanguageChanged;

	public static readonly ValueTuple<string, string>[] AvailableLanguages = new ValueTuple<string, string>[15]
	{
		new ValueTuple<string, string>("", "Auto"),
		new ValueTuple<string, string>("eng", "English"),
		new ValueTuple<string, string>("jpn", "日本語"),
		new ValueTuple<string, string>("zhs", "中文"),
		new ValueTuple<string, string>("kor", "한국어"),
		new ValueTuple<string, string>("deu", "Deutsch"),
		new ValueTuple<string, string>("fra", "Français"),
		new ValueTuple<string, string>("esp", "Español (LA)"),
		new ValueTuple<string, string>("spa", "Español (ES)"),
		new ValueTuple<string, string>("ita", "Italiano"),
		new ValueTuple<string, string>("ptb", "Português (BR)"),
		new ValueTuple<string, string>("rus", "Русский"),
		new ValueTuple<string, string>("pol", "Polski"),
		new ValueTuple<string, string>("tha", "ไทย"),
		new ValueTuple<string, string>("tur", "Türkçe")
	};

	public static event Action LanguageChanged
	{
		[CompilerGenerated]
		add
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Expected O, but got Unknown
			Action val = Localization.m_LanguageChanged;
			Action val2;
			do
			{
				val2 = val;
				Action val3 = (Action)System.Delegate.Combine((System.Delegate)(object)val2, (System.Delegate)(object)value);
				val = Interlocked.CompareExchange<Action>(ref Localization.m_LanguageChanged, val3, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Expected O, but got Unknown
			Action val = Localization.m_LanguageChanged;
			Action val2;
			do
			{
				val2 = val;
				Action val3 = (Action)System.Delegate.Remove((System.Delegate)(object)val2, (System.Delegate)(object)value);
				val = Interlocked.CompareExchange<Action>(ref Localization.m_LanguageChanged, val3, val2);
			}
			while (val != val2);
		}
	}

	public static void Init()
	{
		if (!_initialized)
		{
			_initialized = true;
			LoadEmbeddedEnglish();
			CleanUpStaleJsonFiles();
			Reload();
			TrySubscribe();
		}
	}

	private static void CleanUpStaleJsonFiles()
	{
		try
		{
			string localizationFolder = GetLocalizationFolder();
			if (localizationFolder == null || !Directory.Exists(localizationFolder))
			{
				return;
			}
			int num = 0;
			string[] files = Directory.GetFiles(localizationFolder, "*.json");
			foreach (string text in files)
			{
				try
				{
					File.Delete(text);
					num++;
				}
				catch (System.Exception ex)
				{
					ModLog.Error("Localization: could not delete stale " + text, ex);
				}
			}
			if (num > 0)
			{
				ModLog.Info($"Localization: cleaned up {num} stale .json locale file(s) from previous version");
			}
		}
		catch (System.Exception ex2)
		{
			ModLog.Error("Localization.CleanUpStaleJsonFiles", ex2);
		}
	}

	public static string Get(string key)
	{
		if (!_subscribed)
		{
			TrySubscribe();
		}
		if (string.IsNullOrEmpty(ModSettings.LanguageOverride))
		{
			string text = SafeGetGameLanguage();
			if (text != _lastResolvedLang)
			{
				Reload();
			}
		}
		string result = default(string);
		if (_current.TryGetValue(key, out result))
		{
			return result;
		}
		if (_english.TryGetValue(key, out result))
		{
			return result;
		}
		return key;
	}

	private static void TrySubscribe()
	{
		if (_subscribed)
		{
			return;
		}
		try
		{
			LocManager instance = LocManager.Instance;
			if (instance != null)
			{
				instance.SubscribeToLocaleChange(Reload);
				_subscribed = true;
				ModLog.Info("Localization: subscribed to game locale changes");
			}
		}
		catch (System.Exception ex)
		{
			ModLog.Error("Localization: subscribe locale change", ex);
		}
	}

	private static string SafeGetGameLanguage()
	{
		try
		{
			return LocManager.Instance?.Language ?? "eng";
		}
		catch
		{
			return "eng";
		}
	}

	public static void Reload()
	{
		try
		{
			string text = ResolveLanguageCode();
			_lastResolvedLang = SafeGetGameLanguage();
			string localizationFolder = GetLocalizationFolder();
			Dictionary<string, string> val = null;
			if (localizationFolder != null)
			{
				val = TryLoad(Path.Combine(localizationFolder, text + ".lang"));
				if (val == null && text != "eng")
				{
					val = TryLoad(Path.Combine(localizationFolder, "eng.lang"));
				}
			}
			_current = val ?? _english;
			ModLog.Info($"Localization: loaded {text} ({_current.Count} strings)");
			Action? languageChanged = Localization.m_LanguageChanged;
			if (languageChanged != null)
			{
				languageChanged.Invoke();
			}
		}
		catch (System.Exception ex)
		{
			ModLog.Error("Localization.Reload", ex);
		}
	}

	private static string ResolveLanguageCode()
	{
		if (!string.IsNullOrEmpty(ModSettings.LanguageOverride))
		{
			return ModSettings.LanguageOverride;
		}
		return SafeGetGameLanguage();
	}

	private static string? GetLocalizationFolder()
	{
		try
		{
			string location = Assembly.GetExecutingAssembly().Location;
			string directoryName = Path.GetDirectoryName(location);
			if (string.IsNullOrEmpty(directoryName))
			{
				return null;
			}
			return Path.Combine(directoryName, "BetterSpire2_localization");
		}
		catch
		{
			return null;
		}
	}

	private static Dictionary<string, string>? TryLoad(string path)
	{
		try
		{
			if (!File.Exists(path))
			{
				return null;
			}
			string text = File.ReadAllText(path);
			return JsonSerializer.Deserialize<Dictionary<string, string>>(text, (JsonSerializerOptions)null);
		}
		catch (System.Exception ex)
		{
			ModLog.Error("Localization: failed to load " + path, ex);
			return null;
		}
	}

	private static void LoadEmbeddedEnglish()
	{
		_english = new Dictionary<string, string>
		{
			["settings.title"] = "BetterSpire2 Settings",
			["settings.title_lite"] = "BetterSpire2 Lite Settings",
			["settings.hint"] = "Drag to move | Click outside to close",
			["settings.language"] = "Language",
			["section.combat_hud"] = "Combat HUD",
			["section.multiplayer"] = "Multiplayer",
			["section.gameplay"] = "Gameplay",
			["section.shortcuts"] = "Shortcuts",
			["section.party"] = "Party",
			["section.language"] = "Language",
			["section.hotkey"] = "Settings Hotkey",
			["hotkey.press_any_key"] = "Press any key (Esc to cancel)",
			["hotkey.fkeys_only"] = "F1-F12 only — press again",
			["setting.multi_hit_totals"] = "Multi-Hit Totals (per enemy)",
			["setting.player_damage_total"] = "Total Incoming Damage (above player)",
			["setting.show_expected_hp"] = "Show Expected HP After Damage",
			["setting.reset_damage_position"] = "Reset Damage Number Position",
			["setting.native_hand_badge"] = "Show x2 Badge on Hand Cards",
			["setting.show_teammate_hand"] = "Show Teammate Hand (F3)",
			["setting.always_show"] = "Keep Open (no click-outside close)",
			["setting.auto_show"] = "Auto-Show When Entering Combat",
			["setting.hide_own_hand"] = "Hide Own Hand (teammates only)",
			["setting.compact_view"] = "Compact View (stats only, no cards)",
			["setting.popout_window"] = "Open in Separate Window (drag to another monitor)",
			["setting.card_size"] = "Card Size",
			["setting.opacity"] = "Background Opacity",
			["setting.reset_position"] = "Reset Hand Viewer Position",
			["setting.auto_confirm"] = "Auto-Confirm Discard/Retain",
			["setting.instant_fast_mode"] = "Instant Fast Mode (combat only)",
			["setting.allow_instant_mp"] = "Allow Instant in Multiplayer (all players must enable)",
			["setting.skip_splash"] = "Skip Splash Screen",
			["setting.show_clock"] = "Show Clock",
			["setting.clock_24h"] = "24-Hour Format",
			["setting.show_potion_chance"] = "Show Potion Drop Chance",
			["setting.reset_potion_position"] = "Reset Potion Chance Position",
			["setting.hold_r_restart"] = "Hold R to Restart Run",
			["setting.double_tap_r"] = "Double-Tap R to Reset Round",
			["setting.scale_difficulty"] = "Scale difficulty to active players",
			["party.kick"] = "Kick",
			["party.hide_drawings"] = "Hide Drawings",
			["party.show_drawings"] = "Show Drawings",
			["party.clear_drawings"] = "Clear All Drawings",
			["deck.title"] = "Party Hands",
			["deck.hand"] = "Hand",
			["deck.potions"] = "Potions:"
		};
	}
}
