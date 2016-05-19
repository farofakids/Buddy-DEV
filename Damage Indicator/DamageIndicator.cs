using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Rendering;
using EloBuddy.SDK.Menu.Values;

namespace Damage_Indicator
{
    class DamageIndicator
    {
        public delegate float DamageToUnitDelegate(Obj_AI_Base minion);

        private static int _height;
        private static int _width;
        private static int _xOffset;
        private static int _yOffset;

        private static readonly Text Text = new Text("", new Font(FontFamily.GenericSansSerif, 11, FontStyle.Bold)) { Color = System.Drawing.Color.Red };

        private static DamageToUnitDelegate _damageToUnit;

        public static DamageToUnitDelegate DamageToUnit
        {
            get { return _damageToUnit; }

            set
            {
                if (_damageToUnit == null)
                {
                    Drawing.OnEndScene += OnEndScene;
                }

                _damageToUnit = value;
            }
        }

        private static void OnEndScene(EventArgs args)
        {
            if (_damageToUnit == null) return;

            if (Program.Dind)
            {
                foreach (var hero in EntityManager.Heroes.Enemies
                    .Where(x => x.IsValidTarget()
                                && x.IsHPBarRendered))
                {
                    _height = 9;
                    _width = 104;
                    _xOffset = 2;
                    _yOffset = -5 + 14;

                    DrawLine(hero);
                }
            }
        }

        private static void DrawLine(Obj_AI_Base unit)
        {
            var damage = _damageToUnit(unit);
            if (damage <= 0) return;

            var barPos = unit.HPBarPosition;

            var percentHealthAfterDamage = Math.Max(0, unit.TotalShieldHealth() - damage) /
                                           (unit.MaxHealth + unit.AllShield + unit.AttackShield + unit.MagicShield);
            var yPos = barPos.Y + _yOffset;
            var currentHealthPercentage = unit.TotalShieldHealth() /
                                          (unit.MaxHealth + unit.AllShield + unit.AttackShield + unit.MagicShield);

            var startPoint = barPos.X + _xOffset + percentHealthAfterDamage * _width;
            var endPoint = barPos.X + _xOffset + currentHealthPercentage * _width;


            Drawing.DrawLine(startPoint, yPos, endPoint, yPos, _height, Color.MediumVioletRed);

            if (damage > unit.Health)
            {
                Text.X = (int)barPos.X + _xOffset + 130;
                Text.Y = (int)barPos.Y + _xOffset - 13;
                Text.TextValue = "KILLABLE: " + (unit.Health - damage);
                Text.Draw();
            }
            Drawing.DrawLine(startPoint, yPos, startPoint, yPos + _height, 2, Color.Lime);
        }

    }
}