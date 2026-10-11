using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Godot;

namespace BetterSpire2;

public static class ModLog
{
	private static string? _logPath;

	private static readonly object _lock = new object();

	private static string LogPath => _logPath ?? (_logPath = Path.Combine(OS.GetUserDataDir(), "betterspire2" + InstanceSuffix() + "_log.txt"));

	private static string InstanceSuffix()
	{
		try
		{
			string[] cmdlineArgs = OS.GetCmdlineArgs();
			for (int i = 0; i < cmdlineArgs.Length; i++)
			{
				if (cmdlineArgs[i] == "-clientId" && i + 1 < cmdlineArgs.Length)
				{
					return "_" + cmdlineArgs[i + 1];
				}
				if (cmdlineArgs[i] == "host_standard")
				{
					return "_host";
				}
			}
		}
		catch
		{
		}
		return "";
	}

	public static void Init()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Expected O, but got Unknown
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			string dllPath = GetDllPath();
			StringBuilder val = new StringBuilder();
			StringBuilder val2 = val;
			StringBuilder obj = val2;
			StringBuilder.AppendInterpolatedStringHandler val3 = new StringBuilder.AppendInterpolatedStringHandler(28, 1, val2);
			val3.AppendLiteral("[");
			val3.AppendFormatted<System.DateTime>(System.DateTime.Now, "yyyy-MM-dd HH:mm:ss");
			val3.AppendLiteral("] BetterSpire2 log started\n");
			obj.Append(ref val3);
			val2 = val;
			StringBuilder obj2 = val2;
			val3 = new StringBuilder.AppendInterpolatedStringHandler(12, 1, val2);
			val3.AppendLiteral("  Version: ");
			val3.AppendFormatted(GetModVersion(dllPath));
			val3.AppendLiteral("\n");
			obj2.Append(ref val3);
			val2 = val;
			StringBuilder obj3 = val2;
			val3 = new StringBuilder.AppendInterpolatedStringHandler(12, 1, val2);
			val3.AppendLiteral("  Install: ");
			val3.AppendFormatted(DescribeInstall(dllPath));
			val3.AppendLiteral("\n");
			obj3.Append(ref val3);
			val2 = val;
			StringBuilder obj4 = val2;
			val3 = new StringBuilder.AppendInterpolatedStringHandler(12, 1, val2);
			val3.AppendLiteral("  Game:    ");
			val3.AppendFormatted(GetGameVersion(dllPath));
			val3.AppendLiteral("\n");
			obj4.Append(ref val3);
			val2 = val;
			StringBuilder obj5 = val2;
			val3 = new StringBuilder.AppendInterpolatedStringHandler(15, 2, val2);
			val3.AppendLiteral("  OS:      ");
			val3.AppendFormatted(OS.GetName());
			val3.AppendLiteral(" / ");
			val3.AppendFormatted(OS.GetDistributionName());
			val3.AppendLiteral("\n");
			obj5.Append(ref val3);
			val2 = val;
			StringBuilder obj6 = val2;
			val3 = new StringBuilder.AppendInterpolatedStringHandler(12, 1, val2);
			val3.AppendLiteral("  Godot:   ");
			val3.AppendFormatted<Variant>(Engine.GetVersionInfo()["string"]);
			val3.AppendLiteral("\n");
			obj6.Append(ref val3);
			List<string> siblingMods = GetSiblingMods(dllPath);
			if (siblingMods.Count > 0)
			{
				val2 = val;
				StringBuilder obj7 = val2;
				val3 = new StringBuilder.AppendInterpolatedStringHandler(34, 1, val2);
				val3.AppendLiteral("  Other mods loaded alongside (");
				val3.AppendFormatted<int>(siblingMods.Count);
				val3.AppendLiteral("):\n");
				obj7.Append(ref val3);
				var enumerator = siblingMods.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						string current = enumerator.Current;
						val2 = val;
						StringBuilder obj8 = val2;
						val3 = new StringBuilder.AppendInterpolatedStringHandler(7, 1, val2);
						val3.AppendLiteral("    - ");
						val3.AppendFormatted(current);
						val3.AppendLiteral("\n");
						obj8.Append(ref val3);
					}
				}
				finally
				{
					((System.IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
				}
			}
			File.WriteAllText(LogPath, ((object)val).ToString());
		}
		catch
		{
		}
	}

	private static string GetDllPath()
	{
		try
		{
			return Assembly.GetExecutingAssembly().Location ?? "";
		}
		catch
		{
			return "";
		}
	}

	private static string GetModVersion(string dllPath)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (string.IsNullOrEmpty(dllPath))
			{
				return "unknown";
			}
			string text = Path.ChangeExtension(dllPath, ".json");
			if (!File.Exists(text))
			{
				return "unknown";
			}
			string text2 = File.ReadAllText(text);
			JsonDocument val = JsonDocument.Parse(text2, default(JsonDocumentOptions));
			try
			{
				JsonElement rootElement = val.RootElement;
				JsonElement val2 = default(JsonElement);
				if (rootElement.TryGetProperty("version", out val2))
				{
					return val2.GetString() ?? "unknown";
				}
			}
			finally
			{
				((System.IDisposable)val)?.Dispose();
			}
		}
		catch
		{
		}
		return "unknown";
	}

	private static string DescribeInstall(string dllPath)
	{
		if (string.IsNullOrEmpty(dllPath))
		{
			return "unknown";
		}
		string text = dllPath.Replace('\\', '/');
		string text2 = ((text.IndexOf("/workshop/content/", (StringComparison)5) >= 0) ? "Steam Workshop" : ((text.IndexOf("/mods/", (StringComparison)5) >= 0) ? "Manual (mods folder)" : "Other"));
		return text2 + " — " + dllPath;
	}

	private static string GetGameVersion(string dllPath)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (string.IsNullOrEmpty(dllPath))
			{
				return "unknown";
			}
			string directoryName = Path.GetDirectoryName(dllPath);
			JsonElement val2 = default(JsonElement);
			JsonElement val3 = default(JsonElement);
			for (int i = 0; i < 8; i++)
			{
				if (string.IsNullOrEmpty(directoryName))
				{
					break;
				}
				string text = Path.Combine(directoryName, "release_info.json");
				if (File.Exists(text))
				{
					string text2 = File.ReadAllText(text);
					JsonDocument val = JsonDocument.Parse(text2, default(JsonDocumentOptions));
					try
					{
						JsonElement rootElement = val.RootElement;
						string text3 = (rootElement.TryGetProperty("version", out val2) ? val2.GetString() : "?");
						rootElement = val.RootElement;
						string text4 = (rootElement.TryGetProperty("branch", out val3) ? val3.GetString() : "?");
						return text3 + " (" + text4 + ")";
					}
					finally
					{
						((System.IDisposable)val)?.Dispose();
					}
				}
				directoryName = Path.GetDirectoryName(directoryName);
			}
		}
		catch
		{
		}
		return "unknown";
	}

	private static List<string> GetSiblingMods(string dllPath)
	{
		List<string> val = new List<string>();
		try
		{
			if (string.IsNullOrEmpty(dllPath))
			{
				return val;
			}
			string text = dllPath.Replace('\\', '/');
			string directoryName = Path.GetDirectoryName(dllPath);
			if (directoryName == null)
			{
				return val;
			}
			string fileName = Path.GetFileName(dllPath);
			int num = text.IndexOf("/workshop/content/", (StringComparison)5);
			if (num >= 0)
			{
				string directoryName2 = Path.GetDirectoryName(directoryName);
				if (directoryName2 != null && Directory.Exists(directoryName2))
				{
					string[] directories = Directory.GetDirectories(directoryName2);
					foreach (string text2 in directories)
					{
						if (!string.Equals(text2, directoryName, (StringComparison)5))
						{
							string[] files = Directory.GetFiles(text2, "*.json");
							foreach (string text3 in files)
							{
								val.Add(Path.GetFileName(text3) + " (workshop:" + Path.GetFileName(text2) + ")");
							}
						}
					}
				}
			}
			else
			{
				string[] files2 = Directory.GetFiles(directoryName, "*.dll");
				foreach (string text4 in files2)
				{
					string fileName2 = Path.GetFileName(text4);
					if (!string.Equals(fileName2, fileName, (StringComparison)5))
					{
						val.Add(fileName2);
					}
				}
			}
		}
		catch
		{
		}
		return val;
	}

	private static string GetModVersion()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			string location = Assembly.GetExecutingAssembly().Location;
			if (string.IsNullOrEmpty(location))
			{
				return "unknown";
			}
			string text = Path.ChangeExtension(location, ".json");
			if (!File.Exists(text))
			{
				return "unknown";
			}
			string text2 = File.ReadAllText(text);
			JsonDocument val = JsonDocument.Parse(text2, default(JsonDocumentOptions));
			try
			{
				JsonElement rootElement = val.RootElement;
				JsonElement val2 = default(JsonElement);
				if (rootElement.TryGetProperty("version", out val2))
				{
					return val2.GetString() ?? "unknown";
				}
			}
			finally
			{
				((System.IDisposable)val)?.Dispose();
			}
		}
		catch
		{
		}
		return "unknown";
	}

	public static void Info(string message)
	{
		try
		{
			lock (_lock)
			{
				File.AppendAllText(LogPath, $"[{System.DateTime.Now:HH:mm:ss}] {message}\n");
			}
		}
		catch
		{
		}
	}

	public static void Error(string context, System.Exception ex)
	{
		try
		{
			lock (_lock)
			{
				File.AppendAllText(LogPath, $"[{System.DateTime.Now:HH:mm:ss}] ERROR in {context}: {ex}\n");
			}
		}
		catch
		{
		}
	}
}
