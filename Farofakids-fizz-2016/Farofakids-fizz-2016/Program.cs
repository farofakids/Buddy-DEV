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

namespace Farofakids_fizz_2016
{
    class Program
    {
        public static Menu Menu, comboMenu, harassMenu, miscMenu, drawMenu;
        private static Spell.Targeted Q;
        private static Spell.Active W;
        private static Spell.Skillshot E, R;
        private static Vector3? LastHarassPos { get; set; }
        private static bool JumpBack { get; set; }


        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += GameOnOnGameLoad;
        }

        private static void GameOnOnGameLoad(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Fizz")
            {
                return;
            }
            Q = new Spell.Targeted(SpellSlot.Q, 550);
            W = new Spell.Active(SpellSlot.W, (uint)Player.Instance.GetAutoAttackRange());
            E = new Spell.Skillshot(SpellSlot.E, 400, SkillShotType.Circular, 250, int.MaxValue, 330);
            R = new Spell.Skillshot(SpellSlot.R, 1300, SkillShotType.Linear, 250, 1200, 80);

            E.AllowedCollisionCount = int.MaxValue;
            R.AllowedCollisionCount = 0;
            
            CreateMenu();

            Game.OnUpdate += GameOnOnUpdate;
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
            try 
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                if (target == null)
                {
                    return;
                }
                if (comboMenu["UseQRECombo"].Cast<CheckBox>().CurrentValue && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    if (Q.Cast(target))
                    {
                        if (E.IsReady())
                        {
                            E.Cast(target);
                        }
                    }

                    if (R.IsReady())
                    {
                        if (Qkillable(target))
                        {
                            Q.Cast(target);
                            return;
                        }

                        RCastLogic(target);
                    }
                }
                if (comboMenu["UseQCombo"].Cast<CheckBox>().CurrentValue && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    Q.Cast(target);
                }
                if (comboMenu["UseWCombo"].Cast<CheckBox>().CurrentValue && W.IsReady() && !Q.IsReady() && !E.IsReady()
                 && target.IsValidTarget(W.Range))
                {
                    W.Cast();
                }
                if (comboMenu["UseECombo"].Cast<CheckBox>().CurrentValue && E.IsReady() && target.IsValidTarget(E.Range)
                 && E.Name.ToLower() == "fizzjump")
                {
                    var castPos = E.GetPrediction(target).CastPosition.Extend(Player.Instance.Position, -165);
                    E.Cast(castPos.To3DWorld());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void DoHarass()
        {
            try
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                if (target == null)
                {
                    return;
                }

                if (LastHarassPos == null)
                {
                    LastHarassPos = Player.Instance.ServerPosition;
                }

                if (JumpBack && IsActive("ElFizz.Harass.E"))
                {
                    E.Cast((Vector3)LastHarassPos);
                }

                if (W.IsReady() && IsActive("ElFizz.Harass.W") && (Q.IsReady() || Orbwalker.InAutoAttackRange(target)))
                {
                    W.Cast();
                }

                if (Q.IsReady() && IsActive("ElFizz.Harass.Q"))
                {
                    Q.Cast(target);
                }

                if (E.IsReady() && IsActive("ElFizz.Harass.E")
                    && Menu.Item("ElFizz.Harass.Mode.E").GetValue<StringList>().SelectedIndex == 1)
                {
                    E.Cast(target);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static float GetComboDamage(Obj_AI_Base enemy)
        {
            try
            {
                float damage = 0;

                //   if (!Player.IsWindingUp)
                // {
                damage += (float)ObjectManager.Player.GetAutoAttackDamage(enemy, true);
                //}

                if (Q.IsReady())
                {
                    //damage += Q.GetDamage(enemy);
                    damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.Q);
                }

                if (W.IsReady())
                {
                    //damage += W.GetDamage(enemy);
                    damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.W);
                }

                if (E.IsReady())
                {
                    // damage += E.GetDamage(enemy);
                    damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.E);
                }

                if (Player.HasBuff("lichbane"))
                {
                    /*     damage +=
                             (float)
                             Player.CalcDamage(
                                 enemy,
                                 DamageType.Magical,
                                 (Player.BaseAttackDamage * 0.75)
                                 + (Player.BaseAbilityDamage + Player.FlatMagicDamageMod) * 0.5);*/
                    damage += Player.Instance.CalculateDamageOnUnit(
                        enemy,
                        DamageType.Magical,
                        (Player.Instance.BaseAttackDamage * 0.75f)
                        + (Player.Instance.BaseAbilityDamage + Player.Instance.FlatMagicDamageMod) * 0.5f);
                }


                if (R.IsReady())
                {
                    //damage += R.GetDamage(enemy);
                    damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.R);
                }

                return damage;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return 0;
        }

        private static void CreateMenu()
        {
            Menu = MainMenu.AddMenu("Fizz-Farofakids", "Fizz-Farofakids");

            comboMenu = Menu.AddSubMenu("Combo");
            comboMenu.Add("UseQCombo", new CheckBox("Use Q"));
            comboMenu.Add("UseWCombo", new CheckBox("Use W"));
            comboMenu.Add("UseECombo", new CheckBox("Use E"));
            comboMenu.Add("UseRCombo", new CheckBox("Use R"));
            comboMenu.Add("UseQRECombo", new CheckBox("Use Combo Q R E"));

            harassMenu = Menu.AddSubMenu("Harass");
            harassMenu.Add("UseQMixed", new CheckBox("UseQ"));
            harassMenu.Add("UseWMixed", new CheckBox("UseW"));
            harassMenu.Add("UseEMixed", new CheckBox("UseE"));

            miscMenu = Menu.AddSubMenu("Misc");
            

            drawMenu = Menu.AddSubMenu("Drawing");
            drawMenu.Add("DrawQ", new CheckBox("Draw Q"));
            drawMenu.Add("DrawW", new CheckBox("Draw Q"));
            drawMenu.Add("DrawE", new CheckBox("Draw E"));
            drawMenu.Add("DrawR", new CheckBox("Draw R"));
            drawMenu.Add("drawFill", new CheckBox("drawFill"));
            DamageIndicator.DamageToUnit = GetComboDamage;

        }

        private static bool Qkillable(AIHeroClient target)
        {
            try
            {
                
                return target.Health < Player.Instance.GetSpellDamage(target, SpellSlot.Q) && target.IsValidTarget(Q.Range) && Q.IsReady();

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            return false;
        }

        private static void RCastLogic(AIHeroClient target)
        {
            try
            {
                var predictionEnemy = R.GetPrediction(target).CastPosition;
                {
                    if (!ShieldCheck(target) || Player.Instance.GetSpellDamage(target, SpellSlot.R) > target.Health + 75)
                    {
                        return;
                    }
                    R.Cast(predictionEnemy);
                    return;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static bool ShieldCheck(Obj_AI_Base hero)
        {
            try
            {
                return !hero.HasBuff("summonerbarrier") || !hero.HasBuff("BlackShield")
                       || !hero.HasBuff("SivirShield") || !hero.HasBuff("BansheesVeil")
                       || !hero.HasBuff("ShroudofDarkness");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return false;
        }

    }
}
