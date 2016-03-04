using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using Settings = Farofakids_Swain.Config.Modes.LaneClear;

namespace Farofakids_Swain.Modes
{
    public sealed class LaneClear : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            // Only execute this mode when the orbwalker is on laneclear mode
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear);
        }

        public override void Execute()
        {
            if (Player.Instance.ManaPercent < Settings.Mana)
            {
                return;
            }
            var minion = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.IsValidTarget(R.Range));
            if (Settings.UseR && R.IsReady() && !Program.RavenForm)
            {
                if (minion.Count(m => m.Distance(Player.Instance.ServerPosition) < R.Range) >= Settings.MinNumberR)
                {
                    R.Cast();
                }
            }
        }
    }
}
