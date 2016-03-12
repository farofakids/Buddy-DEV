using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using Settings = Farofakids_Swain.Config.Modes.Harass;


namespace Farofakids_Swain.Modes
{
    public sealed class PermaActive : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            // Since this is permaactive mode, always execute the loop
            return true;
        }


        public override void Execute()
        {
            if (Player.Instance.ManaPercent < Settings.ManaAuto)
            {
                return;
            }
            if (Settings.UseQauto && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                if (target != null)
                { 
                Q.Cast(target);
                }
            }
            if (Settings.UseEauto && E.IsReady())
            {
                var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
                if (target != null)
                {
                    E.Cast(target);
                }
            }
        }

    }
}
