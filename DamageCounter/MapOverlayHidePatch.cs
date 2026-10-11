using System;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace BetterSpire2;

[HarmonyPatch(typeof(NMapScreen), "_Ready")]
internal static class MapOverlayHidePatch
{
	private static void Postfix(NMapScreen __instance)
	{
		try
		{
			__instance.Opened += OnMapOpened;
			__instance.Closed += OnMapClosed;
		}
		catch (System.Exception ex)
		{
			ModLog.Error("MapOverlayHidePatch._Ready", ex);
		}
	}

	private static void OnMapOpened()
	{
		try
		{
			DeckTracker.SetCanvasHidden(hidden: true);
			DamageTracker.Hide();
			NativeHandBadge.RefreshAll();
		}
		catch (System.Exception ex)
		{
			ModLog.Error("MapOverlayHidePatch.OnMapOpened", ex);
		}
	}

	private static void OnMapClosed()
	{
		try
		{
			DeckTracker.SetCanvasHidden(hidden: false);
			DamageTracker.Recalculate();
			NativeHandBadge.RefreshAll();
		}
		catch (System.Exception ex)
		{
			ModLog.Error("MapOverlayHidePatch.OnMapClosed", ex);
		}
	}
}
