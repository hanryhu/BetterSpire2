using System;
using Godot;
using MegaCrit.Sts2.Core.Nodes;

namespace BetterSpire2;

public static class ClockDisplay
{
	private static Label? _label;

	private static CanvasLayer? _layer;

	private static ulong _lastUpdate;

	private static bool _dragging;

	private static Vector2 _dragOffset;

	public static void Toggle(bool on)
	{
		if (on)
		{
			EnsureCreated();
		}
		else
		{
			Remove();
		}
	}

	public static void Update()
	{
		if (!ModSettings.ShowClock)
		{
			return;
		}
		ulong ticksMsec = Time.GetTicksMsec();
		if (ticksMsec - _lastUpdate >= 1000)
		{
			_lastUpdate = ticksMsec;
			EnsureCreated();
			if (_label != null && GodotObject.IsInstanceValid(_label))
			{
				_label.Text = (ModSettings.Clock24Hour ? System.DateTime.Now.ToString("HH:mm") : System.DateTime.Now.ToString("h:mm tt"));
			}
		}
	}

	public static void HandleInput(InputEvent @event)
	{
		if (!ModSettings.ShowClock || _label == null || !GodotObject.IsInstanceValid(_label))
		{
			return;
		}
		if (@event is InputEventMouseButton inputEventMouseButton && inputEventMouseButton.ButtonIndex == MouseButton.Left)
		{
			Vector2 position = inputEventMouseButton.Position;
			Rect2 rect = new Rect2(_label.GlobalPosition, _label.Size);
			if (inputEventMouseButton.Pressed && rect.HasPoint(position))
			{
				_dragging = true;
				_dragOffset = position - _label.Position;
			}
			else if (!inputEventMouseButton.Pressed && _dragging)
			{
				_dragging = false;
				LayoutStore.ClockPos = _label.Position;
				LayoutStore.Save();
			}
		}
		else if (@event is InputEventMouseMotion inputEventMouseMotion && _dragging)
		{
			_label.Position = inputEventMouseMotion.Position - _dragOffset;
		}
	}

	private static void EnsureCreated()
	{
		if (_label != null && GodotObject.IsInstanceValid(_label))
		{
			return;
		}
		NGame instance = NGame.Instance;
		if (instance != null)
		{
			_layer = new CanvasLayer();
			_layer.Layer = 100;
			_label = new Label();
			_label.Text = (ModSettings.Clock24Hour ? System.DateTime.Now.ToString("HH:mm") : System.DateTime.Now.ToString("h:mm tt"));
			_label.HorizontalAlignment = HorizontalAlignment.Right;
			_label.AddThemeColorOverride("font_color", new Color(0.7f, 0.7f, 0.7f, 0.6f));
			_label.AddThemeFontSizeOverride("font_size", 14);
			_label.MouseFilter = Control.MouseFilterEnum.Ignore;
			if (LayoutStore.ClockPos.HasValue)
			{
				_label.Position = LayoutStore.ClockPos.Value;
			}
			else
			{
				Vector2 vector = instance.GetViewport()?.GetVisibleRect().Size ?? new Vector2(1920f, 1080f);
				_label.Position = new Vector2(vector.X - 200f, 148f);
			}
			_label.Size = new Vector2(190f, 20f);
			_layer.AddChild(_label, forceReadableName: false, Node.InternalMode.Disabled);
			instance.AddChild(_layer, forceReadableName: false, Node.InternalMode.Disabled);
		}
	}

	private static void Remove()
	{
		if (_layer != null && GodotObject.IsInstanceValid(_layer))
		{
			_layer.QueueFree();
		}
		_layer = null;
		_label = null;
		_dragging = false;
	}
}
