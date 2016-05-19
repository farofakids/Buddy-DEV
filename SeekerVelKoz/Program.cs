using System;
using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using SharpDX;
using Color = System.Drawing.Color;

namespace SeekerVelKoz
{
    internal class Program
    {
        // Grab Player Attributes
        public static AIHeroClient Champion { get { return Player.Instance; } }
        public static int ChampionSkin;

        public static void Main()
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        public static void Loading_OnLoadingComplete(EventArgs args)
        {
            // Validate Player.Instace is Addon Champion
            if (Champion.ChampionName != "Velkoz") return;
            ChampionSkin = Champion.SkinId;

            // Initialize classes
            SpellManager.Initialize();
            MenuManager.Initialize();
            SpellManager.Perpendiculars = new List<Vector2>();

            // Listen to Events
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Game.OnTick += Game_OnTick;
            //Game.OnUpdate += SpellManager.QSplit;
            //GameObject.OnCreate += SpellManager.OnCreate;
            Interrupter.OnInterruptableSpell += ModeManager.InterruptMode;
            Gapcloser.OnGapcloser += ModeManager.GapCloserMode;
        }

        public static void Game_OnUpdate(EventArgs args)
        {
            // Initialize Skin Designer
            Player.SetSkinId(MenuManager.DesignerMode
                ? MenuManager.DesignerSkin
                : ChampionSkin);
        }

        public static void Drawing_OnDraw(EventArgs args)
        {
            // Wait for Game Load
            if (Game.Time < 10) return;

            // No Responce While Dead
            if (Champion.IsDead) return;


            Color color;

            // Setup Designer Coloration
            switch (Champion.SkinId)
            {
                default:
                    color = Color.Transparent;
                    break;
                case 0:
                    color = Color.FromArgb(168, 27, 168);
                    break;
                case 1:
                    color = Color.FromArgb(254, 35, 39);
                    break;
                case 2:
                    color = Color.FromArgb(240, 187, 55);
                    break;
            }

            // Apply Designer Color into Circle
            if (!MenuManager.DrawerMode) return;
            if (MenuManager.DrawQw && (SpellManager.Q.IsLearned || SpellManager.W.IsLearned))
                Drawing.DrawCircle(Champion.Position, SpellManager.Q.Range, color);
            if (MenuManager.DrawE && SpellManager.E.IsLearned)
                Drawing.DrawCircle(Champion.Position, SpellManager.E.Range, color);
            if (MenuManager.DrawR && SpellManager.R.IsLearned)
                Drawing.DrawCircle(Champion.Position, SpellManager.R.Range, color);
        }

        public static void Game_OnTick(EventArgs args)
        {
            // Initialize Leveler
            if (MenuManager.LevelerMode && Champion.SpellTrainingPoints >= 1)
                LevelerManager.Initialize();

            // No Responce While Dead
            if (Champion.IsDead) return;

            // Mode Activation
            if (MenuManager.UltimateFollower && Champion.HasBuff("VelkozR"))
                ModeManager.UltFollowMode();

            switch (Orbwalker.ActiveModesFlags)
            {
                case Orbwalker.ActiveModes.Combo:
                    ModeManager.ComboMode();
                    break;
                case Orbwalker.ActiveModes.Harass:
                    ModeManager.HarassMode();
                    break;
                case Orbwalker.ActiveModes.JungleClear:
                    ModeManager.JungleMode();
                    break;
                case Orbwalker.ActiveModes.LaneClear:
                    ModeManager.LaneClearMode();
                    break;
                case Orbwalker.ActiveModes.LastHit:
                    ModeManager.LastHitMode();
                    break;
            }
            if (MenuManager.KsMode)
                ModeManager.KsMode();
            if (MenuManager.StackMode)
                ModeManager.StackMode();
        }
    }
}