using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace BetterSpire2;

[HarmonyPatch(typeof(NCard), "_EnterTree")]
internal static class NCardEnterTreePatch
{
	private static void Postfix(NCard __instance)
	{
		NativeHandBadge.OnEnterTree(__instance);
	}
}
