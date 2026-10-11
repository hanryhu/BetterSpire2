using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes;

namespace BetterSpire2;

[HarmonyPatch(typeof(NGame), "ReturnToMainMenu")]
public class HideOnReturnToMainMenuPatch
{
	private static void Prefix()
	{
		DamageTracker.Hide();
		DeckTracker.Hide();
		InstantSpeedHelper.OnCombatEnd();
	}
}
