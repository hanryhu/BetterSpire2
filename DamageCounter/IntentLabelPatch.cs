using System;
using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.addons.mega_text;

namespace BetterSpire2;

[HarmonyPatch(typeof(NIntent), "UpdateVisuals")]
public class IntentLabelPatch
{
	private static void Postfix(AbstractIntent ____intent, Creature ____owner, System.Collections.Generic.IEnumerable<Creature> ____targets, MegaRichTextLabel ____valueLabel)
	{
		try
		{
			if (!ModSettings.MultiHitTotals || !(____intent is AttackIntent attackIntent) || ____targets == null || ____owner == null || ____valueLabel == null)
			{
				return;
			}
			int singleDamage = attackIntent.GetSingleDamage(____targets, ____owner);
			int totalDamage = attackIntent.GetTotalDamage(____targets, ____owner);
			if (singleDamage > 0 && totalDamage > singleDamage)
			{
				string text = ____valueLabel.Text?.Trim() ?? "";
				if (!text.Contains("("))
				{
					____valueLabel.Text = text + $" ({totalDamage})";
				}
			}
		}
		catch (System.Exception ex)
		{
			ModLog.Error("IntentLabelPatch", ex);
		}
	}
}
