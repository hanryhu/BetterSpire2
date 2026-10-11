using System;
using System.Reflection;
using System.Runtime.InteropServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace BetterSpire2;

[ModInitializer("Init")]
public class ModEntry
{
	private static bool _initialized;

	private static readonly System.Type[] _patchClasses = new System.Type[17]
	{
		typeof(IntentLabelPatch),
		typeof(RecalcOnRefreshIntentsPatch),
		typeof(RecalcPeriodicPatch),
		typeof(CombatSetUpPatch),
		typeof(HideOnResetPatch),
		typeof(HideOnWinPatch),
		typeof(HideOnLosePatch),
		typeof(RestoreInstantFastModePatch),
		typeof(HideOnReturnToMainMenuPatch),
		typeof(InputPatch),
		typeof(SkipSplashPatch),
		typeof(NCardUpdateVisualsPatch),
		typeof(NCardEnterTreePatch),
		typeof(NCardExitTreePatch),
		typeof(MapPointRemovePriceTooltipPatch),
		typeof(MapOverlayHidePatch),
		typeof(PotionChanceBadgePatch)
	};

	public static void Init()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (_initialized)
		{
			return;
		}
		_initialized = true;
		ModLog.Init();
		ModLog.Info("ModEntry.Init() starting");
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			try
			{
				ModLog.Info("Linux detected — loading libgcc_s for Harmony compatibility");
				nint num = LinuxNative.dlopen("libgcc_s.so.1", 258);
				if (num == (nint)System.IntPtr.Zero)
				{
					ModLog.Info("  dlopen failed: " + Marshal.PtrToStringAnsi((System.IntPtr)LinuxNative.dlerror()));
				}
				else
				{
					ModLog.Info("  libgcc_s loaded successfully");
				}
			}
			catch (DllNotFoundException)
			{
				ModLog.Info("  libdl.so.2 not found — skipping (likely Android)");
			}
		}
		ModSettings.Load();
		ModLog.Info("Settings loaded");
		LayoutStore.Init();
		Localization.Init();
		Harmony harmony = new Harmony("com.jdr.betterspire2lite");
		int succeeded = 0;
		int failed = 0;
		System.Type[] patchClasses = _patchClasses;
		foreach (System.Type type in patchClasses)
		{
			try
			{
				harmony.CreateClassProcessor(type).Patch();
				ModLog.Info("  Patched: " + ((MemberInfo)type).Name);
				succeeded++;
			}
			catch (System.Exception ex2)
			{
				ModLog.Error("Patch " + ((MemberInfo)type).Name, ex2);
				failed++;
			}
		}
		PatchDrawingMethods(harmony, ref succeeded, ref failed);
		ModLog.Info($"Harmony patching complete: {succeeded} succeeded, {failed} failed");
		ModLog.Info("ModEntry.Init() complete");
		if (ModSettings.ShowClock)
		{
			ClockDisplay.Toggle(on: true);
		}
	}

	private static void PatchDrawingMethods(Harmony harmony, ref int succeeded, ref int failed)
	{
		try
		{
			MethodInfo val = AccessTools.Method(typeof(NMapDrawings), "HandleDrawingMessage");
			if (val != (MethodInfo)null)
			{
				HarmonyMethod prefix = new HarmonyMethod(typeof(MuteDrawingsPatch), "Prefix");
				harmony.Patch((MethodBase)(object)val, prefix);
				ModLog.Info("  Patched: MuteDrawingsPatch (manual)");
				succeeded++;
			}
			else
			{
				ModLog.Info("  Skipped: MuteDrawingsPatch — method not found");
				failed++;
			}
		}
		catch (System.Exception ex)
		{
			ModLog.Error("Patch MuteDrawingsPatch (manual)", ex);
			failed++;
		}
		try
		{
			MethodInfo val2 = AccessTools.Method(typeof(NMapDrawings), "HandleClearMapDrawingsMessage");
			if (val2 != (MethodInfo)null)
			{
				HarmonyMethod prefix2 = new HarmonyMethod(typeof(MuteClearDrawingsPatch), "Prefix");
				harmony.Patch((MethodBase)(object)val2, prefix2);
				ModLog.Info("  Patched: MuteClearDrawingsPatch (manual)");
				succeeded++;
			}
			else
			{
				ModLog.Info("  Skipped: MuteClearDrawingsPatch — method not found");
				failed++;
			}
		}
		catch (System.Exception ex2)
		{
			ModLog.Error("Patch MuteClearDrawingsPatch (manual)", ex2);
			failed++;
		}
	}
}
