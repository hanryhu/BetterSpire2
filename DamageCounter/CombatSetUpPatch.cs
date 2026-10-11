using System;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;

namespace BetterSpire2;

[HarmonyPatch(typeof(CombatManager), "SetUpCombat")]
public class CombatSetUpPatch
{
	private static void Postfix()
	{
		try
		{
			DeckTracker.OnCombatSetUp();
		}
		catch (System.Exception ex)
		{
			ModLog.Error("CombatSetUpPatch", ex);
		}
	}
}
