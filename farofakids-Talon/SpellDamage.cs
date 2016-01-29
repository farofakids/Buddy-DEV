using EloBuddy;
using EloBuddy.SDK;

namespace Talon
{
    public static class SpellDamage
    {
        public static float GetTotalDamage(AIHeroClient target)
        {
            // Auto attack
            var damage = Player.Instance.GetAutoAttackDamage(target);

            // Q
            if (Program.Q.IsReady())
            {
                damage += Program.Q.GetRealDamage(target);
            }

            // W
            if (Program.W.IsReady())
            {
                damage += Program.W.GetRealDamage(target);
            }

            // E
            if (Program.E.IsReady())
            {
                damage += Program.E.GetRealDamage(target);
            }

            // R
            if (Program.R.IsReady())
            {
                damage += Program.R.GetRealDamage(target);
            }

            return damage;
        }

        public static float GetRealDamage(this Spell.SpellBase spell, Obj_AI_Base target)
        {
            return spell.Slot.GetRealDamage(target);
        }

        public static float GetRealDamage(this SpellSlot slot, Obj_AI_Base target)
        {
            // Helpers
            var spellLevel = Player.Instance.Spellbook.GetSpell(slot).Level;
            const DamageType damageType = DamageType.Magical;
            float damage = 0;

            // Validate spell level
            if (spellLevel == 0)
            {
                return 0;
            }
            spellLevel--;

            switch (slot)
            {
                case SpellSlot.Q:

                    damage = new float[] { 30, 60, 90, 120, 150 }[spellLevel] + 0.3f * Player.Instance.FlatPhysicalDamageMod + Player.Instance.GetAutoAttackDamage(target);
                    break;

                case SpellSlot.W:

                    damage = new float[] { 60, 110, 160, 210, 260 }[spellLevel] + 0.6f * Player.Instance.FlatPhysicalDamageMod;
                    break;

                case SpellSlot.E:

                    damage = new float[] { 0, 0, 0, 0, 0 }[spellLevel] + 0.0f * Player.Instance.FlatMagicDamageMod;
                    break;

                case SpellSlot.R:

                    damage = new float[] { 120, 170, 220 }[spellLevel] + 0.75f * Player.Instance.FlatPhysicalDamageMod;
                    break;

            }

            if (damage <= 0)
            {
                return 0;
            }

            return Player.Instance.CalculateDamageOnUnit(target, damageType, damage) - 10;
        }
    }
}
