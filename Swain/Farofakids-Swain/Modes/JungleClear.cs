using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using Settings = Farofakids_Swain.Config.Modes.LaneClear;

namespace Farofakids_Swain.Modes
{
    public sealed class JungleClear : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            // Only execute this mode when the orbwalker is on jungleclear mode
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear);
        }

        public override void Execute()
        {
            if (Player.Instance.ManaPercent < Settings.Mana)
            {
                return;
            }
            var minion = EntityManager.MinionsAndMonsters.Monsters.Where(m => m.IsValidTarget(R.Range));
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
