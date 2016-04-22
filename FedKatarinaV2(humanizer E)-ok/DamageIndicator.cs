using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace FedKatarinaV2
{
    public class DamageIndicator
    {
        public delegate float DamageToUnitDelegate(Obj_AI_Base minion);

        private static int _height;
        private static int _width;
        private static int _xOffset;
        private static int _yOffset;

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

            if (Program.DrawingMenu["draw.enemyDmg"].Cast<CheckBox>().CurrentValue)
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

            //Get remaining HP after damage applied in percent and the current percent of health
            var percentHealthAfterDamage = Math.Max(0, unit.TotalShieldHealth() - damage) /
                                           (unit.MaxHealth + unit.AllShield + unit.AttackShield + unit.MagicShield);
            var currentHealthPercentage = unit.TotalShieldHealth() /
                                          (unit.MaxHealth + unit.AllShield + unit.AttackShield + unit.MagicShield);

            //Calculate start and end point of the bar indicator
            var startPoint = barPos.X + _xOffset + percentHealthAfterDamage * _width;
            var endPoint = barPos.X + _xOffset + currentHealthPercentage * _width;
            var yPos = barPos.Y + _yOffset;

            //Draw the line
            Drawing.DrawLine(startPoint, yPos, endPoint, yPos, _height, Color.MediumVioletRed);
        }
    }
}
