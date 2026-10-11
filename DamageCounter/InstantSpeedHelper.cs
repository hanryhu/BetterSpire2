using System;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;

namespace BetterSpire2;

public static class InstantSpeedHelper
{
	private static bool _combatSpeedActive;

	private static FastModeType _preCombatSpeed;

	public static void OnCombatStart()
	{
		try
		{
			if (!ModSettings.InstantFastMode || _combatSpeedActive)
			{
				return;
			}
			PrefsSave prefsSave = SaveManager.Instance?.PrefsSave;
			if (prefsSave != null)
			{
				bool flag = IsRealMultiplayer();
				FastModeType fastModeType = ((flag && !ModSettings.AllowInstantInMultiplayer) ? FastModeType.Fast : FastModeType.Instant);
				if (prefsSave.FastMode != fastModeType)
				{
					_preCombatSpeed = prefsSave.FastMode;
					prefsSave.FastMode = fastModeType;
					_combatSpeedActive = true;
					ModLog.Info($"Instant Fast Mode: applied {fastModeType} (multiplayer={flag}, allowInstantInMp={ModSettings.AllowInstantInMultiplayer}, was {_preCombatSpeed})");
				}
			}
		}
		catch (System.Exception ex)
		{
			ModLog.Error("InstantSpeedHelper.OnCombatStart", ex);
		}
	}

	public static void OnCombatEnd()
	{
		try
		{
			if (_combatSpeedActive)
			{
				PrefsSave prefsSave = SaveManager.Instance?.PrefsSave;
				if (prefsSave != null)
				{
					prefsSave.FastMode = _preCombatSpeed;
				}
				_combatSpeedActive = false;
			}
		}
		catch (System.Exception ex)
		{
			ModLog.Error("InstantSpeedHelper.OnCombatEnd", ex);
		}
	}

	private static bool IsRealMultiplayer()
	{
		try
		{
			INetGameService netGameService = RunManager.Instance?.NetService;
			return netGameService != null && netGameService.Type != NetGameType.Singleplayer;
		}
		catch
		{
			return false;
		}
	}
}
