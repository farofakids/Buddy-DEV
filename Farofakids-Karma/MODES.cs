using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Enumerations;
using SharpDX;

namespace Farofakids_Karma
{
    internal class MODES
    {
        public static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (!MENUS.InterruptSpells) return;
            if (sender.IsValidTarget(1000) && args.DangerLevel == DangerLevel.High && SPELLS.E.IsReady())
            {
                SPELLS.R.Cast();

                if (!SPELLS.R.IsReady())
                {
                    SPELLS.E.Cast(ObjectManager.Player);
                }
            }
        }

        public static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs gapcloser)
        {
            if (gapcloser.Sender.IsValidTarget(300))
            {
                SPELLS.E.Cast(ObjectManager.Player);
                SPELLS.Q.Cast(gapcloser.Sender);
            }
        }

        public static void Combo()
        {
            var qTarget = TargetSelector.GetTarget(SPELLS.Q.Range, DamageType.Magical);
            var wTarget = TargetSelector.GetTarget(SPELLS.W.Range, DamageType.Magical);

            if (MENUS.UseWCombo && wTarget != null && SPELLS.W.IsReady())
            {
                if ((ObjectManager.Player.Health / ObjectManager.Player.MaxHealth) /
                    (qTarget.Health / qTarget.MaxHealth) < 1)
                {
                    if (MENUS.UseRCombo)
                    {
                        SPELLS.R.Cast();
                    }

                    if (!MENUS.UseRCombo || !SPELLS.R.IsReady())
                    {
                        SPELLS.W.Cast(wTarget);
                    }
                }
            }

            if (MENUS.UseQCombo && qTarget != null && SPELLS.Q.IsReady())
            {
                if (MENUS.UseRCombo)
                {
                    SPELLS.R.Cast();
                }

                if (!MENUS.UseRCombo || !SPELLS.R.IsReady())
                {
                    var qPrediction = SPELLS.Q.GetPrediction(qTarget);
                    if (qPrediction.HitChance >= HitChance.High)
                    {
                        SPELLS.Q.Cast(qPrediction.CastPosition);
                    }
                    else if (qPrediction.HitChance == HitChance.Collision)
                    {
                        var minionsHit = qPrediction.CollisionObjects;
                        var closest =
                            minionsHit.Where(m => m.NetworkId != ObjectManager.Player.NetworkId)
                                .OrderBy(m => m.Distance(ObjectManager.Player))
                                .FirstOrDefault();

                        if (closest != null && closest.Distance(qPrediction.UnitPosition) < 200)
                        {
                            SPELLS.Q.Cast(qPrediction.CastPosition);
                        }
                    }
                }
            }

            if (MENUS.UseWCombo && wTarget != null)
            {
                SPELLS.W.Cast(wTarget);
            }
        }

        public static void Harras()
        {
            var qTarget = TargetSelector.GetTarget(SPELLS.Q.Range, DamageType.Magical);
            var wTarget = TargetSelector.GetTarget(SPELLS.W.Range, DamageType.Magical);

            if (MENUS.UseWHarass && wTarget != null && SPELLS.W.IsReady())
            {
                if ((ObjectManager.Player.Health / ObjectManager.Player.MaxHealth) /
                    (qTarget.Health / qTarget.MaxHealth) < 1)
                {
                    if (MENUS.UseRHarass)
                    {
                        SPELLS.R.Cast();
                    }

                    if (!MENUS.UseRHarass || !SPELLS.R.IsReady())
                    {
                        SPELLS.W.Cast(wTarget);
                    }
                }
            }

            if (MENUS.UseQHarass && qTarget != null && SPELLS.Q.IsReady())
            {
                if (MENUS.UseRHarass)
                {
                    SPELLS.R.Cast();
                }

                if (!MENUS.UseRHarass || !SPELLS.R.IsReady())
                {
                    var qPrediction = SPELLS.Q.GetPrediction(qTarget);
                    if (qPrediction.HitChance >= HitChance.High)
                    {
                        SPELLS.Q.Cast(qPrediction.CastPosition);
                    }
                    else if (qPrediction.HitChance == HitChance.Collision)
                    {
                        var minionsHit = qPrediction.CollisionObjects;
                        var closest =
                            minionsHit.Where(m => m.NetworkId != ObjectManager.Player.NetworkId)
                                .OrderBy(m => m.Distance(ObjectManager.Player))
                                .FirstOrDefault();

                        if (closest != null && closest.Distance(qPrediction.UnitPosition) < 200)
                        {
                            SPELLS.Q.Cast(qPrediction.CastPosition);
                        }
                    }
                }
            }

            if (MENUS.UseWHarass && wTarget != null)
            {
                SPELLS.W.Cast(wTarget);
            }
        }
    }
}

