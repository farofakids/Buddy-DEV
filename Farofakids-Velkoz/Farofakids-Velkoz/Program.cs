using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using SharpDX;
using Color = System.Drawing.Color;

namespace Farofakids_Velkoz
{
    internal class Program
    {

        public static void Main()
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        public static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.BaseSkinName != "Velkoz") return;
            SPELLS.Initialize();
            MENUS.Initialize();

            Game.OnTick += Game_OnTick;

            Interrupter.OnInterruptableSpell += MODES.Interrupter_OnInterruptableSpell;
            GameObject.OnCreate += SPELLS.GameObject_OnCreate;
            Spellbook.OnUpdateChargeableSpell += SPELLS.Spellbook_OnUpdateChargeableSpell;
        }

        public static void Game_OnUpdate(EventArgs args)
        {

        }

        public static void Drawing_OnDraw(EventArgs args)
        {
        }

        public static void Game_OnTick(EventArgs args)
        {
            if (Player.Instance.IsDead) return;

            if (Player.Instance.Spellbook.IsChanneling)
            {
                var endPoint = new Vector2();
                foreach (var obj in ObjectManager.Get<GameObject>())
                {
                    if (obj != null && obj.IsValid && obj.Name.Contains("Velkoz_") &&
                        obj.Name.Contains("_R_Beam_End"))
                    {
                        endPoint = Player.Instance.ServerPosition.To2D() +
                                   SPELLS.R.Range * (obj.Position - Player.Instance.ServerPosition).To2D().Normalized();
                        break;
                    }
                }

                if (endPoint.IsValid())
                {
                    var targets = new List<Obj_AI_Base>();

                    foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(h => h.IsValidTarget(SPELLS.R.Range)))
                    {
                        if (enemy.ServerPosition.To2D().Distance(Player.Instance.ServerPosition.To2D(), endPoint, true) < 400)
                            targets.Add(enemy);
                    }
                    if (targets.Count > 0)
                    {
                        var target = targets.OrderBy(t => t.Health / Player.Instance.GetSpellDamage(t, SpellSlot.Q)).ToList()[0];
                        ObjectManager.Player.Spellbook.UpdateChargeableSpell(SpellSlot.R, target.ServerPosition, false, false);
                    }
                    else
                    {
                        ObjectManager.Player.Spellbook.UpdateChargeableSpell(SpellSlot.R, Game.CursorPos, false, false);
                    }
                }
                return;
            }

            if (SPELLS.QMissile != null && SPELLS.QMissile.IsValid && SPELLS.Q.Handle.ToggleState == 2 &&
              Environment.TickCount - SPELLS.Q.CastDelay < 2000)
            {
                var qMissilePosition = SPELLS.QMissile.Position.To2D();
                var perpendicular = (SPELLS.QMissile.EndPosition - SPELLS.QMissile.StartPosition).To2D().Normalized().Perpendicular();

                var lineSegment1End = qMissilePosition + perpendicular * SPELLS.QSplit.Range;
                var lineSegment2End = qMissilePosition - perpendicular * SPELLS.QSplit.Range;

                var potentialTargets = new List<Obj_AI_Base>();
                foreach (
                    var enemy in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(
                                h =>
                                    h.IsValidTarget() &&
                                    h.ServerPosition.To2D()
                                        .Distance(qMissilePosition, SPELLS.QMissile.EndPosition.To2D(), true) < 700))
                {
                    potentialTargets.Add(enemy);
                }

                SPELLS.QSplit.SourcePosition = qMissilePosition.To3D();

                foreach (
                    var enemy in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(
                                h =>
                                    h.IsValidTarget() &&
                                    (potentialTargets.Count == 0 ||
                                     h.NetworkId == potentialTargets.OrderBy(t => t.Health / Player.Instance.GetSpellDamage(t, SpellSlot.Q)).ToList()[0].NetworkId) &&
                                    (h.ServerPosition.To2D().Distance(qMissilePosition, SPELLS.QMissile.EndPosition.To2D(), true) > SPELLS.Q.Width + h.BoundingRadius)))
                {
                    var prediction = SPELLS.QSplit.GetPrediction(enemy);
                    var d1 = prediction.UnitPosition.To2D().Distance(qMissilePosition, lineSegment1End, true);
                    var d2 = prediction.UnitPosition.To2D().Distance(qMissilePosition, lineSegment2End, true);
                    if (prediction.HitChance >= EloBuddy.SDK.Enumerations.HitChance.High &&
                        (d1 < SPELLS.QSplit.Width + enemy.BoundingRadius || d2 < SPELLS.QSplit.Width + enemy.BoundingRadius))
                    {
                        SPELLS.Q.Cast();
                    }
                }
            }

            switch (Orbwalker.ActiveModesFlags)
            {
                case Orbwalker.ActiveModes.Combo:
                    MODES.Combo();
                    break;
            }
        }
    }
}