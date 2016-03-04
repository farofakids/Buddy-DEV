using EloBuddy;
using EloBuddy.SDK;

namespace FedKatarinaV2
{
    internal class Damage
    {
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        public static double QDamage(Obj_AI_Base target)
        {
            if (!Player.GetSpell(SpellSlot.Q).IsLearned) return 0;
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (float) (new double[] {60, 85, 110, 135, 160}[Program.Q.Level - 1] + 0.4*_Player.FlatMagicDamageMod));
        }
        public static double EDamage(Obj_AI_Base target)
        {
            if (!Player.GetSpell(SpellSlot.E).IsLearned) return 0;
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (float) (new double[] {40, 70, 100, 130, 160}[Program.E.Level - 1] + 0.2*_Player.FlatMagicDamageMod));
        }
        public static double WDamage(Obj_AI_Base target)
        {
            if (!Player.GetSpell(SpellSlot.W).IsLearned) return 0;
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (float)(new double[] { 40, 75, 110, 145, 180 }[Program.W.Level - 1] + 0.2 * _Player.FlatMagicDamageMod));
        }
        public static double RDamage(Obj_AI_Base target)
        {
            if (!Player.GetSpell(SpellSlot.R).IsLearned) return 0;
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (float) (new double[] {350, 550, 750}[Program.R.Level - 1] + 0.2*_Player.FlatMagicDamageMod));
        }
    }
}