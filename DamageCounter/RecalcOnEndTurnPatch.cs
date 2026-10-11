using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;

namespace BetterSpire2;

[HarmonyPatch(typeof(CombatManager), "SetReadyToEndTurn")]
public class RecalcOnEndTurnPatch
{
	private static void Postfix()
	{
		DamageTracker.Recalculate();
	}
}
