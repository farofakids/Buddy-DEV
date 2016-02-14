using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;

namespace Talon
{
    internal class Program
    {
        public static readonly AIHeroClient _Player = (ObjectManager.Player);

        public static Spell.Active Q = new Spell.Active(SpellSlot.Q, (uint)Player.Instance.GetAutoAttackRange() + 100);
        public static Spell.Skillshot W = new Spell.Skillshot(SpellSlot.W, 650, SkillShotType.Linear, 250, 2300, 75);
        public static Spell.Targeted E = new Spell.Targeted(SpellSlot.E, 700);
        public static Spell.Active R = new Spell.Active(SpellSlot.R, 650);
        private static SpellSlot ignite;
        private static Item Tiamat = new Item(3077, 400f);
        private static Item Hydra = new Item(3074, 0f);
        private static Item Youmuu = new Item(3142, 400f);


        public static Menu Menu, comboMenu, harassMenu, waveClearMenu, miscMenu;

        private static List<Spell.SpellBase> SpellList = new List<Spell.SpellBase> { Q, E, W, R };

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += GameOnOnGameLoad;
        }

        private static void GameOnOnGameLoad(EventArgs args)
        {
            if (_Player.ChampionName != "Talon")
            {
                return;
            }
            InitializeMenu();
            Interrupter.OnInterruptableSpell += AntiGapcloser_OnEnemyGapcloser;
            ignite = _Player.GetSpellSlotFromName("summonerdot");
            DamageIndicator.Initialize(SpellDamage.GetTotalDamage);
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalker.OnPostAttack += AfterAttack;
            Chat.Print("<font color=\"#7CFC00\"><b>FAROFAKIDS-TALON</b></font>...Loaded");
        }

        private static void AfterAttack(AttackableUnit target, EventArgs args)
        {
            switch (Orbwalker.ActiveModesFlags)
            {
                case Orbwalker.ActiveModes.Combo:
                    if (target.IsMe && Q.IsReady() && target is AIHeroClient)
                    {
                        Q.Cast();
                        Orbwalker.ResetAutoAttack();
                    }
                    break;
            }
        }

        private static void InitializeMenu()
        {
            Menu = MainMenu.AddMenu("Talon", "talon");

            comboMenu = Menu.AddSubMenu("Combo");
            comboMenu.Add("QCombo", new CheckBox("Use Q"));
            comboMenu.Add("WCombo", new CheckBox("Use W"));
            comboMenu.Add("ECombo", new CheckBox("Use E"));
            comboMenu.Add("RCombo", new CheckBox("Use R"));
            comboMenu.Add("RWhenKill", new CheckBox("Use R only when killable", false));
            comboMenu.Add("SmartUlt", new CheckBox("Use smart ult", false));
            comboMenu.Add("rcount", new Slider("Min target to R >= ", 1, 1, 5));
            comboMenu.Add("UseIgnite", new CheckBox("Use Ignite in combo when killable"));
            comboMenu.AddGroupLabel("Combo mode");
            comboMenu.AddGroupLabel("1 = Default [E->AA->Q->W->R]   " + "[2 = R->E->W->Q]    " + "[E->R->W->Q]");
            comboMenu.Add("Mode", new Slider("Combo mode", 2, 1, 3));
            comboMenu.Add("UseTiamat", new CheckBox("Use Tiamat"));
            comboMenu.Add("UseHydra", new CheckBox("Use Hydra"));
            comboMenu.Add("UseYoumuu", new CheckBox("Use Youmuu"));

            harassMenu = Menu.AddSubMenu("Harass");
            harassMenu.Add("HarassQ", new CheckBox("Use Q"));
            harassMenu.Add("HarassW", new CheckBox("Use W"));
            harassMenu.Add("HarassE", new CheckBox("Use E", false));
            harassMenu.AddGroupLabel("HarassMana");
            harassMenu.Add("HarassMana", new Slider("[Harass] Minimum Mana", 50, 0, 100));

            waveClearMenu = Menu.AddSubMenu("WaveClear");
            waveClearMenu.Add("WaveClearQ", new CheckBox("Use Q"));
            waveClearMenu.Add("WaveClearW", new CheckBox("Use W"));
            waveClearMenu.Add("WaveClearE", new CheckBox("Use E", false));
            waveClearMenu.AddGroupLabel("Mana");
            waveClearMenu.Add("LaneClearMana", new Slider("[WaveClear] Minimum Mana", 80, 0, 100));
            waveClearMenu.Add("HydraClear", new CheckBox("Use hydra"));
            waveClearMenu.Add("TiamatClear", new CheckBox("Use tiamat"));

            miscMenu = Menu.AddSubMenu("Misc||Drawings", "Misc");
            miscMenu.Add("Drawingsoff", new CheckBox("Drawings off"));
            miscMenu.Add("DrawW", new CheckBox("Draw W"));
            miscMenu.Add("DrawE", new CheckBox("Draw E"));
            miscMenu.Add("DrawR", new CheckBox("Draw R"));
            miscMenu.Add("damageIndicatorDraw", new CheckBox("Draw indicator combo damage"));
            miscMenu.Add("Antigap", new CheckBox("Use E for antigapclosers", false));

            Game.OnTick += Game_OnTick;

        }

        private static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                JungleClear();
                LaneClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
        }

        private static void JungleClear()
        {
            var target =
                          EntityManager.MinionsAndMonsters.GetJungleMonsters()
                          .OrderByDescending(j => j.Health)
                          .FirstOrDefault(j => j.IsValidTarget(700));

            if (_Player.ManaPercent >= waveClearMenu["LaneClearMana"].Cast<Slider>().CurrentValue)
            {
                if (waveClearMenu["WaveClearQ"].Cast<CheckBox>().CurrentValue && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    Q.Cast();
                }
                if (waveClearMenu["WaveClearW"].Cast<CheckBox>().CurrentValue && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.Cast(target);
                }
                if (waveClearMenu["WaveClearE"].Cast<CheckBox>().CurrentValue && E.IsReady() && target.IsValidTarget(E.Range))
                {
                    E.Cast(target);
                }
                if (Item.CanUseItem(3074) && waveClearMenu["HydraClear"].Cast<CheckBox>().CurrentValue && target.IsValidTarget(Hydra.Range))
                {
                    Item.UseItem(3074);
                }
                if (Item.CanUseItem(3077) && waveClearMenu["TiamatClear"].Cast<CheckBox>().CurrentValue && target.IsValidTarget(Tiamat.Range))
                {
                    Item.UseItem(3077);
                }
            }
        }

        private static void LaneClear()
        {
            var minion = EntityManager.MinionsAndMonsters.GetLaneMinions()
                    .OrderByDescending(m => m.Health)
                    .FirstOrDefault(m => m.IsValidTarget(W.Range));
            if (minion == null)
            {
                return;
            }
            var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Count(m => m.IsValidTarget(W.Range));
            if (_Player.ManaPercent >= waveClearMenu["LaneClearMana"].Cast<Slider>().CurrentValue)
            {
                if (waveClearMenu["WaveClearQ"].Cast<CheckBox>().CurrentValue && Q.IsReady() && minion.IsValidTarget(Q.Range))
                {
                    Q.Cast();
                }
                if (waveClearMenu["WaveClearW"].Cast<CheckBox>().CurrentValue && W.IsReady() && minion.IsValidTarget(W.Range))
                {
                    if (minions > 2)
                    {
                        W.Cast(minion);
                    }
                }
                if (waveClearMenu["WaveClearE"].Cast<CheckBox>().CurrentValue && E.IsReady() && minion.IsValidTarget(E.Range))
                {
                    E.Cast(minion);
                }
                if (Item.CanUseItem(3074) && waveClearMenu["HydraClear"].Cast<CheckBox>().CurrentValue && minion.IsValidTarget(Hydra.Range) && minions >= 3)
                {
                    Item.UseItem(3074);
                }
                if (Item.CanUseItem(3077) && waveClearMenu["TiamatClear"].Cast<CheckBox>().CurrentValue && minion.IsValidTarget(Tiamat.Range) && minions >= 3)
                {
                    Item.UseItem(3077);
                }
            }
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            if (target == null || !target.IsValid)
            {
                return;
            }

            foreach (var spell in SpellList.Where(y => y.IsReady()))
            {
                if (_Player.ManaPercent >= harassMenu["HarassMana"].Cast<Slider>().CurrentValue)
                {
                    if (spell.Slot == SpellSlot.Q && harassMenu["HarassQ"].Cast<CheckBox>().CurrentValue && Q.IsReady() && target.IsValidTarget(Q.Range)
        && Q.IsReady())
                    {
                        Q.Cast();
                    }

                    if (spell.Slot == SpellSlot.W && harassMenu["HarassW"].Cast<CheckBox>().CurrentValue && W.IsReady())
                    {
                        var pouput = W.GetPrediction(target);
                        if (pouput.HitChance >= HitChance.High)
                        {
                            W.Cast(pouput.CastPosition);
                        }
                    }

                    if (spell.Slot == SpellSlot.E && harassMenu["HarassE"].Cast<CheckBox>().CurrentValue && E.IsReady())
                    {
                        E.Cast(target);
                    }
                }
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            if (target == null || !target.IsValid)
            {
                return;
            }

            var comboDamage = SpellDamage.GetTotalDamage(target);
            var getUltComboDamage = GetUltComboDamage(target);
            if (comboMenu["Mode"].Cast<Slider>().CurrentValue == 1)
            {
                if (comboMenu["ECombo"].Cast<CheckBox>().CurrentValue && E.IsReady())
                {
                    E.Cast(target);
                }
                FightItems();
                if (Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    Q.Cast();
                }
                if (comboMenu["WCombo"].Cast<CheckBox>().CurrentValue && W.IsReady())
                {
                    W.Cast(target);
                }
                if (comboMenu["RWhenKill"].Cast<CheckBox>().CurrentValue && E.IsReady() && comboMenu["RCombo"].Cast<CheckBox>().CurrentValue && Q.IsReady()
                    && ObjectManager.Get<AIHeroClient>().Count(aiHero => aiHero.IsValidTarget(R.Range)) >= comboMenu["rcount"].Cast<Slider>().CurrentValue)
                {
                    if (comboDamage >= target.Health)
                    {
                        R.Cast();
                    }
                }
                if (comboMenu["RWhenKill"].Cast<CheckBox>().CurrentValue && R.IsReady() && comboMenu["SmartUlt"].Cast<CheckBox>().CurrentValue)
                {
                    if (getUltComboDamage >= target.Health)
                    {
                        R.Cast();
                    }
                }
                if (!comboMenu["RWhenKill"].Cast<CheckBox>().CurrentValue && E.IsReady() && comboMenu["RCombo"].Cast<CheckBox>().CurrentValue && R.IsReady()
                    && ObjectManager.Get<AIHeroClient>().Count(aiHero => aiHero.IsValidTarget(R.Range)) >= comboMenu["rcount"].Cast<Slider>().CurrentValue)
                {
                    R.Cast();
                }
            }
            if (comboMenu["Mode"].Cast<Slider>().CurrentValue == 2)
            {
                if (R.IsReady() && R.IsInRange(target))
                {
                    R.Cast();
                }

                if (comboMenu["ECombo"].Cast<CheckBox>().CurrentValue && E.IsReady())
                {
                    E.Cast(target);
                }

                if (comboMenu["WCombo"].Cast<CheckBox>().CurrentValue && W.IsReady())
                {
                    W.Cast(target);
                }
                if (Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    Q.Cast();
                }
                FightItems();
            }
            if (comboMenu["Mode"].Cast<Slider>().CurrentValue == 3)
            {
                if (comboMenu["ECombo"].Cast<CheckBox>().CurrentValue && E.IsReady())
                {
                    E.Cast(target);
                }

                if (R.IsReady() && R.IsInRange(target))
                {
                    R.Cast();
                }

                if (comboMenu["WCombo"].Cast<CheckBox>().CurrentValue && W.IsReady())
                {
                    W.Cast(target);
                }
                if (Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    Q.Cast();
                }
            }
            if (Item.CanUseItem(3074) && comboMenu["UseYoumuu"].Cast<CheckBox>().CurrentValue && _Player.Distance(target) <= 400f && Youmuu.IsReady())
            {
                Youmuu.Cast();
            }
            if (target.IsValidTarget(600f) && IgniteDamage(target) >= target.Health && comboMenu["UseIgnite"].Cast<CheckBox>().CurrentValue)
            {
                _Player.Spellbook.CastSpell(ignite, target);
            }

        }

        private static float IgniteDamage(AIHeroClient target)
        {
            if (ignite == SpellSlot.Unknown || _Player.Spellbook.CanUseSpell(ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)_Player.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Ignite);
        }

        private static void FightItems()
        {
            var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);

            if (Item.CanUseItem(3074) && comboMenu["UseHydra"].Cast<CheckBox>().CurrentValue && _Player.Distance(target) <= 400f)
            {
                Item.UseItem(3074);
            }

            if (Item.CanUseItem(3077) && comboMenu["UseTiamat"].Cast<CheckBox>().CurrentValue && _Player.Distance(target) <= 400f)
            {
                Item.UseItem(3077);
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            if (target == null)
            {
                return;
            }

            if (miscMenu["Antigap"].Cast<CheckBox>().CurrentValue && E.IsReady() && sender.Distance(_Player) < 700)
            {
                E.Cast(target);
            }
        }

        private static float GetUltComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;

            if (R.IsReady())
            {
                damage += _Player.GetSpellDamage(enemy, SpellSlot.R);
            }

            return (float)damage;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (miscMenu["Drawingsoff"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }
            if (miscMenu["DrawW"].Cast<CheckBox>().CurrentValue)
            {
                if (W.Level > 0)
                {
                    Circle.Draw(Color.Red, W.Range, 1f, Player.Instance);
                }

            }
            if (miscMenu["DrawE"].Cast<CheckBox>().CurrentValue)
            {
                if (E.Level > 0)
                {
                    Circle.Draw(Color.Red, E.Range, 1f, Player.Instance);
                }

            }
            if (miscMenu["DrawR"].Cast<CheckBox>().CurrentValue)
            {
                if (R.Level > 0)
                {
                    Circle.Draw(Color.Red, R.Range, 1f, Player.Instance);
                }

            }
  
        }
    }
}