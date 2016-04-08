namespace ElAurelion_Sol
{
    using System;
    using System.Linq;
    using EloBuddy;
    using SharpDX;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu.Values;
    using Color = System.Drawing.Color;

    internal class AurelionSol
    {

        public static Spell.Targeted IgniteSpell { get; set; }

        private static Spell.Active E { get; set; }

        private static Menu Menu { get; set; }

        private static Menu comboMenu, harassMenu, laneclearMenu, jungleclearMenu, killstealMenu, miscMenu,  drawingsMenu;

        private static AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        private static Spell.Skillshot Q { get; set; }

        private static Spell.Skillshot R { get; set; }

        private static Spell.Active W { get; set; }

        private static Spell.Active W1 { get; set; }

        private static bool HasPassive()
        {
            return Player.HasBuff("AurelionSolWActive");
        }

        public static void OnGameLoad(EventArgs args)
        {
            try
            {
                if (Player.ChampionName != "AurelionSol")
                {
                    return;
                }

                var igniteSlot = Player.GetSpellSlotFromName("summonerdot");


                if (igniteSlot != SpellSlot.Unknown)
                {   
                    IgniteSpell = new Spell.Targeted(igniteSlot, 600);
                }
                //Q = new Spell.Skillshot(SpellSlot.Q, 650, SkillShotType.Circular, 420, 800, 180);
                Q = new Spell.Skillshot(SpellSlot.Q, 1075, SkillShotType.Circular, 420, 800, 180);
                W1 = new Spell.Active(SpellSlot.W, 350);
                W = new Spell.Active(SpellSlot.W, 600);
                E = new Spell.Active(SpellSlot.E, 400);
                R = new Spell.Skillshot(SpellSlot.R, 1500, SkillShotType.Linear, 250, 1750, 180);

                GenerateMenu();
                Game.OnUpdate += OnUpdate;
                Drawing.OnDraw += OnDraw;
                Orbwalker.OnPreAttack += OrbwalkingBeforeAttack;
                Gapcloser.OnGapcloser += AntiGapcloser_OnEnemyGapcloser;
                Interrupter.OnInterruptableSpell += Interrupter2_OnInterruptableTarget;

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static void GenerateMenu()
        {
            try
            {
                Menu = MainMenu.AddMenu("ElAurelion Sol", "AurelionSol");

                   comboMenu = Menu.AddSubMenu("Combo");
                   comboMenu.Add("Combo.Q", new CheckBox("Use Q"));
                   comboMenu.Add("Combo.W", new CheckBox("Use W"));
                   comboMenu.Add("Combo.R", new CheckBox("Use R"));
                   comboMenu.Add("Combo.R.Multiple", new CheckBox("R multiple targets:"));
                   comboMenu.Add("Combo.R.Count", new Slider("Auto R on:", 3, 2, 4));

                   harassMenu = Menu.AddSubMenu("Harass");
                   harassMenu.Add("Harass.Q", new CheckBox("Use Q"));

                   laneclearMenu = Menu.AddSubMenu("Laneclear");
                   laneclearMenu.Add("laneclear.Q", new CheckBox("Use Q"));
                   laneclearMenu.Add("laneclear.minionshit", new Slider("Minimum minions hit (Q)", 2, 1, 5));
                   laneclearMenu.Add("laneclear.Mana", new Slider("Minimum mana", 20, 0, 100));

                   jungleclearMenu = Menu.AddSubMenu("Jungleclear");
                   jungleclearMenu.Add("jungleclear.Q", new CheckBox("Use Q"));
                   jungleclearMenu.Add("jungleclear.minionshit",new Slider("Minimum minions killable (Q)", 1, 1, 5));
                   jungleclearMenu.Add("jungleclear.Mana", new Slider("Minimum mana", 20, 0, 100));


                   killstealMenu = Menu.AddSubMenu("Killsteal");
                   killstealMenu.Add("Killsteal.Active", new CheckBox("Activate killsteal"));
                   killstealMenu.Add("Killsteal.R", new CheckBox("Use R"));
                   killstealMenu.Add("Ignite", new CheckBox("Use Ignite"));

                   miscMenu = Menu.AddSubMenu("Misc");
                   miscMenu.Add("Misc.Auto.W",  new CheckBox("Auto deactivate W"));
                   miscMenu.Add("AA.Block",  new CheckBox("Don't use AA in combo", false));
                   miscMenu.Add("inter",  new CheckBox("Anti interupt"));
                   miscMenu.Add("gapcloser",  new CheckBox("Anti gapcloser"));

                   drawingsMenu = Menu.AddSubMenu("Drawings");
                   drawingsMenu.Add("Draw.Off", new CheckBox("Disable drawings", false));
                   drawingsMenu.Add("Draw.Q", new CheckBox("Draw Q"));
                   drawingsMenu.Add("Draw.W", new CheckBox("Draw W"));
                   drawingsMenu.Add("Draw.R", new CheckBox("Draw R"));

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static void HandleIgnite()
        {
            try
            {
                if (IgniteSpell != null && IgniteSpell.IsReady())
                {
                    var ignitableEnemies =
                        EntityManager.Heroes.Enemies.Where(
                            t =>
                                t.IsValidTarget(IgniteSpell.Range) && !t.HasUndyingBuff() &&
                                CalculateDamage(t, false, false, false, false, true) >= t.Health);
                    var igniteEnemy = TargetSelector.GetTarget(ignitableEnemies, DamageType.True);

                    if (igniteEnemy != null)
                    {
                        if (IgniteSpell != null && killstealMenu["Ignite"].Cast<CheckBox>().CurrentValue)
                        {
                            if (IgniteSpell.IsInRange(igniteEnemy))
                            {
                                IgniteSpell.Cast(igniteEnemy);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static double RDamage(Obj_AI_Base target)
        {
            return Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (float)new double[] { 200, 400, 600 }[R.Level - 1] + 0.70f * Player.TotalMagicalDamage);
        }

        private static double QDamage(Obj_AI_Base target)
        {
            return Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (float)new double[] { 70 , 110 , 150 , 190 , 230 }[Q.Level - 1] + 0.65f * Player.TotalMagicalDamage);
        }

        private static void OnCombo()
        {
            try
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                if (target == null)
                {
                    return;
                }

                if (Q.IsReady() && comboMenu["Combo.Q"].Cast<CheckBox>().CurrentValue)
                {
                    var prediction = Q.GetPrediction(target);
                    if (prediction.HitChance >= HitChance.High)
                    {
                        Q.Cast(prediction.CastPosition);
                    }
                }

                if (W.IsReady() && comboMenu["Combo.W"].Cast<CheckBox>().CurrentValue)
                {
                    if (!HasPassive())
                    {
                        if (target.IsValidTarget(W1.Range))
                        {
                            return;
                        }

                        if (Player.Distance(target) > W1.Range && Player.Distance(target) < W.Range)
                        {
                            W.Cast();
                        }
                    }
                    else if(HasPassive())
                    {
                        if (Player.Distance(target) > W1.Range && Player.Distance(target) < W.Range + 100)
                        {
                            return;
                        }

                        if (Player.Distance(target) > W1.Range + 150)
                        {
                            W.Cast();
                        }
                    }
                }

                if (R.IsReady() && comboMenu["Combo.R"].Cast<CheckBox>().CurrentValue && RDamage(target) > target.Health + target.AttackShield) 
                {
                    var prediction = R.GetPrediction(target);
                    if (prediction.HitChance >= HitChance.High)
                    {   
                        R.Cast(prediction.CastPosition);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void OrbwalkingBeforeAttack(AttackableUnit target,  Orbwalker.PreAttackArgs args)
        {
            if (miscMenu["AA.Block"].Cast<CheckBox>().CurrentValue && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                args.Process = false;
            }
            else
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    args.Process =
                        !(Q.IsReady() 
                          || Player.Distance(args.Target) >= 1000);
                }
            }
        }

        private static void Interrupter2_OnInterruptableTarget(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {

            if (!miscMenu["inter"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            if (args.DangerLevel != DangerLevel.High
                || sender.Distance(Player) > Q.Range)
            {
                return;
            }

            if (sender.IsValidTarget(Q.Range) && args.DangerLevel == DangerLevel.High
                && Q.IsReady())
            {
                var prediction = Q.GetPrediction(sender);
                if (prediction.HitChance >= HitChance.High)
                {
                    Q.Cast(prediction.CastPosition);
                }
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(AIHeroClient Sender, Gapcloser.GapcloserEventArgs gapcloser)
        {
            if (!miscMenu["gapcloser"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            if (Q.IsReady() && gapcloser.Sender.Distance(Player) < Q.Range)
            {
                if (gapcloser.Sender.IsValidTarget(Q.Range) && Q.IsReady())
                {
                    var prediction = Q.GetPrediction(gapcloser.Sender);
                    if (prediction.HitChance >= HitChance.High)
                    {
                        Q.Cast(prediction.CastPosition);
                    }
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            try
            {
                if (drawingsMenu["Draw.Off"].Cast<CheckBox>().CurrentValue)
                {
                    return;
                }

                if (drawingsMenu["Draw.W"].Cast<CheckBox>().CurrentValue)
                {

                    if (!HasPassive())
                    {
                        if (W.Level > 0)
                        {
                            Drawing.DrawCircle(ObjectManager.Player.Position, W1.Range, Color.Red);
                        }
                    }
                    else
                    {
                        if (W.Level > 0)
                        {
                            Drawing.DrawCircle(ObjectManager.Player.Position, W.Range, Color.MediumVioletRed);
                        }
                    }
                }
                if (drawingsMenu["Draw.Q"].Cast<CheckBox>().CurrentValue)
                {
                    if (Q.Level > 0)
                    {
                        Drawing.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Goldenrod);
                    }
                }

                if (drawingsMenu["Draw.R"].Cast<CheckBox>().CurrentValue)
                {
                    if (R.Level > 0)
                    {
                        Drawing.DrawCircle(ObjectManager.Player.Position, R.Range, Color.DeepSkyBlue);
                    }
                }

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static void OnHarass()
        {
            try
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                if (target == null)
                {
                    return;
                }

                if (Q.IsReady() && harassMenu["Harass.Q"].Cast<CheckBox>().CurrentValue)
                {
                    var prediction = Q.GetPrediction(target);
                    if (prediction.HitChance >= HitChance.High)
                    {
                        Q.Cast(prediction.CastPosition);
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void OnJungleclear()
        {
            try
            {
                var minion = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Position, Q.Range);

                if (minion == null)
                {
                    return;
                }

                if (Player.ManaPercent < jungleclearMenu["jungleclear.Mana"].Cast<Slider>().CurrentValue)
                {
                    return;
                }

                if (jungleclearMenu["jungleclear.Q"].Cast<CheckBox>().CurrentValue && Q.IsReady())
                {
                    var prediction = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(minion, Q.Width, (int)Q.Range);
                    if (prediction.HitNumber >= 2)
                    {
                        Q.Cast(prediction.CastPosition);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static void OnKillsteal()
        {
            
            try
            {
                foreach (
                    var enemy in EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(R.Range) && !x.IsDead && !x.IsZombie))
                {
                    if (Q.IsReady() && enemy.IsValidTarget(Q.Range) && enemy.Health < QDamage(enemy))
                    {
                        var prediction = Q.GetPrediction(enemy);
                        if (prediction.HitChance >= HitChance.High)
                        {
                            Q.Cast(prediction.CastPosition);
                        }
                    }

                    if(R.IsReady() && enemy.IsValidTarget(R.Range) && enemy.Health < RDamage(enemy))
                    {
                        var prediction = R.GetPrediction(enemy);
                        if (prediction.HitChance >= HitChance.High)
                        {
                            R.Cast(prediction.CastPosition);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static void OnLaneclear()
        {
            try
            {
                if (Player.ManaPercent < laneclearMenu["laneclear.Mana"].Cast<Slider>().CurrentValue)
                {
                    return;
                }
                var minion = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Position, Q.Range);
                if (minion == null)
                {
                    return;
                }

                if (laneclearMenu["laneclear.Q"].Cast<CheckBox>().CurrentValue && Q.IsReady())
                {
                    var prediction = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(minion, Q.Width, (int)Q.Range);
                    if (prediction.HitNumber >= 2)
                    {
                        Q.Cast(prediction.CastPosition);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            try
            {
                if (Player.IsDead)
                {
                    return;
                }

                switch (Orbwalker.ActiveModesFlags)
                {
                    case Orbwalker.ActiveModes.Combo:
                        OnCombo();
                        break;
                    case Orbwalker.ActiveModes.Harass:
                        OnHarass();
                        break;
                    case Orbwalker.ActiveModes.LaneClear:
                        OnLaneclear();
                        OnJungleclear();
                        break;
                }

                if (killstealMenu["Killsteal.Active"].Cast<CheckBox>().CurrentValue)
                {
                    OnKillsteal();
                }

                if (killstealMenu["Ignite"].Cast<CheckBox>().CurrentValue)
                {
                    HandleIgnite();
                }

                if (miscMenu["Misc.Auto.W"].Cast<CheckBox>().CurrentValue)
                {
                       if (HasPassive() && Player.CountEnemiesInRange(2000) == 0)
                    {
                        W.Cast();
                    }
                }

                if (comboMenu["Combo.R.Multiple"].Cast<CheckBox>().CurrentValue)
                {
                    var minREnemies = comboMenu["Combo.R.Count"].Cast<Slider>().CurrentValue;
                    foreach (var enemy in from enemy in EntityManager.Heroes.Enemies let startPos = enemy.ServerPosition let endPos = Player.ServerPosition.Extend(startPos, Player.Distance(enemy) + R.Range) let rectangle = new Geometry.Polygon.Rectangle((Vector2)startPos, endPos, R.Radius) where EntityManager.Heroes.Enemies.Count(x => rectangle.IsInside(x)) >= minREnemies select enemy)
                    {
                        R.Cast(enemy.Position);
                    }
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

        public static float CalculateDamage(Obj_AI_Base target, bool q, bool w, bool e, bool r, bool ignite)
        {
            var totaldamage = 0f;

            if (target == null) return totaldamage;

            if (ignite && IgniteSpell != null && IgniteSpell.IsReady() && IgniteSpell.IsInRange(target))
            {
                totaldamage += Player.GetSummonerSpellDamage(target, EloBuddy.SDK.DamageLibrary.SummonerSpells.Ignite);
            }

            return totaldamage;
        }

    }
}