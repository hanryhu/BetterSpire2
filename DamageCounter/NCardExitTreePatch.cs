using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace BetterSpire2;

[HarmonyPatch(typeof(NCard), "_ExitTree")]
internal static class NCardExitTreePatch
{
	private static void Postfix(NCard __instance)
	{
		NativeHandBadge.OnExitTree(__instance);
	}
}
