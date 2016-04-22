using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace FedKatarinaV2
{
    internal class StateManager
    {
        public static Menu ComboMenu, HarassMenu, FarmMenu, JungleMenu;
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        public static void Init()
        {
            ComboMenu = Program.menu.AddSubMenu("Combo", "FedSeriesCombo");
            ComboMenu.AddGroupLabel("Combo Settings");

            ComboMenu.AddLabel("Enjoy Holding SpaceBar :3");
            ComboMenu.Add("cQ", new CheckBox("Use Q", true));
            ComboMenu.Add("cW", new CheckBox("Use W", true));
            ComboMenu.Add("cE", new CheckBox("Use E", true));
            ComboMenu.AddLabel("Please Only Check Combo 1 Or 2. Not Both.");
            ComboMenu.Add("combo1", new CheckBox("Use Combo 1", true));
            ComboMenu.Add("combo2", new CheckBox("Use Combo 2", false));
            ComboMenu.Add("cR", new CheckBox("Use Smart R", false));
            ComboMenu.AddLabel("Uncheck Smart R if you just want Regular Ult usage");

            HarassMenu = Program.menu.AddSubMenu("Harass Settings");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.Add("hQ", new CheckBox("Use Q", true));
            HarassMenu.Add("hW", new CheckBox("Use W", false));
            HarassMenu.Add("autoharass", new CheckBox("Auto Harass with Q W?", false));

            FarmMenu = Program.menu.AddSubMenu("Farming Settings");
            FarmMenu.AddGroupLabel("Farm Settings");
            FarmMenu.Add("fQ", new CheckBox("Q to WaveClear"));
            FarmMenu.Add("fW", new CheckBox("W to WaveClear"));
            FarmMenu.AddGroupLabel("LastHit Settings");
            FarmMenu.Add("lQ", new CheckBox("Use Q to Last Hit"));
            FarmMenu.Add("lW", new CheckBox("Use W to Last Hit"));


            JungleMenu = Program.menu.AddSubMenu("Jungle Settings");
            JungleMenu.AddGroupLabel("Jungle Settings");
            JungleMenu.Add("jQ", new CheckBox("Use Q in Jungle"));
            JungleMenu.Add("jW", new CheckBox("Use W in Jungle"));

            Game.OnTick += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (HasRBuff())
            {
                Orbwalker.DisableMovement = true;
                Orbwalker.DisableAttacking = true;
            }
            else
            {
                Orbwalker.DisableMovement = false;
                Orbwalker.DisableAttacking = false;
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) Combo();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)) Harass();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit)) LastHit();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) Jungle();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)) WaveClear();
            if (HarassMenu["autoharass"].Cast<CheckBox>().CurrentValue)
            {
                AutoHarass();
            }
        }
        private static bool HasRBuff()
        {
            return _Player.HasBuff("KatarinaR") || Player.Instance.Spellbook.IsChanneling || _Player.HasBuff("katarinarsound");
        }
  
        public static void LastHit()
        {
            var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(a => a.Distance(Player.Instance) < 1400).OrderBy(a => a.Health);
            var minion =
                minions.FirstOrDefault(
                    a => Extended.BuffedEnemy != null && a.NetworkId == Extended.BuffedEnemy.NetworkId) ??
                minions.FirstOrDefault();
            if (minion == null || !minion.IsValidTarget()) return;
            if (FarmMenu["lQ"].Cast<CheckBox>().CurrentValue && (Damage.QDamage(minion) > minion.Health) && Program.Q.IsReady() && Program.Q.IsInRange(minion))
            {
                Program.Q.Cast(minion);
            }
            if (FarmMenu["lW"].Cast<CheckBox>().CurrentValue && (Damage.WDamage(minion) > minion.Health) && Program.W.IsReady() && Program.W.IsInRange(minion))
            {
                Program.W.Cast();
            }
        }

        public static void WaveClear()
        {
            var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(a => a.Distance(Player.Instance) < 1400).OrderBy(a => a.Health);
            var minion = (Extended.BuffedEnemy != null && Extended.BuffedEnemy.IsValidTarget(1400)) ? Extended.BuffedEnemy : minions.FirstOrDefault();
            if (minion == null) return;
            
            if (FarmMenu["fQ"].Cast<CheckBox>().CurrentValue && (Damage.QDamage(minion) > minion.Health) && Program.Q.IsReady() && Program.Q.IsInRange(minion))
            {
                Program.Q.Cast(minion);
            }
            if (FarmMenu["fW"].Cast<CheckBox>().CurrentValue && (Damage.WDamage(minion) > minion.Health) && Program.W.IsReady() && Program.W.IsInRange(minion))
            {
                Program.W.Cast();
            }
        }

        public static void Jungle()
        {
            var source = EntityManager.MinionsAndMonsters.GetJungleMonsters().OrderByDescending(a => a.MaxHealth).FirstOrDefault(b => b.Distance(Player.Instance) < 1300);
            if (source == null || !source.IsValidTarget()) return;

            if (JungleMenu["jQ"].Cast<CheckBox>().CurrentValue && Program.Q.IsReady() && source.Distance(_Player) < Program.Q.Range)
            {
                Program.Q.Cast(source);
            }
            if (JungleMenu["jW"].Cast<CheckBox>().CurrentValue && Program.W.IsReady() && source.Distance(_Player) < Program.W.Range)
            {
                Program.W.Cast();
            }
        }

        public static void Combo()
        {
            var target = TargetSelector.GetTarget(Program.E.Range, EloBuddy.DamageType.Magical);

            if (ComboMenu["combo1"].Cast<CheckBox>().CurrentValue)
            {
                // Chat.Print("Combo 1");
                if (!target.IsValidTarget(Program.E.Range))
                    return;

                if (Program.Q.IsReady() && _Player.Distance(target.Position) <= Program.Q.Range)
                {
                    Program.Q.Cast(target);
                }

                if (Program.E.IsReady() && _Player.Distance(target.Position) < Program.E.Range && !Program.Q.IsReady())
                {
                    if (ComboMenu["cR"].Cast<CheckBox>().CurrentValue &&
                        _Player.CountEnemiesInRange(500) > 2 &&
                        (!Program.R.IsReady() || !(Program.R.State == SpellState.Surpressed && Program.R.Level > 0)))
                        return;

                    Orbwalker.DisableAttacking = true;
                    Orbwalker.DisableMovement = true;
                    //Program.E.Cast(target);
                    Core.DelayAction(() => Program.E.Cast(target), new Random().Next(WardJumper.WardjumpMenu["checkTime"].Cast<Slider>().CurrentValue));
                }
            }
            else if (ComboMenu["combo2"].Cast<CheckBox>().CurrentValue) //eqw
            {
                // Chat.Print("Combo 2");
                if (Program.E.IsReady() && _Player.Distance(target.Position) < Program.E.Range)
                {
                    if (ComboMenu["cR"].Cast<CheckBox>().CurrentValue &&
                        _Player.CountEnemiesInRange(500) > 2 &&
                        (!Program.R.IsReady() || !(Program.R.State == SpellState.Surpressed && Program.R.Level > 0)))
                        return;


                    Orbwalker.DisableAttacking = true;
                    Orbwalker.DisableMovement = true;
                    //Program.E.Cast(target);
                    Core.DelayAction(() => Program.E.Cast(target), new Random().Next(WardJumper.WardjumpMenu["checkTime"].Cast<Slider>().CurrentValue));
                }

                if (Program.Q.IsReady() && _Player.Distance(target.Position) <= Program.Q.Range)
                {
                    Program.Q.Cast(target);
                }
            }

            if (Program.W.IsReady() && _Player.Distance(target.Position) <= Program.W.Range && Program.Q.IsOnCooldown)
            {
                Program.W.Cast();
            }

            if (Program.R.IsReady() &&
                _Player.CountEnemiesInRange(Program.R.Range) > 0)
            {
                if (!Program.Q.IsReady() && !Program.E.IsReady() && !Program.W.IsReady())
                {
                    Orbwalker.DisableAttacking = true;
                    Orbwalker.DisableMovement = true;
                    Program.R.Cast();
                }
            }
        }

        public static void Harass()
        {
            var target = TargetSelector.GetTarget(Program.Q.Range, EloBuddy.DamageType.Magical);
            if (target.IsValidTarget())
            {
                if (HarassMenu["hQ"].Cast<CheckBox>().CurrentValue && Program.Q.IsReady() && Program.Q.IsInRange(target))
                {
                    Program.Q.Cast(target);
                }
                if (HarassMenu["hW"].Cast<CheckBox>().CurrentValue && Program.W.IsInRange(target) && Program.W.IsReady())
                {

                    Program.W.Cast();

                }
            }
        }

        static void AutoHarass()
        {
            var target = TargetSelector.GetTarget(Program.Q.Range, EloBuddy.DamageType.Magical);
            if (target.IsValidTarget())
            {
                if (Program.Q.IsInRange(target))
                {
                    Program.Q.Cast(target);
                }
                if (Program.W.IsInRange(target))
                {
                    Program.W.Cast();
                }
            }
        }
     }
}