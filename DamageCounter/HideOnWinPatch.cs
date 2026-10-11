using System;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;

namespace BetterSpire2;

[HarmonyPatch(typeof(CombatManager), "EndCombatInternal", new System.Type[] { })]
public class HideOnWinPatch
{
	private static void Prefix()
	{
		DamageTracker.Hide();
		DeckTracker.Hide();
		InstantSpeedHelper.OnCombatEnd();
		PlayCountHelper.ClearCache();
	}
}
