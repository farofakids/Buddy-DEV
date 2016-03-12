using System;
using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using System.Linq;

namespace FedKatarinaV2
{
    internal class Program
    {
                //Spells
        public static Spell.Targeted Q;
        public static Spell.Active W;
        public static Spell.Targeted E;
        public static Spell.Active R;
        public static Menu menu, DrawingMenu, KillStealMenu;

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        private static void Main(string[] args)
        {
            if (args != null)
            {
                try
                {
                    Loading.OnLoadingComplete += Load_OnLoad;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private static void Load_OnLoad(EventArgs a)
        {
            if (Player.Instance.Hero != Champion.Katarina) return;

            menu = MainMenu.AddMenu("FedKatarinaV2", "FedSeries");
            menu.AddGroupLabel("Fed KatarinaV2");
            menu.AddLabel("Version: " + "1.0.0.0");
            menu.AddSeparator();
            menu.AddLabel("MostlyPride");
            menu.AddSeparator();
            menu.AddLabel("+Rep If you use this :)");

            DrawingMenu = menu.AddSubMenu("Drawing", "FedSeriesDrawings");
            DrawingMenu.AddGroupLabel("Drawing Settings");
            DrawingMenu.Add("dQ", new CheckBox("Draw Q", true));
            DrawingMenu.Add("dW", new CheckBox("Draw W", true));
            DrawingMenu.Add("dE", new CheckBox("Draw E", true));
            DrawingMenu.Add("dR", new CheckBox("Draw R", true));
            DrawingMenu.AddSeparator();
            DrawingMenu.AddGroupLabel("Enemy Damage Indicator Settings");
            DrawingMenu.Add("draw.enemyDmg", new CheckBox("Draw damage on enemy healthbar"));



            KillStealMenu = menu.AddSubMenu("KillSteal", "FedSeriesKillSteal");
            KillStealMenu.AddGroupLabel("KillSteal Settings");
            KillStealMenu.Add("kQ", new CheckBox("Use Q", true));
            KillStealMenu.Add("kW", new CheckBox("Use W", true));
            KillStealMenu.Add("kE", new CheckBox("Use E", true));

            Q = new Spell.Targeted(SpellSlot.Q, 675);

            W = new Spell.Active(SpellSlot.W, 375);

            E = new Spell.Targeted(SpellSlot.E, 700);

            R = new Spell.Active(SpellSlot.R, 550);

            DamageIndicator.DamageToUnit = GetActualRawComboDamage;
            Drawing.OnDraw += Drawing_OnDraw;
            //SupaKS.Init();
            StateManager.Init();
            WardJumper.Init();
            Game.OnTick += Game_OnTick;

            Chat.Print("FedKatarinaV2 Loaded!", System.Drawing.Color.LightBlue);
        }

        private static void Game_OnTick(EventArgs args)
        {
            KillSteal();
        }

        public static void KillSteal()
        {
              foreach (var enemy in EntityManager.Heroes.Enemies.Where(a => !a.IsDead && !a.IsZombie && a.Health > 0))
            {
                if (enemy.IsValidTarget(E.Range) && enemy.HealthPercent <= 40)
                {

                    if (_Player.GetSpellDamage(enemy, SpellSlot.Q) + _Player.GetSpellDamage(enemy, SpellSlot.W) + _Player.GetSpellDamage(enemy, SpellSlot.E) >= enemy.Health)
                    {
                        if (KillStealMenu["kQ"].Cast<CheckBox>().CurrentValue && (_Player.GetSpellDamage(enemy, Q.Slot) >= enemy.Health) && Q.IsInRange(enemy) && Q.IsReady())
                        { Q.Cast(enemy); }
                        if (KillStealMenu["kW"].Cast<CheckBox>().CurrentValue && (_Player.GetSpellDamage(enemy, W.Slot) >= enemy.Health) && W.IsInRange(enemy) && W.IsReady())
                        { W.Cast(); }
                        if (KillStealMenu["kE"].Cast<CheckBox>().CurrentValue && (_Player.GetSpellDamage(enemy, E.Slot) >= enemy.Health) && E.IsInRange(enemy) && E.IsReady())
                        { //E.Cast(enemy); 
                            Core.DelayAction(() => Program.E.Cast(enemy), new Random().Next(WardJumper.WardjumpMenu["checkTime"].Cast<Slider>().CurrentValue));
                        }
                    }

                }
            }

        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (DrawingMenu["dQ"].Cast<CheckBox>().CurrentValue)
            {
               Circle.Draw(Q.IsReady() ? Color.Green : Color.Red, Q.Range, Player.Instance.Position); 
            }
            if (DrawingMenu["dW"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(W.IsReady() ? Color.Green : Color.Red, W.Range, Player.Instance.Position);
            }
            if (DrawingMenu["dE"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(E.IsReady() ? Color.Green : Color.Red, E.Range, Player.Instance.Position);
            }
            if (DrawingMenu["dR"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(R.IsReady() ? Color.Green : Color.Red, R.Range, Player.Instance.Position);
            }
        }

        #region Damages

        #region BaseDamages

        private static readonly float[] QDamage = { 0, 60, 85, 110, 135, 160 };
        private static readonly float[] BonusQDamage = { 0, 15, 30, 45, 60, 75 };
        private static readonly float[] WDamage = { 0, 40, 75, 110, 145, 180 };
        private static readonly float[] EDamage = { 0, 40, 70, 100, 130, 160 };
        private static readonly float[] RDamage = { 0, 350, 550, 750 };

        #endregion

        #region GetSpellDamage

        private static float GetSpellDamage(SpellSlot slot)
        {
            try
            {
                var qbasedamage = QDamage[Q.Level];
                var wbasedamage = WDamage[W.Level];
                var ebasedamage = EDamage[E.Level];
                var rbasedamage = RDamage[R.Level];

                var qbonusdamage = 45f / 100f * Player.Instance.FlatMagicDamageMod;
                var wbonusdamage = 25f / 100f * Player.Instance.FlatMagicDamageMod;
                var ebonusdamage = 25f / 100f * Player.Instance.FlatMagicDamageMod;
                var rbonusdamage = 25f / 100f * Player.Instance.FlatMagicDamageMod;

                if (slot == SpellSlot.Q)
                    return qbasedamage + qbonusdamage +
                           (BonusQDamage[Q.Level] + 15f / 100f * Player.Instance.FlatMagicDamageMod);
                if (slot == SpellSlot.W)
                    return wbasedamage + wbonusdamage + 60f / 100f * Player.Instance.FlatPhysicalDamageMod;
                if (slot == SpellSlot.E)
                    return ebasedamage + ebonusdamage;
                if (slot == SpellSlot.R)
                    return rbasedamage + rbonusdamage + 375f / 1000f * Player.Instance.FlatPhysicalDamageMod;

                //if (raw)
                //return Player.Instance.CalculateDamageOnUnit(target, DamageTyp_e.Magical, damage, true, true);
                return 0f;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Chat.Print(
                    "<font color='#23ADDB'>Marksman AIO:</font><font color='#E81A0C'> an error ocurred. (Code GETSPELLDAMAGE)</font>");
                return 0f;
            }
        }

        #endregion

        #region RawComboDamage

        private static float GetActualRawComboDamage(Obj_AI_Base enemy)
        {
            try
            {
                var damage = 0f;

                var spells = new List<SpellSlot>();
                spells.Add(SpellSlot.Q);
                spells.Add(SpellSlot.W);
                spells.Add(SpellSlot.E);
                spells.Add(SpellSlot.R);
                foreach (
                    var spell in
                        spells.Where(
                            s => Player.Instance.Spellbook.CanUseSpell(s) == SpellState.Ready && s != SpellSlot.R))
                {
                    if (Player.Instance.Spellbook.CanUseSpell(spell) == SpellState.Ready)
                        damage += GetSpellDamage(spell);
                }
                if (Player.Instance.Spellbook.CanUseSpell(SpellSlot.R) != SpellState.Cooldown &&
                    Player.Instance.Spellbook.CanUseSpell(SpellSlot.R) != SpellState.NotLearned)
                    damage += GetSpellDamage(SpellSlot.R);
                return damage;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Chat.Print(
                    "<font color='#23ADDB'>Marksman AIO:</font><font color='#E81A0C'> an error ocurred. (Code GETACTUALRAWCOMBODAMAGE)</font>");
                return 0f;
            }
        }

        #endregion

        #endregion Damages
    }
}