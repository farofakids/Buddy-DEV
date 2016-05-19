using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using SharpDX;

namespace Farofakids_Velkoz
{
    internal class SPELLS
    {
        public static Spell.Skillshot Q, W, E, R, QSplit, QDummy;
        public static MissileClient QMissile;

        public static void Initialize()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 1200, SkillShotType.Linear, 250, 1300, 50);
            QSplit = new Spell.Skillshot(SpellSlot.Q, 1100, SkillShotType.Linear, 250, 2100, 55);
            QDummy = new Spell.Skillshot(SpellSlot.Q, (uint)Math.Sqrt(Math.Pow(Q.Range, 2) + Math.Pow(QSplit.Range, 2)), SkillShotType.Linear, 300, int.MaxValue, 1);
            W = new Spell.Skillshot(SpellSlot.W, 1200, SkillShotType.Linear, 250, 1700, 85);
            E = new Spell.Skillshot(SpellSlot.E, 800, SkillShotType.Circular, 500, 1500, 100);
            R = new Spell.Skillshot(SpellSlot.R, 1550, SkillShotType.Linear, 300, int.MaxValue, 1);

        }

        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;

            //if (Q.IsReady() && Q.GetCollision(ObjectManager.Player.ServerPosition.To2D(), new List<Vector2> { enemy.ServerPosition.To2D() }).Count == 0)
            if (Q.IsReady())
                damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.Q);

            if (W.IsReady())
            damage += W.Handle.Ammo * 
                    Player.Instance.GetSpellDamage(enemy, SpellSlot.W);

            if (E.IsReady())
                damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.E);

            if (R.IsReady())
                damage += 7 * Player.Instance.GetSpellDamage(enemy, SpellSlot.R) / 10;

            return (float)damage;
        }

        public static void Obj_SpellMissile_OnCreate(GameObject sender, EventArgs args)
        {
            if (!(sender is MissileClient)) return;
            var missile = (MissileClient)sender;
            if (missile.SpellCaster != null && missile.SpellCaster.IsValid && missile.SpellCaster.IsMe &&
                missile.SData.Name.Equals("VelkozQMissile", StringComparison.InvariantCultureIgnoreCase))
            {
                QMissile = missile;
            }
        }

        public static void Spellbook_OnUpdateChargeableSpell(Spellbook sender, SpellbookUpdateChargeableSpellEventArgs args)
        {
            if (sender.Owner.IsMe)
            {
                args.Process =
                        !(Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.Combo &&
                        MENUS.UseRCombo);
            }
        }

        public static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (!(sender is MissileClient)) return;
            var missile = (MissileClient)sender;
            if (missile.SpellCaster != null && missile.SpellCaster.IsValid && missile.SpellCaster.IsMe &&
                missile.SData.Name.Equals("VelkozQMissile", StringComparison.InvariantCultureIgnoreCase))
            {
                QMissile = missile;
            }
        }

    }
}
