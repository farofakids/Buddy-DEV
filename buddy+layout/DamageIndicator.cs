using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using SharpDX.Direct3D9;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace buddy_layout
{
    public static class DamageIndicator
    {
        private const int BarWidth = 104;
        private const int LineThickness = 9;

        public delegate float DamageToUnitDelegate(AIHeroClient hero);

        private static DamageToUnitDelegate DamageToUnit { get; set; }

        private static readonly Vector2 BarOffset = new Vector2(1, 0);

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
            if (Program.DrawingsMenu["damageIndicatorDraw"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var unit in EntityManager.Heroes.Enemies.Where(u => u.IsValidTarget() && u.IsHPBarRendered))
                {
                    var damage = DamageToUnit(unit);

                    if (damage <= 0)
                    {
                        continue;
                    }

                    if (Program.DrawingsMenu["damageIndicatorDraw"].Cast<CheckBox>().CurrentValue)
                    {
                        var damagePercentage = ((unit.TotalShieldHealth() - damage) > 0 ? (unit.TotalShieldHealth() - damage) : 0) /
                                                (unit.MaxHealth + unit.AllShield + unit.AttackShield + unit.MagicShield);
                        var currentHealthPercentage = unit.TotalShieldHealth() / (unit.MaxHealth + unit.AllShield + unit.AttackShield + unit.MagicShield);

                        var startPoint = new Vector2((int)(unit.HPBarPosition.X + BarOffset.X + damagePercentage * BarWidth), (int)(unit.HPBarPosition.Y + BarOffset.Y) + 10);
                        var endPoint = new Vector2((int)(unit.HPBarPosition.X + BarOffset.X + currentHealthPercentage * BarWidth) + 1, (int)(unit.HPBarPosition.Y + BarOffset.Y) + 10);

                        Drawing.DrawLine(startPoint, endPoint, LineThickness, System.Drawing.Color.Aquamarine);
                    }
                }
            }
        }
    }
}
