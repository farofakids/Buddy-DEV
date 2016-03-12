using System.Linq;
using EloBuddy;
using EloBuddy.SDK;


// Using the config like this makes your life easier, trust me
using Settings = Farofakids_Swain.Config.Modes.Combo;

namespace Farofakids_Swain.Modes
{
    public sealed class Combo : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            // Only execute this mode when the orbwalker is on combo mode
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
        }

        public override void Execute()  
        {
            // TODO: Add combo logic here
            // See how I used the Settings.UseQ here, this is why I love my way of using
            // the menu in the Config class!
            var target = TargetSelector.GetTarget(800, DamageType.Magical);
            if (target != null) 
            {
                if (Settings.UseZhonya &&  Program.Zhonyas.IsReady() && target.IsValidTarget(550))
                {
                    if (Program.RavenForm && Player.Instance.HealthPercent <= Settings.UseZhonyaheal)
                    { 
                    Program.Zhonyas.Cast();
                    }
                }
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
                if (Settings.UseR && R.IsReady() && target.IsValidTarget(R.Range) && !Program.RavenForm)
                {
                    R.Cast();
                }

            }
        }

    }
}
