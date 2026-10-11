using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using Microsoft.CSharp.RuntimeBinder;
using Binder = Microsoft.CSharp.RuntimeBinder.Binder;

namespace BetterSpire2;

public static class DeckTracker
{
	[Flags]
	private enum ResizeEdge
	{
		None = 0,
		Right = 1,
		Bottom = 2
	}

	private class HandPanel : PanelContainer
	{
	}

	private class PlayerHandSection
	{
		private class PotionIcon : TextureRect
		{
			[CompilerGenerated]
			private static class _003C_003Eo__3
			{
				public static CallSite<Func<CallSite, object, object>> _003C_003Ep__0;

				public static CallSite<Func<CallSite, object, Texture2D>> _003C_003Ep__1;

				public static CallSite<Func<CallSite, object, object>> _003C_003Ep__2;

				public static CallSite<Func<CallSite, object, object>> _003C_003Ep__3;

				public static CallSite<Func<CallSite, object, string>> _003C_003Ep__4;

				public static CallSite<Func<CallSite, object, object>> _003C_003Ep__5;

				public static CallSite<Func<CallSite, object, object>> _003C_003Ep__6;

				public static CallSite<Func<CallSite, object, string>> _003C_003Ep__7;
			}

			private readonly string _potionName;

			private readonly string _potionDesc;

			private Control? _tooltip;

			public PotionIcon(dynamic potion)
			{
				//IL_0314: Unknown result type (might be due to invalid IL or missing references)
				//IL_031e: Expected O, but got Unknown
				//IL_0327: Unknown result type (might be due to invalid IL or missing references)
				//IL_0331: Expected O, but got Unknown
				try
				{
					if (_003C_003Eo__3._003C_003Ep__1 == null)
					{
						_003C_003Eo__3._003C_003Ep__1 = CallSite<Func<CallSite, object, Texture2D>>.Create(Binder.Convert((CSharpBinderFlags)0, typeof(Texture2D), typeof(PotionIcon)));
					}
					Func<CallSite, object, Texture2D> target = _003C_003Eo__3._003C_003Ep__1.Target;
					CallSite<Func<CallSite, object, Texture2D>> _003C_003Ep__ = _003C_003Eo__3._003C_003Ep__1;
					if (_003C_003Eo__3._003C_003Ep__0 == null)
					{
						_003C_003Eo__3._003C_003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember((CSharpBinderFlags)0, "Image", typeof(PotionIcon), (System.Collections.Generic.IEnumerable<CSharpArgumentInfo>)(object)new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create((CSharpArgumentInfoFlags)0, (string)null) }));
					}
					base.Texture = target.Invoke((CallSite)(object)_003C_003Ep__, _003C_003Eo__3._003C_003Ep__0.Target.Invoke((CallSite)(object)_003C_003Eo__3._003C_003Ep__0, (object)potion));
					if (_003C_003Eo__3._003C_003Ep__4 == null)
					{
						_003C_003Eo__3._003C_003Ep__4 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert((CSharpBinderFlags)0, typeof(string), typeof(PotionIcon)));
					}
					Func<CallSite, object, string> target2 = _003C_003Eo__3._003C_003Ep__4.Target;
					CallSite<Func<CallSite, object, string>> _003C_003Ep__2 = _003C_003Eo__3._003C_003Ep__4;
					if (_003C_003Eo__3._003C_003Ep__2 == null)
					{
						_003C_003Eo__3._003C_003Ep__2 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember((CSharpBinderFlags)0, "Title", typeof(PotionIcon), (System.Collections.Generic.IEnumerable<CSharpArgumentInfo>)(object)new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create((CSharpArgumentInfoFlags)0, (string)null) }));
					}
					object obj = _003C_003Eo__3._003C_003Ep__2.Target.Invoke((CallSite)(object)_003C_003Eo__3._003C_003Ep__2, (object)potion);
					object obj2;
					if (obj == null)
					{
						obj2 = null;
					}
					else
					{
						if (_003C_003Eo__3._003C_003Ep__3 == null)
						{
							_003C_003Eo__3._003C_003Ep__3 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember((CSharpBinderFlags)0, "GetFormattedText", (System.Collections.Generic.IEnumerable<System.Type>)null, typeof(PotionIcon), (System.Collections.Generic.IEnumerable<CSharpArgumentInfo>)(object)new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create((CSharpArgumentInfoFlags)0, (string)null) }));
						}
						obj2 = _003C_003Eo__3._003C_003Ep__3.Target.Invoke((CallSite)(object)_003C_003Eo__3._003C_003Ep__3, obj);
					}
					if (obj2 == null)
					{
						obj2 = "???";
					}
					_potionName = target2.Invoke((CallSite)(object)_003C_003Ep__2, obj2);
					if (_003C_003Eo__3._003C_003Ep__7 == null)
					{
						_003C_003Eo__3._003C_003Ep__7 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert((CSharpBinderFlags)0, typeof(string), typeof(PotionIcon)));
					}
					Func<CallSite, object, string> target3 = _003C_003Eo__3._003C_003Ep__7.Target;
					CallSite<Func<CallSite, object, string>> _003C_003Ep__3 = _003C_003Eo__3._003C_003Ep__7;
					if (_003C_003Eo__3._003C_003Ep__5 == null)
					{
						_003C_003Eo__3._003C_003Ep__5 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember((CSharpBinderFlags)0, "DynamicDescription", typeof(PotionIcon), (System.Collections.Generic.IEnumerable<CSharpArgumentInfo>)(object)new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create((CSharpArgumentInfoFlags)0, (string)null) }));
					}
					obj = _003C_003Eo__3._003C_003Ep__5.Target.Invoke((CallSite)(object)_003C_003Eo__3._003C_003Ep__5, (object)potion);
					object obj3;
					if (obj == null)
					{
						obj3 = null;
					}
					else
					{
						if (_003C_003Eo__3._003C_003Ep__6 == null)
						{
							_003C_003Eo__3._003C_003Ep__6 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember((CSharpBinderFlags)0, "GetFormattedText", (System.Collections.Generic.IEnumerable<System.Type>)null, typeof(PotionIcon), (System.Collections.Generic.IEnumerable<CSharpArgumentInfo>)(object)new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create((CSharpArgumentInfoFlags)0, (string)null) }));
						}
						obj3 = _003C_003Eo__3._003C_003Ep__6.Target.Invoke((CallSite)(object)_003C_003Eo__3._003C_003Ep__6, obj);
					}
					if (obj3 == null)
					{
						obj3 = "";
					}
					_potionDesc = target3.Invoke((CallSite)(object)_003C_003Ep__3, obj3);
				}
				catch
				{
					_potionName = "???";
					_potionDesc = "";
				}
				_potionDesc = Regex.Replace(_potionDesc, "\\[/?[^\\]]+\\]", "");
				_potionDesc = Regex.Replace(_potionDesc, "res://\\S+", "").Trim();
				base.CustomMinimumSize = new Vector2(22f, 22f);
				base.ExpandMode = ExpandModeEnum.IgnoreSize;
				base.StretchMode = StretchModeEnum.KeepAspectCentered;
				base.MouseFilter = MouseFilterEnum.Stop;
				base.MouseEntered += new Action(ShowTooltip);
				base.MouseExited += new Action(HideTooltip);
			}

			private void ShowTooltip()
			{
				HideTooltip();
				try
				{
					PanelContainer panelContainer = new PanelContainer();
					StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
					styleBoxFlat.BgColor = new Color(0.1f, 0.1f, 0.15f, 0.95f);
					styleBoxFlat.BorderColor = new Color(0.6f, 0.5f, 0.2f);
					styleBoxFlat.SetBorderWidthAll(1);
					styleBoxFlat.SetCornerRadiusAll(4);
					styleBoxFlat.SetContentMarginAll(8f);
					panelContainer.AddThemeStyleboxOverride("panel", styleBoxFlat);
					VBoxContainer vBoxContainer = new VBoxContainer();
					vBoxContainer.AddThemeConstantOverride("separation", 4);
					panelContainer.AddChild(vBoxContainer, forceReadableName: false, InternalMode.Disabled);
					Label label = new Label();
					label.Text = _potionName;
					label.AddThemeColorOverride("font_color", new Color(0.9f, 0.8f, 0.3f));
					label.AddThemeFontSizeOverride("font_size", 13);
					vBoxContainer.AddChild(label, forceReadableName: false, InternalMode.Disabled);
					if (!string.IsNullOrEmpty(_potionDesc))
					{
						Label label2 = new Label();
						label2.Text = _potionDesc;
						label2.AddThemeColorOverride("font_color", new Color(0.8f, 0.8f, 0.8f));
						label2.AddThemeFontSizeOverride("font_size", 11);
						label2.AutowrapMode = TextServer.AutowrapMode.WordSmart;
						label2.CustomMinimumSize = new Vector2(200f, 0f);
						vBoxContainer.AddChild(label2, forceReadableName: false, InternalMode.Disabled);
					}
					panelContainer.ZIndex = 300;
					if (_canvasLayer != null && GodotObject.IsInstanceValid(_canvasLayer))
					{
						_canvasLayer.AddChild(panelContainer, forceReadableName: false, InternalMode.Disabled);
						Vector2 position = base.GlobalPosition + new Vector2(0f, 26f);
						panelContainer.Position = position;
						_tooltip = panelContainer;
					}
				}
				catch (System.Exception ex)
				{
					ModLog.Error("PotionIcon tooltip show", ex);
				}
			}

			private void HideTooltip()
			{
				try
				{
					if (_tooltip != null && GodotObject.IsInstanceValid(_tooltip))
					{
						_tooltip.QueueFree();
					}
					_tooltip = null;
				}
				catch
				{
				}
			}
		}

		[CompilerGenerated]
		private static class _003C_003Eo__53
		{
			public static CallSite<Func<CallSite, System.Type, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, Control>> _003C_003Ep__1;
		}

		private readonly Player _player;

		private CardPile? _hand;

		private readonly List<Control> _cardControls = new List<Control>();

		private readonly List<CardModel> _subscribedCards = new List<CardModel>();

		private Container? _cardRow;

		private Label? _nameLabel;

		private HBoxContainer? _statsRow;

		private HBoxContainer? _potionRow;

		private readonly ulong _localNetId;

		private readonly string? _debugSuffix;

		private const int MaxHandSize = 12;

		private const int BaseCardWidth = 58;

		private const int BasePortraitHeight = 44;

		private static readonly Color AttackColor = new Color(0.9f, 0.35f, 0.3f);

		private static readonly Color SkillColor = new Color(0.3f, 0.55f, 0.9f);

		private static readonly Color PowerColor = new Color(0.9f, 0.75f, 0.2f);

		private static readonly Color StatusColor = new Color(0.5f, 0.5f, 0.5f);

		private static readonly Color CurseColor = new Color(0.7f, 0.2f, 0.7f);

		private static readonly Color DefaultCardColor = new Color(0.7f, 0.7f, 0.7f);

		private static readonly Color EnergyColor = new Color(0.3f, 0.7f, 1f);

		private static readonly Color StarsColor = new Color(1f, 0.85f, 0.3f);

		private bool _subscribedToHistory;

		private const int PotionIconSize = 22;

		private static Texture2D? _iconVuln;

		private static Texture2D? _iconWeak;

		private static Texture2D? _iconDefend;

		private static Texture2D? _iconStars;

		private static readonly Dictionary<string, Texture2D?> _energyIconCache = new Dictionary<string, Texture2D>();

		private const int StatIconSize = 14;

		private static float CardScale => (float)ModSettings.CardScalePercent / 100f;

		private static int CardWidth => (int)(58f * CardScale);

		private static int PortraitHeight => (int)(44f * CardScale);

		public bool IsHandNull => _hand == null;

		private static int ScaledFont(int baseSize)
		{
			return Math.Max(7, (int)((float)baseSize * CardScale));
		}

		public PlayerHandSection(Player player, ulong localNetId, string? debugSuffix = null)
		{
			_player = player;
			_hand = player.PlayerCombatState?.Hand;
			_localNetId = localNetId;
			_debugSuffix = debugSuffix;
		}

		public void AddTo(VBoxContainer parent)
		{
			parent.AddChild(new HSeparator(), forceReadableName: false, Node.InternalMode.Disabled);
			bool flag = _player.NetId == _localNetId;
			string playerName = PlatformUtil.GetPlayerName(PlatformType.Steam, _player.NetId);
			string text = _player.Character?.Title?.GetFormattedText();
			string text2 = ((!string.IsNullOrEmpty(playerName)) ? playerName : (text ?? "Player"));
			if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(playerName))
			{
				text2 = playerName + " (" + text + ")";
			}
			if (flag && _debugSuffix == null)
			{
				text2 += " (You)";
			}
			if (_debugSuffix != null)
			{
				text2 += _debugSuffix;
			}
			_nameLabel = new Label();
			UpdateNameLabel(text2);
			_nameLabel.AddThemeColorOverride("font_color", flag ? new Color(0.5f, 0.8f, 0.5f) : new Color(0.9f, 0.9f, 0.9f));
			_nameLabel.AddThemeFontSizeOverride("font_size", 14);
			parent.AddChild(_nameLabel, forceReadableName: false, Node.InternalMode.Disabled);
			HBoxContainer hBoxContainer = new HBoxContainer();
			hBoxContainer.AddThemeConstantOverride("separation", 6);
			parent.AddChild(hBoxContainer, forceReadableName: false, Node.InternalMode.Disabled);
			_statsRow = new HBoxContainer();
			_statsRow.AddThemeConstantOverride("separation", 10);
			hBoxContainer.AddChild(_statsRow, forceReadableName: false, Node.InternalMode.Disabled);
			_potionRow = new HBoxContainer();
			_potionRow.AddThemeConstantOverride("separation", 4);
			hBoxContainer.AddChild(_potionRow, forceReadableName: false, Node.InternalMode.Disabled);
			RefreshPotions();
			if (!ModSettings.CompactHandViewer)
			{
				HFlowContainer hFlowContainer = new HFlowContainer();
				hFlowContainer.AddThemeConstantOverride("h_separation", 6);
				hFlowContainer.AddThemeConstantOverride("v_separation", 6);
				_cardRow = hFlowContainer;
				parent.AddChild(_cardRow, forceReadableName: false, Node.InternalMode.Disabled);
			}
			RefreshCards();
			SubscribeHand();
			_player.PotionProcured += OnPotionChanged;
			_player.PotionDiscarded += OnPotionChanged;
			_player.UsedPotionRemoved += OnPotionChanged;
			if (_player.Creature != null)
			{
				_player.Creature.PowerApplied += OnPowerChanged;
				_player.Creature.PowerRemoved += OnPowerChanged;
				_player.Creature.PowerIncreased += OnPowerStackChanged;
				_player.Creature.PowerDecreased += OnPowerStackChanged;
			}
			if (_player.PlayerCombatState != null)
			{
				_player.PlayerCombatState.EnergyChanged += OnEnergyChanged;
				_player.PlayerCombatState.StarsChanged += OnStarsChanged;
			}
		}

		public void Cleanup()
		{
			UnsubscribeHand();
			_player.PotionProcured -= OnPotionChanged;
			_player.PotionDiscarded -= OnPotionChanged;
			_player.UsedPotionRemoved -= OnPotionChanged;
			if (_player.Creature != null)
			{
				_player.Creature.PowerApplied -= OnPowerChanged;
				_player.Creature.PowerRemoved -= OnPowerChanged;
				_player.Creature.PowerIncreased -= OnPowerStackChanged;
				_player.Creature.PowerDecreased -= OnPowerStackChanged;
			}
			if (_player.PlayerCombatState != null)
			{
				_player.PlayerCombatState.EnergyChanged -= OnEnergyChanged;
				_player.PlayerCombatState.StarsChanged -= OnStarsChanged;
			}
			UnsubscribeCardUpgrades();
			ClearCards();
		}

		private void OnStarsChanged(int _, int __)
		{
			try
			{
				RefreshCards();
			}
			catch (System.Exception ex)
			{
				ModLog.Error("PlayerHandSection.OnStarsChanged", ex);
			}
		}

		private void OnEnergyChanged(int _, int __)
		{
			try
			{
				RefreshCards();
			}
			catch (System.Exception ex)
			{
				ModLog.Error("PlayerHandSection.OnEnergyChanged", ex);
			}
		}

		private void SubscribeHand()
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected O, but got Unknown
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Expected O, but got Unknown
			if (_hand != null)
			{
				_hand.ContentsChanged += new Action(OnHandChanged);
				_hand.CardAdded += OnCardAddedOrRemoved;
				_hand.CardRemoved += OnCardAddedOrRemoved;
				CombatHistory combatHistory = CombatManager.Instance?.History;
				if (combatHistory != null && !_subscribedToHistory)
				{
					combatHistory.Changed += new Action(OnHistoryChanged);
					_subscribedToHistory = true;
				}
			}
		}

		private void UnsubscribeHand()
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected O, but got Unknown
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Expected O, but got Unknown
			if (_hand != null)
			{
				_hand.ContentsChanged -= new Action(OnHandChanged);
				_hand.CardAdded -= OnCardAddedOrRemoved;
				_hand.CardRemoved -= OnCardAddedOrRemoved;
				CombatHistory combatHistory = CombatManager.Instance?.History;
				if (combatHistory != null && _subscribedToHistory)
				{
					combatHistory.Changed -= new Action(OnHistoryChanged);
					_subscribedToHistory = false;
				}
			}
		}

		private void OnHistoryChanged()
		{
			try
			{
				RefreshCards();
			}
			catch (System.Exception ex)
			{
				ModLog.Error("PlayerHandSection.OnHistoryChanged", ex);
			}
		}

		public void RefreshHandReference()
		{
			CardPile cardPile = _player.PlayerCombatState?.Hand;
			if (cardPile != _hand || _hand == null)
			{
				UnsubscribeHand();
				_hand = cardPile;
				SubscribeHand();
				RefreshCards();
			}
		}

		private void OnHandChanged()
		{
			try
			{
				RefreshCards();
			}
			catch (System.Exception ex)
			{
				ModLog.Error("PlayerHandSection.OnHandChanged", ex);
			}
		}

		private void OnCardAddedOrRemoved(CardModel _)
		{
			try
			{
				RefreshCards();
			}
			catch (System.Exception ex)
			{
				ModLog.Error("PlayerHandSection.OnCardAddedOrRemoved", ex);
			}
		}

		private void OnPotionChanged(PotionModel _)
		{
			try
			{
				RefreshPotions();
			}
			catch (System.Exception ex)
			{
				ModLog.Error("PlayerHandSection.OnPotionChanged", ex);
			}
		}

		private void OnPowerChanged(PowerModel _)
		{
			try
			{
				RefreshCards();
			}
			catch (System.Exception ex)
			{
				ModLog.Error("PlayerHandSection.OnPowerChanged", ex);
			}
		}

		private void OnPowerStackChanged(PowerModel _, int _2, bool _3)
		{
			try
			{
				RefreshCards();
			}
			catch (System.Exception ex)
			{
				ModLog.Error("PlayerHandSection.OnPowerStackChanged", ex);
			}
		}

		private void OnPowerStackChanged(PowerModel _, bool _2)
		{
			try
			{
				RefreshCards();
			}
			catch (System.Exception ex)
			{
				ModLog.Error("PlayerHandSection.OnPowerStackChanged", ex);
			}
		}

		private void UnsubscribeCardUpgrades()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			var enumerator = _subscribedCards.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					CardModel current = enumerator.Current;
					try
					{
						current.Upgraded -= new Action(OnCardUpgraded);
					}
					catch
					{
					}
				}
			}
			finally
			{
				((System.IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
			_subscribedCards.Clear();
		}

		private void SubscribeCardUpgrades()
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Expected O, but got Unknown
			if (_hand?.Cards == null)
			{
				return;
			}
			System.Collections.Generic.IEnumerator<CardModel> enumerator = ((System.Collections.Generic.IEnumerable<CardModel>)_hand.Cards).GetEnumerator();
			try
			{
				while (((System.Collections.IEnumerator)enumerator).MoveNext())
				{
					CardModel current = enumerator.Current;
					try
					{
						current.Upgraded += new Action(OnCardUpgraded);
						_subscribedCards.Add(current);
					}
					catch
					{
					}
				}
			}
			finally
			{
				((System.IDisposable)enumerator)?.Dispose();
			}
		}

		private void OnCardUpgraded()
		{
			try
			{
				RefreshCards();
			}
			catch (System.Exception ex)
			{
				ModLog.Error("PlayerHandSection.OnCardUpgraded", ex);
			}
		}

		private void RefreshCards()
		{
			if (_hand == null)
			{
				return;
			}
			UnsubscribeCardUpgrades();
			int num = ((System.Collections.Generic.IReadOnlyCollection<CardModel>)_hand.Cards)?.Count ?? 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			if (num > 0)
			{
				System.Collections.Generic.IEnumerator<CardModel> enumerator = ((System.Collections.Generic.IEnumerable<CardModel>)_hand.Cards).GetEnumerator();
				try
				{
					while (((System.Collections.IEnumerator)enumerator).MoveNext())
					{
						CardModel current = enumerator.Current;
						try
						{
							object obj = current;
							try
							{
								object value = Traverse.Create(obj).Property("DynamicVars").GetValue();
								MethodInfo method = obj.GetType().GetMethod("UpdateDynamicVarPreview");
								if (method != (MethodInfo)null && value != null)
								{
									System.Type type = System.Type.GetType("MegaCrit.Sts2.Core.Entities.Cards.CardPreviewMode, sts2");
									if (type != (System.Type)null)
									{
										object obj2 = System.Enum.Parse(type, "Normal");
										((MethodBase)method).Invoke(obj, new object[3] { obj2, null, value });
									}
								}
							}
							catch (System.Exception ex)
							{
								ModLog.Error("UpdateDynVarPreview", ex);
							}
							object value2 = Traverse.Create(obj).Property("DynamicVars").GetValue();
							if (value2 == null)
							{
								continue;
							}
							MethodInfo method2 = value2.GetType().GetMethod("TryGetValue");
							if (method2 == (MethodInfo)null)
							{
								continue;
							}
							object[] array = new object[2] { "Damage", null };
							if ((bool)((MethodBase)method2).Invoke(value2, array) && array[1] != null)
							{
								try
								{
									num2 += (int)Traverse.Create(array[1]).Property<decimal>("PreviewValue").Value;
								}
								catch
								{
									try
									{
										num2 += Traverse.Create(array[1]).Property<int>("IntValue").Value;
									}
									catch
									{
									}
								}
							}
							else
							{
								array = new object[2] { "CalculatedDamage", null };
								if ((bool)((MethodBase)method2).Invoke(value2, array) && array[1] != null)
								{
									try
									{
										num2 += (int)Traverse.Create(array[1]).Property<decimal>("PreviewValue").Value;
									}
									catch
									{
										try
										{
											num2 += Traverse.Create(array[1]).Property<int>("IntValue").Value;
										}
										catch
										{
										}
									}
								}
							}
							if (((MemberInfo)((object)current).GetType()).Name != "SecondWind")
							{
								array = new object[2] { "Block", null };
								if ((bool)((MethodBase)method2).Invoke(value2, array) && array[1] != null)
								{
									try
									{
										num3 += (int)Traverse.Create(array[1]).Property<decimal>("PreviewValue").Value;
									}
									catch
									{
										try
										{
											num3 += Traverse.Create(array[1]).Property<int>("IntValue").Value;
										}
										catch
										{
										}
									}
								}
								else
								{
									array = new object[2] { "CalculatedBlock", null };
									if ((bool)((MethodBase)method2).Invoke(value2, array) && array[1] != null)
									{
										try
										{
											num3 += (int)Traverse.Create(array[1]).Property<decimal>("PreviewValue").Value;
										}
										catch
										{
											try
											{
												num3 += Traverse.Create(array[1]).Property<int>("IntValue").Value;
											}
											catch
											{
											}
										}
									}
								}
							}
							array = new object[2] { "WeakPower", null };
							if ((bool)((MethodBase)method2).Invoke(value2, array) && array[1] != null)
							{
								try
								{
									num4 += Traverse.Create(array[1]).Property<int>("IntValue").Value;
								}
								catch
								{
									try
									{
										num4 += (int)Traverse.Create(array[1]).Property<float>("BaseValue").Value;
									}
									catch
									{
									}
								}
							}
							array = new object[2] { "VulnerablePower", null };
							if ((bool)((MethodBase)method2).Invoke(value2, array) && array[1] != null)
							{
								try
								{
									num5 += Traverse.Create(array[1]).Property<int>("IntValue").Value;
								}
								catch
								{
									try
									{
										num5 += (int)Traverse.Create(array[1]).Property<float>("BaseValue").Value;
									}
									catch
									{
									}
								}
							}
							array = new object[2] { "Power", null };
							if (!(bool)((MethodBase)method2).Invoke(value2, array) || array[1] == null)
							{
								continue;
							}
							int num6 = 0;
							try
							{
								num6 = Traverse.Create(array[1]).Property<int>("IntValue").Value;
							}
							catch
							{
								try
								{
									num6 = (int)Traverse.Create(array[1]).Property<float>("BaseValue").Value;
								}
								catch
								{
								}
							}
							if (num6 > 0)
							{
								num4 += num6;
								num5 += num6;
							}
						}
						catch
						{
						}
					}
				}
				finally
				{
					((System.IDisposable)enumerator)?.Dispose();
				}
			}
			UpdateNameLabel(null, num, num2, num3, num4, num5);
			SubscribeCardUpgrades();
			if (ModSettings.CompactHandViewer || _cardRow == null || !GodotObject.IsInstanceValid(_cardRow))
			{
				return;
			}
			ClearCards();
			if (num == 0)
			{
				return;
			}
			int num7 = Math.Min(num, 12);
			for (int i = 0; i < num7; i++)
			{
				try
				{
					CardModel cardModel = _hand.Cards[i];
					if (cardModel != null)
					{
						Control control = CreateCompactCard(cardModel);
						_cardRow.AddChild(control, forceReadableName: false, Node.InternalMode.Disabled);
						_cardControls.Add(control);
					}
				}
				catch (System.Exception ex2)
				{
					ModLog.Error($"PlayerHandSection: failed to create card {i}", ex2);
				}
			}
			if (num > 12)
			{
				Label label = new Label();
				label.Text = $"+{num - 12}";
				label.AddThemeColorOverride("font_color", new Color(0.6f, 0.6f, 0.6f));
				label.AddThemeFontSizeOverride("font_size", 12);
				_cardRow.AddChild(label, forceReadableName: false, Node.InternalMode.Disabled);
				_cardControls.Add((Control)label);
			}
		}

		private void RefreshPotions()
		{
			if (_potionRow == null || !GodotObject.IsInstanceValid(_potionRow))
			{
				return;
			}
			System.Collections.Generic.IEnumerator<Node> enumerator = _potionRow.GetChildren().GetEnumerator();
			try
			{
				while (((System.Collections.IEnumerator)enumerator).MoveNext())
				{
					Node current = enumerator.Current;
					current.QueueFree();
				}
			}
			finally
			{
				((System.IDisposable)enumerator)?.Dispose();
			}
			try
			{
				System.Collections.Generic.IEnumerable<PotionModel> potions = _player.Potions;
				if (potions == null)
				{
					return;
				}
				bool flag = false;
				System.Collections.Generic.IEnumerator<PotionModel> enumerator2 = potions.GetEnumerator();
				try
				{
					while (((System.Collections.IEnumerator)enumerator2).MoveNext())
					{
						PotionModel current2 = enumerator2.Current;
						if (current2 != null)
						{
							if (!flag)
							{
								Label label = new Label();
								label.Text = Localization.Get("deck.potions");
								label.AddThemeColorOverride("font_color", new Color(0.6f, 0.6f, 0.6f));
								label.AddThemeFontSizeOverride("font_size", 11);
								label.MouseFilter = Control.MouseFilterEnum.Ignore;
								_potionRow.AddChild(label, forceReadableName: false, Node.InternalMode.Disabled);
								flag = true;
							}
							PotionIcon node = new PotionIcon(current2);
							_potionRow.AddChild(node, forceReadableName: false, Node.InternalMode.Disabled);
						}
					}
				}
				finally
				{
					((System.IDisposable)enumerator2)?.Dispose();
				}
			}
			catch (System.Exception ex)
			{
				ModLog.Error("RefreshPotions", ex);
			}
		}

		private static Control CreateCompactCard(dynamic card)
		{
			try
			{
				Traverse traverse = Traverse.Create((object)card);
				Texture2D portrait = null;
				string text = "???";
				string costText = "";
				string text2 = "";
				Color typeColor = DefaultCardColor;
				try
				{
					portrait = traverse.Property<Texture2D>("Portrait").Value;
				}
				catch
				{
				}
				try
				{
					string value = traverse.Property<string>("Title").Value;
					if (!string.IsNullOrEmpty(value))
					{
						text = value;
					}
					if (traverse.Property<bool>("IsUpgraded").Value && text != "???")
					{
						text += "+";
					}
				}
				catch
				{
				}
				try
				{
					object value2 = traverse.Property("EnergyCost").GetValue();
					if (value2 != null)
					{
						if (Traverse.Create(value2).Property<bool>("CostsX").Value)
						{
							costText = "X";
						}
						else
						{
							int value3 = Traverse.Create(value2).Property<int>("Canonical").Value;
							if (value3 >= 0)
							{
								costText = value3.ToString();
							}
						}
					}
				}
				catch
				{
				}
				try
				{
					object value4 = traverse.Property("Type").GetValue();
					if (value4 != null)
					{
						string text3 = value4.ToString() ?? "";
						if (text3.Contains("Attack"))
						{
							typeColor = AttackColor;
						}
						else if (text3.Contains("Skill"))
						{
							typeColor = SkillColor;
						}
						else if (text3.Contains("Power"))
						{
							typeColor = PowerColor;
						}
						else if (text3.Contains("Status"))
						{
							typeColor = StatusColor;
						}
						else if (text3.Contains("Curse"))
						{
							typeColor = CurseColor;
						}
					}
				}
				catch
				{
				}
				text2 = ResolveDescription((object)card);
				int playMult = 1;
				if (card is CardModel card2)
				{
					playMult = PlayCountHelper.GetEffectivePlayCount(card2);
				}
				return BuildCompactCard(portrait, text, costText, text2, typeColor, playMult);
			}
			catch (System.Exception ex)
			{
				ModLog.Error("CreateCompactCard", ex);
			}
			if (_003C_003Eo__53._003C_003Ep__1 == null)
			{
				_003C_003Eo__53._003C_003Ep__1 = CallSite<Func<CallSite, object, Control>>.Create(Binder.Convert((CSharpBinderFlags)0, typeof(Control), typeof(PlayerHandSection)));
			}
			Func<CallSite, object, Control> target = _003C_003Eo__53._003C_003Ep__1.Target;
			CallSite<Func<CallSite, object, Control>> _003C_003Ep__ = _003C_003Eo__53._003C_003Ep__1;
			if (_003C_003Eo__53._003C_003Ep__0 == null)
			{
				_003C_003Eo__53._003C_003Ep__0 = CallSite<Func<CallSite, System.Type, object, object>>.Create(Binder.InvokeMember((CSharpBinderFlags)0, "CreateCardFallback", (System.Collections.Generic.IEnumerable<System.Type>)null, typeof(PlayerHandSection), (System.Collections.Generic.IEnumerable<CSharpArgumentInfo>)(object)new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create((CSharpArgumentInfoFlags)33, (string)null),
					CSharpArgumentInfo.Create((CSharpArgumentInfoFlags)0, (string)null)
				}));
			}
			return target.Invoke((CallSite)(object)_003C_003Ep__, _003C_003Eo__53._003C_003Ep__0.Target.Invoke((CallSite)(object)_003C_003Eo__53._003C_003Ep__0, typeof(PlayerHandSection), (object)card));
		}

		private static Control BuildCompactCard(Texture2D? portrait, string cardName, string costText, string description, Color typeColor, int playMult = 1)
		{
			CardPanel cardPanel = new CardPanel();
			cardPanel.CustomMinimumSize = new Vector2(CardWidth, PortraitHeight + 16);
			cardPanel.CardName = cardName;
			cardPanel.CardDescription = description;
			cardPanel.WireUpTooltip();
			StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
			styleBoxFlat.BgColor = new Color(0.08f, 0.08f, 0.12f, 0.95f);
			styleBoxFlat.BorderColor = typeColor;
			styleBoxFlat.SetBorderWidthAll(1);
			styleBoxFlat.SetCornerRadiusAll(4);
			styleBoxFlat.SetContentMarginAll(0f);
			cardPanel.AddThemeStyleboxOverride("panel", styleBoxFlat);
			VBoxContainer vBoxContainer = new VBoxContainer();
			vBoxContainer.AddThemeConstantOverride("separation", 0);
			vBoxContainer.MouseFilter = Control.MouseFilterEnum.Pass;
			cardPanel.AddChild(vBoxContainer, forceReadableName: false, Node.InternalMode.Disabled);
			Control control = new Control();
			control.CustomMinimumSize = new Vector2(CardWidth, PortraitHeight);
			control.ClipContents = true;
			control.MouseFilter = Control.MouseFilterEnum.Pass;
			vBoxContainer.AddChild(control, forceReadableName: false, Node.InternalMode.Disabled);
			if (portrait != null)
			{
				TextureRect textureRect = new TextureRect();
				textureRect.Texture = portrait;
				textureRect.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
				textureRect.StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered;
				textureRect.SetAnchorsPreset(Control.LayoutPreset.FullRect);
				textureRect.MouseFilter = Control.MouseFilterEnum.Ignore;
				control.AddChild(textureRect, forceReadableName: false, Node.InternalMode.Disabled);
			}
			if (!string.IsNullOrEmpty(costText))
			{
				PanelContainer panelContainer = new PanelContainer();
				panelContainer.MouseFilter = Control.MouseFilterEnum.Ignore;
				StyleBoxFlat styleBoxFlat2 = new StyleBoxFlat();
				styleBoxFlat2.BgColor = new Color(0f, 0f, 0f, 0.75f);
				styleBoxFlat2.SetCornerRadiusAll(3);
				styleBoxFlat2.ContentMarginLeft = 3f;
				styleBoxFlat2.ContentMarginRight = 3f;
				styleBoxFlat2.ContentMarginTop = 1f;
				styleBoxFlat2.ContentMarginBottom = 1f;
				panelContainer.AddThemeStyleboxOverride("panel", styleBoxFlat2);
				panelContainer.Position = new Vector2(2f, 1f);
				Label label = new Label();
				label.Text = costText;
				label.MouseFilter = Control.MouseFilterEnum.Ignore;
				label.AddThemeColorOverride("font_color", Colors.White);
				label.AddThemeFontSizeOverride("font_size", ScaledFont(10));
				panelContainer.AddChild(label, forceReadableName: false, Node.InternalMode.Disabled);
				control.AddChild(panelContainer, forceReadableName: false, Node.InternalMode.Disabled);
			}
			if (playMult > 1)
			{
				PanelContainer panelContainer2 = new PanelContainer();
				panelContainer2.MouseFilter = Control.MouseFilterEnum.Ignore;
				StyleBoxFlat styleBoxFlat3 = new StyleBoxFlat();
				styleBoxFlat3.BgColor = new Color(0.85f, 0.2f, 0.2f, 0.9f);
				styleBoxFlat3.SetCornerRadiusAll(3);
				styleBoxFlat3.ContentMarginLeft = 3f;
				styleBoxFlat3.ContentMarginRight = 3f;
				styleBoxFlat3.ContentMarginTop = 1f;
				styleBoxFlat3.ContentMarginBottom = 1f;
				panelContainer2.AddThemeStyleboxOverride("panel", styleBoxFlat3);
				panelContainer2.SetAnchorsPreset(Control.LayoutPreset.TopRight);
				panelContainer2.GrowHorizontal = Control.GrowDirection.Begin;
				panelContainer2.OffsetRight = -2f;
				panelContainer2.OffsetTop = 1f;
				Label label2 = new Label();
				label2.Text = $"x{playMult}";
				label2.MouseFilter = Control.MouseFilterEnum.Ignore;
				label2.AddThemeColorOverride("font_color", Colors.White);
				label2.AddThemeFontSizeOverride("font_size", ScaledFont(10));
				panelContainer2.AddChild(label2, forceReadableName: false, Node.InternalMode.Disabled);
				control.AddChild(panelContainer2, forceReadableName: false, Node.InternalMode.Disabled);
			}
			PanelContainer panelContainer3 = new PanelContainer();
			panelContainer3.MouseFilter = Control.MouseFilterEnum.Pass;
			StyleBoxFlat styleBoxFlat4 = new StyleBoxFlat();
			styleBoxFlat4.BgColor = new Color(0.05f, 0.05f, 0.08f, 0.95f);
			styleBoxFlat4.SetContentMarginAll(0f);
			styleBoxFlat4.ContentMarginLeft = 3f;
			styleBoxFlat4.ContentMarginRight = 3f;
			styleBoxFlat4.ContentMarginTop = 2f;
			styleBoxFlat4.ContentMarginBottom = 2f;
			panelContainer3.AddThemeStyleboxOverride("panel", styleBoxFlat4);
			Label label3 = new Label();
			label3.Text = cardName;
			label3.HorizontalAlignment = HorizontalAlignment.Center;
			label3.MouseFilter = Control.MouseFilterEnum.Ignore;
			label3.AddThemeColorOverride("font_color", typeColor);
			label3.AddThemeFontSizeOverride("font_size", ScaledFont(9));
			label3.ClipText = true;
			label3.CustomMinimumSize = new Vector2(CardWidth - 6, 0f);
			panelContainer3.AddChild(label3, forceReadableName: false, Node.InternalMode.Disabled);
			vBoxContainer.AddChild(panelContainer3, forceReadableName: false, Node.InternalMode.Disabled);
			return cardPanel;
		}

		private static PanelContainer CreateCardFallback(dynamic card)
		{
			string text = "???";
			try
			{
				Traverse traverse = Traverse.Create((object)card);
				string value = traverse.Property<string>("Title").Value;
				if (!string.IsNullOrEmpty(value))
				{
					text = value;
				}
			}
			catch
			{
			}
			PanelContainer panelContainer = new PanelContainer();
			StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
			styleBoxFlat.BgColor = new Color(0.15f, 0.15f, 0.2f, 0.9f);
			styleBoxFlat.BorderColor = DefaultCardColor;
			styleBoxFlat.SetBorderWidthAll(1);
			styleBoxFlat.SetCornerRadiusAll(4);
			styleBoxFlat.ContentMarginLeft = 4f;
			styleBoxFlat.ContentMarginRight = 4f;
			styleBoxFlat.ContentMarginTop = 2f;
			styleBoxFlat.ContentMarginBottom = 2f;
			panelContainer.AddThemeStyleboxOverride("panel", styleBoxFlat);
			Label label = new Label();
			label.Text = text;
			label.AddThemeColorOverride("font_color", DefaultCardColor);
			label.AddThemeFontSizeOverride("font_size", ScaledFont(11));
			panelContainer.AddChild(label, forceReadableName: false, Node.InternalMode.Disabled);
			return panelContainer;
		}

		private void ClearCards()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			var enumerator = _cardControls.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Control current = enumerator.Current;
					if (GodotObject.IsInstanceValid(current))
					{
						current.QueueFree();
					}
				}
			}
			finally
			{
				((System.IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
			_cardControls.Clear();
		}

		private static Texture2D? GetIconVuln()
		{
			return LoadIcon("res://images/powers/vulnerable_power.png", ref _iconVuln);
		}

		private static Texture2D? GetIconWeak()
		{
			return LoadIcon("res://images/powers/weak_power.png", ref _iconWeak);
		}

		private static Texture2D? GetIconDefend()
		{
			return LoadIcon("res://images/packed/intents/intent_defend.png", ref _iconDefend);
		}

		private static Texture2D? GetIconStars()
		{
			return LoadIcon("res://images/packed/sprite_fonts/star_icon.png", ref _iconStars);
		}

		private static Texture2D? LoadIcon(string path, ref Texture2D? cache)
		{
			if (cache != null && GodotObject.IsInstanceValid(cache))
			{
				return cache;
			}
			try
			{
				cache = GD.Load<Texture2D>(path);
			}
			catch
			{
				cache = null;
			}
			return cache;
		}

		private static Texture2D? GetEnergyIcon(Player player)
		{
			string text;
			try
			{
				text = player.Character?.CardPool?.EnergyColorName ?? "colorless";
			}
			catch
			{
				text = "colorless";
			}
			Texture2D texture2D = default(Texture2D);
			if (_energyIconCache.TryGetValue(text, out texture2D) && texture2D != null && GodotObject.IsInstanceValid(texture2D))
			{
				return texture2D;
			}
			Texture2D texture2D2 = null;
			try
			{
				texture2D2 = GD.Load<Texture2D>("res://images/packed/sprite_fonts/" + text + "_energy_icon.png");
			}
			catch
			{
			}
			_energyIconCache[text] = texture2D2;
			return texture2D2;
		}

		private void UpdateNameLabel(string? baseName = null, int? count = null, int totalDamage = 0, int totalBlock = 0, int weakStacks = 0, int vulnStacks = 0)
		{
			if (_nameLabel == null || !GodotObject.IsInstanceValid(_nameLabel))
			{
				return;
			}
			int num = count ?? (((System.Collections.Generic.IReadOnlyCollection<CardModel>)_hand?.Cards)?.Count).GetValueOrDefault();
			if (baseName != null)
			{
				_nameLabel.Text = $"{baseName} — {Localization.Get("deck.hand")} ({num})";
			}
			else
			{
				string text = _nameLabel.Text;
				int num2 = text.IndexOf('—');
				if (num2 > 0)
				{
					_nameLabel.Text = text.Substring(0, num2) + $"— {Localization.Get("deck.hand")} ({num})";
				}
			}
			if (_statsRow == null || !GodotObject.IsInstanceValid(_statsRow))
			{
				return;
			}
			System.Collections.Generic.IEnumerator<Node> enumerator = _statsRow.GetChildren().GetEnumerator();
			try
			{
				while (((System.Collections.IEnumerator)enumerator).MoveNext())
				{
					Node current = enumerator.Current;
					current.QueueFree();
				}
			}
			finally
			{
				((System.IDisposable)enumerator)?.Dispose();
			}
			PlayerCombatState playerCombatState = _player.PlayerCombatState;
			if (playerCombatState != null)
			{
				AddStatEntry(_statsRow, GetEnergyIcon(_player), $"{playerCombatState.Energy}/{playerCombatState.MaxEnergy}", EnergyColor);
				if (playerCombatState.Stars > 0)
				{
					AddStatEntry(_statsRow, GetIconStars(), $"{playerCombatState.Stars}", StarsColor);
				}
			}
			if (totalDamage > 0)
			{
				AddStatEntry(_statsRow, null, $"⚔ {totalDamage}", AttackColor);
			}
			if (totalBlock > 0)
			{
				AddStatEntry(_statsRow, GetIconDefend(), $"{totalBlock}", SkillColor);
			}
			if (weakStacks > 0)
			{
				AddStatEntry(_statsRow, GetIconWeak(), $"{weakStacks}", new Color(0.4f, 0.8f, 0.3f));
			}
			if (vulnStacks > 0)
			{
				AddStatEntry(_statsRow, GetIconVuln(), $"{vulnStacks}", new Color(1f, 0.6f, 0.2f));
			}
		}

		private static void AddStatEntry(HBoxContainer row, Texture2D? icon, string text, Color color)
		{
			HBoxContainer hBoxContainer = new HBoxContainer();
			hBoxContainer.AddThemeConstantOverride("separation", 2);
			if (icon != null && GodotObject.IsInstanceValid(icon))
			{
				TextureRect textureRect = new TextureRect();
				textureRect.Texture = icon;
				textureRect.CustomMinimumSize = new Vector2(14f, 14f);
				textureRect.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
				textureRect.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
				textureRect.MouseFilter = Control.MouseFilterEnum.Ignore;
				hBoxContainer.AddChild(textureRect, forceReadableName: false, Node.InternalMode.Disabled);
			}
			Label label = new Label();
			label.Text = text;
			label.AddThemeColorOverride("font_color", color);
			label.AddThemeFontSizeOverride("font_size", 12);
			label.MouseFilter = Control.MouseFilterEnum.Ignore;
			hBoxContainer.AddChild(label, forceReadableName: false, Node.InternalMode.Disabled);
			row.AddChild(hBoxContainer, forceReadableName: false, Node.InternalMode.Disabled);
		}
	}

	private class CardPanel : PanelContainer
	{
		public string CardName = "";

		public string CardDescription = "";

		private Control? _tooltip;

		public void WireUpTooltip()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Expected O, but got Unknown
			base.MouseFilter = MouseFilterEnum.Stop;
			base.MouseEntered += new Action(ShowTooltip);
			base.MouseExited += new Action(HideTooltip);
		}

		private void ShowTooltip()
		{
			HideTooltip();
			try
			{
				if (_canvasLayer != null && GodotObject.IsInstanceValid(_canvasLayer))
				{
					PanelContainer panelContainer = new PanelContainer();
					panelContainer.MouseFilter = MouseFilterEnum.Ignore;
					StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
					styleBoxFlat.BgColor = new Color(0.1f, 0.1f, 0.15f, 0.95f);
					styleBoxFlat.BorderColor = new Color(0.6f, 0.5f, 0.2f);
					styleBoxFlat.SetBorderWidthAll(1);
					styleBoxFlat.SetCornerRadiusAll(4);
					styleBoxFlat.SetContentMarginAll(8f);
					panelContainer.AddThemeStyleboxOverride("panel", styleBoxFlat);
					VBoxContainer vBoxContainer = new VBoxContainer();
					vBoxContainer.AddThemeConstantOverride("separation", 4);
					panelContainer.AddChild(vBoxContainer, forceReadableName: false, InternalMode.Disabled);
					Label label = new Label();
					label.Text = CardName;
					label.AddThemeColorOverride("font_color", new Color(0.9f, 0.8f, 0.3f));
					label.AddThemeFontSizeOverride("font_size", 13);
					vBoxContainer.AddChild(label, forceReadableName: false, InternalMode.Disabled);
					string text = CleanText(CardDescription);
					if (!string.IsNullOrEmpty(text))
					{
						Label label2 = new Label();
						label2.Text = text;
						label2.AddThemeColorOverride("font_color", new Color(0.85f, 0.85f, 0.85f));
						label2.AddThemeFontSizeOverride("font_size", 11);
						label2.AutowrapMode = TextServer.AutowrapMode.WordSmart;
						label2.CustomMinimumSize = new Vector2(220f, 0f);
						vBoxContainer.AddChild(label2, forceReadableName: false, InternalMode.Disabled);
					}
					panelContainer.ZIndex = 500;
					_canvasLayer.AddChild(panelContainer, forceReadableName: false, InternalMode.Disabled);
					panelContainer.GlobalPosition = GetViewport().GetMousePosition() + new Vector2(16f, 0f);
					_tooltip = panelContainer;
				}
			}
			catch (System.Exception ex)
			{
				ModLog.Error("CardPanel tooltip show", ex);
			}
		}

		private void HideTooltip()
		{
			try
			{
				if (_tooltip != null && GodotObject.IsInstanceValid(_tooltip))
				{
					_tooltip.QueueFree();
				}
				_tooltip = null;
			}
			catch (System.Exception ex)
			{
				ModLog.Error("CardPanel tooltip hide", ex);
			}
		}

		private static string CleanText(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return "";
			}
			s = Regex.Replace(s, "\\[/?[^\\]]+\\]", "");
			s = Regex.Replace(s, "res://\\S+", "");
			return s.Trim();
		}
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Action _003C_003E9__34_2;

		public static Action _003C_003E9__34_3;

		public static Action _003C_003E9__34_1;

		public static MatchEvaluator _003C_003E9__46_0;

		internal void _003CBuildUI_003Eb__34_2()
		{
			PrevPage();
		}

		internal void _003CBuildUI_003Eb__34_3()
		{
			NextPage();
		}

		internal void _003CBuildUI_003Eb__34_1()
		{
			Hide();
		}

		internal string _003CCleanDescriptionText_003Eb__46_0(Match match)
		{
			int count = Regex.Matches(((Capture)match).Value, "\\[img\\]").Count;
			Match val = Regex.Match(((Capture)match).Value, "res://\\S+?/([^/]+?)(?:_icon)?\\.png");
			string filename = (((Group)val).Success ? ((Capture)val.Groups[1]).Value : "?");
			filename = MapIconLabel(filename);
			return (count > 1) ? $"{count} {filename}" : filename;
		}
	}

	public static int DebugDuplicatePlayers = 0;

	private static CanvasLayer? _canvasLayer;

	private static HandPanel? _panel;

	private static bool _visible;

	private static Label? _gripLabel;

	private static readonly List<PlayerHandSection> _sections = new List<PlayerHandSection>();

	private static Window? _popoutWindow;

	private static int _currentPage;

	private const int PlayersPerPage = 4;

	private static bool _resizing;

	private static ResizeEdge _resizeEdge;

	private const float ResizeMargin = 8f;

	private const float DefaultPanelWidth = 420f;

	private const float MinPanelWidth = 230f;

	private const float MinPanelHeight = 180f;

	private static bool? _savedGuiEmbedSubwindows;

	private static bool _dragging;

	private static Vector2 _dragOffset;

	private static Control? _contentClip;

	private static ScrollContainer? _contentScroll;

	private static bool _offsetDragging;

	private static float _offsetDragStartMouseY;

	private static float _offsetDragStartValue;

	private static bool IsPopoutMode => _popoutWindow != null && GodotObject.IsInstanceValid(_popoutWindow);

	public static bool IsVisible => _visible;

	public static void Toggle()
	{
		ResetStaleVisibleState();
		ModLog.Info($"DeckTracker.Toggle: _visible={_visible}");
		if (_visible)
		{
			Hide();
		}
		else
		{
			Show();
		}
	}

	private static void ResetStaleVisibleState()
	{
		if (_visible)
		{
			bool flag = _canvasLayer != null && GodotObject.IsInstanceValid(_canvasLayer);
			bool flag2 = _panel != null && GodotObject.IsInstanceValid(_panel);
			if (!(flag && flag2))
			{
				ModLog.Info($"DeckTracker: stale _visible detected (canvas={flag}, panel={flag2}) — resetting");
				_sections.Clear();
				_canvasLayer = null;
				_panel = null;
				_gripLabel = null;
				_popoutWindow = null;
				_visible = false;
				_resizing = false;
			}
		}
	}

	public static void ResetPosition()
	{
		bool visible = _visible;
		if (visible)
		{
			Hide();
		}
		LayoutStore.HandViewerPos = null;
		LayoutStore.HandViewerSize = null;
		LayoutStore.HandViewerWindowPos = null;
		LayoutStore.HandViewerWindowSize = null;
		LayoutStore.HandViewerContentOffsetY = 0f;
		LayoutStore.Save();
		if (visible)
		{
			Show();
		}
	}

	public static bool ShowIfInCombat()
	{
		ResetStaleVisibleState();
		if (!_visible)
		{
			Show();
		}
		return _visible;
	}

	public static void RefreshIfVisible()
	{
		if (_visible)
		{
			Refresh();
		}
	}

	public static bool HasAnyNullHands()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (!_visible)
		{
			return false;
		}
		var enumerator = _sections.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				PlayerHandSection current = enumerator.Current;
				if (current.IsHandNull)
				{
					return true;
				}
			}
		}
		finally
		{
			((System.IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		return false;
	}

	public static void OnCombatSetUp()
	{
		CombatManager instance = CombatManager.Instance;
		if (instance != null)
		{
			ResetStaleVisibleState();
			instance.TurnStarted -= OnTurnStarted;
			instance.CombatEnded -= OnCombatEnded;
			instance.TurnStarted += OnTurnStarted;
			instance.CombatEnded += OnCombatEnded;
			RefreshIfVisible();
			if (ModSettings.AutoShowTeammateHand && ModSettings.ShowTeammateHand)
			{
				ShowIfInCombat();
			}
		}
	}

	private static void OnTurnStarted(CombatState _)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (!_visible)
		{
			return;
		}
		var enumerator = _sections.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				PlayerHandSection current = enumerator.Current;
				try
				{
					current.RefreshHandReference();
				}
				catch (System.Exception ex)
				{
					ModLog.Error("DeckTracker.OnTurnStarted", ex);
				}
			}
		}
		finally
		{
			((System.IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
	}

	private static void OnCombatEnded(CombatRoom _)
	{
		try
		{
			Hide();
			CombatManager instance = CombatManager.Instance;
			if (instance != null)
			{
				instance.TurnStarted -= OnTurnStarted;
				instance.CombatEnded -= OnCombatEnded;
			}
		}
		catch (System.Exception ex)
		{
			ModLog.Error("DeckTracker.OnCombatEnded", ex);
		}
	}

	public static void NextPage()
	{
		if (_visible)
		{
			int playerCount = GetPlayerCount();
			int num = Math.Max(0, (playerCount - 1) / 4);
			if (_currentPage < num)
			{
				_currentPage++;
				Refresh();
			}
		}
	}

	public static void PrevPage()
	{
		if (_visible && _currentPage > 0)
		{
			_currentPage--;
			Refresh();
		}
	}

	private static int GetPlayerCount()
	{
		RunState value = Traverse.Create(RunManager.Instance).Property<RunState>("State").Value;
		if (value?.Players == null)
		{
			return 0;
		}
		return (DebugDuplicatePlayers > 0) ? DebugDuplicatePlayers : ((System.Collections.Generic.IReadOnlyCollection<Player>)value.Players).Count;
	}

	private static void Show()
	{
		if (!ModSettings.ShowTeammateHand)
		{
			ModLog.Info("DeckTracker.Show: blocked — ShowTeammateHand is off");
			return;
		}
		CombatManager instance = CombatManager.Instance;
		if (instance == null)
		{
			ModLog.Info("DeckTracker.Show: blocked — CombatManager.Instance is null");
			return;
		}
		CombatState combatState = instance.DebugOnlyGetState();
		if (combatState == null)
		{
			ModLog.Info("DeckTracker.Show: blocked — combatState is null");
			return;
		}
		RunState value = Traverse.Create(RunManager.Instance).Property<RunState>("State").Value;
		if (value?.Players == null || ((System.Collections.Generic.IReadOnlyCollection<Player>)value.Players).Count == 0)
		{
			ModLog.Info("DeckTracker.Show: blocked — runState.Players is " + ((value?.Players == null) ? "null" : "empty"));
			return;
		}
		ModLog.Info($"DeckTracker.Show: building UI for {((System.Collections.Generic.IReadOnlyCollection<Player>)value.Players).Count} players");
		_currentPage = 0;
		BuildUI(value);
	}

	private static void Refresh()
	{
		if (IsPopoutMode && _popoutWindow != null)
		{
			LayoutStore.HandViewerWindowPos = _popoutWindow.Position;
			LayoutStore.HandViewerWindowSize = _popoutWindow.Size;
		}
		else if (_panel != null && GodotObject.IsInstanceValid(_panel))
		{
			LayoutStore.HandViewerPos = _panel.Position;
			LayoutStore.HandViewerSize = _panel.Size;
		}
		RunState value = Traverse.Create(RunManager.Instance).Property<RunState>("State").Value;
		if (value?.Players != null)
		{
			CleanupUI();
			BuildUI(value);
		}
	}

	private static void BuildUI(RunState runState)
	{
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Expected O, but got Unknown
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0580: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Expected O, but got Unknown
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Expected O, but got Unknown
		if (_visible)
		{
			CleanupUI();
		}
		NGame instance = NGame.Instance;
		if (instance == null)
		{
			return;
		}
		if (ModSettings.HandViewerPopout && !IsExclusiveFullscreen())
		{
			_popoutWindow = BuildPopoutWindow(instance);
		}
		_canvasLayer = new CanvasLayer();
		_canvasLayer.Layer = 10;
		if (_popoutWindow != null)
		{
			_popoutWindow.AddChild(_canvasLayer, forceReadableName: false, Node.InternalMode.Disabled);
		}
		else
		{
			instance.AddChild(_canvasLayer, forceReadableName: false, Node.InternalMode.Disabled);
		}
		ulong localNetId = 0uL;
		try
		{
			localNetId = (RunManager.Instance?.NetService?.NetId).GetValueOrDefault();
		}
		catch
		{
		}
		List<Player> val = new List<Player>();
		if (DebugDuplicatePlayers > 0)
		{
			Player player = runState.Players[0];
			for (int i = 0; i < DebugDuplicatePlayers; i++)
			{
				val.Add(player);
			}
		}
		else
		{
			val.AddRange((System.Collections.Generic.IEnumerable<Player>)runState.Players);
		}
		if (ModSettings.HideOwnHand && localNetId != 0)
		{
			val.RemoveAll((Predicate<Player>)((Player p) => p.NetId == localNetId));
		}
		int count = val.Count;
		int num = Math.Max(0, (count - 1) / 4);
		_currentPage = Math.Min(_currentPage, num);
		int num2 = _currentPage * 4;
		int num3 = Math.Min(num2 + 4, count);
		_panel = new HandPanel();
		_panel.ZIndex = 200;
		_panel.ClipContents = true;
		Vector2 size = LayoutStore.HandViewerSize ?? new Vector2(420f, 0f);
		if (size.X < 230f)
		{
			size.X = 230f;
		}
		_panel.CustomMinimumSize = new Vector2(230f, 180f);
		if (LayoutStore.HandViewerSize.HasValue)
		{
			_panel.Size = size;
		}
		StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
		float num4 = Math.Clamp((float)ModSettings.HandViewerOpacityPercent / 100f, 0f, 1f);
		styleBoxFlat.BgColor = new Color(0.05f, 0.05f, 0.08f, num4);
		styleBoxFlat.BorderColor = new Color(0f, 0f, 0f, 0f);
		styleBoxFlat.SetBorderWidthAll(0);
		styleBoxFlat.SetCornerRadiusAll((num4 > 0f) ? 6 : 0);
		styleBoxFlat.SetContentMarginAll(14f);
		_panel.AddThemeStyleboxOverride("panel", styleBoxFlat);
		// Plain (non-container) clip control so the scroll content can be shifted up past the
		// panel's top edge, hiding part of the header (Shift+drag adjusts the offset).
		_contentClip = new Control();
		_contentClip.ClipContents = true;
		_contentClip.MouseFilter = Control.MouseFilterEnum.Ignore;
		_contentClip.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
		_contentClip.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		_panel.AddChild(_contentClip, forceReadableName: false, Node.InternalMode.Disabled);
		ScrollContainer scrollContainer = new ScrollContainer();
		scrollContainer.HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled;
		_contentClip.AddChild(scrollContainer, forceReadableName: false, Node.InternalMode.Disabled);
		_contentScroll = scrollContainer;
		scrollContainer.MinimumSizeChanged += SyncContentClipMinSize;
		ApplyContentOffset();
		VBoxContainer vBoxContainer = new VBoxContainer();
		vBoxContainer.AddThemeConstantOverride("separation", 6);
		vBoxContainer.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		scrollContainer.AddChild(vBoxContainer, forceReadableName: false, Node.InternalMode.Disabled);
		HBoxContainer hBoxContainer = new HBoxContainer();
		hBoxContainer.AddThemeConstantOverride("separation", 8);
		hBoxContainer.Alignment = BoxContainer.AlignmentMode.Center;
		vBoxContainer.AddChild(hBoxContainer, forceReadableName: false, Node.InternalMode.Disabled);
		if (num > 0)
		{
			Button button = CreateArrowButton("<", _currentPage > 0);
			object obj2 = _003C_003Ec._003C_003E9__34_2;
			if (obj2 == null)
			{
				Action val2 = delegate
				{
					PrevPage();
				};
				_003C_003Ec._003C_003E9__34_2 = val2;
				obj2 = (object)val2;
			}
			button.Pressed += (Action)obj2;
			hBoxContainer.AddChild(button, forceReadableName: false, Node.InternalMode.Disabled);
		}
		Label label = new Label();
		string text = Localization.Get("deck.title");
		if (num > 0)
		{
			text += $"  [{_currentPage + 1}/{num + 1}]";
		}
		label.Text = text;
		label.HorizontalAlignment = HorizontalAlignment.Center;
		label.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		label.MouseFilter = Control.MouseFilterEnum.Ignore;
		label.AddThemeColorOverride("font_color", new Color(0.9f, 0.7f, 0.2f));
		label.AddThemeFontSizeOverride("font_size", 18);
		hBoxContainer.AddChild(label, forceReadableName: false, Node.InternalMode.Disabled);
		if (num > 0)
		{
			Button button2 = CreateArrowButton(">", _currentPage < num);
			object obj3 = _003C_003Ec._003C_003E9__34_3;
			if (obj3 == null)
			{
				Action val3 = delegate
				{
					NextPage();
				};
				_003C_003Ec._003C_003E9__34_3 = val3;
				obj3 = (object)val3;
			}
			button2.Pressed += (Action)obj3;
			hBoxContainer.AddChild(button2, forceReadableName: false, Node.InternalMode.Disabled);
		}
		Button button3 = new Button();
		button3.Text = "X";
		button3.CustomMinimumSize = new Vector2(28f, 28f);
		object obj4 = _003C_003Ec._003C_003E9__34_1;
		if (obj4 == null)
		{
			Action val4 = delegate
			{
				Hide();
			};
			_003C_003Ec._003C_003E9__34_1 = val4;
			obj4 = (object)val4;
		}
		button3.Pressed += (Action)obj4;
		StyleBoxFlat styleBoxFlat2 = new StyleBoxFlat();
		styleBoxFlat2.BgColor = new Color(0.3f, 0.1f, 0.1f);
		styleBoxFlat2.BorderColor = new Color(0.6f, 0.2f, 0.2f);
		styleBoxFlat2.SetBorderWidthAll(1);
		styleBoxFlat2.SetCornerRadiusAll(4);
		styleBoxFlat2.SetContentMarginAll(2f);
		button3.AddThemeStyleboxOverride("normal", styleBoxFlat2);
		StyleBoxFlat styleBoxFlat3 = new StyleBoxFlat();
		styleBoxFlat3.BgColor = new Color(0.5f, 0.15f, 0.15f);
		styleBoxFlat3.BorderColor = new Color(0.8f, 0.3f, 0.3f);
		styleBoxFlat3.SetBorderWidthAll(1);
		styleBoxFlat3.SetCornerRadiusAll(4);
		styleBoxFlat3.SetContentMarginAll(2f);
		button3.AddThemeStyleboxOverride("hover", styleBoxFlat3);
		button3.AddThemeStyleboxOverride("pressed", styleBoxFlat3);
		button3.AddThemeColorOverride("font_color", new Color(0.9f, 0.4f, 0.4f));
		button3.AddThemeFontSizeOverride("font_size", 14);
		hBoxContainer.AddChild(button3, forceReadableName: false, Node.InternalMode.Disabled);
		if (ModSettings.CompactHandViewer)
		{
			GridContainer gridContainer = new GridContainer();
			gridContainer.Columns = 2;
			gridContainer.AddThemeConstantOverride("h_separation", 16);
			gridContainer.AddThemeConstantOverride("v_separation", 8);
			vBoxContainer.AddChild(gridContainer, forceReadableName: false, Node.InternalMode.Disabled);
			for (int num5 = num2; num5 < num3; num5++)
			{
				Player player2 = val[num5];
				string debugSuffix = ((DebugDuplicatePlayers > 0) ? $" #{num5 + 1}" : null);
				PlayerHandSection playerHandSection = new PlayerHandSection(player2, localNetId, debugSuffix);
				VBoxContainer vBoxContainer2 = new VBoxContainer();
				vBoxContainer2.AddThemeConstantOverride("separation", 2);
				vBoxContainer2.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
				playerHandSection.AddTo(vBoxContainer2);
				gridContainer.AddChild(vBoxContainer2, forceReadableName: false, Node.InternalMode.Disabled);
				_sections.Add(playerHandSection);
			}
		}
		else
		{
			for (int num6 = num2; num6 < num3; num6++)
			{
				Player player3 = val[num6];
				string debugSuffix2 = ((DebugDuplicatePlayers > 0) ? $" #{num6 + 1}" : null);
				PlayerHandSection playerHandSection2 = new PlayerHandSection(player3, localNetId, debugSuffix2);
				playerHandSection2.AddTo(vBoxContainer);
				_sections.Add(playerHandSection2);
			}
		}
		Label label2 = new Label();
		if (IsPopoutMode)
		{
			label2.Text = "Drag title bar to move | F3 or X to close";
		}
		else if (ModSettings.AlwaysShowTeammateHand)
		{
			label2.Text = "Drag to move | Shift+drag to shift content | Drag edges to resize | F3 or X to close";
		}
		else
		{
			label2.Text = "Drag to move | Shift+drag to shift content | Drag edges to resize | Click outside to close";
		}
		label2.HorizontalAlignment = HorizontalAlignment.Center;
		label2.MouseFilter = Control.MouseFilterEnum.Ignore;
		label2.AddThemeColorOverride("font_color", new Color(0.5f, 0.5f, 0.5f));
		label2.AddThemeFontSizeOverride("font_size", 11);
		vBoxContainer.AddChild(label2, forceReadableName: false, Node.InternalMode.Disabled);
		_canvasLayer.AddChild(_panel, forceReadableName: false, Node.InternalMode.Disabled);
		if (!IsPopoutMode)
		{
			_gripLabel = new Label();
			_gripLabel.Text = "◢";
			_gripLabel.AddThemeColorOverride("font_color", new Color(1f, 1f, 1f, 0.3f));
			_gripLabel.AddThemeFontSizeOverride("font_size", 14);
			_gripLabel.MouseFilter = Control.MouseFilterEnum.Ignore;
			_canvasLayer.AddChild(_gripLabel, forceReadableName: false, Node.InternalMode.Disabled);
		}
		if (IsPopoutMode)
		{
			_panel.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect, Control.LayoutPresetMode.Minsize);
			_panel.CustomMinimumSize = Vector2.Zero;
		}
		else if (LayoutStore.HandViewerPos.HasValue)
		{
			_panel.Position = LayoutStore.HandViewerPos.Value;
		}
		else
		{
			_panel.CallDeferred("set_position", GetCenteredPosition());
		}
		_visible = true;
	}

	private static void SyncContentClipMinSize()
	{
		if (_contentClip != null && GodotObject.IsInstanceValid(_contentClip) && _contentScroll != null && GodotObject.IsInstanceValid(_contentScroll))
		{
			// Keep the panel at least as wide as its content, as when the scroll container was its direct child.
			_contentClip.CustomMinimumSize = new Vector2(_contentScroll.GetCombinedMinimumSize().X, 0f);
		}
	}

	private static void ApplyContentOffset()
	{
		if (_contentScroll != null && GodotObject.IsInstanceValid(_contentScroll))
		{
			_contentScroll.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
			_contentScroll.OffsetTop = 0f - LayoutStore.HandViewerContentOffsetY;
		}
	}

	private static void UpdateGripPosition()
	{
		if (_gripLabel != null && GodotObject.IsInstanceValid(_gripLabel) && _panel != null && GodotObject.IsInstanceValid(_panel))
		{
			Vector2 position = _panel.Position;
			Vector2 size = _panel.Size;
			_gripLabel.Position = new Vector2(position.X + size.X - 18f, position.Y + size.Y - 20f);
		}
	}

	private static Vector2 GetCenteredPosition()
	{
		float num = (NGame.Instance?.GetViewport()?.GetVisibleRect().Size ?? new Vector2(1920f, 1080f)).X / 2f - 500f;
		if (num < 0f)
		{
			num = 0f;
		}
		return new Vector2(num, 20f);
	}

	private static bool IsExclusiveFullscreen()
	{
		try
		{
			DisplayServer.WindowMode windowMode = DisplayServer.WindowGetMode();
			return windowMode == DisplayServer.WindowMode.ExclusiveFullscreen;
		}
		catch
		{
			return false;
		}
	}

	private static Window BuildPopoutWindow(Node parent)
	{
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Expected O, but got Unknown
		try
		{
			Window window = parent.GetTree()?.Root;
			if (window != null)
			{
				_savedGuiEmbedSubwindows = window.GuiEmbedSubwindows;
				window.GuiEmbedSubwindows = false;
			}
		}
		catch (System.Exception ex)
		{
			ModLog.Error("DeckTracker popout embed-flag toggle", ex);
		}
		Window win = new Window
		{
			Title = "BetterSpire2 — Hand",
			Unfocusable = true,
			TransparentBg = false,
			MinSize = new Vector2I(230, 180),
			Visible = false
		};
		win.Size = LayoutStore.HandViewerWindowSize ?? new Vector2I(420, 480);
		parent.AddChild(win, forceReadableName: false, Node.InternalMode.Disabled);
		try
		{
			win.ForceNative = true;
		}
		catch (System.Exception ex2)
		{
			ModLog.Error("DeckTracker popout ForceNative", ex2);
		}
		if (LayoutStore.HandViewerWindowPos.HasValue)
		{
			win.Position = LayoutStore.HandViewerWindowPos.Value;
		}
		else
		{
			try
			{
				int screen = DisplayServer.WindowGetCurrentScreen();
				Vector2I vector2I = DisplayServer.ScreenGetPosition(screen);
				Vector2I vector2I2 = DisplayServer.ScreenGetSize(screen);
				win.Position = vector2I + (vector2I2 - win.Size) / 2;
			}
			catch (System.Exception ex3)
			{
				ModLog.Error("DeckTracker popout center", ex3);
			}
		}
		win.CloseRequested += (Action)delegate
		{
			try
			{
				LayoutStore.HandViewerWindowPos = win.Position;
				LayoutStore.HandViewerWindowSize = win.Size;
				LayoutStore.Save();
				ModSettings.HandViewerPopout = false;
				ModSettings.Save();
				Hide();
			}
			catch (System.Exception ex4)
			{
				ModLog.Error("DeckTracker popout CloseRequested", ex4);
			}
		};
		win.Visible = true;
		return win;
	}

	private static Button CreateArrowButton(string text, bool enabled)
	{
		Button button = new Button();
		button.Text = text;
		button.CustomMinimumSize = new Vector2(32f, 28f);
		button.Disabled = !enabled;
		StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
		styleBoxFlat.BgColor = (enabled ? new Color(0.2f, 0.18f, 0.12f) : new Color(0.15f, 0.15f, 0.15f));
		styleBoxFlat.BorderColor = (enabled ? new Color(0.8f, 0.6f, 0.2f) : new Color(0.4f, 0.4f, 0.4f));
		styleBoxFlat.SetBorderWidthAll(1);
		styleBoxFlat.SetCornerRadiusAll(4);
		styleBoxFlat.SetContentMarginAll(4f);
		button.AddThemeStyleboxOverride("normal", styleBoxFlat);
		StyleBoxFlat styleBoxFlat2 = new StyleBoxFlat();
		styleBoxFlat2.BgColor = new Color(0.3f, 0.25f, 0.15f);
		styleBoxFlat2.BorderColor = new Color(0.9f, 0.7f, 0.2f);
		styleBoxFlat2.SetBorderWidthAll(1);
		styleBoxFlat2.SetCornerRadiusAll(4);
		styleBoxFlat2.SetContentMarginAll(4f);
		button.AddThemeStyleboxOverride("hover", styleBoxFlat2);
		StyleBoxFlat styleBoxFlat3 = new StyleBoxFlat();
		styleBoxFlat3.BgColor = new Color(0.35f, 0.3f, 0.15f);
		styleBoxFlat3.BorderColor = new Color(1f, 0.8f, 0.3f);
		styleBoxFlat3.SetBorderWidthAll(1);
		styleBoxFlat3.SetCornerRadiusAll(4);
		styleBoxFlat3.SetContentMarginAll(4f);
		button.AddThemeStyleboxOverride("pressed", styleBoxFlat3);
		button.AddThemeStyleboxOverride("disabled", styleBoxFlat);
		button.AddThemeColorOverride("font_color", enabled ? new Color(0.9f, 0.7f, 0.2f) : new Color(0.4f, 0.4f, 0.4f));
		button.AddThemeColorOverride("font_disabled_color", new Color(0.4f, 0.4f, 0.4f));
		button.AddThemeFontSizeOverride("font_size", 16);
		return button;
	}

	public static bool IsPointInPanel(Vector2 point)
	{
		if (!_visible || _panel == null || !GodotObject.IsInstanceValid(_panel))
		{
			return false;
		}
		return _panel.GetGlobalRect().HasPoint(point);
	}

	public static void Hide()
	{
		if (IsPopoutMode && _popoutWindow != null)
		{
			LayoutStore.HandViewerWindowPos = _popoutWindow.Position;
			LayoutStore.HandViewerWindowSize = _popoutWindow.Size;
			LayoutStore.Save();
		}
		else if (_panel != null && GodotObject.IsInstanceValid(_panel))
		{
			LayoutStore.HandViewerPos = _panel.Position;
			LayoutStore.HandViewerSize = _panel.Size;
			LayoutStore.Save();
		}
		CleanupUI();
		_visible = false;
		_resizing = false;
	}

	public static void SetCanvasHidden(bool hidden)
	{
		if (!IsPopoutMode && _canvasLayer != null && GodotObject.IsInstanceValid(_canvasLayer))
		{
			_canvasLayer.Visible = !hidden;
		}
	}

	private static void CleanupUI()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		var enumerator = _sections.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				PlayerHandSection current = enumerator.Current;
				try
				{
					current.Cleanup();
				}
				catch (System.Exception ex)
				{
					ModLog.Error("DeckTracker section cleanup", ex);
				}
			}
		}
		finally
		{
			((System.IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		_sections.Clear();
		if (_canvasLayer != null && GodotObject.IsInstanceValid(_canvasLayer))
		{
			_canvasLayer.QueueFree();
		}
		if (_popoutWindow != null && GodotObject.IsInstanceValid(_popoutWindow))
		{
			_popoutWindow.QueueFree();
		}
		if (_savedGuiEmbedSubwindows.HasValue)
		{
			try
			{
				Window window = NGame.Instance?.GetTree()?.Root;
				if (window != null)
				{
					window.GuiEmbedSubwindows = _savedGuiEmbedSubwindows.Value;
				}
			}
			catch (System.Exception ex2)
			{
				ModLog.Error("DeckTracker popout embed-flag restore", ex2);
			}
			_savedGuiEmbedSubwindows = null;
		}
		_canvasLayer = null;
		_panel = null;
		_contentClip = null;
		_contentScroll = null;
		_gripLabel = null;
		_popoutWindow = null;
		_offsetDragging = false;
	}

	internal static string ResolveDescription(object cardObj)
	{
		try
		{
			Traverse traverse = Traverse.Create(cardObj);
			try
			{
				object value = traverse.Property("Pile").GetValue();
				object obj = null;
				if (value != null)
				{
					obj = Traverse.Create(value).Property("PileType").GetValue();
				}
				MethodInfo method = cardObj.GetType().GetMethod("GetDescriptionForPile");
				if (method != (MethodInfo)null)
				{
					object obj2 = ((MethodBase)method).Invoke(cardObj, new object[2] { obj, null });
					if (obj2 is string text && !string.IsNullOrEmpty(text))
					{
						return CleanDescriptionText(text);
					}
				}
			}
			catch
			{
			}
			object value2 = traverse.Property("Description").GetValue();
			if (value2 == null)
			{
				return "";
			}
			string value3 = Traverse.Create(value2).Method("GetFormattedText").GetValue<string>();
			if (string.IsNullOrEmpty(value3))
			{
				return "";
			}
			string text2 = ResolveVarPlaceholders(cardObj, value3);
			return CleanDescriptionText(text2);
		}
		catch
		{
			return "";
		}
	}

	private static string CleanDescriptionText(string text)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		string text2 = text;
		object obj = _003C_003Ec._003C_003E9__46_0;
		if (obj == null)
		{
			MatchEvaluator val = delegate(Match match)
			{
				int count = Regex.Matches(((Capture)match).Value, "\\[img\\]").Count;
				Match val2 = Regex.Match(((Capture)match).Value, "res://\\S+?/([^/]+?)(?:_icon)?\\.png");
				string filename = (((Group)val2).Success ? ((Capture)val2.Groups[1]).Value : "?");
				filename = MapIconLabel(filename);
				return (count > 1) ? $"{count} {filename}" : filename;
			};
			_003C_003Ec._003C_003E9__46_0 = val;
			obj = (object)val;
		}
		text = Regex.Replace(text2, "(\\[img\\]res://\\S+?/([^/]+?)(?:_icon)?\\.png\\[/img\\])+", (MatchEvaluator)obj, (RegexOptions)1);
		text = Regex.Replace(text, "\\[/?[^\\]]+\\]", "");
		text = Regex.Replace(text, "res://\\S+", "");
		return text.Trim();
	}

	private static string MapIconLabel(string filename)
	{
		filename = filename.ToLowerInvariant();
		if (filename.Contains("energy"))
		{
			return "Energy";
		}
		if (filename.Contains("block"))
		{
			return "Block";
		}
		if (filename.Contains("star"))
		{
			return "Star";
		}
		if (filename.Contains("orb"))
		{
			return "Orb";
		}
		return char.ToUpper(filename[0]) + filename.Substring(1);
	}

	private static string ResolveVarPlaceholders(object cardObj, string text)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		object dvObj = Traverse.Create(cardObj).Property("DynamicVars").GetValue();
		if (dvObj == null)
		{
			return text;
		}
		return Regex.Replace(text, "\\{(\\w+)(?::[^}]*)?\\}", (MatchEvaluator)delegate(Match match)
		{
			string value = ((Capture)match.Groups[1]).Value;
			try
			{
				MethodInfo method = dvObj.GetType().GetMethod("TryGetValue");
				if (method != (MethodInfo)null)
				{
					object[] array = new object[2] { value, null };
					if ((bool)((MethodBase)method).Invoke(dvObj, array) && array[1] != null)
					{
						return Traverse.Create(array[1]).Property<int>("IntValue").Value.ToString();
					}
				}
			}
			catch
			{
			}
			return ((Capture)match).Value;
		});
	}

	public static void HandleMouseInput(InputEvent @event)
	{
		if (!_visible || _panel == null || !GodotObject.IsInstanceValid(_panel) || IsPopoutMode)
		{
			return;
		}
		UpdateGripPosition();
		if (@event is InputEventMouseButton inputEventMouseButton && inputEventMouseButton.ButtonIndex == MouseButton.Left)
		{
			Vector2 position = inputEventMouseButton.Position;
			Rect2 globalRect = _panel.GetGlobalRect();
			if (inputEventMouseButton.Pressed)
			{
				if (globalRect.HasPoint(position))
				{
					ResizeEdge resizeEdge = GetResizeEdge(position, globalRect);
					if (resizeEdge != ResizeEdge.None)
					{
						_resizing = true;
						_resizeEdge = resizeEdge;
					}
					else if (inputEventMouseButton.ShiftPressed)
					{
						_offsetDragging = true;
						_offsetDragStartMouseY = position.Y;
						_offsetDragStartValue = LayoutStore.HandViewerContentOffsetY;
					}
					else
					{
						_dragging = true;
						_dragOffset = position - _panel.Position;
					}
				}
				else if (!SettingsMenu.IsPointInPanel(position) && !ModSettings.AlwaysShowTeammateHand)
				{
					_dragging = false;
					_resizing = false;
					Hide();
				}
			}
			else
			{
				if (_dragging || _resizing || _offsetDragging)
				{
					LayoutStore.HandViewerPos = _panel.Position;
					LayoutStore.HandViewerSize = _panel.Size;
					LayoutStore.Save();
				}
				_dragging = false;
				_resizing = false;
				_offsetDragging = false;
			}
		}
		else
		{
			if (!(@event is InputEventMouseMotion inputEventMouseMotion))
			{
				return;
			}
			if (_resizing)
			{
				Vector2 position2 = _panel.Position;
				Vector2 size = _panel.Size;
				if (((System.Enum)_resizeEdge).HasFlag((System.Enum)ResizeEdge.Right))
				{
					size.X = Math.Max(230f, inputEventMouseMotion.Position.X - position2.X);
				}
				if (((System.Enum)_resizeEdge).HasFlag((System.Enum)ResizeEdge.Bottom))
				{
					size.Y = Math.Max(180f, inputEventMouseMotion.Position.Y - position2.Y);
				}
				_panel.CustomMinimumSize = size;
				_panel.Size = size;
			}
			else if (_offsetDragging)
			{
				// Dragging up shifts content up (hiding more of the header); keep at least 20px of content visible.
				float max = (_contentClip != null && GodotObject.IsInstanceValid(_contentClip)) ? Math.Max(0f, _contentClip.Size.Y - 20f) : 0f;
				LayoutStore.HandViewerContentOffsetY = Math.Clamp(_offsetDragStartValue - (inputEventMouseMotion.Position.Y - _offsetDragStartMouseY), 0f, max);
				ApplyContentOffset();
			}
			else if (_dragging)
			{
				_panel.Position = inputEventMouseMotion.Position - _dragOffset;
			}
			else
			{
				UpdateResizeCursor(inputEventMouseMotion.Position, _panel.GetGlobalRect());
			}
		}
	}

	private static ResizeEdge GetResizeEdge(Vector2 mousePos, Rect2 rect)
	{
		ResizeEdge resizeEdge = ResizeEdge.None;
		if (mousePos.X >= rect.End.X - 8f)
		{
			resizeEdge |= ResizeEdge.Right;
		}
		if (mousePos.Y >= rect.End.Y - 8f)
		{
			resizeEdge |= ResizeEdge.Bottom;
		}
		return resizeEdge;
	}

	private static void UpdateResizeCursor(Vector2 mousePos, Rect2 rect)
	{
		if (!rect.HasPoint(mousePos))
		{
			_panel.MouseDefaultCursorShape = Control.CursorShape.Arrow;
			return;
		}
		ResizeEdge resizeEdge = GetResizeEdge(mousePos, rect);
		HandPanel panel = _panel;
		if (1 == 0)
		{
		}
		Control.CursorShape mouseDefaultCursorShape = resizeEdge switch
		{
			ResizeEdge.Right | ResizeEdge.Bottom => Control.CursorShape.Fdiagsize, 
			ResizeEdge.Right => Control.CursorShape.Hsize, 
			ResizeEdge.Bottom => Control.CursorShape.Vsize, 
			_ => Control.CursorShape.Arrow, 
		};
		if (1 == 0)
		{
		}
		panel.MouseDefaultCursorShape = mouseDefaultCursorShape;
	}
}
