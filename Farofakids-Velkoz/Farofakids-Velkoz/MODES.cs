using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using SharpDX;

namespace Farofakids_Velkoz
{
    internal class MODES
    {
        public static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (!MENUS.InterruptSpells) return;
            SPELLS.E.Cast(sender);
            if (sender != null)
            {
                var target = TargetSelector.GetTarget(SPELLS.E.Range, DamageType.Magical);
                if (target != null)
                    SPELLS.E.Cast(target);
            }
                
        }

        public static void Combo()
        {
            UseSpells(MENUS.UseQCombo, MENUS.UseWCombo,
                MENUS.UseECombo, MENUS.UseRCombo);
        }

        public static void UseSpells(bool useQ, bool useW, bool useE, bool useR)
        {
            var qTarget = TargetSelector.GetTarget(SPELLS.Q.Range, DamageType.Magical);
            var qDummyTarget = TargetSelector.GetTarget(SPELLS.QDummy.Range, DamageType.Magical);
            var wTarget = TargetSelector.GetTarget(SPELLS.W.Range, DamageType.Magical);
            var eTarget = TargetSelector.GetTarget(SPELLS.E.Range, DamageType.Magical);
            var rTarget = TargetSelector.GetTarget(SPELLS.R.Range, DamageType.Magical);


           if (useW && wTarget != null && SPELLS.W.IsReady())
            {
                SPELLS.W.Cast(wTarget);
                return;
            }

            if (useE && eTarget != null && SPELLS.E.IsReady())
            {
                SPELLS.E.Cast(eTarget);
                return;
            }

            if (useQ && qTarget != null && SPELLS.Q.IsReady() && SPELLS.Q.Handle.ToggleState == 0)
            {
                //if (SPELLS.Q.Cast(qTarget) == Spell.CastStates.SuccessfullyCasted)
                SPELLS.Q.Cast(qTarget);
                return;
            }

            if (qDummyTarget != null && useQ && SPELLS.Q.IsReady() && SPELLS.Q.Handle.ToggleState == 0)
            {
                if (qTarget != null) qDummyTarget = qTarget;
                //   QDummy.Delay = Q.Delay + Q.Range / Q.Speed * 1000 + QSplit.Range / QSplit.Speed * 1000;
                SPELLS.QDummy.CastDelay = 1054;
                var predictedPos = SPELLS.QDummy.GetPrediction(qDummyTarget);
                if (predictedPos.HitChance >= EloBuddy.SDK.Enumerations.HitChance.High)
                {
                    for (var i = -1; i < 1; i = i + 2)
                    {
                        var alpha = 28 * (float)Math.PI / 180;
                        var cp = ObjectManager.Player.ServerPosition.To2D() +
                                 (predictedPos.CastPosition.To2D() - ObjectManager.Player.ServerPosition.To2D()).Rotated
                                     (i * alpha);
                        /*if (
                            Q.GetCollision(ObjectManager.Player.ServerPosition.To2D(), new List<Vector2> { cp }).Count ==
                            0 &&
                            QSplit.GetCollision(cp, new List<Vector2> { predictedPos.CastPosition.To2D() }).Count == 0)*/
                            if (SPELLS.Q.AllowedCollisionCount == 0 && SPELLS.QSplit.AllowedCollisionCount == 0)
                        {
                            SPELLS.Q.Cast(qTarget);
                            //Q.Cast(cp);
                            //return;
                        }
                    }
                }
            }

            if (useR && rTarget != null && SPELLS.R.IsReady() &&
                 /*Player.Instance.GetSpellDamage(rTarget, SpellSlot.R) / 10 * (Player.Instance.Distance(rTarget) < (SPELLS.R.Range - 500) ? 10 : 6) > rTarget.Health &&*/
                 Player.Instance.GetSpellDamage(rTarget, SpellSlot.R) > rTarget.Health && (!Player.HasBuff("VelkozR") ||
                Environment.TickCount > 350))
            {
                SPELLS.R.Cast(rTarget);
            }
        }

    }
}
