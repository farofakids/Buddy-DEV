using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Rendering;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK;
using SharpDX;

namespace Farofakids_Swain
{
    public static class Program
    {
        // Change this line to the champion you want to make the addon for,
        // watch out for the case being correct!
        public const string ChampName = "Swain";
        public static bool RavenForm;

        public static readonly Item Zhonyas = new Item(ItemId.Zhonyas_Hourglass);
        

        public static void Main(string[] args)
        {
            // Wait till the loading screen has passed
            Loading.OnLoadingComplete += OnLoadingComplete;
            GameObject.OnCreate += OnCreateObject;
            GameObject.OnDelete += OnDeleteObject;
        }

        private static void OnLoadingComplete(EventArgs args)
        {
            // Verify the champion we made this addon for
            if (Player.Instance.ChampionName != ChampName)
            {
                // Champion is not the one we made this addon for,
                // therefore we return
                return;
            }

            // Initialize the classes that we need
            Config.Initialize();
            SpellManager.Initialize();
            ModeManager.Initialize();

            // Listen to events we need
            DamageIndicator.Initialize(Damages.GetTotalDamage);
            DamageIndicator.DrawingColor = System.Drawing.Color.Aqua;
            Drawing.OnDraw += OnDraw;
            Interrupter.OnInterruptableSpell += OnInterruptableSpell;
            Gapcloser.OnGapcloser += Gapcloser_OnGap;
        }

        private static void OnDraw(EventArgs args)
        {
            // All circles
            foreach (var spell in SpellManager.Spells)
            {
                switch (spell.Slot)
                {
                    case SpellSlot.Q:
                        if (!Config.Modes.Drawing.DrawQ)
                        {
                            continue;
                        }
                        break;
                    case SpellSlot.W:
                        if (!Config.Modes.Drawing.DrawW)
                        {
                            continue;
                        }
                        break;
                    case SpellSlot.E:
                        if (!Config.Modes.Drawing.DrawE)
                        {
                            continue;
                        }
                        break;
                    case SpellSlot.R:
                        if (!Config.Modes.Drawing.DrawR)
                        {
                            continue;
                        }
                        break;
                }

                Circle.Draw(spell.GetColor(), spell.Range, Player.Instance);

                DamageIndicator.HealthbarEnabled = Config.Modes.Drawing.IndicatorHealthbar;
            }

        }

        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
            if (!(sender.Name.Contains("swain_demonForm")))
                return;
            RavenForm = true;
        }

        private static void OnDeleteObject(GameObject sender, EventArgs args)
        {
            if (!(sender.Name.Contains("swain_demonForm")))
                return;
            RavenForm = false;
        }

        public static bool SafeWCast(AIHeroClient target)
        {
            if (target == null)
                return false;

            if (SpellManager.W.GetPrediction(target).HitChance == HitChance.Immobile)
                return true;
            if (target.HasBuffOfType(BuffType.Slow) && SpellManager.W.GetPrediction(target).HitChance >= HitChance.High)
                return true;
            return SpellManager.W.GetPrediction(target).HitChance == HitChance.High;
            
        }

        public static bool HasSpell(string s)
        {
            return Player.Spells.FirstOrDefault(o => o.SData.Name.Contains(s)) != null;
        }

        private static void OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (sender.IsEnemy && args.DangerLevel == DangerLevel.High && Config.Modes.Misc.UseWint && SpellManager.W.IsReady() && SpellManager.W.IsInRange(sender))
            {
                SpellManager.W.Cast(sender);
            }
        }
        private static void Gapcloser_OnGap(AIHeroClient Sender, Gapcloser.GapcloserEventArgs args)
        {
            var predw = SpellManager.W.GetPrediction(Sender);
            if (Sender.IsValidTarget(SpellManager.W.Range) && Config.Modes.Misc.UseWint && SpellManager.W.IsReady() && !Sender.IsAlly && !Sender.IsMe)
            {
                SpellManager.W.Cast(predw.CastPosition);
            }
        }
    }
}
