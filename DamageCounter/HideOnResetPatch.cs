using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;

namespace BetterSpire2;

[HarmonyPatch(typeof(CombatManager), "Reset")]
public class HideOnResetPatch
{
	private static void Postfix()
	{
		DamageTracker.Hide();
	}
}
