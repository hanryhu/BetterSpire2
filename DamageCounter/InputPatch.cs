using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Nodes;

namespace BetterSpire2;

[HarmonyPatch(typeof(NGame), "_Input")]
public class InputPatch
{
	private static bool _lastPaused;

	private static bool _feedbackLookupAttempted;

	private static FieldInfo? _feedbackScreenField;

	private static PropertyInfo? _feedbackScreenProp;

	private static PropertyInfo? _feedbackVisibleProp;

	[MethodImpl((MethodImplOptions)8)]
	private static bool IsFeedbackScreenVisible()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (!_feedbackLookupAttempted)
			{
				_feedbackLookupAttempted = true;
				BindingFlags val = (BindingFlags)52;
				_feedbackScreenProp = typeof(NGame).GetProperty("FeedbackScreen", val);
				_feedbackScreenField = typeof(NGame).GetField("FeedbackScreen", val);
				PropertyInfo? feedbackScreenProp = _feedbackScreenProp;
				object obj = ((feedbackScreenProp != null) ? feedbackScreenProp.PropertyType : null);
				if (obj == null)
				{
					FieldInfo? feedbackScreenField = _feedbackScreenField;
					obj = ((feedbackScreenField != null) ? feedbackScreenField.FieldType : null);
				}
				System.Type type = (System.Type)obj;
				if (type != (System.Type)null)
				{
					_feedbackVisibleProp = type.GetProperty("Visible", (BindingFlags)20);
				}
			}
			if (_feedbackVisibleProp == (PropertyInfo)null)
			{
				return false;
			}
			NGame instance = NGame.Instance;
			if (instance == null)
			{
				return false;
			}
			PropertyInfo? feedbackScreenProp2 = _feedbackScreenProp;
			object obj2 = ((feedbackScreenProp2 != null) ? feedbackScreenProp2.GetValue((object)instance) : null);
			if (obj2 == null)
			{
				FieldInfo? feedbackScreenField2 = _feedbackScreenField;
				obj2 = ((feedbackScreenField2 != null) ? feedbackScreenField2.GetValue((object)instance) : null);
			}
			object obj3 = obj2;
			if (obj3 == null)
			{
				return false;
			}
			object value = _feedbackVisibleProp.GetValue(obj3);
			bool flag = default(bool);
			int num;
			if (value is bool)
			{
				flag = (bool)value;
				num = 1;
			}
			else
			{
				num = 0;
			}
			return (byte)((uint)num & (flag ? 1u : 0u)) != 0;
		}
		catch
		{
			return false;
		}
	}

	private static void Prefix(InputEvent inputEvent)
	{
		try
		{
		}
		catch (System.Exception ex)
		{
			ModLog.Error("InputPatch.Prefix", ex);
		}
	}

	private static void Postfix(InputEvent inputEvent)
	{
		try
		{
			ClockDisplay.Update();
			ClockDisplay.HandleInput(inputEvent);
			ModConfigBridge.TryRegister();
			bool flag = CombatManager.Instance?.IsPaused ?? false;
			SettingsMenu.SetCanvasHidden(flag);
			DeckTracker.SetCanvasHidden(flag);
			if (flag != _lastPaused)
			{
				_lastPaused = flag;
				NativeHandBadge.RefreshAll();
			}
			if ((inputEvent is InputEventKey && IsFeedbackScreenVisible()) || (inputEvent is InputEventKey ev && SettingsMenu.IsCapturingHotkey && SettingsMenu.HandleHotkeyCapture(ev)))
			{
				return;
			}
			if (inputEvent is InputEventKey inputEventKey && (int)inputEventKey.Keycode == ModSettings.SettingsHotkey && inputEventKey.Pressed && !inputEventKey.IsEcho())
			{
				SettingsMenu.Toggle();
				return;
			}
			if (inputEvent is InputEventKey inputEventKey2 && inputEventKey2.Keycode == Key.F3 && inputEventKey2.Pressed && !inputEventKey2.IsEcho())
			{
				DeckTracker.Toggle();
				return;
			}
			if (inputEvent is InputEventKey { Pressed: not false } inputEventKey3 && !inputEventKey3.IsEcho())
			{
				if (inputEventKey3.Keycode == Key.Pagedown)
				{
					DeckTracker.NextPage();
					return;
				}
				if (inputEventKey3.Keycode == Key.Pageup)
				{
					DeckTracker.PrevPage();
					return;
				}
			}
			if ((inputEvent is InputEventMouseButton || inputEvent is InputEventMouseMotion) ? true : false)
			{
				SettingsMenu.HandleMouseInput(inputEvent);
				DeckTracker.HandleMouseInput(inputEvent);
				DamageTracker.HandleMouseInput(inputEvent);
				PotionChanceBadgePatch.HandleMouseInput(inputEvent);
			}
		}
		catch (System.Exception ex)
		{
			ModLog.Error("InputPatch", ex);
		}
	}
}
