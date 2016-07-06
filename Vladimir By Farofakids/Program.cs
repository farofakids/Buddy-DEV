using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;

namespace Vladimir_By_Farofakids
{
    class Program
    {
        private static Menu Menu, comboMenu, harassMenu, farmMenu, ksMenu, miscMenu, drawMenu, fleeMenu;
        private static Spell.Targeted Q;
        private static Spell.Active W;
        private static Spell.Active E;
        private static Spell.Skillshot R;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Vladimir") return;

            Q = new Spell.Targeted(SpellSlot.Q, 600);
            W = new Spell.Active(SpellSlot.W, 350);
            E = new Spell.Active(SpellSlot.E, 600);
            R = new Spell.Skillshot(SpellSlot.R, 700, SkillShotType.Circular, 250, 1200, 150);
            R.AllowedCollisionCount = 0; R.MinimumHitChance = HitChance.Medium;

            MENU();
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Chat.Print("Vladimir by Farofakids......Loaded.........");
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnTick += Game_OnTick;
        }

        #region LOADS
        private static void MENU()
        {
            Menu = MainMenu.AddMenu("Vladimir by Farofakids", "Vladimir_by_Farofakids");

            //combo
            comboMenu = Menu.AddSubMenu("Combo Settings", "Combo");
            comboMenu.AddLabel(":: COMBO SETTINGS ::");
            comboMenu.Add("useQ", new CheckBox("Use Q"));
            comboMenu.Add("useW", new CheckBox("Use W", false));
            comboMenu.Add("useE", new CheckBox("Use E"));
            comboMenu.Add("useR", new CheckBox("Use R"));
            comboMenu.Add("smartULTI", new CheckBox("Use Smart Ultimate"));
            comboMenu.Add("countR", new Slider("R when >= target", 2, 1, 5));

            //harass
            harassMenu = Menu.AddSubMenu("Harass Settings", "Harass");
            harassMenu.AddLabel(":: HARASS SETTINGS ::");
            harassMenu.Add("useQ", new CheckBox("Use Q"));
            harassMenu.Add("useE", new CheckBox("Use E"));


            //farm
            farmMenu = Menu.AddSubMenu("Farm Settings", "Farm");
            farmMenu.AddLabel(":: FARM SETTINGS ::");
            farmMenu.Add("useQ", new CheckBox("Use Q"));
            farmMenu.Add("useE", new CheckBox("Use E"));
            farmMenu.Add("countE", new Slider("E when >= minions", 3, 1, 12));

            //ks
            ksMenu = Menu.AddSubMenu("Kill Steal Settings", "KillSteal");
            ksMenu.AddLabel(":: KILL STEAL SETTINGS ::");
            ksMenu.Add("useQ", new CheckBox("Use Q"));
            ksMenu.Add("useW", new CheckBox("Use W"));
            ksMenu.Add("useE", new CheckBox("Use E"));
            ksMenu.Add("useR", new CheckBox("Use R", false));

            //misc
            miscMenu = Menu.AddSubMenu("Misc Settings", "Misc");
            miscMenu.AddLabel(":: MISC SETTINGS ::");
            miscMenu.Add("useWgap", new CheckBox("Use W on enemy gapcloser"));
            miscMenu.Add("autoQ", new Slider("Auto Q when Life% <=",60,0,100));

            //flee
            fleeMenu = Menu.AddSubMenu("Flee Settings", "Flee");
            fleeMenu.AddLabel(":: FLEE SETTINGS ::");
            fleeMenu.Add("useQ", new CheckBox("Use Q"));
            fleeMenu.Add("useW", new CheckBox("Use W"));

            //draw
            drawMenu = Menu.AddSubMenu("Draw Settings", "Draw");
            drawMenu.AddLabel(":: Draw SETTINGS ::");
            drawMenu.Add("useQ", new CheckBox("Use Draw Q"));
            drawMenu.Add("useW", new CheckBox("Use Draw W"));
            drawMenu.Add("useE", new CheckBox("Use Draw E"));
            drawMenu.Add("useR", new CheckBox("Use Draw R"));

        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (drawMenu["useQ"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(Q.IsReady() ? SharpDX.Color.Aqua : SharpDX.Color.Red, Q.Range, Player.Instance.Position);
            }

            if (drawMenu["useW"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(W.IsReady() ? SharpDX.Color.Aqua : SharpDX.Color.Red, W.Range, Player.Instance.Position);
            }

            if (drawMenu["useE"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(E.IsReady() ? SharpDX.Color.Aqua : SharpDX.Color.Red, E.Range, Player.Instance.Position);
            }

            if (drawMenu["useR"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(R.IsReady() ? SharpDX.Color.Aqua : SharpDX.Color.Red, R.Range, Player.Instance.Position);
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) Combo();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)) Harass();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)) LaneClear();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee)) Flee();
            KillSteal();
            AutoQ();
        }

        private static void AutoQ()
        {
            if (Player.Instance.HealthPercent > miscMenu["autoQ"].Cast<Slider>().CurrentValue) return;
            var target = TargetSelector.GetTarget(700, DamageType.Magical);
            var minions =
                  EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position, Q.Range)
                  .FirstOrDefault();
    
            if (target !=null && Q.IsReady()
                 && target.IsValidTarget(Q.Range))
            {
                Q.Cast(target);
            }
            else if (minions != null && Q.IsReady())
            {
                Q.Cast(minions);
            }

        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!e.Sender.IsValidTarget() || e.Sender.Type != Player.Instance.Type || !e.Sender.IsEnemy)
                return;

            if (miscMenu["useWgap"].Cast<CheckBox>().CurrentValue && W.IsReady()
                && e.Sender.Distance(Player.Instance) < W.Range
                && Player.Instance.CountEnemiesInRange(W.Range) >= 1)
            {
                W.Cast();
            }
        }

        private static void AreaOfEffectUltimate()
        {
            if (comboMenu["useR"].Cast<CheckBox>().CurrentValue && R.IsReady())
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                if (target == null)
                {
                    return;
                }

                var hits = EntityManager.Heroes.Enemies.Where(x => x.Distance(target) <= 400).ToList();
                if (
                    hits.Any(
                        hit =>
                        hits.Count >= comboMenu["countR"].Cast<Slider>().CurrentValue))
                {
                    var pred = R.GetPrediction(target);
                    R.Cast(pred.CastPosition);
                    Circle.Draw(SharpDX.Color.Red, 400, pred.CastPosition);
                }
            }
        }
        #endregion LOADS

        #region MODES
        private static void KillSteal()
        {
            var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            if (target == null) return;
            if (ksMenu["useQ"].Cast<CheckBox>().CurrentValue && Q.IsReady()
                && target.IsValidTarget(Q.Range)
                && target.Health < Qdamage(target))
            {
                Q.Cast(target);
            }

            if (ksMenu["useE"].Cast<CheckBox>().CurrentValue && E.IsReady()
                && target.IsValidTarget(E.Range)
                && target.Health < Edamage(target))
            {
                if (Player.Instance.Distance(target) < E.Range)
                {
                    E.Cast();
                }
            }
            if (ksMenu["useW"].Cast<CheckBox>().CurrentValue && W.IsReady()
                && target.IsValidTarget(W.Range)
                && target.Health < Wdamage(target))
            {
                W.Cast();
            }
            if (ksMenu["useR"].Cast<CheckBox>().CurrentValue && R.IsReady()
                 && target.Health < Rdamage(target))
            {
                var pred = R.GetPrediction(target);
                if (pred.HitChance >= HitChance.High)
                {
                    R.Cast(pred.CastPosition);
                }
            }
        }

        private static void Combo()
        {
            try { 
            var target = TargetSelector.GetTarget(700, DamageType.Magical);
            if (target == null) return;


            if (comboMenu["useQ"].Cast<CheckBox>().CurrentValue && Q.IsReady()
               && target.IsValidTarget(Q.Range))
            {
                Q.Cast(target);
            }

            if (comboMenu["useE"].Cast<CheckBox>().CurrentValue && E.IsReady()
                 && target.IsValidTarget(E.Range))
            {
                if (Player.Instance.Distance(target) < E.Range)
                {
                    E.Cast();
                }
            }
            if (comboMenu["useW"].Cast<CheckBox>().CurrentValue && W.IsReady()
                && target.IsValidTarget(W.Range))
            {
                W.Cast();
            }

            if (comboMenu["smartULTI"].Cast<CheckBox>().CurrentValue)
            {
                if (R.IsReady() && target.Health <= DamageToUnit(target) && !target.IsDead)
                {
                    var pred = R.GetPrediction(target);
                    if (pred.HitChance >= HitChance.High)
                    {
                        R.Cast(pred.CastPosition);
                    }
                }
            }
            AreaOfEffectUltimate();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Chat.Print(
                    "<font color='#23ADDB'>Marksman AIO:</font><font color='#E81A0C'> an error ocurred. (Code MENU)</font>");
            }
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(600, DamageType.Magical);
            if (target == null)
            {
                return;
            }
            if (harassMenu["useQ"].Cast<CheckBox>().CurrentValue && Q.IsReady()
                 && target.IsValidTarget(Q.Range))
            {
                Q.Cast(target);
            }

            if (harassMenu["useE"].Cast<CheckBox>().CurrentValue && E.IsReady()
                 && target.IsValidTarget(E.Range))
            {
                if (Player.Instance.Distance(target) < E.Range)
                {
                    E.Cast();
                }
            }
        }

        private static void LaneClear()
        {
            if (farmMenu["useQ"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                var minion =
                    EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position, Q.Range)
                    .FirstOrDefault(
                        x => Qdamage(x) > x.Health);

                if (minion != null)
                {

                    Q.Cast(minion);
                }
            }
            if (farmMenu["useE"].Cast<CheckBox>().CurrentValue && E.IsReady())
            {
                var minion =
                    EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position, E.Range)
                    .OrderBy(x => x.MaxHealth);
                if (minion.Count() <= 0)
                {
                    return;
                }

                if (minion.Count() > farmMenu["countE"].Cast<Slider>().CurrentValue)
                {
                    E.Cast();
                }
            }
        }

        private static void Flee()
        {
            var target = TargetSelector.GetTarget(700, DamageType.Magical);
            if (target == null) return;


            if (fleeMenu["useQ"].Cast<CheckBox>().CurrentValue && Q.IsReady()
               && target.IsValidTarget(Q.Range))
            {
                Q.Cast(target);
            }

            if (fleeMenu["useW"].Cast<CheckBox>().CurrentValue && W.IsReady()
               && target.IsValidTarget(W.Range))
            {
                W.Cast();
            }
        }
        #endregion MODES

        #region DAMAGE

        private static float Qdamage(Obj_AI_Base hero)
        {
            return Player.Instance.CalculateDamageOnUnit(hero, DamageType.Magical,
             new [] { 0, 80, 100, 120, 140, 160 }[Q.Level] + (Player.Instance.TotalMagicalDamage * 0.45f));
        }

        private static float Wdamage(Obj_AI_Base hero)
        {
            return Player.Instance.CalculateDamageOnUnit(hero, DamageType.Magical,
             new[] { 0, 80, 135, 190, 245, 300 }[W.Level] + (Player.Instance.Health * 0.15f));
        }

        private static float Edamage(Obj_AI_Base hero)
        {
            return Player.Instance.CalculateDamageOnUnit(hero, DamageType.Magical,
             new [] { 0, 30, 45, 60, 75, 90 }[E.Level] + (Player.Instance.TotalMagicalDamage * 0.35f));
        }

        private static float Rdamage(Obj_AI_Base hero)
        {
            return Player.Instance.CalculateDamageOnUnit(hero, DamageType.Magical,
             new [] { 0, 150, 250, 350 }[R.Level] + (Player.Instance.TotalMagicalDamage * 0.7f));
        }

        private static float DamageToUnit(Obj_AI_Base target)
        {
            var totaldamage = 0f;

            if (target == null) return totaldamage;

            if (Q.IsReady())
            {
                totaldamage += Qdamage(target);
            }

            if (W.IsReady())
            {
                totaldamage = Wdamage(target);
            }

            if (E.IsReady())
            {
                totaldamage += Edamage(target);
            }

            if (R.IsReady())
            {
                totaldamage += Rdamage(target);
            }

            return totaldamage;
        }
        #endregion DAMAGE

    }
}
