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
            if (Program.HasSpell("SummonerDot"))
            { 
            if (_ignite.IsReady() || !Player.Instance.IsDead)
            { 
            if (!Config.Modes.Combo.UseIgnite) return;
            foreach (
                var source in
                    EntityManager.Heroes.Enemies
                        .Where(
                            a => a.IsValidTarget(_ignite.Range) &&
                                a.Health < 50 + 20 * Player.Instance.Level - (a.HPRegenRate / 5 * 3)))
            {
                _ignite.Cast(source);
                return;
            }
            }
            }
            

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
