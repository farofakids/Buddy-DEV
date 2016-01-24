using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using SharpDX.Direct3D9;

namespace Zed
{
   /* internal class DamageIndicator
    {
        public delegate float DamageToUnitDelegate(AIHeroClient hero);

        private const int XOffset = 10;
        private const int YOffset = 20;
        private const int Width = 103;
        private const int Height = 8;
        public static Color Color = Color.Lime;
        public static Color FillColor = Color.Goldenrod;
        private static DamageToUnitDelegate _damageToUnit;

       private static readonly Render.Text Text = new Render.Text(0, 0, "", 11, new ColorBGRA(255, 0, 0, 255), "monospace");


          public static DamageToUnitDelegate DamageToUnit
        {
            get { return _damageToUnit; }

            set
            {
                if (_damageToUnit == null)
                {
                    Drawing.OnDraw += Drawing_OnDraw;
                }
                _damageToUnit = value;
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {

            foreach (
                var unit in ObjectManager.Get<AIHeroClient>().Where(h => h.IsValid && h.IsEnemy && h.IsHPBarRendered))
            {
                var barPos = unit.HPBarPosition;
                var damage = _damageToUnit(unit);
                var percentHealthAfterDamage = Math.Max(0, unit.Health - damage) / unit.MaxHealth;
                var yPos = barPos.Y + YOffset;
                var xPosDamage = barPos.X + XOffset + Width * percentHealthAfterDamage;
                var xPosCurrentHp = barPos.X + XOffset + Width * unit.Health / unit.MaxHealth;

                if (damage > unit.Health)
                {
                    Text.X = (int)barPos.X + XOffset;
                    Text.Y = (int)barPos.Y + YOffset - 13;
                    Text.text = ((int)(unit.Health - damage)).ToString();
                    Text.OnEndScene();
                }

                Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + Height / 2, 2, Color);

                if (Program._config.Item("DrawHP").GetValue<bool>())
                {
                    var differenceInHP = xPosCurrentHp - xPosDamage;
                    var pos1 = barPos.X + 9 + (107 * percentHealthAfterDamage);

                    for (var i = 0; i < differenceInHP; i++)
                    {
                        Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height / 2, 1, FillColor);
                    }
                }
            }
        }
    }*/

    internal class DamageIndicator
    {
        private const int BarWidth = 106;
        private const float LineThickness = 9.8f;

        public delegate float DamageToUnitDelegate(AIHeroClient hero);

        //private static DamageToUnitDelegate DamageToUnit { get; set; }
        private static DamageToUnitDelegate _damageToUnit;

        public static DamageToUnitDelegate DamageToUnit
        {
            get { return _damageToUnit; }

            set
            {
                if (_damageToUnit == null)
                {
                    Drawing.OnDraw += OnEndScene;
                }
                _damageToUnit = value;
            }
        }

        private static Font _Font;

        public static void Initialize(DamageToUnitDelegate damageToUnit)
        {
            DamageToUnit = damageToUnit;
            Drawing.OnEndScene += OnEndScene;

            _Font = new Font(
                Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Segoi UI",
                    Height = 18,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.Default
                });
        }

        private static void OnEndScene(EventArgs args)
        {
           // if (Settings.DrawHealth || Settings.DrawPercent)
            {
                foreach (var unit in EntityManager.Heroes.Enemies.Where(u => u.IsValidTarget() && u.IsHPBarRendered))
                {
                    var damage = DamageToUnit(unit);

                    if (damage <= 0)
                    {
                        continue;
                    }

            //        if (Settings.DrawHealth)
                    {
                        var damagePercentage = ((unit.TotalShieldHealth() - damage) > 0 ? (unit.TotalShieldHealth() - damage) : 0) /
                                               (unit.MaxHealth + unit.AllShield + unit.AttackShield + unit.MagicShield);
                        var currentHealthPercentage = unit.TotalShieldHealth() / (unit.MaxHealth + unit.AllShield + unit.AttackShield + unit.MagicShield);

                        var startPoint = new Vector2((int)(unit.HPBarPosition.X + damagePercentage * BarWidth), (int)unit.HPBarPosition.Y - 5);
                        var endPoint = new Vector2((int)(unit.HPBarPosition.X + currentHealthPercentage * BarWidth) + 1, (int)unit.HPBarPosition.Y - 5);

                      //  var colorH = System.Drawing.Color.FromArgb(Settings.HealthColor.A - 120, Settings.HealthColor.R,
                        //    Settings.HealthColor.G, Settings.HealthColor.B);

                       // Drawing.DrawLine(startPoint, endPoint, LineThickness, colorH);
                    }

              //      if (Settings.DrawPercent)
                    {
                        //var color = new Color(Settings.HealthColor.R, Settings.HealthColor.G, Settings.HealthColor.B, Settings.HealthColor.A - 5);
                     //   _Font.DrawText(null, string.Concat(Math.Ceiling(damage / unit.TotalShieldHealth() * 100), "%"), (int)unit.HPBarPosition[0] + 102, (int)unit.HPBarPosition[1] + 29, color);
                    }
                }
            }
        }
    }
}
