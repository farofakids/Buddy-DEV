using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using SharpDX;

namespace Farofakids_Riven
{
    internal class SPELLS
    {
        public static Spell.Active Q, W, E, R;
        public static Spell.Skillshot R2;


        public static void Initialize()
        {
            Q = new Spell.Active(SpellSlot.Q, 275);
            W = new Spell.Active(SpellSlot.W, 260);
            E = new Spell.Active(SpellSlot.E, 250);
            R = new Spell.Active(SpellSlot.R, 200);
            R2 = new Spell.Skillshot(SpellSlot.R, 900, SkillShotType.Circular, 250, 1600, 45);
        }

    }
}
