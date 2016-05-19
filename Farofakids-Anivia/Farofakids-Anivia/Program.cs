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
using SharpDX;

namespace Farofakids_Anivia
{
    class Program
    {
        public static Menu Config;
        private static Spell.Targeted E;
        private static Spell.Skillshot Q, W, R;
        private static Vector3 RlastCast;
        
        private static bool castedForChampion = false;
        private static bool castedForMinions = false;
        private static AIHeroClient castedOn = null;
        private static MissileClient missile = null;
        static Vector2 perfectCast = new Vector2(0, 0);

        public static void Main(string[] args)
        {
            Loading.OnLoadingComplete += GameOnOnGameLoad;
        }

        private static void GameOnOnGameLoad(EventArgs args)
        {
            if (Player.Instance.BaseSkinName != "Anivia")
            {
                return;
            }
            Q = new Spell.Skillshot(SpellSlot.Q, 1000, SkillShotType.Linear, 250, 870, 110);
            W = new Spell.Skillshot(SpellSlot.W, 950, SkillShotType.Linear, 60, int.MaxValue, 1);
            E = new Spell.Targeted(SpellSlot.E, 650);
            R = new Spell.Skillshot(SpellSlot.R, 685, SkillShotType.Circular, 20, int.MaxValue, 400);

            CreateMenu();

            Game.OnUpdate += GameOnOnUpdate;
           // Dash.OnDash += Dash_OnDash;

            missileTracker();
        }

        private static void missileTracker()
        {
            if (missile == null || missile.IsDead)
            {
                missile = null;
                return;
            }
            Vector2 posi = missile.Position.Extend(missile.EndPosition, Q.Speed * (Game.Ping / 500));
            if (!castedForChampion && !castedForMinions)
            {
                foreach (var e in EntityManager.Heroes.Enemies.Where(t => Prediction.Position.Collision.LinearMissileCollision(t, missile.StartPosition.To2D(), missile.StartPosition.Extend(missile.EndPosition, Q.Range), Q.Speed, Q.Width, Q.CastDelay)).OrderBy(t => t.MaxHealth))
                {
                    castedOn = e;
                    castedForChampion = true;
                    break;
                }
                if (!castedForChampion)
                    castedForMinions = true;
            }
            if (castedForChampion)
            {
                if (castedOn == null)
                    return;

                if (Prediction.Position.Collision.LinearMissileCollision(castedOn, missile.StartPosition.To2D(), missile.StartPosition.Extend(missile.EndPosition, Q.Range), Q.Speed, 150, Q.CastDelay))
                {
                    if (posi.Distance(castedOn) < 150)
                    {
                        Q.Cast(castedOn);
                        missile = null;
                        castedForChampion = false;
                        castedForMinions = false;
                        castedOn = null;
                        perfectCast = new Vector2(0, 0);
                    }
                }
                if (missile != null && missile.Position.CountEnemiesInRange(150) > 0)
                {
                    Q.Cast(castedOn);
                    missile = null;
                    castedForChampion = false;
                    castedForMinions = false;
                    castedOn = null;
                    perfectCast = new Vector2(0, 0);
                }
            }
           /* else
            {
                //Casted for farming issues
                Vector2 pos = posi;
                int count = 0;
                int i = 0;
                while (pos.Distance(missile.EndPosition) > Settings.accuracyQ && !missile.IsDead)
                {
                    int amount = 0;
                    foreach (var e in EntityManager.MinionsAndMonsters.CombinedAttackable.Where(t => t.IsInRange(pos, 150) && t.HealthPercent > 0))
                    {
                        amount++;
                    }
                    if (amount >= count)
                    {
                        perfectCast = pos;
                        count = amount;
                    }
                    i++;
                    pos = posi.Extend(missile.EndPosition, Settings.accuracyQ * i);
                }
                if (perfectCast.Distance(posi) <= Settings.accuracyQ * 2)
                {
                    Q.Cast(Player.Instance);
                    missile = null;
                    castedForChampion = false;
                    castedForMinions = false;
                    castedOn = null;
                    perfectCast = new Vector2(0, 0);
                }
            }*/
        }

        private static void Dash_OnDash(Obj_AI_Base sender, Dash.DashEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void GameOnOnUpdate(EventArgs args)
        {
            try
            {
                if (Player.Instance.IsDead)
                {
                    return;
                }

                switch (Orbwalker.ActiveModesFlags)
                {
                    case Orbwalker.ActiveModes.Harass:
                        DoHarass();
                        break;
                    case Orbwalker.ActiveModes.Combo:
                        DoCombo();
                        break;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static void DoCombo()
        {
            if (Config["UseQCombo"].Cast<CheckBox>().CurrentValue && Q.IsReady() && missile == null && Player.Instance.Spellbook.GetSpell(SpellSlot.Q).ToggleState == 1)
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                if (target != null && Q.GetPrediction(target).HitChance >= HitChance.Medium)
                {
                    castedForChampion = true;
                    castedForMinions = false;
                    castedOn = target;
                    Q.Cast(Q.GetPrediction(target).CastPosition);
                }
            }
            if (Config["UseECombo"].Cast<CheckBox>().CurrentValue && Player.Instance.Spellbook.GetSpell(SpellSlot.R).ToggleState != 2)
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                if (target != null)
                {
                    if (E.IsReady() && Config["UseECombo"].Cast<CheckBox>().CurrentValue && target.IsValid && target.IsEnemy && !target.IsDead && !target.IsInvulnerable && !target.IsZombie && target.IsInRange(Player.Instance, E.Range))
                    {
                        E.Cast(target);
                    }
                    R.Cast(target);
                    RlastCast = R.GetPrediction(target).UnitPosition;
                    if (Config["UseWCombo"].Cast<CheckBox>().CurrentValue  && W.IsReady() && target.IsValid && target.IsEnemy && !target.IsDead && !target.IsInvulnerable && !target.IsZombie && target.IsInRange(Player.Instance, W.Range))
                    {
                        W.Cast(target);
                    }
                }
            }
            if (Config["UseECombo"].Cast<CheckBox>().CurrentValue && E.IsReady())
            {
                var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
                if (target != null)
                {
                    if (Config["comboUseEDoubleOnly"].Cast<CheckBox>().CurrentValue)
                    {
                        foreach (BuffInstance b in target.Buffs)
                        {
                            if (b.Name == "chilled")
                            {
                                E.Cast(target);
                            }
                        }
                    }
                    else
                    {
                        E.Cast(target);
                    }
                }
            }

            if (Config["comboUseWEscaping"].Cast<CheckBox>().CurrentValue && W.IsReady())
            {
                var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);
                var wallPos = target.Position.Extend(target, 450).To3D();
                      if (target != null)
                      {
                          if (target.IsValid && target.IsEnemy && !target.IsDead && !target.IsInvulnerable && !target.IsZombie && !target.IsInRange(Player.Instance, W.Range - 200) && target.IsInRange(Player.Instance, W.Range - 50))
                          {
                              //W.Cast(Prediction.Position.PredictUnitPosition(target, 650).To3D());
                              W.Cast(wallPos);
                          }
                      }
                }
        }



        private static void DoHarass()
        {
            if (Config["harassMana"].Cast<Slider>().CurrentValue >= Player.Instance.ManaPercent)
                return;

            if (Config["UseQHarass"].Cast<CheckBox>().CurrentValue && Q.IsReady() && Player.Instance.Spellbook.GetSpell(SpellSlot.Q).ToggleState == 1 && missile == null)
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                if (target != null && Q.GetPrediction(target).HitChance >= HitChance.High)
                {
                    castedForChampion = true;
                    castedForMinions = false;
                    castedOn = target;
                    Q.Cast(Q.GetPrediction(target).CastPosition);
                }
            }
            if (Config["UseRHarass"].Cast<CheckBox>().CurrentValue && R.IsReady() && Player.Instance.Spellbook.GetSpell(SpellSlot.R).ToggleState != 2)
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                if (target != null)
                {
                    if (E.IsReady() && Config["UseEHarass"].Cast<CheckBox>().CurrentValue && target.IsValid() && target.IsValid && target.IsEnemy && !target.IsDead && !target.IsInvulnerable && !target.IsZombie && target.IsInRange(Player.Instance, E.Range))
                    {
                        E.Cast(target);
                    }
                    R.Cast(target);
                    RlastCast = R.GetPrediction(target).UnitPosition;
                    if (Config["UseWHarass"].Cast<CheckBox>().CurrentValue && W.IsReady() && target.IsValid() && target.IsValid && target.IsEnemy && !target.IsDead && !target.IsInvulnerable && !target.IsZombie && target.IsInRange(Player.Instance, W.Range))
                    {
                        W.Cast(target);
                    }
                }
            }
            if (Config["UseEHarass"].Cast<CheckBox>().CurrentValue && E.IsReady())
            {
                var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
                if (target != null)
                {
                    foreach (BuffInstance b in target.Buffs)
                    {
                        if (b.Name == "chilled")
                        {
                            E.Cast(target);
                        }
                    }
                }
            }

        }

        private static void CreateMenu()
        {
            Config = MainMenu.AddMenu("Farofakids-Anivia", "Farofakids-Anivia");

            Config.AddGroupLabel("COMBO");
            Config.Add("UseQCombo", new CheckBox("Use Q"));
            Config.Add("UseWCombo", new CheckBox("Use W"));
            Config.Add("comboUseWEscaping", new CheckBox("Use W on escaping enemies"));
            Config.Add("UseECombo", new CheckBox("Use E"));
            Config.Add("comboUseEDoubleOnly", new CheckBox("Use E only for doubled damage"));
            Config.Add("UseRCombo", new CheckBox("Use R"));

            Config.AddGroupLabel("HARASS");
            Config.Add("UseQHarass", new CheckBox("Use Q"));
            Config.Add("UseWHarass", new CheckBox("Use W"));
            Config.Add("UseEHarass", new CheckBox("Use E(double damage only)"));
            Config.Add("UseRHarass", new CheckBox("Use R"));
            Config.Add("harassMana", new Slider("Maximum mana usage in percent ({0}%)", 40));

            Config.AddGroupLabel("MISC");

            Config.AddGroupLabel("DRAW");
            Config.Add("DrawQ", new CheckBox("Use Q", false));
            Config.Add("DrawW", new CheckBox("Use W", false));
            Config.Add("DrawE", new CheckBox("Use E", false));
            Config.Add("DrawR", new CheckBox("Use R", false));

        }

    }
}
