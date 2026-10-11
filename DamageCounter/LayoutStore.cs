using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Godot;

namespace BetterSpire2;

public static class LayoutStore
{
	private sealed class Dto
	{
		[field: CompilerGenerated]
		public float[]? clockPos
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		public float[]? potionPos
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		public HandViewerDto? handViewer
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		public Dictionary<string, float[]>? damageOffsets
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}
	}

	private sealed class HandViewerDto
	{
		[field: CompilerGenerated]
		public float[]? pos
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		public float[]? size
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		public int[]? windowPos
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		public int[]? windowSize
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}
	}

	public static Vector2? ClockPos;

	public static Vector2? PotionPos;

	public static Vector2? HandViewerPos;

	public static Vector2? HandViewerSize;

	public static Vector2I? HandViewerWindowPos;

	public static Vector2I? HandViewerWindowSize;

	public static readonly Dictionary<string, Vector2> DamageOffsets = new Dictionary<string, Vector2>();

	private static bool _initialized;

	private static string LayoutPath => Path.Combine(OS.GetUserDataDir(), "betterspire2_layout.json");

	public static void Init()
	{
		if (_initialized)
		{
			return;
		}
		_initialized = true;
		try
		{
			if (File.Exists(LayoutPath))
			{
				LoadUnified();
			}
			else
			{
				MigrateFromLegacy();
			}
		}
		catch (System.Exception ex)
		{
			ModLog.Error("LayoutStore.Init", ex);
		}
		TryDelete(Path.Combine(OS.GetUserDataDir(), "betterspire2_damage_offset.txt"));
	}

	public static void Save()
	{
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		try
		{
			Dto dto = new Dto
			{
				clockPos = ToArr(ClockPos),
				potionPos = ToArr(PotionPos),
				handViewer = ((HandViewerPos.HasValue || HandViewerSize.HasValue || HandViewerWindowPos.HasValue || HandViewerWindowSize.HasValue) ? new HandViewerDto
				{
					pos = ToArr(HandViewerPos),
					size = ToArr(HandViewerSize),
					windowPos = ToArrI(HandViewerWindowPos),
					windowSize = ToArrI(HandViewerWindowSize)
				} : null),
				damageOffsets = ((DamageOffsets.Count > 0) ? ToOffsetDict(DamageOffsets) : null)
			};
			JsonSerializerOptions val = new JsonSerializerOptions
			{
				WriteIndented = true
			};
			File.WriteAllText(LayoutPath, JsonSerializer.Serialize<Dto>(dto, val));
		}
		catch (System.Exception ex)
		{
			ModLog.Error("LayoutStore.Save", ex);
		}
	}

	private static void LoadUnified()
	{
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		string text = File.ReadAllText(LayoutPath);
		Dto dto = JsonSerializer.Deserialize<Dto>(text, (JsonSerializerOptions)null);
		if (dto == null)
		{
			return;
		}
		ClockPos = FromArr(dto.clockPos);
		PotionPos = FromArr(dto.potionPos);
		HandViewerPos = FromArr(dto.handViewer?.pos);
		HandViewerSize = FromArr(dto.handViewer?.size);
		HandViewerWindowPos = FromArrI(dto.handViewer?.windowPos);
		HandViewerWindowSize = FromArrI(dto.handViewer?.windowSize);
		DamageOffsets.Clear();
		if (dto.damageOffsets == null)
		{
			return;
		}
		var enumerator = dto.damageOffsets.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, float[]> current = enumerator.Current;
				Vector2? vector = FromArr(current.Value);
				if (vector.HasValue)
				{
					DamageOffsets[current.Key] = vector.Value;
				}
			}
		}
		finally
		{
			((System.IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
	}

	private static void MigrateFromLegacy()
	{
		string userDataDir = OS.GetUserDataDir();
		string path = Path.Combine(userDataDir, "betterspire2_clock_pos.txt");
		string path2 = Path.Combine(userDataDir, "betterspire2_potion_pos.txt");
		string path3 = Path.Combine(userDataDir, "betterspire2_handviewer_pos.txt");
		string path4 = Path.Combine(userDataDir, "betterspire2_damage_offsets.json");
		ClockPos = TryReadVec2Pair(path);
		PotionPos = TryReadVec2Pair(path2);
		TryReadHandViewer(path3, out HandViewerPos, out HandViewerSize);
		TryReadDamageOffsets(path4);
		Save();
		if (File.Exists(LayoutPath))
		{
			TryDelete(path);
			TryDelete(path2);
			TryDelete(path3);
			TryDelete(path4);
		}
	}

	private static Vector2? TryReadVec2Pair(string path)
	{
		try
		{
			if (!File.Exists(path))
			{
				return null;
			}
			string[] array = File.ReadAllText(path).Split(',', (StringSplitOptions)0);
			float x = default(float);
			float y = default(float);
			if (array.Length == 2 && float.TryParse(array[0], out x) && float.TryParse(array[1], out y))
			{
				return new Vector2(x, y);
			}
		}
		catch (System.Exception ex)
		{
			ModLog.Error("LayoutStore.TryReadVec2Pair(" + path + ")", ex);
		}
		return null;
	}

	private static void TryReadHandViewer(string path, out Vector2? pos, out Vector2? size)
	{
		pos = null;
		size = null;
		try
		{
			if (!File.Exists(path))
			{
				return;
			}
			string[] array = File.ReadAllText(path).Split('|', (StringSplitOptions)0);
			if (array.Length >= 1 && !string.IsNullOrEmpty(array[0]))
			{
				string[] array2 = array[0].Split(',', (StringSplitOptions)0);
				float x = default(float);
				float y = default(float);
				if (array2.Length == 2 && float.TryParse(array2[0], out x) && float.TryParse(array2[1], out y))
				{
					pos = new Vector2(x, y);
				}
			}
			if (array.Length >= 2 && !string.IsNullOrEmpty(array[1]))
			{
				string[] array3 = array[1].Split(',', (StringSplitOptions)0);
				float x2 = default(float);
				float y2 = default(float);
				if (array3.Length == 2 && float.TryParse(array3[0], out x2) && float.TryParse(array3[1], out y2))
				{
					size = new Vector2(x2, y2);
				}
			}
		}
		catch (System.Exception ex)
		{
			ModLog.Error("LayoutStore.TryReadHandViewer(" + path + ")", ex);
		}
	}

	private static void TryReadDamageOffsets(string path)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (!File.Exists(path))
			{
				return;
			}
			string text = File.ReadAllText(path);
			Dictionary<string, float[]> val = JsonSerializer.Deserialize<Dictionary<string, float[]>>(text, (JsonSerializerOptions)null);
			if (val == null)
			{
				return;
			}
			var enumerator = val.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, float[]> current = enumerator.Current;
					if (current.Value != null && current.Value.Length == 2)
					{
						DamageOffsets[current.Key] = new Vector2(current.Value[0], current.Value[1]);
					}
				}
			}
			finally
			{
				((System.IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
		}
		catch (System.Exception ex)
		{
			ModLog.Error("LayoutStore.TryReadDamageOffsets(" + path + ")", ex);
		}
	}

	private static void TryDelete(string path)
	{
		try
		{
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}
		catch (System.Exception ex)
		{
			ModLog.Error("LayoutStore.TryDelete(" + path + ")", ex);
		}
	}

	private static float[]? ToArr(Vector2? v)
	{
		return (!v.HasValue) ? null : new float[2]
		{
			v.Value.X,
			v.Value.Y
		};
	}

	private static int[]? ToArrI(Vector2I? v)
	{
		return (!v.HasValue) ? null : new int[2]
		{
			v.Value.X,
			v.Value.Y
		};
	}

	private static Vector2? FromArr(float[]? a)
	{
		return (a != null && a.Length == 2) ? new Vector2?(new Vector2(a[0], a[1])) : ((Vector2?)null);
	}

	private static Vector2I? FromArrI(int[]? a)
	{
		return (a != null && a.Length == 2) ? new Vector2I?(new Vector2I(a[0], a[1])) : ((Vector2I?)null);
	}

	private static Dictionary<string, float[]> ToOffsetDict(Dictionary<string, Vector2> src)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<string, float[]> val = new Dictionary<string, float[]>(src.Count);
		var enumerator = src.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, Vector2> current = enumerator.Current;
				val[current.Key] = new float[2]
				{
					current.Value.X,
					current.Value.Y
				};
			}
		}
		finally
		{
			((System.IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		return val;
	}
}
