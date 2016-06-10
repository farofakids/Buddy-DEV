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

namespace Farofakids_Riven
{
    internal class MODES
    {
        internal static void Combo()
        {
            var target = TargetSelector.GetTarget(SPELLS.E.Range + SPELLS.W.Range + 200, DamageType.Physical);
            if (target == null) return;
            try
            {
                if (SPELLS.R.IsReady())
                {
                    SPELLS.R.Cast();
                }
                if (target.Distance(Player.Instance) > SPELLS.W.Range && SPELLS.E.IsReady())
                {
                    Player.CastSpell(SpellSlot.E, target.Position);
                }
                if (target.Distance(Player.Instance) <= SPELLS.W.Range && SPELLS.W.IsReady())
                {
                    SPELLS.W.Cast();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

}

