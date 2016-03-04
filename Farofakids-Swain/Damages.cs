using EloBuddy;
using EloBuddy.SDK;

namespace Farofakids_Swain
{
    public static class Damages
    {
        public static float GetTotalDamage(AIHeroClient target)
        {
            // Auto attack
            var damage = Player.Instance.GetAutoAttackDamage(target);

            // Q
            if (SpellManager.Q.IsReady())
            {
                damage += SpellManager.Q.GetRealDamage(target);
            }

            // W
            if (SpellManager.W.IsReady())
            {
                damage += SpellManager.W.GetRealDamage(target);
            }

            // E
            if (SpellManager.E.IsReady())
            {
                damage += SpellManager.E.GetRealDamage(target);
            }

            // R
            if (SpellManager.R.IsReady())
            {
                damage += SpellManager.R.GetRealDamage(target);
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
                    damage = new float[] { 25, 40, 55, 70, 85 }[spellLevel] + 0.3f * Player.Instance.TotalMagicalDamage *3;
                    break;

                case SpellSlot.W:
                    damage = new float[] { 80, 120, 160, 200, 240 }[spellLevel] + 0.7f * Player.Instance.TotalMagicalDamage;
                    break;

                case SpellSlot.E:
                    damage = new float[] { 75, 115, 155, 195, 235 }[spellLevel] + 0.8f * Player.Instance.TotalMagicalDamage *4;
                    break;

                case SpellSlot.R:
                    damage = new float[] { 50, 70, 90 }[spellLevel] + 0.2f * Player.Instance.TotalMagicalDamage;
                    break;
            }

            // No damage set
            if (damage <= 0)
            {
                return 0;
            }

            // Calculate damage on target and return (-20 to make it actually more accurate Kappa)
            return Player.Instance.CalculateDamageOnUnit(target, damageType, damage) - 20;
        }
    }
}
