using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace BetterSpire2;

[HarmonyPatch(typeof(NCard), "UpdateVisuals")]
internal static class NCardUpdateVisualsPatch
{
	private static void Postfix(NCard __instance)
	{
		NativeHandBadge.UpdateBadge(__instance);
	}
}
