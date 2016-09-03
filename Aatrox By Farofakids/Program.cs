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

namespace Aatrox_By_Farofakids
{
    class Program
    {
        private static Menu Menu, comboMenu, harassMenu, farmMenu, ksMenu, miscMenu, drawMenu, fleeMenu;
        private static Spell.Skillshot Q, Q2;
        private static Spell.Active W;
        private static Spell.Skillshot E;
        private static Spell.Active R;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Aatrox") return;

            Q = new Spell.Skillshot(SpellSlot.Q, 650, SkillShotType.Circular, 600, 2000, 250);
            Q2 = new Spell.Skillshot(SpellSlot.Q, 650, SkillShotType.Circular, 600, 2000, 150);
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Skillshot(SpellSlot.E, 1000, SkillShotType.Linear, 250, 1250, 35);
            R = new Spell.Active(SpellSlot.R, 550);
            Q.AllowedCollisionCount = 0; Q.MinimumHitChance = HitChance.Medium;
            Q2.AllowedCollisionCount = 0; Q2.MinimumHitChance = HitChance.Medium;
            E.AllowedCollisionCount = 0; E.MinimumHitChance = HitChance.Medium;

            MENU();
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Chat.Print("Aatrox by Farofakids......Loaded.........");
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnTick += Game_OnTick;
        }

        #region LOADS
        private static void MENU()
        {
            Menu = MainMenu.AddMenu("Aatrox by Farofakids", "Aatrox_by_Farofakids");

            //combo
            comboMenu = Menu.AddSubMenu("Combo Settings", "Combo");
            comboMenu.AddLabel(":: COMBO SETTINGS ::");
            comboMenu.Add("useQ", new CheckBox("Use Q"));
            comboMenu.Add("useW", new CheckBox("Use W"));
            comboMenu.Add("WHpU", new Slider("-> Switch To Heal If Hp <", 50));
            comboMenu.Add("useE", new CheckBox("Use E"));
            comboMenu.Add("useR", new CheckBox("Use R SMART"));
            comboMenu.Add("RHpU", new Slider("-> If Enemy Hp <", 60));
            comboMenu.Add("countR", new Slider("R when >= target", 2, 1, 5));

            //harass
            harassMenu = Menu.AddSubMenu("Harass Settings", "Harass");
            harassMenu.AddLabel(":: HARASS SETTINGS ::");
            harassMenu.Add("useQ", new CheckBox("Use Q"));
            harassMenu.Add("QHpA", new Slider("->If Hp >= ", 20));
            harassMenu.Add("useE", new CheckBox("Use E"));
            harassMenu.Add("AutoE", new CheckBox("Auto E"));
            harassMenu.Add("AutoEHpA", new Slider("-> If Hp >=", 50));
/*
            //farm
            farmMenu = Menu.AddSubMenu("Farm Settings", "Farm");
            farmMenu.AddLabel(":: FARM SETTINGS ::");
            farmMenu.Add("useQ", new CheckBox("Use Q"));
            farmMenu.Add("useE", new CheckBox("Use E"));
            farmMenu.Add("countE", new Slider("E when >= minions", 3, 1, 12));
*/
            //ks
            ksMenu = Menu.AddSubMenu("Kill Steal Settings", "KillSteal");
            ksMenu.AddLabel(":: KILL STEAL SETTINGS ::");
            ksMenu.Add("useQ", new CheckBox("Use Q"));
            ksMenu.Add("useE", new CheckBox("Use E"));
            ksMenu.Add("useR", new CheckBox("Use R", false));

            //misc
            miscMenu = Menu.AddSubMenu("Misc Settings", "Misc");
            miscMenu.AddLabel(":: MISC SETTINGS ::");
            miscMenu.Add("useQgap", new CheckBox("Use Q on enemy gapcloser"));
            miscMenu.Add("useQinterrupt", new CheckBox("Use Q on enemy skills")); ;

            //flee
            fleeMenu = Menu.AddSubMenu("Flee Settings", "Flee");
            fleeMenu.AddLabel(":: FLEE SETTINGS ::");
            fleeMenu.Add("useQ", new CheckBox("Use Q"));
            fleeMenu.Add("useE", new CheckBox("Use E"));

            //draw
            drawMenu = Menu.AddSubMenu("Draw Settings", "Draw");
            drawMenu.AddLabel(":: Draw SETTINGS ::");
            drawMenu.Add("useQ", new CheckBox("Use Draw Q"));
            drawMenu.Add("useE", new CheckBox("Use Draw E"));
            drawMenu.Add("useR", new CheckBox("Use Draw R"));

        }

        private static bool HaveWDmg
        {
            get { return Player.Instance.HasBuff("AatroxWPower"); }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (drawMenu["useQ"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(Q.IsReady() ? SharpDX.Color.Aqua : SharpDX.Color.Red, Q.Range, Player.Instance.Position);
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
            //if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)) LaneClear();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee)) Flee();
         //   KillSteal();
            AutoE();
        }

        private static void AutoE()
        {
            if (!harassMenu["autoE"].Cast<CheckBox>().CurrentValue ||
                Player.Instance.HealthPercent < harassMenu["AutoEHpA"].Cast<Slider>().CurrentValue)
            {
                return;
            }
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            E.CastMinimumHitchance(target, HitChance.Medium);
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (Player.Instance.IsDead || !e.Sender.IsValidTarget() || e.Sender.Type != Player.Instance.Type
                || !e.Sender.IsEnemy || !miscMenu["useQgap"].Cast<CheckBox>().CurrentValue ||
                 !Q.IsReady())
                return;
            Q2.Cast(e.Sender);
        }

        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (Player.Instance.IsDead || !miscMenu["useQinterrupt"].Cast<CheckBox>().CurrentValue ||
                !Q.IsReady() || e.DangerLevel <= DangerLevel.Medium)
            {
                return;
            }
            Q2.Cast(sender);
        }

        #endregion LOADS

        #region MODES
        private static void KillSteal()
        {
            if (ksMenu["useQ"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                var target = Q.GetTarget();
                if (target != null && target.Health < Qdamage(target))
                {
                    Q2.Cast(target);
                    return;
                }
            }

            if (ksMenu["useE"].Cast<CheckBox>().CurrentValue && E.IsReady())
            {
                var target = E.GetTarget();
                if (target != null && target.Health < Edamage(target))
                {
                    E.CastMinimumHitchance(target, HitChance.Medium);
                    return;
                }
            }

            if (ksMenu["useR"].Cast<CheckBox>().CurrentValue && E.IsReady())
            {
                var target = R.GetTarget();
                if (target != null && target.Health < Rdamage(target))
                {
                    R.Cast();
                    return;
                }
            }
        }

        private static void Combo()
        {
            try {
                var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                if (target == null) return;

                if (comboMenu["useQ"].Cast<CheckBox>().CurrentValue && Q.IsReady()
                   && target.IsValidTarget(Q.Range))
                {
                    Q.CastMinimumHitchance(target, HitChance.Medium);
                }

                if (comboMenu["useE"].Cast<CheckBox>().CurrentValue && E.IsReady()
                     && target.IsValidTarget(E.Range))
                {
                    E.CastMinimumHitchance(target, HitChance.Medium);
                }
                if (comboMenu["useW"].Cast<CheckBox>().CurrentValue && W.IsReady())
                {
                    if (Player.Instance.HealthPercent >= comboMenu["WHpU"].Cast<Slider>().CurrentValue)
                    {
                        if (!HaveWDmg)
                        {
                            W.Cast();
                        }
                    }
                    else if (HaveWDmg)
                    {
                        W.Cast();
                    }
                }

            if (comboMenu["useR"].Cast<CheckBox>().CurrentValue && R.IsReady() && !Player.Instance.IsDashing())
            {
                    if ((target.CountAlliesInRange(400) > 1 && target.Health < DamageToUnit(target)) ||
                         target.HealthPercent < comboMenu["RHpU"].Cast<Slider>().CurrentValue ||
                         target.CountEnemiesInRange(400) >= comboMenu["countR"].Cast<Slider>().CurrentValue)
                    {
                        R.Cast();
                    }
                }
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
             && Player.Instance.HealthPercent >= harassMenu["QHpA"].Cast<Slider>().CurrentValue
               && target.IsValidTarget(Q.Range))
            {
                Q.CastMinimumHitchance(target, HitChance.Medium);
            }

            if (harassMenu["useE"].Cast<CheckBox>().CurrentValue && E.IsReady()
                 && target.IsValidTarget(E.Range))
            {
                E.CastMinimumHitchance(target, HitChance.Medium);
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
            var target = TargetSelector.GetTarget(E.Range, DamageType.Mixed);
            if (target == null) return;
            if (fleeMenu["useQ"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                Q.Cast(Game.CursorPos);
            }
            if (fleeMenu["useE"].Cast<CheckBox>().CurrentValue && E.IsReady()
               && target.IsValidTarget(E.Range))
            {
                E.Cast(target);
            }
        }
        #endregion MODES

        #region DAMAGE

        private static float Qdamage(Obj_AI_Base hero)
        {
            return Player.Instance.CalculateDamageOnUnit(hero, DamageType.Magical,
             new [] { 0, 70, 115, 160, 205, 250 }[Q.Level] + (Player.Instance.TotalAttackDamage * 0.60f));
        }

        private static float Wdamage(Obj_AI_Base hero)
        {
            return Player.Instance.CalculateDamageOnUnit(hero, DamageType.Magical,
             new[] { 0, 20, 25, 30, 35, 40 }[W.Level] + (Player.Instance.TotalAttackDamage * 0.25f));
        }

        private static float WdamageA(Obj_AI_Base hero)
        {
            return Player.Instance.CalculateDamageOnUnit(hero, DamageType.Magical,
             new[] { 0, 60, 95, 130, 165, 200 }[W.Level] + (Player.Instance.TotalAttackDamage * 1f));
        }

        private static float Edamage(Obj_AI_Base hero)
        {
            return Player.Instance.CalculateDamageOnUnit(hero, DamageType.Magical,
             new [] { 0, 75, 110, 145, 180, 215 }[E.Level] + (Player.Instance.TotalMagicalDamage + Player.Instance.TotalAttackDamage * 0.6f));
        }

        private static float Rdamage(Obj_AI_Base hero)
        {
            return Player.Instance.CalculateDamageOnUnit(hero, DamageType.Magical,
             new [] { 0, 200, 300, 400 }[R.Level] + (Player.Instance.TotalMagicalDamage * 1f));
        }

        private static float DamageToUnit(Obj_AI_Base target)
        {
            var totaldamage = 0f;

            if (target == null) return totaldamage;

            if (Q.IsReady())
            {
                totaldamage += Qdamage(target);
            }

            if (W.IsReady() & !HaveWDmg)
            {
                totaldamage = Wdamage(target);
            }
            else
            {
                totaldamage = WdamageA(target);
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
