using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace BetterSpire2;

public static class PlayCountHelper
{
	private static MethodInfo? _modifyCardPlayCount;

	private static bool _lookupFailed;

	private static readonly Dictionary<CardModel, ValueTuple<ulong, int>> _cache = new Dictionary<CardModel, ValueTuple<ulong, int>>();

	private const ulong CacheTtlMs = 150uL;

	public static void ClearCache()
	{
		_cache.Clear();
	}

	public static int GetEffectivePlayCount(CardModel? card)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		if (card == null)
		{
			return 1;
		}
		try
		{
			ulong ticksMsec = Time.GetTicksMsec();
			ValueTuple<ulong, int> val = default(ValueTuple<ulong, int>);
			if (_cache.TryGetValue(card, out val) && ticksMsec - val.Item1 < 150)
			{
				return val.Item2;
			}
			CombatState combatState = CombatManager.Instance?.DebugOnlyGetState();
			if (combatState == null)
			{
				return 1;
			}
			int num = card.GetEnchantedReplayCount() + 1;
			if (_modifyCardPlayCount == (MethodInfo)null && !_lookupFailed)
			{
				_modifyCardPlayCount = Enumerable.FirstOrDefault<MethodInfo>((System.Collections.Generic.IEnumerable<MethodInfo>)typeof(Hook).GetMethods((BindingFlags)24), (Func<MethodInfo, bool>)((MethodInfo m) => ((MemberInfo)m).Name == "ModifyCardPlayCount"));
				if (_modifyCardPlayCount == (MethodInfo)null)
				{
					_lookupFailed = true;
					ModLog.Info("PlayCountHelper: Hook.ModifyCardPlayCount not found — returning baseline");
				}
			}
			if (_modifyCardPlayCount == (MethodInfo)null)
			{
				int num2 = ((num < 1) ? 1 : num);
				_cache[card] = new ValueTuple<ulong, int>(ticksMsec, num2);
				return num2;
			}
			object[] array = new object[5] { combatState, card, num, null, null };
			int num3 = (int)((MethodBase)_modifyCardPlayCount).Invoke((object)null, array);
			int num4 = ((num3 < 1) ? 1 : num3);
			_cache[card] = new ValueTuple<ulong, int>(ticksMsec, num4);
			return num4;
		}
		catch (System.Exception ex)
		{
			ModLog.Error("PlayCountHelper.GetEffectivePlayCount", ex);
			return 1;
		}
	}
}
