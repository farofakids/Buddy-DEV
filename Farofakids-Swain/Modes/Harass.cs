using EloBuddy;
using EloBuddy.SDK;
using Settings = Farofakids_Swain.Config.Modes.Harass;

namespace Farofakids_Swain.Modes
{
    public sealed class Harass : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass);
        }

        public override void Execute()
        {
            if (Player.Instance.ManaPercent < Settings.Mana)
            {
                return;
            }
            var target = TargetSelector.GetTarget(800, DamageType.Magical);
            if (target != null)
            {
                if (Settings.UseE && E.IsReady())
                {
                    E.Cast(target);
                }
                if (Settings.UseQ && Q.IsReady())
                {
                    Q.Cast(target);
                }

                if (Settings.UseW && W.IsReady() && target.IsValidTarget(W.Range) && Program.SafeWCast(target))
                {
                    var pred = W.GetPrediction(target);
                    W.Cast(pred.CastPosition);
                }
            }
        }
    }
}
