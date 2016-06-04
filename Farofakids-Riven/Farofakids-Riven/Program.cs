using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Constants;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace Farofakids_Riven
{
    class Program
    {
        private static Menu menu, misc, combo;
        private static Spell.Active Q, W, E;
        private static Spell.Skillshot R;
        private static AttackableUnit QTarget;
        private const string IsFirstR = "RivenFengShuiEngine";
        private const string IsSecondR = "RivenIzunaBlade";
        private static float LastQ;
        private static int QStack = 1;
        private static float LastR;
        private static bool forceQ;
        private static bool forceItem;
        private static bool forceW;
        private static bool forceR;
        private static bool forceR2;
        private static int LastAATick;


        public static bool Qstrange    { get { return misc["Qstrange"].Cast<CheckBox>().CurrentValue; } }
        private static bool WInterrupt { get { return misc["WInterrupt"].Cast<CheckBox>().CurrentValue; } }
        private static bool KeepQ { get { return misc["KeepQ"].Cast<CheckBox>().CurrentValue; } }
        private static int QD          { get { return misc["QD"].Cast<Slider>().CurrentValue; } }

        private static bool AlwaysR   { get { return combo["AlwaysR"].Cast<CheckBox>().CurrentValue; } }
        private static bool UseHoola  { get { return combo["UseHoola"].Cast<CheckBox>().CurrentValue; } }
        private static bool ComboW    { get { return combo["ComboW"].Cast<CheckBox>().CurrentValue; } }       
        private static bool RKillable { get { return combo["RKillable"].Cast<CheckBox>().CurrentValue; } }

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Riven") return;
            Chat.Print("Farofakids Riven - Loaded.....");
            Q = new Spell.Active(SpellSlot.Q);
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Active(SpellSlot.E, 300);
            R = new Spell.Skillshot(SpellSlot.R, 900, SkillShotType.Cone, 250, 1600, 45);

            OnMenuLoad();

            Game.OnTick += Game_OnTick;
           // Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnSpellCast;
            Obj_AI_Base.OnPlayAnimation += Obj_AI_Base_OnPlayAnimation;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;

        }

        private static void OnMenuLoad()
        {
            menu = MainMenu.AddMenu("Farofakids Riven", "Farofakids-Riven");
            menu.AddGroupLabel("Farofakids Riven");

            combo = menu.AddSubMenu("Combo", "Combo");
            combo.Add("AlwaysR", new CheckBox("Always Use R (Toggle)"));
            combo.Add("UseHoola", new CheckBox("Use Hoola Combo Logic (Toggle)"));
            combo.Add("ComboW", new CheckBox("Always use W"));
            combo.Add("RKillable", new CheckBox("Use R When Target Can Killable"));

            misc = menu.AddSubMenu("MISC", "MISC");
            misc.Add("Qstrange", new CheckBox("Strange Q For Speed"));
            misc.Add("Winterrupt", new CheckBox("W interrupt"));
            misc.Add("KeepQ", new CheckBox("Keep Q Alive"));
            misc.Add("QD", new Slider("First,Second Q Delay", 29, 23, 43));
        }

        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (sender.IsEnemy && W.IsReady() && sender.IsValidTarget() && !sender.IsZombie && WInterrupt)
            {
                if (sender.IsValidTarget(125 + Player.Instance.BoundingRadius + sender.BoundingRadius)) W.Cast();
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Player.Instance.IsDead)
                return;
            if (!sender.IsMe) return;

            if (args.SData.Name.Contains("ItemTiamatCleave")) forceItem = false;
            if (args.SData.Name.Contains("RivenTriCleave")) forceQ = false;
            if (args.SData.Name.Contains("RivenMartyr")) forceW = false;
            if (args.SData.Name == IsFirstR) forceR = false;
            if (args.SData.Name == IsSecondR) forceR2 = false;
        }

        private static void Reset()
        {
            Player.DoEmote(Emote.Dance);
            LastAATick = 0;
            //Player.IssueOrder(GameObjectOrder.MoveTo, Player.Position.Extend(Game.CursorPos, Player.Distance(Game.CursorPos) + 10));
            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos + 10);
        }

        private static void Obj_AI_Base_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (!sender.IsMe) return;

            switch (args.Animation)
            {
                case "Spell1a":
                    LastQ = Environment.TickCount;
                    if (Qstrange && Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.None) Player.DoEmote(Emote.Dance);
                    QStack = 2;
                    if (Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.None && Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.LastHit && Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.Flee) Core.DelayAction(Reset, QD * 10 + 1);
                    break;
                case "Spell1b":
                    LastQ = Environment.TickCount;
                    if (Qstrange && Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.None) Player.DoEmote(Emote.Dance);
                    QStack = 3;
                    if (Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.None && Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.LastHit && Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.Flee) Core.DelayAction(Reset, QD * 10 + 1);
                    break;
                case "Spell1c":
                    LastQ = Environment.TickCount;
                    if (Qstrange && Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.None) Player.DoEmote(Emote.Dance);
                    QStack = 1;
                    if (Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.None && Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.LastHit && Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.Flee) Core.DelayAction(Reset, QD * 10 + 3);
                    break;
                case "Spell3":
                    if ((//Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Burst ||
                        Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.Combo ||
                        //Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.FastHarass ||
                        Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.Flee)) /*&& Youmu) CastYoumoo()*/;
                    break;
                case "Spell4a":
                    LastR = Environment.TickCount;
                    break;
                case "Spell4b":
                    var target = TargetSelector.SelectedTarget;
                    if (Q.IsReady() && target.IsValidTarget()) ForceCastQ(target);
                    break;
            }
        }

        private static void ForceCastQ(AttackableUnit target)
        {
            forceQ = true;
            QTarget = target;
        }

        private static void Obj_AI_Base_OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            throw new NotImplementedException();
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.Combo) Combo();
            if (Environment.TickCount - LastQ >= 3650 && QStack != 1 && !Player.Instance.IsRecalling() && KeepQ && Q.IsReady()) Q.Cast(Game.CursorPos);
        }

        private static void Combo()
        {
            var targetR = TargetSelector.GetTarget(250 + Player.Instance.AttackRange + 70, DamageType.Physical);
            if (R.IsReady() && R.Name == IsFirstR && Player.Instance.IsInAutoAttackRange(targetR) && AlwaysR && targetR != null) ForceR();
            if (R.IsReady() && R.Name == IsFirstR && W.IsReady() && InWRange(targetR) && ComboW && AlwaysR && targetR != null)
            {
                ForceR();
                //Utility.DelayAction.Add(1, ForceW);
                Core.DelayAction(ForceW, 1);
            }
            if (W.IsReady() && InWRange(targetR) && ComboW && targetR != null) W.Cast();
            if (UseHoola && R.IsReady() && R.Name == IsFirstR && W.IsReady() && targetR != null && E.IsReady() && targetR.IsValidTarget() && !targetR.IsZombie && (IsKillableR(targetR) || AlwaysR))
            {
                if (!InWRange(targetR))
                {
                    E.Cast(targetR.Position);
                    ForceR();
                    //Utility.DelayAction.Add(200, ForceW);
                    Core.DelayAction(ForceW, 200);
                    //Utility.DelayAction.Add(305, () => ForceCastQ(targetR));
                    Core.DelayAction(() => ForceCastQ(targetR), 305);
                }
            }
            else if (!UseHoola && R.IsReady() && R.Name == IsFirstR && W.IsReady() && targetR != null && E.IsReady() && targetR.IsValidTarget() && !targetR.IsZombie && (IsKillableR(targetR) || AlwaysR))
            {
                if (!InWRange(targetR))
                {
                    E.Cast(targetR.Position);
                    ForceR();
                    //Utility.DelayAction.Add(200, ForceW);
                    Core.DelayAction(ForceW, 200);
                }
            }
            else if (UseHoola && W.IsReady() && E.IsReady())
            {
                if (targetR.IsValidTarget() && targetR != null && !targetR.IsZombie && !InWRange(targetR))
                {
                    E.Cast(targetR.Position);
                    //Utility.DelayAction.Add(10, ForceItem);
                    //Utility.DelayAction.Add(200, ForceW);
                    Core.DelayAction(ForceW, 200);
                    //Utility.DelayAction.Add(305, () => ForceCastQ(targetR));
                    Core.DelayAction(() => ForceCastQ(targetR), 305);
                }
            }
            else if (!UseHoola && W.IsReady() && targetR != null && E.IsReady())
            {
                if (targetR.IsValidTarget() && targetR != null && !targetR.IsZombie && !InWRange(targetR))
                {
                    E.Cast(targetR.Position);
                    //Utility.DelayAction.Add(10, ForceItem);
                    //Utility.DelayAction.Add(240, ForceW);
                    Core.DelayAction(ForceW, 240);
                }
            }
            else if (E.IsReady())
            {
                if (targetR.IsValidTarget() && !targetR.IsZombie && !InWRange(targetR))
                {
                    E.Cast(targetR.Position);
                }
            }
        }

        private static void ForceR()
        {
            forceR = (R.IsReady() && R.Name == IsFirstR);
            //Utility.DelayAction.Add(500, () => forceR = false);
            Core.DelayAction(() => forceR = false, 500);
        }

        private static void ForceW()
        {
            forceW = W.IsReady();
            //Utility.DelayAction.Add(500, () => forceW = false);
            Core.DelayAction(() => forceW = false, 500);
        }

        //private static bool InWRange(GameObject target) => (Player.HasBuff("RivenFengShuiEngine") && target != null) ?
          //    330 >= Player.Instance.Distance(target.Position) : 265 >= Player.Instance.Distance(target.Position);

        private static bool InWRange(GameObject target)
        {
            if (target == null || !target.IsValid) return false;
            return (Player.HasBuff("RivenFengShuiEngine"))
            ? 330 >= Player.Instance.Distance(target.Position)
            : 265 >= Player.Instance.Distance(target.Position);

        }

        private static bool IsKillableR(AIHeroClient target)
        {
            if (RKillable && target.IsValidTarget() && (totaldame(target) >= target.Health
                 && basicdmg(target) <= target.Health) || Player.Instance.CountEnemiesInRange(900) >= 2 && (!target.HasBuff("kindrednodeathbuff") && !target.HasBuff("Undying Rage") && !target.HasBuff("JudicatorIntervention")))
            {
                return true;
            }
            return false;
        }

        private static double Rdame(Obj_AI_Base target, float health)
        {
            if (target != null)
            {
                var missinghealth = (target.MaxHealth - health) / target.MaxHealth > 0.75f ? 0.75f : (target.MaxHealth - health) / target.MaxHealth;
                var pluspercent = missinghealth * (8 / 3);
                var rawdmg = new float[] { 80, 120, 160 }[R.Level - 1] + 0.6f * Player.Instance.FlatPhysicalDamageMod;
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, rawdmg * (1 + pluspercent));
            }
            return 0;
        }

        private static double totaldame(Obj_AI_Base target)
        {
            if (target != null)
            {
                float dmg = 0;
                float passivenhan = 0;
                if (Player.Instance.Level >= 18) { passivenhan = 0.5f; }
                else if (Player.Instance.Level >= 15) { passivenhan = 0.45f; }
                else if (Player.Instance.Level >= 12) { passivenhan = 0.4f; }
                else if (Player.Instance.Level >= 9) { passivenhan = 0.35f; }
                else if (Player.Instance.Level >= 6) { passivenhan = 0.3f; }
                else if (Player.Instance.Level >= 3) { passivenhan = 0.25f; }
                else { passivenhan = 0.2f; }
                //if (HasItem()) dmg = dmg + Player.GetAutoAttackDamage(target) * 0.7;
                if (W.IsReady()) dmg = dmg + Player.Instance.GetSpellDamage(target, SpellSlot.W);
                if (Q.IsReady())
                {
                    var qnhan = 4 - QStack;
                    dmg = dmg + Player.Instance.GetSpellDamage(target, SpellSlot.Q) * qnhan + Player.Instance.GetAutoAttackDamage(target) * qnhan * (1 + passivenhan);
                }
                dmg = dmg + Player.Instance.GetAutoAttackDamage(target) * (1 + passivenhan);
                if (R.IsReady())
                {
                    var rdmg = Rdame(target, target.Health - dmg * 1.2f);
                    return dmg * 1.2 + rdmg;
                }
                return dmg;
            }
            return 0;
        }

        private static double basicdmg(Obj_AI_Base target)
        {
            if (target != null)
            {
                double dmg = 0;
                double passivenhan = 0;
                if (Player.Instance.Level >= 18) { passivenhan = 0.5; }
                else if (Player.Instance.Level >= 15) { passivenhan = 0.45; }
                else if (Player.Instance.Level >= 12) { passivenhan = 0.4; }
                else if (Player.Instance.Level >= 9) { passivenhan = 0.35; }
                else if (Player.Instance.Level >= 6) { passivenhan = 0.3; }
                else if (Player.Instance.Level >= 3) { passivenhan = 0.25; }
                else { passivenhan = 0.2; }
                //if (HasItem()) dmg = dmg + Player.GetAutoAttackDamage(target) * 0.7;
                if (W.IsReady()) dmg = dmg + Player.Instance.GetSpellDamage(target, SpellSlot.W); 
                if (Q.IsReady())
                {
                    var qnhan = 4 - QStack;
                    dmg = dmg + Player.Instance.GetSpellDamage(target, SpellSlot.Q) * qnhan + Player.Instance.GetAutoAttackDamage(target) * qnhan * (1 + passivenhan);
                }
                dmg = dmg + Player.Instance.GetAutoAttackDamage(target) * (1 + passivenhan);
                return dmg;
            }
            return 0;
        }
    }
}
