using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace BetterSpire2;

public static class DamageTracker
{
	private static readonly Dictionary<Creature, Label> _labels = new Dictionary<Creature, Label>();

	private static Font? _cachedFont;

	private static bool _dragging;

	private static Vector2 _dragStartMousePos;

	private static Vector2 _dragStartOffset;

	private static string? _dragKey;

	private static MethodInfo? _modifyDamageMethod;

	private static ParameterInfo[]? _modifyDamageParams;

	private static bool _modifyDamageLookupTried;

	private static string GetOffsetKey(Creature creature)
	{
		try
		{
			if (creature.Player != null)
			{
				return $"p{creature.Player.NetId}";
			}
			Player petOwner = creature.PetOwner;
			if (petOwner != null)
			{
				return $"pet{petOwner.NetId}";
			}
		}
		catch
		{
		}
		return "unknown";
	}

	private static Vector2 GetOffset(Creature creature)
	{
		Vector2 vector = default(Vector2);
		return LayoutStore.DamageOffsets.TryGetValue(GetOffsetKey(creature), out vector) ? vector : Vector2.Zero;
	}

	public static void ResetDragOffset()
	{
		LayoutStore.DamageOffsets.Clear();
		LayoutStore.Save();
		Recalculate();
	}

	public static void HandleMouseInput(InputEvent @event)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (_labels.Count == 0)
			{
				return;
			}
			if (@event is InputEventMouseButton inputEventMouseButton && inputEventMouseButton.ButtonIndex == MouseButton.Left)
			{
				if (inputEventMouseButton.Pressed && !_dragging)
				{
					var enumerator = _labels.GetEnumerator();
					try
					{
						Vector2 vector = default(Vector2);
						while (enumerator.MoveNext())
						{
							KeyValuePair<Creature, Label> current = enumerator.Current;
							Label value = current.Value;
							if (value == null || !GodotObject.IsInstanceValid(value) || !value.Visible || !new Rect2(value.GlobalPosition, value.Size).HasPoint(inputEventMouseButton.Position))
							{
								continue;
							}
							_dragging = true;
							_dragStartMousePos = inputEventMouseButton.Position;
							_dragKey = GetOffsetKey(current.Key);
							_dragStartOffset = (LayoutStore.DamageOffsets.TryGetValue(_dragKey, out vector) ? vector : Vector2.Zero);
							break;
						}
						return;
					}
					finally
					{
						((System.IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
					}
				}
				if (!inputEventMouseButton.Pressed && _dragging)
				{
					_dragging = false;
					_dragKey = null;
					LayoutStore.Save();
				}
			}
			else if (@event is InputEventMouseMotion inputEventMouseMotion && _dragging && _dragKey != null)
			{
				LayoutStore.DamageOffsets[_dragKey] = _dragStartOffset + (inputEventMouseMotion.Position - _dragStartMousePos);
				Recalculate();
			}
		}
		catch (System.Exception ex)
		{
			ModLog.Error("DamageTracker.HandleMouseInput", ex);
		}
	}

	public static void Recalculate()
	{
		//IL_1451: Unknown result type (might be due to invalid IL or missing references)
		//IL_1456: Unknown result type (might be due to invalid IL or missing references)
		//IL_1906: Unknown result type (might be due to invalid IL or missing references)
		//IL_190b: Unknown result type (might be due to invalid IL or missing references)
		//IL_145f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1464: Unknown result type (might be due to invalid IL or missing references)
		//IL_1536: Unknown result type (might be due to invalid IL or missing references)
		//IL_153b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b22: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b27: Unknown result type (might be due to invalid IL or missing references)
		//IL_1544: Unknown result type (might be due to invalid IL or missing references)
		//IL_1549: Unknown result type (might be due to invalid IL or missing references)
		//IL_15ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_15f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_15fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1600: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d23: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d59: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f21: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e04: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (!ModSettings.PlayerDamageTotal)
			{
				Hide();
				return;
			}
			NCapstoneContainer instance = NCapstoneContainer.Instance;
			if (instance != null && instance.InUse)
			{
				Hide();
				return;
			}
			NOverlayStack instance2 = NOverlayStack.Instance;
			if (instance2 != null && instance2.ScreenCount > 0)
			{
				Hide();
				return;
			}
			NMapScreen instance3 = NMapScreen.Instance;
			if (instance3 != null && instance3.Visible)
			{
				Hide();
				return;
			}
			CombatManager instance4 = CombatManager.Instance;
			CombatState state = instance4.DebugOnlyGetState();
			Dictionary<Creature, int> simEnemyHp;
			Dictionary<Creature, int> simEnemyBlock;
			Dictionary<Creature, int> simEnemyShellBudget;
			Dictionary<Creature, int> simEnemyHardToKillCap;
			Dictionary<Creature, int> simEnemyIntangibleCap;
			HashSet<Creature> fatalEnemies;
			if (state == null)
			{
				Hide();
			}
			else
			{
				if (instance4.EndingPlayerTurnPhaseOne || instance4.EndingPlayerTurnPhaseTwo || instance4.IsEnemyTurnStarted)
				{
					return;
				}
				System.Collections.Generic.IReadOnlyList<Creature> playerCreatures = state.PlayerCreatures;
				int num = 0;
				System.Collections.Generic.IEnumerator<Creature> enumerator = ((System.Collections.Generic.IEnumerable<Creature>)playerCreatures).GetEnumerator();
				try
				{
					while (((System.Collections.IEnumerator)enumerator).MoveNext())
					{
						Creature current = enumerator.Current;
						if (!current.IsDead && !current.IsPet && current.IsPlayer)
						{
							AccelerantPower power = current.GetPower<AccelerantPower>();
							if (power != null)
							{
								num += power.Amount;
							}
						}
					}
				}
				finally
				{
					((System.IDisposable)enumerator)?.Dispose();
				}
				simEnemyHp = new Dictionary<Creature, int>();
				simEnemyBlock = new Dictionary<Creature, int>();
				simEnemyShellBudget = new Dictionary<Creature, int>();
				Dictionary<Creature, int> val = new Dictionary<Creature, int>();
				simEnemyHardToKillCap = new Dictionary<Creature, int>();
				simEnemyIntangibleCap = new Dictionary<Creature, int>();
				System.Collections.Generic.IEnumerator<Creature> enumerator2 = ((System.Collections.Generic.IEnumerable<Creature>)state.Enemies).GetEnumerator();
				try
				{
					while (((System.Collections.IEnumerator)enumerator2).MoveNext())
					{
						Creature current2 = enumerator2.Current;
						if (!current2.IsDead && current2.Monster != null)
						{
							simEnemyHp[current2] = current2.CurrentHp;
							simEnemyBlock[current2] = Math.Max(0, current2.Block);
							HardToKillPower power2 = current2.GetPower<HardToKillPower>();
							simEnemyHardToKillCap[current2] = ((power2 != null && power2.Amount > 0) ? power2.Amount : 2147483647);
							simEnemyIntangibleCap[current2] = ((current2.GetPower<IntangiblePower>() != null) ? 1 : 2147483647);
							HardenedShellPower power3 = current2.GetPower<HardenedShellPower>();
							simEnemyShellBudget[current2] = ((power3 != null && power3.Amount > 0) ? Math.Max(0, power3.DisplayAmount) : 2147483647);
							val[current2] = ((power3 != null && power3.Amount > 0) ? power3.Amount : 2147483647);
						}
					}
				}
				finally
				{
					((System.IDisposable)enumerator2)?.Dispose();
				}
				fatalEnemies = new HashSet<Creature>();
				System.Collections.Generic.IEnumerator<Creature> enumerator3 = ((System.Collections.Generic.IEnumerable<Creature>)playerCreatures).GetEnumerator();
				try
				{
					while (((System.Collections.IEnumerator)enumerator3).MoveNext())
					{
						Creature current3 = enumerator3.Current;
						if (current3.IsDead || current3.IsPet || !current3.IsPlayer)
						{
							continue;
						}
						try
						{
							HailstormPower power4 = current3.GetPower<HailstormPower>();
							if (power4 != null && power4.Amount > 0)
							{
								int num2 = 1;
								if (power4.DynamicVars.TryGetValue("FrostOrbs", out DynamicVar value))
								{
									num2 = (int)value.BaseValue;
								}
								int num3 = 0;
								System.Collections.Generic.IReadOnlyList<OrbModel> readOnlyList = current3.Player?.PlayerCombatState?.OrbQueue?.Orbs;
								if (readOnlyList != null)
								{
									System.Collections.Generic.IEnumerator<OrbModel> enumerator4 = ((System.Collections.Generic.IEnumerable<OrbModel>)readOnlyList).GetEnumerator();
									try
									{
										while (((System.Collections.IEnumerator)enumerator4).MoveNext())
										{
											OrbModel current4 = enumerator4.Current;
											if (current4 is FrostOrb)
											{
												num3++;
											}
										}
									}
									finally
									{
										((System.IDisposable)enumerator4)?.Dispose();
									}
								}
								if (num3 >= num2)
								{
									int amount = power4.Amount;
									System.Collections.Generic.IEnumerator<Creature> enumerator5 = ((System.Collections.Generic.IEnumerable<Creature>)state.Enemies).GetEnumerator();
									try
									{
										while (((System.Collections.IEnumerator)enumerator5).MoveNext())
										{
											Creature current5 = enumerator5.Current;
											if (!current5.IsDead && current5.Monster != null && current5.IsHittable)
											{
												ApplyEnemyDamage(current5, amount, unblockable: false);
											}
										}
									}
									finally
									{
										((System.IDisposable)enumerator5)?.Dispose();
									}
								}
							}
						}
						catch
						{
						}
						try
						{
							System.Collections.Generic.IReadOnlyList<OrbModel> readOnlyList2 = current3.Player?.PlayerCombatState?.OrbQueue?.Orbs;
							if (readOnlyList2 != null && ((System.Collections.Generic.IReadOnlyCollection<OrbModel>)readOnlyList2).Count > 0)
							{
								Creature creature = null;
								int num4 = 0;
								System.Collections.Generic.IEnumerator<Creature> enumerator6 = ((System.Collections.Generic.IEnumerable<Creature>)state.Enemies).GetEnumerator();
								try
								{
									while (((System.Collections.IEnumerator)enumerator6).MoveNext())
									{
										Creature current6 = enumerator6.Current;
										if (!current6.IsDead && current6.Monster != null && current6.IsHittable && !fatalEnemies.Contains(current6))
										{
											num4++;
											creature = current6;
											if (num4 > 1)
											{
												break;
											}
										}
									}
								}
								finally
								{
									((System.IDisposable)enumerator6)?.Dispose();
								}
								if (num4 == 1 && creature != null)
								{
									Player? player = current3.Player;
									int num5;
									if (player == null)
									{
										num5 = 0;
									}
									else
									{
										System.Collections.Generic.IReadOnlyList<RelicModel> relics = player.Relics;
										num5 = ((((relics != null) ? new bool?(Enumerable.Any<GoldPlatedCables>(Enumerable.OfType<GoldPlatedCables>((System.Collections.IEnumerable)relics))) : ((bool?)null)) == true) ? 1 : 0);
									}
									bool flag = (byte)num5 != 0;
									for (int i = 0; i < ((System.Collections.Generic.IReadOnlyCollection<OrbModel>)readOnlyList2).Count; i++)
									{
										if (!(readOnlyList2[i] is LightningOrb lightningOrb))
										{
											continue;
										}
										int dmg = Math.Max(0, (int)lightningOrb.PassiveVal);
										int num6 = ((!flag || i != 0) ? 1 : 2);
										for (int j = 0; j < num6; j++)
										{
											ApplyEnemyDamage(creature, dmg, unblockable: false);
											if (fatalEnemies.Contains(creature))
											{
												break;
											}
										}
										if (fatalEnemies.Contains(creature))
										{
											break;
										}
									}
								}
							}
						}
						catch
						{
						}
						try
						{
							InfernoPower power5 = current3.GetPower<InfernoPower>();
							if (power5 == null || power5.Amount <= 0)
							{
								continue;
							}
							int num7 = CountInfernoTriggers(current3, instance4, state);
							if (num7 <= 0)
							{
								continue;
							}
							int dmg2 = power5.Amount * num7;
							System.Collections.Generic.IEnumerator<Creature> enumerator7 = ((System.Collections.Generic.IEnumerable<Creature>)state.Enemies).GetEnumerator();
							try
							{
								while (((System.Collections.IEnumerator)enumerator7).MoveNext())
								{
									Creature current7 = enumerator7.Current;
									if (!current7.IsDead && current7.Monster != null && current7.IsHittable)
									{
										ApplyEnemyDamage(current7, dmg2, unblockable: false);
									}
								}
							}
							finally
							{
								((System.IDisposable)enumerator7)?.Dispose();
							}
						}
						catch
						{
						}
					}
				}
				finally
				{
					((System.IDisposable)enumerator3)?.Dispose();
				}
				System.Collections.Generic.IEnumerator<Creature> enumerator8 = ((System.Collections.Generic.IEnumerable<Creature>)state.Enemies).GetEnumerator();
				try
				{
					while (((System.Collections.IEnumerator)enumerator8).MoveNext())
					{
						Creature current8 = enumerator8.Current;
						if (!current8.IsDead && current8.Monster != null && !fatalEnemies.Contains(current8))
						{
							simEnemyShellBudget[current8] = val[current8];
						}
					}
				}
				finally
				{
					((System.IDisposable)enumerator8)?.Dispose();
				}
				System.Collections.Generic.IEnumerator<Creature> enumerator9 = ((System.Collections.Generic.IEnumerable<Creature>)state.Enemies).GetEnumerator();
				try
				{
					while (((System.Collections.IEnumerator)enumerator9).MoveNext())
					{
						Creature current9 = enumerator9.Current;
						if (current9.IsDead || current9.Monster == null || fatalEnemies.Contains(current9))
						{
							continue;
						}
						PoisonPower power6 = current9.GetPower<PoisonPower>();
						if (power6 == null || power6.Amount <= 0)
						{
							continue;
						}
						int num8 = Math.Min(power6.Amount, 1 + num);
						int num9 = power6.Amount;
						for (int k = 0; k < num8; k++)
						{
							ApplyEnemyDamage(current9, num9, unblockable: true);
							if (fatalEnemies.Contains(current9))
							{
								break;
							}
							num9 = Math.Max(0, num9 - 1);
						}
					}
				}
				finally
				{
					((System.IDisposable)enumerator9)?.Dispose();
				}
				if (!Enumerable.Any<Creature>((System.Collections.Generic.IEnumerable<Creature>)state.Enemies, (Func<Creature, bool>)((Creature e) => !e.IsDead && e.Monster != null && e.IsPrimaryEnemy && !fatalEnemies.Contains(e))))
				{
					System.Collections.Generic.IEnumerator<Creature> enumerator10 = ((System.Collections.Generic.IEnumerable<Creature>)state.Enemies).GetEnumerator();
					try
					{
						while (((System.Collections.IEnumerator)enumerator10).MoveNext())
						{
							Creature current10 = enumerator10.Current;
							if (!current10.IsDead && current10.Monster != null && current10.IsSecondaryEnemy)
							{
								fatalEnemies.Add(current10);
							}
						}
					}
					finally
					{
						((System.IDisposable)enumerator10)?.Dispose();
					}
				}
				List<Creature> val2 = new List<Creature>();
				System.Collections.Generic.IEnumerator<Creature> enumerator11 = ((System.Collections.Generic.IEnumerable<Creature>)state.Enemies).GetEnumerator();
				try
				{
					while (((System.Collections.IEnumerator)enumerator11).MoveNext())
					{
						Creature current11 = enumerator11.Current;
						if (!current11.IsDead && current11.Monster != null && !fatalEnemies.Contains(current11))
						{
							val2.Add(current11);
						}
					}
				}
				finally
				{
					((System.IDisposable)enumerator11)?.Dispose();
				}
				Creature creature2 = null;
				try
				{
					System.Collections.Generic.IEnumerator<Creature> enumerator12 = ((System.Collections.Generic.IEnumerable<Creature>)playerCreatures).GetEnumerator();
					try
					{
						while (((System.Collections.IEnumerator)enumerator12).MoveNext())
						{
							Creature current12 = enumerator12.Current;
							if (!current12.IsDead && !current12.IsPet)
							{
								InterceptPower power7 = current12.GetPower<InterceptPower>();
								if (power7 != null)
								{
									creature2 = current12;
									break;
								}
							}
						}
					}
					finally
					{
						((System.IDisposable)enumerator12)?.Dispose();
					}
				}
				catch
				{
				}
				HashSet<Creature> activeCreatures = new HashSet<Creature>();
				System.Collections.Generic.IEnumerator<Creature> enumerator13 = ((System.Collections.Generic.IEnumerable<Creature>)playerCreatures).GetEnumerator();
				try
				{
					while (((System.Collections.IEnumerator)enumerator13).MoveNext())
					{
						Creature player2 = enumerator13.Current;
						if (player2.IsDead || player2.IsPet || !player2.IsPlayer)
						{
							continue;
						}
						List<ValueTuple<Creature, int>> val3 = new List<ValueTuple<Creature, int>>();
						if (creature2 == null || creature2 == player2)
						{
							var enumerator14 = val2.GetEnumerator();
							try
							{
								while (enumerator14.MoveNext())
								{
									Creature current13 = enumerator14.Current;
									System.Collections.Generic.IEnumerator<AbstractIntent> enumerator15 = ((System.Collections.Generic.IEnumerable<AbstractIntent>)current13.Monster.NextMove.Intents).GetEnumerator();
									try
									{
										while (((System.Collections.IEnumerator)enumerator15).MoveNext())
										{
											AbstractIntent current14 = enumerator15.Current;
											if (current14 is AttackIntent { DamageCalc: not null } attackIntent)
											{
												decimal damage = attackIntent.DamageCalc.Invoke();
												decimal num10 = InvokeModifyDamage(player2.Player.RunState, state, player2, current13, damage);
												int num11 = Math.Max(0, (int)num10);
												int repeats = attackIntent.Repeats;
												for (int num12 = 0; num12 < repeats; num12++)
												{
													val3.Add(new ValueTuple<Creature, int>(current13, num11));
												}
											}
										}
									}
									finally
									{
										((System.IDisposable)enumerator15)?.Dispose();
									}
								}
							}
							finally
							{
								((System.IDisposable)enumerator14/*cast due to .constrained prefix*/).Dispose();
							}
						}
						Creature creature3 = null;
						System.Collections.Generic.IReadOnlyList<Creature> pets = player2.Pets;
						if (pets != null && ((System.Collections.Generic.IReadOnlyCollection<Creature>)pets).Count > 0)
						{
							creature3 = Enumerable.FirstOrDefault<Creature>((System.Collections.Generic.IEnumerable<Creature>)pets, (Func<Creature, bool>)((Creature p) => !p.IsDead));
						}
						List<ValueTuple<Creature, int>> val4 = new List<ValueTuple<Creature, int>>();
						List<ValueTuple<Creature, int>> val5 = new List<ValueTuple<Creature, int>>();
						System.Collections.Generic.IEnumerator<Creature> enumerator16 = Enumerable.Concat<Creature>((System.Collections.Generic.IEnumerable<Creature>)new Creature[1] { player2 }, (System.Collections.Generic.IEnumerable<Creature>)((creature3 == null) ? System.Array.Empty<Creature>() : new Creature[1] { creature3 })).GetEnumerator();
						try
						{
							while (((System.Collections.IEnumerator)enumerator16).MoveNext())
							{
								Creature current15 = enumerator16.Current;
								if (current15.IsDead)
								{
									continue;
								}
								ConstrictPower power8 = current15.GetPower<ConstrictPower>();
								if (power8 != null && power8.Amount > 0)
								{
									val4.Add(new ValueTuple<Creature, int>(current15, power8.Amount));
								}
								DemisePower power9 = current15.GetPower<DemisePower>();
								if (power9 != null && power9.Amount > 0)
								{
									val5.Add(new ValueTuple<Creature, int>(current15, power9.Amount));
								}
								System.Collections.Generic.IEnumerator<MagicBombPower> enumerator17 = current15.GetPowerInstances<MagicBombPower>().GetEnumerator();
								try
								{
									while (((System.Collections.IEnumerator)enumerator17).MoveNext())
									{
										MagicBombPower current16 = enumerator17.Current;
										if (current16.Amount > 0 && current16.Applier != null && !current16.Applier.IsDead)
										{
											val4.Add(new ValueTuple<Creature, int>(current15, current16.Amount));
										}
									}
								}
								finally
								{
									((System.IDisposable)enumerator17)?.Dispose();
								}
								DisintegrationPower power10 = current15.GetPower<DisintegrationPower>();
								if (power10 != null && power10.Amount > 0)
								{
									val4.Add(new ValueTuple<Creature, int>(current15, power10.Amount));
								}
							}
						}
						finally
						{
							((System.IDisposable)enumerator16)?.Dispose();
						}
						if (player2.Player?.PlayerCombatState != null)
						{
							System.Collections.Generic.IEnumerator<CardModel> enumerator18 = ((System.Collections.Generic.IEnumerable<CardModel>)player2.Player.PlayerCombatState.Hand.Cards).GetEnumerator();
							try
							{
								while (((System.Collections.IEnumerator)enumerator18).MoveNext())
								{
									CardModel current17 = enumerator18.Current;
									if (!current17.HasTurnEndInHandEffect)
									{
										continue;
									}
									DynamicVar value3;
									if (current17.DynamicVars.TryGetValue("Damage", out DynamicVar value2))
									{
										int num13 = (int)value2.BaseValue;
										if (num13 > 0)
										{
											val4.Add(new ValueTuple<Creature, int>(player2, num13));
										}
									}
									else if (current17.DynamicVars.TryGetValue("HpLoss", out value3))
									{
										int num14 = (int)value3.BaseValue;
										if (num14 > 0)
										{
											val5.Add(new ValueTuple<Creature, int>(player2, num14));
										}
									}
								}
							}
							finally
							{
								((System.IDisposable)enumerator18)?.Dispose();
							}
						}
						int num15 = Enumerable.Sum<ValueTuple<Creature, int>>((System.Collections.Generic.IEnumerable<ValueTuple<Creature, int>>)val4, (Func<ValueTuple<Creature, int>, int>)((ValueTuple<Creature, int> d) => d.Item2)) + Enumerable.Sum<ValueTuple<Creature, int>>((System.Collections.Generic.IEnumerable<ValueTuple<Creature, int>>)val5, (Func<ValueTuple<Creature, int>, int>)((ValueTuple<Creature, int> d) => d.Item2)) + Enumerable.Sum<ValueTuple<Creature, int>>((System.Collections.Generic.IEnumerable<ValueTuple<Creature, int>>)val3, (Func<ValueTuple<Creature, int>, int>)((ValueTuple<Creature, int> h) => h.Item2));
						if (num15 <= 0)
						{
							continue;
						}
						int num16 = player2.Block;
						bool flag2 = creature3 != null && !creature3.IsDead && creature3.MaxHp > 0 && (creature3.Monster == null || creature3.Monster.IsHealthBarVisible);
						int num17 = (flag2 ? creature3.CurrentHp : 0);
						int num18 = (flag2 ? creature3.Block : 0);
						int num19 = 0;
						int num20 = 0;
						if (!instance4.IsEnemyTurnStarted)
						{
							if (num16 == 0 && player2.Player?.Relics != null)
							{
								if (Enumerable.Any<Orichalcum>(Enumerable.OfType<Orichalcum>((System.Collections.IEnumerable)player2.Player.Relics)))
								{
									num16 += 6;
								}
								if (Enumerable.Any<FakeOrichalcum>(Enumerable.OfType<FakeOrichalcum>((System.Collections.IEnumerable)player2.Player.Relics)))
								{
									num16 += 3;
								}
							}
							try
							{
								Player? player3 = player2.Player;
								object obj5;
								if (player3 == null)
								{
									obj5 = null;
								}
								else
								{
									System.Collections.Generic.IReadOnlyList<RelicModel> relics2 = player3.Relics;
									obj5 = ((relics2 != null) ? Enumerable.FirstOrDefault<RippleBasin>(Enumerable.OfType<RippleBasin>((System.Collections.IEnumerable)relics2)) : null);
								}
								RippleBasin rippleBasin = (RippleBasin)obj5;
								if (rippleBasin != null && !Enumerable.Any<CardPlayFinishedEntry>(instance4.History.CardPlaysFinished, (Func<CardPlayFinishedEntry, bool>)((CardPlayFinishedEntry e) => e.HappenedThisTurn(state) && ((object)e.CardPlay.Card.Type/*cast due to .constrained prefix*/).ToString().Contains("Attack") && e.CardPlay.Card.Owner == player2.Player)) && rippleBasin.DynamicVars.TryGetValue("Block", out DynamicVar value4))
								{
									num16 += (int)value4.BaseValue;
								}
							}
							catch
							{
							}
							PlatingPower power11 = player2.GetPower<PlatingPower>();
							if (power11 != null && power11.Amount > 0)
							{
								num16 += power11.Amount;
							}
							try
							{
								Player? player4 = player2.Player;
								object obj7;
								if (player4 == null)
								{
									obj7 = null;
								}
								else
								{
									System.Collections.Generic.IReadOnlyList<RelicModel> relics3 = player4.Relics;
									obj7 = ((relics3 != null) ? Enumerable.FirstOrDefault<CloakClasp>(Enumerable.OfType<CloakClasp>((System.Collections.IEnumerable)relics3)) : null);
								}
								CloakClasp cloakClasp = (CloakClasp)obj7;
								if (cloakClasp != null)
								{
									int valueOrDefault = (((System.Collections.Generic.IReadOnlyCollection<CardModel>)player2.Player?.PlayerCombatState?.Hand?.Cards)?.Count).GetValueOrDefault();
									if (valueOrDefault > 0)
									{
										int num21 = 1;
										if (cloakClasp.DynamicVars.TryGetValue("Block", out DynamicVar value5))
										{
											num21 = (int)value5.BaseValue;
										}
										num16 += valueOrDefault * num21;
									}
								}
							}
							catch
							{
							}
						}
						bool flag3 = player2.GetPower<IntangiblePower>() != null;
						Player? player5 = player2.Player;
						int num22;
						if (player5 == null)
						{
							num22 = 0;
						}
						else
						{
							System.Collections.Generic.IReadOnlyList<RelicModel> relics4 = player5.Relics;
							num22 = ((((relics4 != null) ? new bool?(Enumerable.Any<TungstenRod>(Enumerable.OfType<TungstenRod>((System.Collections.IEnumerable)relics4))) : ((bool?)null)) == true) ? 1 : 0);
						}
						bool flag4 = (byte)num22 != 0;
						int num23 = 2147483647;
						try
						{
							Player? player6 = player2.Player;
							object obj9;
							if (player6 == null)
							{
								obj9 = null;
							}
							else
							{
								System.Collections.Generic.IReadOnlyList<RelicModel> relics5 = player6.Relics;
								obj9 = ((relics5 != null) ? Enumerable.FirstOrDefault<BeatingRemnant>(Enumerable.OfType<BeatingRemnant>((System.Collections.IEnumerable)relics5)) : null);
							}
							BeatingRemnant beatingRemnant = (BeatingRemnant)obj9;
							if (beatingRemnant != null)
							{
								int num24 = 20;
								if (beatingRemnant.DynamicVars.TryGetValue("MaxHpLoss", out DynamicVar value6))
								{
									num24 = (int)value6.BaseValue;
								}
								int num25 = (int)Traverse.Create(beatingRemnant).Field("_damageReceivedThisTurn").GetValue<decimal>();
								num23 = Math.Max(0, num24 - num25);
							}
						}
						catch
						{
						}
						if (player2.Player?.PlayerCombatState?.OrbQueue != null)
						{
							System.Collections.Generic.IReadOnlyList<OrbModel> orbs = player2.Player.PlayerCombatState.OrbQueue.Orbs;
							System.Collections.Generic.IReadOnlyList<RelicModel> relics6 = player2.Player.Relics;
							bool flag5 = relics6 != null && Enumerable.Any<GoldPlatedCables>(Enumerable.OfType<GoldPlatedCables>((System.Collections.IEnumerable)relics6));
							for (int num26 = 0; num26 < ((System.Collections.Generic.IReadOnlyCollection<OrbModel>)orbs).Count; num26++)
							{
								if (orbs[num26] is FrostOrb frostOrb)
								{
									int num27 = ((!flag5 || num26 != 0) ? 1 : 2);
									num16 += Math.Max((int)frostOrb.PassiveVal, 0) * num27;
								}
							}
						}
						var enumerator19 = val4.GetEnumerator();
						try
						{
							while (enumerator19.MoveNext())
							{
								ValueTuple<Creature, int> current18 = enumerator19.Current;
								Creature item = current18.Item1;
								int item2 = current18.Item2;
								int num28 = ((flag3 && !item.IsPet) ? Math.Min(item2, 1) : item2);
								if (item.IsPet)
								{
									int num29 = Math.Min(num18, num28);
									num18 -= num29;
									int num30 = num28 - num29;
									num17 -= num30;
									num19 += num30;
									continue;
								}
								int num31 = Math.Min(num16, num28);
								num16 -= num31;
								int num32 = num28 - num31;
								if (num32 > 0 && flag4)
								{
									num32 = Math.Max(num32 - 1, 0);
								}
								num32 = Math.Min(num32, num23);
								num23 -= num32;
								num20 += num32;
							}
						}
						finally
						{
							((System.IDisposable)enumerator19/*cast due to .constrained prefix*/).Dispose();
						}
						var enumerator20 = val5.GetEnumerator();
						try
						{
							while (enumerator20.MoveNext())
							{
								ValueTuple<Creature, int> current19 = enumerator20.Current;
								Creature item3 = current19.Item1;
								int item4 = current19.Item2;
								int num33 = ((flag3 && !item3.IsPet) ? Math.Min(item4, 1) : item4);
								if (item3.IsPet)
								{
									num17 -= num33;
									num19 += num33;
									continue;
								}
								if (flag4 && num33 > 0)
								{
									num33 = Math.Max(num33 - 1, 0);
								}
								num33 = Math.Min(num33, num23);
								num23 -= num33;
								num20 += num33;
							}
						}
						finally
						{
							((System.IDisposable)enumerator20/*cast due to .constrained prefix*/).Dispose();
						}
						var enumerator21 = val3.GetEnumerator();
						try
						{
							while (enumerator21.MoveNext())
							{
								ValueTuple<Creature, int> current20 = enumerator21.Current;
								Creature item5 = current20.Item1;
								int item6 = current20.Item2;
								int num34 = (flag3 ? Math.Min(item6, 1) : item6);
								int num35 = Math.Min(num16, num34);
								num16 -= num35;
								num34 -= num35;
								if (num17 > 0 && num34 > 0)
								{
									int num36 = Math.Min(num17, num34);
									num17 -= num36;
									num19 += num36;
									num34 -= num36;
								}
								if (num34 > 0 && flag4)
								{
									num34 = Math.Max(num34 - 1, 0);
								}
								num34 = Math.Min(num34, num23);
								num23 -= num34;
								num20 += num34;
							}
						}
						finally
						{
							((System.IDisposable)enumerator21/*cast due to .constrained prefix*/).Dispose();
						}
						int num37 = 0;
						RegenPower power12 = player2.GetPower<RegenPower>();
						if (power12 != null && power12.Amount > 0)
						{
							num37 = power12.Amount;
						}
						activeCreatures.Add(player2);
						if (num20 > 0)
						{
							string text = (ModSettings.ShowExpectedHp ? $"{num20} ({Math.Min(player2.CurrentHp - num20 + num37, player2.MaxHp)})" : num20.ToString());
							ShowLabel(player2, text, new Color(1f, 0.3f, 0.3f));
						}
						else if (ModSettings.ShowExpectedHp && num37 > 0)
						{
							ShowLabel(player2, $"0 ({Math.Min(player2.CurrentHp + num37, player2.MaxHp)})", new Color(0.3f, 1f, 0.3f));
						}
						else
						{
							ShowLabel(player2, "0", new Color(0.3f, 1f, 0.3f));
						}
						if (flag2 && num19 > 0)
						{
							activeCreatures.Add(creature3);
							ShowLabel(creature3, num19.ToString(), new Color(1f, 0.6f, 0.2f));
						}
					}
				}
				finally
				{
					((System.IDisposable)enumerator13)?.Dispose();
				}
				List<Creature> val6 = Enumerable.ToList<Creature>(Enumerable.Where<Creature>((System.Collections.Generic.IEnumerable<Creature>)_labels.Keys, (Func<Creature, bool>)((Creature c) => !activeCreatures.Contains(c))));
				var enumerator22 = val6.GetEnumerator();
				try
				{
					while (enumerator22.MoveNext())
					{
						Creature current21 = enumerator22.Current;
						RemoveLabel(current21);
					}
					return;
				}
				finally
				{
					((System.IDisposable)enumerator22/*cast due to .constrained prefix*/).Dispose();
				}
			}
			void ApplyEnemyDamage(Creature enemy, int num39, bool unblockable)
			{
				if (!fatalEnemies.Contains(enemy) && simEnemyHp.ContainsKey(enemy))
				{
					int num38 = num39;
					if (!unblockable)
					{
						int num40 = Math.Min(simEnemyBlock[enemy], num38);
						Dictionary<Creature, int> val7 = simEnemyBlock;
						Creature creature4 = enemy;
						val7[creature4] -= num40;
						num38 -= num40;
					}
					if (num38 > 0)
					{
						int num41 = Math.Min(num38, simEnemyIntangibleCap[enemy]);
						num41 = Math.Min(num41, simEnemyHardToKillCap[enemy]);
						num41 = Math.Min(num41, simEnemyShellBudget[enemy]);
						simEnemyShellBudget[enemy] = Math.Max(0, simEnemyShellBudget[enemy] - num41);
						Dictionary<Creature, int> val7 = simEnemyHp;
						Creature creature4 = enemy;
						val7[creature4] -= num41;
						if (simEnemyHp[enemy] <= 0)
						{
							fatalEnemies.Add(enemy);
						}
					}
				}
			}
		}
		catch (System.Exception ex)
		{
			ModLog.Error("DamageTracker.Recalculate", ex);
		}
	}

	private static int CountInfernoTriggers(Creature player, CombatManager combatManager, CombatState state)
	{
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Player p = player.Player;
			if (p?.PlayerCombatState == null)
			{
				return 0;
			}
			bool flag = player.GetPower<IntangiblePower>() != null;
			int num = player.Block;
			if (num == 0 && p.Relics != null)
			{
				if (Enumerable.Any<Orichalcum>(Enumerable.OfType<Orichalcum>((System.Collections.IEnumerable)p.Relics)))
				{
					num += 6;
				}
				if (Enumerable.Any<FakeOrichalcum>(Enumerable.OfType<FakeOrichalcum>((System.Collections.IEnumerable)p.Relics)))
				{
					num += 3;
				}
			}
			try
			{
				System.Collections.Generic.IReadOnlyList<RelicModel> relics = p.Relics;
				RippleBasin rippleBasin = ((relics != null) ? Enumerable.FirstOrDefault<RippleBasin>(Enumerable.OfType<RippleBasin>((System.Collections.IEnumerable)relics)) : null);
				if (rippleBasin != null && !Enumerable.Any<CardPlayFinishedEntry>(combatManager.History.CardPlaysFinished, (Func<CardPlayFinishedEntry, bool>)((CardPlayFinishedEntry e) => e.HappenedThisTurn(state) && ((object)e.CardPlay.Card.Type/*cast due to .constrained prefix*/).ToString().Contains("Attack") && e.CardPlay.Card.Owner == p)) && rippleBasin.DynamicVars.TryGetValue("Block", out DynamicVar value))
				{
					num += (int)value.BaseValue;
				}
			}
			catch
			{
			}
			PlatingPower power = player.GetPower<PlatingPower>();
			if (power != null && power.Amount > 0)
			{
				num += power.Amount;
			}
			try
			{
				System.Collections.Generic.IReadOnlyList<RelicModel> relics2 = p.Relics;
				CloakClasp cloakClasp = ((relics2 != null) ? Enumerable.FirstOrDefault<CloakClasp>(Enumerable.OfType<CloakClasp>((System.Collections.IEnumerable)relics2)) : null);
				if (cloakClasp != null)
				{
					int valueOrDefault = (((System.Collections.Generic.IReadOnlyCollection<CardModel>)p.PlayerCombatState.Hand?.Cards)?.Count).GetValueOrDefault();
					if (valueOrDefault > 0)
					{
						int num2 = 1;
						if (cloakClasp.DynamicVars.TryGetValue("Block", out DynamicVar value2))
						{
							num2 = (int)value2.BaseValue;
						}
						num += valueOrDefault * num2;
					}
				}
			}
			catch
			{
			}
			System.Collections.Generic.IReadOnlyList<OrbModel> readOnlyList = p.PlayerCombatState.OrbQueue?.Orbs;
			if (readOnlyList != null)
			{
				System.Collections.Generic.IReadOnlyList<RelicModel> relics3 = p.Relics;
				bool flag2 = relics3 != null && Enumerable.Any<GoldPlatedCables>(Enumerable.OfType<GoldPlatedCables>((System.Collections.IEnumerable)relics3));
				for (int num3 = 0; num3 < ((System.Collections.Generic.IReadOnlyCollection<OrbModel>)readOnlyList).Count; num3++)
				{
					if (readOnlyList[num3] is FrostOrb frostOrb)
					{
						int num4 = ((!flag2 || num3 != 0) ? 1 : 2);
						num += Math.Max((int)frostOrb.PassiveVal, 0) * num4;
					}
				}
			}
			List<ValueTuple<int, bool>> val = new List<ValueTuple<int, bool>>();
			ConstrictPower power2 = player.GetPower<ConstrictPower>();
			if (power2 != null && power2.Amount > 0)
			{
				val.Add(new ValueTuple<int, bool>(power2.Amount, false));
			}
			DemisePower power3 = player.GetPower<DemisePower>();
			if (power3 != null && power3.Amount > 0)
			{
				val.Add(new ValueTuple<int, bool>(power3.Amount, true));
			}
			System.Collections.Generic.IEnumerator<MagicBombPower> enumerator = player.GetPowerInstances<MagicBombPower>().GetEnumerator();
			try
			{
				while (((System.Collections.IEnumerator)enumerator).MoveNext())
				{
					MagicBombPower current = enumerator.Current;
					if (current.Amount > 0 && current.Applier != null && !current.Applier.IsDead)
					{
						val.Add(new ValueTuple<int, bool>(current.Amount, false));
					}
				}
			}
			finally
			{
				((System.IDisposable)enumerator)?.Dispose();
			}
			DisintegrationPower power4 = player.GetPower<DisintegrationPower>();
			if (power4 != null && power4.Amount > 0)
			{
				val.Add(new ValueTuple<int, bool>(power4.Amount, false));
			}
			System.Collections.Generic.IEnumerator<CardModel> enumerator2 = ((System.Collections.Generic.IEnumerable<CardModel>)p.PlayerCombatState.Hand.Cards).GetEnumerator();
			try
			{
				while (((System.Collections.IEnumerator)enumerator2).MoveNext())
				{
					CardModel current2 = enumerator2.Current;
					if (!current2.HasTurnEndInHandEffect)
					{
						continue;
					}
					DynamicVar value4;
					if (current2.DynamicVars.TryGetValue("Damage", out DynamicVar value3))
					{
						int num5 = (int)value3.BaseValue;
						if (num5 > 0)
						{
							val.Add(new ValueTuple<int, bool>(num5, false));
						}
					}
					else if (current2.DynamicVars.TryGetValue("HpLoss", out value4))
					{
						int num6 = (int)value4.BaseValue;
						if (num6 > 0)
						{
							val.Add(new ValueTuple<int, bool>(num6, true));
						}
					}
				}
			}
			finally
			{
				((System.IDisposable)enumerator2)?.Dispose();
			}
			int num7 = 0;
			var enumerator3 = val.GetEnumerator();
			try
			{
				while (enumerator3.MoveNext())
				{
					ValueTuple<int, bool> current3 = enumerator3.Current;
					int item = current3.Item1;
					bool item2 = current3.Item2;
					int num8 = (flag ? Math.Min(item, 1) : item);
					if (!item2)
					{
						int num9 = Math.Min(num, num8);
						num -= num9;
						num8 -= num9;
					}
					if (num8 > 0)
					{
						num7++;
					}
				}
			}
			finally
			{
				((System.IDisposable)enumerator3/*cast due to .constrained prefix*/).Dispose();
			}
			return num7;
		}
		catch (System.Exception ex)
		{
			ModLog.Error("DamageTracker.CountInfernoTriggers", ex);
			return 0;
		}
	}

	private static void ShowLabel(Creature creature, string text, Color color)
	{
		NCombatRoom instance = NCombatRoom.Instance;
		if (instance == null)
		{
			return;
		}
		Label label = default(Label);
		if (!_labels.TryGetValue(creature, out label) || !GodotObject.IsInstanceValid(label))
		{
			label = CreateLabel(instance);
			if (label == null)
			{
				return;
			}
			_labels[creature] = label;
		}
		label.Text = text;
		label.AddThemeColorOverride("font_color", color);
		label.Visible = true;
		NCreature creatureNode = instance.GetCreatureNode(creature);
		if (creatureNode != null)
		{
			Vector2 offset = GetOffset(creature);
			label.GlobalPosition = new Vector2(creatureNode.GlobalPosition.X + 10f + offset.X, creatureNode.GlobalPosition.Y - 320f + offset.Y);
		}
	}

	public static void Hide()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		var enumerator = _labels.Values.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Label current = enumerator.Current;
				if (current != null && GodotObject.IsInstanceValid(current))
				{
					current.QueueFree();
				}
			}
		}
		finally
		{
			((System.IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		_labels.Clear();
		_cachedFont = null;
	}

	private static void RemoveLabel(Creature creature)
	{
		Label label = default(Label);
		if (_labels.TryGetValue(creature, out label))
		{
			if (label != null && GodotObject.IsInstanceValid(label))
			{
				label.QueueFree();
			}
			_labels.Remove(creature);
		}
	}

	private static Label? CreateLabel(Node parent)
	{
		Label label = new Label();
		label.HorizontalAlignment = HorizontalAlignment.Center;
		label.ZIndex = 100;
		label.MouseFilter = Control.MouseFilterEnum.Ignore;
		try
		{
			if (_cachedFont != null && !GodotObject.IsInstanceValid(_cachedFont))
			{
				_cachedFont = null;
			}
			if (_cachedFont == null)
			{
				_cachedFont = FindGameFont(parent);
			}
			if (_cachedFont != null)
			{
				label.AddThemeFontOverride("font", _cachedFont);
			}
		}
		catch (System.Exception ex)
		{
			_cachedFont = null;
			ModLog.Error("DamageTracker.CreateLabel font", ex);
		}
		label.AddThemeColorOverride("font_color", new Color(1f, 0.3f, 0.3f));
		label.AddThemeFontSizeOverride("font_size", 22);
		label.AddThemeConstantOverride("outline_size", 5);
		label.AddThemeColorOverride("font_outline_color", new Color(0f, 0f, 0f));
		parent.AddChild(label, forceReadableName: false, Node.InternalMode.Disabled);
		return label;
	}

	private static decimal InvokeModifyDamage(IRunState runState, CombatState combatState, Creature target, Creature dealer, decimal damage)
	{
		if (!_modifyDamageLookupTried)
		{
			_modifyDamageLookupTried = true;
			try
			{
				_modifyDamageMethod = Enumerable.FirstOrDefault<MethodInfo>((System.Collections.Generic.IEnumerable<MethodInfo>)Enumerable.OrderByDescending<MethodInfo, int>(Enumerable.Where<MethodInfo>((System.Collections.Generic.IEnumerable<MethodInfo>)typeof(Hook).GetMethods((BindingFlags)24), (Func<MethodInfo, bool>)((MethodInfo m) => ((MemberInfo)m).Name == "ModifyDamage" && m.ReturnType == typeof(decimal))), (Func<MethodInfo, int>)((MethodInfo m) => ((MethodBase)m).GetParameters().Length)));
				MethodInfo? modifyDamageMethod = _modifyDamageMethod;
				_modifyDamageParams = ((modifyDamageMethod != null) ? ((MethodBase)modifyDamageMethod).GetParameters() : null);
				if (_modifyDamageMethod == (MethodInfo)null)
				{
					ModLog.Info("DamageTracker: Hook.ModifyDamage not found via reflection");
				}
				else
				{
					ModLog.Info($"DamageTracker: Hook.ModifyDamage resolved ({_modifyDamageParams.Length} params)");
				}
			}
			catch (System.Exception ex)
			{
				ModLog.Error("DamageTracker.InvokeModifyDamage lookup", ex);
			}
		}
		if (_modifyDamageMethod == (MethodInfo)null || _modifyDamageParams == null)
		{
			return damage;
		}
		try
		{
			ParameterInfo[] modifyDamageParams = _modifyDamageParams;
			object[] array = new object[modifyDamageParams.Length];
			for (int num = 0; num < modifyDamageParams.Length; num++)
			{
				string name = modifyDamageParams[num].Name;
				string text = name;
				switch (text)
				{
				case "runState":
					array[num] = runState;
					continue;
				case "combatState":
					array[num] = combatState;
					continue;
				case "target":
					array[num] = target;
					continue;
				case "dealer":
					array[num] = dealer;
					continue;
				case "damage":
					array[num] = damage;
					continue;
				case "props":
					array[num] = ValueProp.Move;
					continue;
				case "modifyDamageHookType":
					array[num] = ModifyDamageHookType.All;
					continue;
				case "previewMode":
					array[num] = CardPreviewMode.None;
					continue;
				}
				System.Type type = modifyDamageParams[num].ParameterType;
				if (type.IsByRef)
				{
					type = type.GetElementType();
				}
				array[num] = (type.IsValueType ? Activator.CreateInstance(type) : null);
			}
			return (((MethodBase)_modifyDamageMethod).Invoke((object)null, array) is decimal num2) ? num2 : damage;
		}
		catch (System.Exception ex2)
		{
			ModLog.Error("DamageTracker.InvokeModifyDamage", ex2);
			return damage;
		}
	}

	private static Font? FindGameFont(Node root)
	{
		try
		{
			System.Collections.Generic.IEnumerator<Node> enumerator = root.GetChildren().GetEnumerator();
			try
			{
				while (((System.Collections.IEnumerator)enumerator).MoveNext())
				{
					Node current = enumerator.Current;
					if (!(current is NCreature nCreature))
					{
						continue;
					}
					Control intentContainer = nCreature.IntentContainer;
					if (intentContainer == null)
					{
						continue;
					}
					System.Collections.Generic.IEnumerator<Node> enumerator2 = intentContainer.GetChildren().GetEnumerator();
					try
					{
						while (((System.Collections.IEnumerator)enumerator2).MoveNext())
						{
							Node current2 = enumerator2.Current;
							if (!(current2 is NIntent))
							{
								continue;
							}
							Control nodeOrNull = current2.GetNodeOrNull<Control>("%Value");
							if (nodeOrNull is RichTextLabel richTextLabel)
							{
								Font themeFont = richTextLabel.GetThemeFont("normal_font");
								if (themeFont != null)
								{
									return themeFont;
								}
							}
							else if (nodeOrNull is Label label)
							{
								Font themeFont2 = label.GetThemeFont("font");
								if (themeFont2 != null)
								{
									return themeFont2;
								}
							}
						}
					}
					finally
					{
						((System.IDisposable)enumerator2)?.Dispose();
					}
				}
			}
			finally
			{
				((System.IDisposable)enumerator)?.Dispose();
			}
			return FindFontRecursive(root, 3);
		}
		catch (System.Exception ex)
		{
			ModLog.Error("FindGameFont", ex);
			return null;
		}
	}

	private static Font? FindFontRecursive(Node node, int depth)
	{
		if (depth <= 0)
		{
			return null;
		}
		System.Collections.Generic.IEnumerator<Node> enumerator = node.GetChildren().GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				Node current = enumerator.Current;
				if (current is Label label)
				{
					Font themeFont = label.GetThemeFont("font");
					if (themeFont != null)
					{
						return themeFont;
					}
				}
				Font font = FindFontRecursive(current, depth - 1);
				if (font != null)
				{
					return font;
				}
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
		return null;
	}
}
