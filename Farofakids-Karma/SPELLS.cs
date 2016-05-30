using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using SharpDX;

namespace Farofakids_Karma
{
    internal class SPELLS
    {
        public static Spell.Skillshot Q;
        public static Spell.Active R;
        public static Spell.Targeted W, E;

        public static void Initialize()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 950, SkillShotType.Linear, 250, 1700, 60);
            W = new Spell.Targeted(SpellSlot.W, 700)
            {
                CastDelay = 250
            };
            E = new Spell.Targeted(SpellSlot.E, 800)
            {
                CastDelay = 250
            };
            R = new Spell.Active(SpellSlot.R, 1100);
        }

        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;

            if (Q.IsReady())
                damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.Q);

            if (W.IsReady())
                damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.W);

            if (E.IsReady())
                damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.E);

            if (R.IsReady())
                damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.R);

            return (float)damage;
        }

    }
}
