using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using SharpDX;
using Color = System.Drawing.Color;

namespace Farofakids_Riven
{
    internal class Program
    {
        public static void Main()
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        public static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.BaseSkinName != "Riven") return;
            SPELLS.Initialize();
            MENUS.Initialize();
            Drawing.OnDraw += Drawing_OnDraw;

            Game.OnTick += Game_OnTick;

        }

        private static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                MODES.Combo();
            }
        }

        public static void Drawing_OnDraw(EventArgs args)
        {
            if (MENUS.QRange && SPELLS.Q.Handle.IsLearned)
                Drawing.DrawCircle(Player.Instance.Position, SPELLS.Q.Range, Color.Red);
            if (MENUS.WRange && SPELLS.W.Handle.IsLearned)
                Drawing.DrawCircle(Player.Instance.Position, SPELLS.W.Range, Color.Red);
            if (MENUS.ERange && SPELLS.E.Handle.IsLearned)
                Drawing.DrawCircle(Player.Instance.Position, SPELLS.E.Range, Color.Red);
            if (MENUS.RRange && SPELLS.R.Handle.IsLearned)
                Drawing.DrawCircle(Player.Instance.Position, SPELLS.R.Range, Color.Red);
        }

    }
}