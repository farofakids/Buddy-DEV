using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Rendering;
using SharpDX;


namespace Farofakids_Lucian
{
    class Program
    {
        public static readonly AIHeroClient _Player = (ObjectManager.Player);

        public static Menu Menu, DrawingsMenu;

        public static Spell.Targeted Q;
        public static Spell.Skillshot Q1, W, E, R;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += GameOnOnGameLoad;
        }

        private static void GameOnOnGameLoad(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Lucian") //YOUR CHAMPION NAME
            {
            return;
            }

            // Initialize spells
            Q = new Spell.Targeted(SpellSlot.Q, 675);
            Q1 = new Spell.Skillshot(SpellSlot.Q, 1200, SkillShotType.Linear, 500, int.MaxValue, 50)
            {
                AllowedCollisionCount = int.MaxValue
            }; 
            W = new Spell.Skillshot(SpellSlot.W, 1200, SkillShotType.Linear, 300, 1600, 80)
            {
                AllowedCollisionCount = 0
            };
            E = new Spell.Skillshot(SpellSlot.E, 475, SkillShotType.Linear)
            {
                AllowedCollisionCount = int.MaxValue
            };
            R = new Spell.Skillshot(SpellSlot.R, 1400, SkillShotType.Linear, 200, 2500, 110)
            {
                AllowedCollisionCount = 0
            };

            InitializeMenu();
            DamageIndicator.Initialize(SpellDamage.GetTotalDamage);
            Drawing.OnDraw += OnDraw;
            Interrupter.OnInterruptableSpell += AntiGapcloser_OnEnemyGapcloser;
            Gapcloser.OnGapcloser += OnGapCloser;
            Chat.Print("<font color=\"#7CFC00\"><b>FAROFAKIDS-LUCIAN</b></font>...Loaded"); //your script name
        }

        private static void InitializeMenu()
        {
            Menu = MainMenu.AddMenu("Lucian-Farofakids", "Lucian-Farofakids"); //YOUR NAME MENU

            //comboMenu
            Menu.AddLabel("Combo");
            Menu.Add("QCombo", new CheckBox("Use Q"));
            Menu.Add("WCombo", new CheckBox("Use W"));
            Menu.Add("ECombo", new CheckBox("Use E to mouse", false));
            Menu.Add("RCombo", new CheckBox("Use R", false));

            //harassMenu
            Menu.AddLabel("Harass");
            Menu.Add("HarassQ", new CheckBox("Use Q"));
            Menu.Add("HarassW", new CheckBox("Use W"));
            Menu.Add("HarassE", new CheckBox("Use E to mouse", false));
            Menu.Add("HarassMana", new Slider("[Harass] Minimum Mana", 50, 0, 100));

            //waveClearMenu
            Menu.AddLabel("WaveClear");
            Menu.Add("WaveClearQ", new CheckBox("Use Q"));
            Menu.Add("WaveClearW", new CheckBox("Use W"));
            Menu.Add("WaveClearMana", new Slider("[WaveClear] Minimum Mana", 80, 0, 100));

            //miscMenu
            Menu.AddLabel("Misc");
            Menu.Add("Antigap", new CheckBox("Use E for antigapclosers", false));
            Menu.Add("Gapclose", new CheckBox("Use E for Gapclose", false));
            Menu.Add("FleeE", new CheckBox("Use E for Flee", false));
            Menu.Add("FleeW", new CheckBox("Use W for Flee", false));

            //DrawingsMenu
            DrawingsMenu = Menu.AddSubMenu("Drawings");
            DrawingsMenu.Add("DrawQ", new CheckBox("Draw Q"));
            DrawingsMenu.Add("DrawW", new CheckBox("Draw W"));
            DrawingsMenu.Add("DrawE", new CheckBox("Draw E"));
            DrawingsMenu.Add("DrawR", new CheckBox("Draw R"));
            DrawingsMenu.Add("damageIndicatorDraw", new CheckBox("Draw indicator combo damage"));

            Game.OnTick += Game_OnTick;

        }

        private static void Game_OnTick(EventArgs args) //your orbwalker
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                LaneClear();
                JungleClear();
            }
            else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Flee();
            }

        }

        private static void Flee()
        {
            var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);

            if (Menu["FleeE"].Cast<CheckBox>().CurrentValue && E.IsReady())
            {
                E.Cast((Game.CursorPos));
            }
            if (Menu["FleeW"].Cast<CheckBox>().CurrentValue && W.IsReady() && target.IsValidTarget(W.Range))
            {
                W.Cast(target);
            }

        }

        private static void LaneClear()
        {
            var minion = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position, 500);

            if (Menu["WaveClearMana"].Cast<Slider>().CurrentValue > _Player.ManaPercent) return; //check your mana
            
                if (Menu["WaveClearQ"].Cast<CheckBox>().CurrentValue && Q1.IsReady())
                {
                    var minions =
                        EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                            Player.Instance.Position, Q1.Range);
                    var aiMinions = minions as Obj_AI_Minion[] ?? minions.ToArray();
                    foreach (var m in from m in aiMinions
                                      let p = new Geometry.Polygon.Rectangle((Vector2)Player.Instance.ServerPosition,
Player.Instance.ServerPosition.Extend(m.ServerPosition, Q1.Range), 65)
                                      where aiMinions.Count(x =>
                                          p.IsInside(x.ServerPosition)) >= 3
                                      select m)
                    {
                        Q.Cast(m);
                        break;
                    }
                }
                if (Menu["WaveClearW"].Cast<CheckBox>().CurrentValue && W.IsReady())
                {
                    var minions =
                        EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                            Player.Instance.Position, 500)
                            .FirstOrDefault(x => x.IsValidTarget(500));
                    if (minions != null)
                       W.Cast(minions);
                }

        }

        private static void JungleClear()
        {
            if (Menu["WaveClearMana"].Cast<Slider>().CurrentValue > _Player.ManaPercent) return;  //check your mana

                if (Menu["WaveClearQ"].Cast<CheckBox>().CurrentValue && Q.IsReady())
                {
                var monster = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.ServerPosition, Q.Range).FirstOrDefault(x => x.IsValidTarget(Q.Range));
                if (monster != null) Q.Cast(monster);
                }
                if (Menu["WaveClearW"].Cast<CheckBox>().CurrentValue && W.IsReady())
                {
                var monster = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.ServerPosition, 600).FirstOrDefault(x => x.IsValidTarget());
                if (monster != null) W.Cast(monster.ServerPosition);
                }
        }

        private static void Harass()
        {
            if (_Player.ManaPercent >= Menu["HarassMana"].Cast<Slider>().CurrentValue)
            {
                var targetQ1 = TargetSelector.GetTarget(Q1.Range, DamageType.Physical);
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (Menu["HarassQ"].Cast<CheckBox>().CurrentValue && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    var champs = EntityManager.Heroes.Enemies.Where(m => m.Distance(_Player) <= Q.Range);
                    var predPos = Q1.GetPrediction(targetQ1);
                    var minions =
                    EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.Distance(_Player) <= Q.Range);
                        foreach (var minion in from minion in minions
                                               let polygon = new Geometry.Polygon.Rectangle(
                                                   (Vector2)Player.Instance.ServerPosition,
                                                   Player.Instance.ServerPosition.Extend(minion.ServerPosition, Q1.Range), 65f)
                                               where polygon.IsInside(predPos.CastPosition)
                                               select minion)
                        {
                            Q.Cast(minion);
                        }
                }
                if (Menu["HarassQ"].Cast<CheckBox>().CurrentValue && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    Q.Cast(target);
                }
                if (Menu["HarassE"].Cast<CheckBox>().CurrentValue && E.IsReady() && target.IsValidTarget(E.Range + Q1.Range))
                {
                    E.Cast((Game.CursorPos));
                }
                if (Menu["HarassW"].Cast<CheckBox>().CurrentValue && W.IsReady() && target.IsValidTarget(W.Range))
                   {
                           var targetW = TargetSelector.GetTarget(W.Range, DamageType.Magical);
                           var w = W.GetPrediction(targetW);
                           if (w.HitChance >= HitChance.High)
                           {
                               W.Cast(w.CastPosition);
                           }

                   }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (DrawingsMenu["DrawQ"].Cast<CheckBox>().CurrentValue && Q.IsLearned)
            {
                Circle.Draw(Q.IsReady() ? Color.Blue : Color.Red, Q.Range, Player.Instance);
            }
            if (DrawingsMenu["DrawW"].Cast<CheckBox>().CurrentValue && W.IsLearned)
            {
                Circle.Draw(W.IsReady() ? Color.Blue : Color.Red, W.Range, Player.Instance);
            }
            if (DrawingsMenu["DrawE"].Cast<CheckBox>().CurrentValue && E.IsLearned)
            {
                Circle.Draw(E.IsReady() ? Color.Blue : Color.Red, E.Range, Player.Instance);
            }
            if (DrawingsMenu["DrawR"].Cast<CheckBox>().CurrentValue && R.IsLearned)
            {
                Circle.Draw(R.IsReady() ? Color.Blue : Color.Red, R.Range, Player.Instance);
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (Menu["RCombo"].Cast<CheckBox>().CurrentValue && R.IsReady() && target.IsValidTarget(R.Range))
            {
                var col = Prediction.Position.PredictLinearMissile(target, R.Range, R.Width, R.CastDelay, R.Speed, 0, null,
    true);
                var allies = EntityManager.Heroes.Allies.Count(
    allied => !allied.IsDead && allied.Distance(target) <= 500);
                if ((Q.IsReady() && _Player.Distance(target.ServerPosition) < 400) ||
    ((col.HitChance == HitChance.Collision &&
     col.CollisionObjects.OfType<Obj_AI_Minion>().Count() > 3) ||
     !target.IsValidTarget()) ||
    target.HasBuffOfType(BuffType.Invulnerability) ||
    target.IsZombie ||
    allies > 1)
                    return;
                R.Cast(target);
            }
            if (Menu["ECombo"].Cast<CheckBox>().CurrentValue && E.IsReady() && target.IsValidTarget(E.Range))
            {
                E.Cast((Game.CursorPos));
            }
            if (Menu["QCombo"].Cast<CheckBox>().CurrentValue && Q.IsReady() && target.IsValidTarget(Q.Range))
            {
                Q.Cast(target);
            }
            if (Menu["WCombo"].Cast<CheckBox>().CurrentValue && W.IsReady() && target.IsValidTarget(W.Range))
            {
                W.Cast(target);
            }


        }

        private static void OnGapCloser(Obj_AI_Base sender, Gapcloser.GapcloserEventArgs args)
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (target == null)
            {
                return;
            }
            if (Menu["Gapclose"].Cast<CheckBox>().CurrentValue && E.IsReady() && sender.Distance(_Player) < E.Range)
            {
                E.Cast(target);
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (target == null)
            {
                return;
            }

            if (Menu["Antigap"].Cast<CheckBox>().CurrentValue && E.IsReady() && sender.Distance(_Player) < E.Range)
            {
                E.Cast(Game.CursorPos);
            }
        }

    }
}
