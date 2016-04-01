using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Events;
using SharpDX;

namespace AkaYasuo.Events
{
    class _game
    {
        public static void AutoQ()
        {
            if (!MenuManager.HarassMenu["AutoQ"].Cast<KeyBind>().CurrentValue || Variables.isDashing
                || (Variables.HaveQ3 && !MenuManager.HarassMenu["AutoQ3"].Cast<CheckBox>().CurrentValue)    
                || (Variables._Player.IsUnderEnemyturret() && !MenuManager.HarassMenu["QTower"].Cast<CheckBox>().CurrentValue))
            {
                return;
            }
            var target = TargetSelector.GetTarget(!Variables.HaveQ3 ? Variables.QRange : Variables.Q2Range, DamageType.Physical);
            if (target == null)
            {
                return;
            }
            (!Variables.HaveQ3 ? Program.Q : Program.Q2).Cast(target);
            
            if (Variables.HaveQ3)
                            {
                                var hit = -1;
                                var predPos = new Vector3();
                                foreach (var hero in EntityManager.Heroes.Enemies.Where(i => i.IsValidTarget(Variables.Q2Range)))
                                {
                                    var pred = Prediction.Position.PredictLinearMissile(hero, Variables.Q2Range, Program.Q2.Width, Program.Q2.CastDelay, Program.Q2.Speed, int.MaxValue, Variables._Player.ServerPosition, true);
                                    var pred2 = pred.GetCollisionObjects<AIHeroClient>();
                                    if (pred.HitChance >= EloBuddy.SDK.Enumerations.HitChance.High && pred2.Length > hit)
                                    {
                                        hit = pred2.Length;
                                        predPos = pred.CastPosition;
                                    }
                                }
                                if (predPos.IsValid())
                                {
                                    Core.DelayAction(() => Program.Q2.Cast(predPos), 250);
                                }
                                else
                                {
                                    Core.DelayAction(() => Program.Q2.Cast(target.Position), 250);
                                }
                            }
        }

        public static void StackQ()
        {
            if (!MenuManager.MiscMenu["StackQ"].Cast<CheckBox>().CurrentValue || !Program.Q.IsReady() || Variables.isDashing || Variables.HaveQ3)
            {
                return;
            }

            var target = TargetSelector.GetTarget(Program.Q.Range, DamageType.Physical);
            if (target != null && (!Variables._Player.IsUnderEnemyturret() || !target.ServerPosition.IsUnderTurret()))
            {
                Program.Q.Cast(target.ServerPosition);
            }
            else
            {
                var minionObj = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Variables._Player.ServerPosition, Variables.QRange);
                if (minionObj.Any())
                {
                    var obj = minionObj.FirstOrDefault(i => DamageManager._GetQDmg(i) >= i.Health)
          ?? minionObj.MinOrDefault(i => i.Distance(Variables._Player));
                    if (obj != null)
                    {
                        Program.Q.Cast(obj);
                    }
                }
            }
        }

        public static void LevelUpSpells()
        {
            if (!MenuManager.MiscMenu["autolvl"].Cast<CheckBox>().CurrentValue) return;

            var qL = Variables._Player.Spellbook.GetSpell(SpellSlot.Q).Level + Variables.QOff;
            var wL = Variables._Player.Spellbook.GetSpell(SpellSlot.W).Level + Variables.WOff;
            var eL = Variables._Player.Spellbook.GetSpell(SpellSlot.E).Level + Variables.EOff;
            var rL = Variables._Player.Spellbook.GetSpell(SpellSlot.R).Level + Variables.ROff;
            if (qL + wL + eL + rL >= Variables._Player.Level) return;
            int[] level = { 0, 0, 0, 0 };
            for (var i = 0; i < Variables._Player.Level; i++)
            {
                level[Variables.abilitySequence[i] - 1] = level[Variables.abilitySequence[i] - 1] + 1;
            }
            if (qL < level[0]) Variables._Player.Spellbook.LevelSpell(SpellSlot.Q);
            if (wL < level[1]) Variables._Player.Spellbook.LevelSpell(SpellSlot.W);
            if (eL < level[2]) Variables._Player.Spellbook.LevelSpell(SpellSlot.E);
            if (rL < level[3]) Variables._Player.Spellbook.LevelSpell(SpellSlot.R);
        }
    }
}
