using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;

namespace SeekerVelKoz
{
    internal class ModeManager
    {
        // Clone Character Object
        public static AIHeroClient Champion = Program.Champion;

        public static void ComboMode()
        {
            if (MenuManager.ComboUseQ)
            {
                var target = TargetManager.GetChampionTarget(SpellManager.Q.Range, DamageType.Magical, false, true);
                if (target != null)
                    SpellManager.CastQ(target);
            }
            if (MenuManager.ComboUseE)
            {
                var target = TargetManager.GetChampionTarget(SpellManager.E.Range, DamageType.Magical);
                if (target != null && ((MenuManager.ComboESlow && target.HasBuffOfType(BuffType.Slow)) || !MenuManager.ComboESlow))
                    SpellManager.CastE(target);
            }
            if (MenuManager.ComboUseW)
            {
                var target = TargetManager.GetChampionTarget(SpellManager.W.Range, DamageType.Magical);
                if (target != null && ((MenuManager.ComboWKnock && target.HasBuffOfType(BuffType.Knockback)) || !MenuManager.ComboWKnock))
                    SpellManager.CastW(target);
            }
            if (MenuManager.ComboUseR && Champion.CountEnemiesInRange(SpellManager.R.Range) >= MenuManager.ComboRLimiter && !Champion.IsUnderTurret())
            {
                var target = TargetManager.GetChampionTarget(SpellManager.R.Range, DamageType.Magical);
                if (target != null && ((MenuManager.ComboRCooldown && !SpellManager.Q.IsReady() && !SpellManager.W.IsReady() && !SpellManager.E.IsReady()) || !MenuManager.ComboRCooldown))
                    SpellManager.CastR(target);
            }
        }

        public static void HarassMode()
        {
            if (Champion.ManaPercent <= MenuManager.HarassMana) return;
            if (MenuManager.HarassUseQ)
            {
                var target = TargetManager.GetChampionTarget(SpellManager.Q.Range, DamageType.Magical, false, true);
                if (target != null)
                    SpellManager.CastQ(target);
            }
            if (MenuManager.HarassUseE)
            {
                var target = TargetManager.GetChampionTarget(SpellManager.E.Range, DamageType.Magical);
                if (target != null)
                    SpellManager.CastE(target);
            }
        }

        public static void JungleMode()
        {
            if (Champion.ManaPercent < MenuManager.JungleMana) return;
            if (MenuManager.JungleUseQ)
            {
                var target = TargetManager.GetMinionTarget(SpellManager.Q.Range, DamageType.Magical, false, true, true);
                if (target != null)
                    SpellManager.CastQ(target);
            }
            if (MenuManager.JungleUseE)
            {
                var target = TargetManager.GetMinionTarget(SpellManager.E.Range, DamageType.Magical, false, true);
                if (target != null)
                    SpellManager.CastE(target);
            }
            if (MenuManager.JungleUseW)
            {
                var target = TargetManager.GetMinionTarget(SpellManager.W.Range, DamageType.Magical, false, true);
                if (target != null)
                    SpellManager.CastW(target);
            }
        }

        public static void LaneClearMode()
        {
            if (Champion.ManaPercent < MenuManager.LaneClearMana) return;
            if (MenuManager.LaneClearUseQ)
            {
                var target = TargetManager.GetMinionTarget(SpellManager.Q.Range, DamageType.Magical, false, false, true);
                if (target != null)
                    SpellManager.CastQ(target);
            }
            if (MenuManager.LaneClearUseE)
            {
                var target = TargetManager.GetMinionTarget(SpellManager.E.Range, DamageType.Magical);
                if (target != null)
                    SpellManager.CastE(target);
            }
            if (MenuManager.LaneClearUseW)
            {
                var target = TargetManager.GetMinionTarget(SpellManager.W.Range, DamageType.Magical);
                if (target != null)
                    SpellManager.CastW(target);
            }
        }

        public static void LastHitMode()
        {
            if (Champion.ManaPercent < MenuManager.LastHitMana) return;
            if (MenuManager.LastHitUseQ)
            {
                var target = TargetManager.GetMinionTarget(SpellManager.Q.Range, DamageType.Magical, false, false, true, SpellManager.QDamage());
                if (target != null)
                    SpellManager.CastQ(target);
            }
            if (MenuManager.LastHitUseE)
            {
                var target = TargetManager.GetMinionTarget(SpellManager.E.Range, DamageType.Magical, false, false, false, SpellManager.EDamage());
                if (target != null)
                    SpellManager.CastE(target);
            }
            if (MenuManager.LastHitUseW)
            {
                var target = TargetManager.GetMinionTarget(SpellManager.W.Range, DamageType.Magical, false, false, false, SpellManager.WDamage());
                if (target != null)
                    SpellManager.CastW(target);
            }
        }

        public static void KsMode()
        {
            if (MenuManager.KsUseQ)
            {
                var target = TargetManager.GetChampionTarget(SpellManager.Q.Range, DamageType.Magical, false, true, SpellManager.QDamage());
                if (target != null)
                    SpellManager.CastQ(target);
            }
            if (MenuManager.KsUseE)
            {
                var target = TargetManager.GetChampionTarget(SpellManager.E.Range, DamageType.Magical, false, false, SpellManager.EDamage());
                if (target != null)
                    SpellManager.CastE(target);
            }
            if (MenuManager.KsUseW)
            {
                var target = TargetManager.GetChampionTarget(SpellManager.W.Range, DamageType.Magical, false, false, SpellManager.WDamage());
                if (target != null)
                    SpellManager.CastW(target);
            }
            if (MenuManager.KsUseR && Champion.CountEnemiesInRange(SpellManager.R.Range) >= MenuManager.KsUltLimiter && !Champion.IsUnderTurret())
            {
                var target = TargetManager.GetChampionTarget(SpellManager.R.Range, DamageType.Magical, false, false, SpellManager.RDamage());
                if (target != null)
                    SpellManager.CastR(target);
            }
        }

        public static void UltFollowMode()
        {
            var target = TargetManager.GetChampionTarget(SpellManager.R.Range, DamageType.Magical);
            if (target != null)
                Champion.Spellbook.UpdateChargeableSpell(SpellSlot.R, target.ServerPosition, false, false);
            else
            {
                var mtarget = TargetManager.GetMinionTarget(SpellManager.R.Range, DamageType.Magical);
                if (mtarget != null)
                    Champion.Spellbook.UpdateChargeableSpell(SpellSlot.R, mtarget.ServerPosition, false, false);
            }
        }

        public static void StackMode()
        {
            foreach (var item in Champion.InventoryItems)
            {
                if ((item.Id == ItemId.Tear_of_the_Goddess || item.Id == ItemId.Tear_of_the_Goddess_Crystal_Scar ||
                     item.Id == ItemId.Archangels_Staff || item.Id == ItemId.Archangels_Staff_Crystal_Scar ||
                     item.Id == ItemId.Manamune || item.Id == ItemId.Manamune_Crystal_Scar)
                    && Champion.IsInShopRange())
                {
                    if ((int)(Game.Time - SpellManager.StackerStamp) >= 2)
                    {
                        SpellManager.CastQ(Champion);
                        SpellManager.StackerStamp = Game.Time;
                    }
                }
            }
        }

        public static void InterruptMode(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (!MenuManager.InterrupterMode) return;
            if (sender != null && MenuManager.InterrupterUseE)
            {
                var target = TargetManager.GetChampionTarget(SpellManager.E.Range, DamageType.Magical);
                if (target != null)
                    SpellManager.CastE(target);
            }
        }

        public static void GapCloserMode(Obj_AI_Base sender, Gapcloser.GapcloserEventArgs args)
        {
            if (!MenuManager.GapCloserMode) return;
            if (sender != null && MenuManager.GapCloserUseE)
            {
                var target = TargetManager.GetChampionTarget(SpellManager.E.Range, DamageType.Magical);
                if (target != null)
                    SpellManager.CastE(target);
            }
        }
    }
}
