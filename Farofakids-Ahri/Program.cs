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

namespace Farofakids_Ahri
{
    class Program
    {
        public static readonly AIHeroClient _Player = (ObjectManager.Player);

        public static Menu Menu, DrawingsMenu;

        public static Spell.Skillshot Q, E, R;
        public static Spell.Active W;

        private static Vector3 MousePos // flee to mouse
        {
            get { return Game.CursorPos; }
        }

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += GameOnOnGameLoad;
        }

        private static void GameOnOnGameLoad(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Ahri") //YOUR CHAMPION NAME
            {
            return;
            }

            // Initialize spells
            Q = new Spell.Skillshot(SpellSlot.Q, 870, SkillShotType.Linear, 250, 1550, 90)
            {
                AllowedCollisionCount = int.MaxValue
            };
            W = new Spell.Active(SpellSlot.W, 580);
            E = new Spell.Skillshot(SpellSlot.E, 700, SkillShotType.Linear, 200, 1550, 70)
            {
                AllowedCollisionCount = 0
            };
            R = new Spell.Skillshot(SpellSlot.R, 600, SkillShotType.Circular)
            {
                AllowedCollisionCount = int.MaxValue
            };

            InitializeMenu();
            DamageIndicator.Initialize(SpellDamage.GetTotalDamage);
            Drawing.OnDraw += OnDraw;
            Interrupter.OnInterruptableSpell += AntiGapcloser_OnEnemyGapcloser;
            Gapcloser.OnGapcloser += OnGapCloser;
            Chat.Print("<font color=\"#7CFC00\"><b>YOUR SCRIPT YOUR NAME</b></font>...Loaded"); //your script name
        }

        private static void InitializeMenu()
        {
            Menu = MainMenu.AddMenu("Ahri-Farofakids", "Ahri-Farofakids"); //YOUR NAME MENU

            //comboMenu
            Menu.AddLabel("Combo");
            Menu.Add("QCombo", new CheckBox("Use Q"));
            Menu.Add("WCombo", new CheckBox("Use W"));
            Menu.Add("ECombo", new CheckBox("Use E"));
            Menu.Add("RCombo", new CheckBox("Use R"));

            //harassMenu
            Menu.AddLabel("Harass");
            Menu.Add("HarassQ", new CheckBox("Use Q"));
            Menu.Add("HarassW", new CheckBox("Use W"));
            Menu.Add("HarassE", new CheckBox("Use E", false));
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
            Menu.Add("FleeQ", new CheckBox("Use Q for Flee", false));
            Menu.Add("FleeR", new CheckBox("Use R for Flee", false));

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
            else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear();
                JungleClear();

            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Flee();
            }



        }

        private static void Flee()
        {
            if (Menu["FleeQ"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                Q.Cast(MousePos);
            }
            if (Menu["FleeR"].Cast<CheckBox>().CurrentValue && R.IsReady())
            {
                R.Cast(MousePos);
            }
        }

        private static void LaneClear()
        {
            var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, _Player.ServerPosition, Q.Range, false).ToArray();
            if (minions.Length == 0)
            {
                return;
            }

            if (Menu["WaveClearMana"].Cast<Slider>().CurrentValue > _Player.ManaPercent) return; //check your mana
            
                if (Menu["WaveClearQ"].Cast<CheckBox>().CurrentValue && Q.IsReady())
                {
                    if (minions.Length >= 3)
                    {
                        var farmLocation = EntityManager.MinionsAndMonsters.GetLineFarmLocation(minions, Q.Width, (int)Q.Range);
                        if (EntityManager.MinionsAndMonsters.GetLineFarmLocation(minions, Q.Width, (int)Q.Range).HitNumber >= 3)
                            if (farmLocation.HitNumber >= 3)
                            {
                                if (Q.Cast(farmLocation.CastPosition))
                                {
                                    return;
                                }
                            }
                    }
                }
                if (Menu["WaveClearW"].Cast<CheckBox>().CurrentValue && W.IsReady())
                {
                    if (minions.Length >= 3)
                    {
                        W.Cast();
                    }
                }

        }

        private static void JungleClear()
        {
            var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters(_Player.ServerPosition, W.Range, false).ToArray();
            if (minions.Length == 0)
            {
                return;
            }

            if (Menu["WaveClearMana"].Cast<Slider>().CurrentValue > _Player.ManaPercent) return;  //check your mana

                if (Menu["WaveClearQ"].Cast<CheckBox>().CurrentValue && Q.IsReady())
                {
                    if (minions.Length >= 2)
                    {
                        var farmLocation = EntityManager.MinionsAndMonsters.GetLineFarmLocation(minions, Q.Width, (int)Q.Range);
                        if (EntityManager.MinionsAndMonsters.GetLineFarmLocation(minions, Q.Width, (int)Q.Range).HitNumber >= 2)
                            if (farmLocation.HitNumber >= 2)
                            {
                                if (Q.Cast(farmLocation.CastPosition))
                                {
                                    return;
                                }
                            }
                    }
                }
                if (Menu["WaveClearW"].Cast<CheckBox>().CurrentValue && W.IsReady())
                {
                    if (minions.Length >= 2)
                    {
                        W.Cast();
                    }
                }
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (_Player.ManaPercent >= Menu["HarassMana"].Cast<Slider>().CurrentValue)
            {
                   if (Menu["HarassQ"].Cast<CheckBox>().CurrentValue && Q.IsReady() && target.IsValidTarget(Q.Range))
                   {
                       var q = Q.GetPrediction(target);
                       if (q.HitChance >= HitChance.High)
                       {
                           Q.Cast(q.CastPosition);
                       }
                   }
                   if (Menu["HarassE"].Cast<CheckBox>().CurrentValue && E.IsReady() && target.IsValidTarget(E.Range))
                   {
                       var e = E.GetPrediction(target);
                       if (e.HitChance >= HitChance.High)
                       {
                           E.Cast(e.CastPosition);
                       }
                   }
                   if (Menu["HarassW"].Cast<CheckBox>().CurrentValue && W.IsReady() && target.IsValidTarget(W.Range))
                   {
                           W.Cast();

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
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);

            if (Menu["RCombo"].Cast<CheckBox>().CurrentValue && R.IsReady() && target.IsValidTarget(R.Range))
            {
                R.Cast(target);
            }
            if (Menu["ECombo"].Cast<CheckBox>().CurrentValue && E.IsReady() && target.IsValidTarget(E.Range))
            {
                E.Cast(target);
            }
            if (Menu["QCombo"].Cast<CheckBox>().CurrentValue && Q.IsReady() && target.IsValidTarget(Q.Range))
            {
                Q.Cast(target);
            }
            if (Menu["WCombo"].Cast<CheckBox>().CurrentValue && W.IsReady() && target.IsValidTarget(W.Range))
            {
                W.Cast();
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
                E.Cast(target);
            }
        }

    }
}
