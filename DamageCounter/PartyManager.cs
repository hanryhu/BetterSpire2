using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace BetterSpire2;

public static class PartyManager
{
	private static readonly HashSet<ulong> _mutedDrawings = new HashSet<ulong>();

	public static NMapDrawings? MapDrawings;

	private static readonly MethodInfo _getDrawingState = AccessTools.Method(typeof(NMapDrawings), "GetDrawingStateForPlayer");

	private static readonly MethodInfo _clearForPlayer = AccessTools.Method(typeof(NMapDrawings), "ClearAllLinesForPlayer");

	private static readonly MethodInfo _clearAll = AccessTools.Method(typeof(NMapDrawings), "ClearAllLines");

	public static bool IsDrawingMuted(ulong netId)
	{
		return _mutedDrawings.Contains(netId);
	}

	public static void ToggleDrawingMute(ulong netId)
	{
		if (!_mutedDrawings.Remove(netId))
		{
			_mutedDrawings.Add(netId);
		}
	}

	public static void ClearDrawingsForPlayer(ulong netId)
	{
		try
		{
			if (MapDrawings != null && !(_getDrawingState == (MethodInfo)null) && !(_clearForPlayer == (MethodInfo)null))
			{
				object obj = ((MethodBase)_getDrawingState).Invoke((object)MapDrawings, new object[1] { netId });
				if (obj != null)
				{
					((MethodBase)_clearForPlayer).Invoke((object)MapDrawings, new object[1] { obj });
				}
			}
		}
		catch (System.Exception ex)
		{
			ModLog.Error("PartyManager.ClearDrawingsForPlayer", ex);
		}
	}

	public static void ClearAllDrawings()
	{
		try
		{
			if (MapDrawings != null && !(_clearAll == (MethodInfo)null))
			{
				((MethodBase)_clearAll).Invoke((object)MapDrawings, (object[])null);
			}
		}
		catch (System.Exception ex)
		{
			ModLog.Error("PartyManager.ClearAllDrawings", ex);
		}
	}

	public static void ClearMutes()
	{
		_mutedDrawings.Clear();
	}
}
