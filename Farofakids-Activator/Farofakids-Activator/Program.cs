using EloBuddy.SDK.Events;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using System.Linq;
using System;

namespace Farofakids_Activator
{
    class Program
    {
        public static Menu MENU;

        public static Spell.Targeted Ignite;
        public static Spell.Active Heal, Cleanse;

        public static readonly Item Zhonyas = new Item(ItemId.Zhonyas_Hourglass);
        public static readonly Item Cutlass = new Item(ItemId.Bilgewater_Cutlass);
        public static readonly Item Botrk = new Item(ItemId.Blade_of_the_Ruined_King);
        public static readonly Item Youmuu = new Item(ItemId.Youmuus_Ghostblade);


        public static bool HasSpell(string s)
        {
            return Player.Spells.FirstOrDefault(o => o.SData.Name.Contains(s)) != null;
        }

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        #region bool & int MENUVALLUES

        public static bool UseIgnite
        {
            get { return MENU["useIgnite"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseHeal
        {
            get { return MENU["useHeal"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseCleanse
        {
            get { return MENU["useCleanse"].Cast<CheckBox>().CurrentValue; }
        }

        public static int UseZhonyaheal
        {
            get { return MENU["useZhonyaheal"].Cast<Slider>().CurrentValue; }
        }

        public static bool UseZhonyas
        {
            get { return MENU["useZhonyas"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseQss
        {
            get { return MENU["useQss"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseTiamat
        {
            get { return MENU["useTiamat"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseCutlass
        {
            get { return MENU["useBotrk"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseYoumuu
        {
            get { return MENU["useYoumuu"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseBotrk
        {
            get { return MENU["useBotrk"].Cast<CheckBox>().CurrentValue; }
        }

        public static int UseHealheal
        {
            get { return MENU["useHealheal"].Cast<Slider>().CurrentValue; }
        }

        public static int MinHPBotrk
        {
            get { return MENU["minHPBotrk"].Cast<Slider>().CurrentValue; }
        }

        public static int EnemyMinHPBotrk
        {
            get { return MENU["enemyMinHPBotrk"].Cast<Slider>().CurrentValue; }
        }

        #endregion bool & int MENUVALLUES

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            MENU = MainMenu.AddMenu("Farofakids-Activator", "Farofakids-Activator");

            if (HasSpell("SummonerDot"))
            {
                MENU.AddLabel("IGNITE");
                MENU.Add("useIgnite", new CheckBox("Use Ignite"));
                Ignite = new Spell.Targeted(ObjectManager.Player.GetSpellSlotFromName("SummonerDot"), 600);
                Chat.Print("Farofakids-Activator: Ignite Loaded", System.Drawing.Color.Red);
                MENU.AddSeparator();
            }
            if (HasSpell("SummonerHeal"))
            {
                MENU.AddLabel("Heal");
                MENU.Add("useHeal", new CheckBox("Use Heal"));
                MENU.Add("useHealheal", new Slider("Heal at Health (%)", 15));
                Heal = new Spell.Active(ObjectManager.Player.GetSpellSlotFromName("SummonerHeal"), 850);
                Chat.Print("Farofakids-Activator: Heal Loaded", System.Drawing.Color.Green);
                MENU.AddSeparator();
            }
            if (HasSpell("SummonerBoost"))
            {
                MENU.AddLabel("Cleanse");
                MENU.Add("useCleanse", new CheckBox("Use Cleanse"));
                Cleanse = new Spell.Active(ObjectManager.Player.GetSpellSlotFromName("SummonerBoost"), 0);
                Chat.Print("Farofakids-Activator: Cleanse Loaded", System.Drawing.Color.Green);
                MENU.AddSeparator();
            }
            MENU.AddLabel("ITENS");
            MENU.Add("useZhonyas", new CheckBox("Use Zhonyas"));
            MENU.Add("useZhonyaheal", new Slider("Zhonya at Health (%)", 30));
            MENU.Add("useQss", new CheckBox("Use Quicksilver or Mercurial Scimitar"));
            MENU.Add("useBotrk", new CheckBox("Use Botrk"));
            MENU.Add("minHPBotrk", new Slider("Min health to use Botrk ({0}%)", 80));
            MENU.Add("enemyMinHPBotrk", new Slider("Min enemy health to use Botrk ({0}%)", 80));
            MENU.Add("useYoumuu", new CheckBox("Use Youmuu's Ghostblade"));
            MENU.Add("useCutlass", new CheckBox("Use Botrk"));
            MENU.Add("useTiamat", new CheckBox("Use Tiamat or Hydra or Titanic"));
            Chat.Print("Farofakids-Activator: ITENS Loaded", System.Drawing.Color.HotPink);

            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.OnPostAttack += OnPostAttack;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            Execute();
        }

        public static float CalculateDamage(Obj_AI_Base target, bool q, bool w, bool e, bool r, bool ignite)
        {
            var totaldamage = 0f;

            if (target == null) return totaldamage;

            if (ignite && Program.Ignite != null && Program.Ignite.IsReady() && Program.Ignite.IsInRange(target))
            {
                totaldamage += Player.Instance.GetSummonerSpellDamage(target, EloBuddy.SDK.DamageLibrary.SummonerSpells.Ignite);
            }

            return totaldamage;
        }

        public static void Execute()
        {
            #region Ignite

            if (Ignite != null && Ignite.IsReady())
            {
                var ignitableEnemies =
                    EntityManager.Heroes.Enemies.Where(
                        t =>
                            t.IsValidTarget(Ignite.Range) && !t.HasUndyingBuff() &&
                            CalculateDamage(t, false, false, false, false, true) >= t.Health);
                var igniteEnemy = TargetSelector.GetTarget(ignitableEnemies, DamageType.True);

                if (igniteEnemy != null)
                {
                    if (Ignite != null && UseIgnite)
                    {
                        if (Ignite.IsInRange(igniteEnemy))
                        {
                            Ignite.Cast(igniteEnemy);
                        }
                    }
                }
            }

            #endregion ignite

            #region heal

            if (Heal != null)
            {
                    if (UseHeal && Heal.IsReady())
                    {
                        if (Player.Instance.HealthPercent <= UseHealheal)
                        {
                            Heal.Cast();
                        }
                    }
            }

            #endregion heal

            #region Zhonyas

            if (UseZhonyas && Zhonyas.IsReady())
            {
                if (Player.Instance.HealthPercent <= UseZhonyaheal)
                {
                    Zhonyas.Cast();
                }
            }

            #endregion Zhonyas

            #region cleanse

            if (Cleanse != null)
            {
                if (UseCleanse && Cleanse.IsReady())
                {
                    if (Player.Instance.IsDead || Player.Instance.IsInvulnerable || !Player.Instance.IsTargetable || Player.Instance.IsZombie || Player.Instance.IsInShopRange())
                        return;
                    if (Player.HasBuff("PoppyDiplomaticImmunity") || Player.HasBuff("MordekaiserChildrenOfTheGrave") || Player.HasBuff("FizzMarinerDoom") || Player.HasBuff("VladimirHemoplague") ||
                            Player.HasBuff("zedulttargetmark") || Player.HasBuff("AlZaharNetherGrasp") || Player.HasBuffOfType(BuffType.Suppression) || Player.HasBuffOfType(BuffType.Charm) || Player.HasBuffOfType(BuffType.Flee) || Player.HasBuffOfType(BuffType.Blind) ||
                            Player.HasBuffOfType(BuffType.Polymorph) || Player.HasBuffOfType(BuffType.Snare) || Player.HasBuffOfType(BuffType.Stun) || Player.HasBuffOfType(BuffType.Taunt))
                    {
                        Core.DelayAction(() => Cleanse.Cast(), 110);
                    }

                }
            }

            #endregion cleanse

            #region Qss

            if (UseQss)
            {
                if (Player.Instance.IsDead || Player.Instance.IsInvulnerable || !Player.Instance.IsTargetable || Player.Instance.IsZombie || Player.Instance.IsInShopRange())
                    return;
                InventorySlot[] inv = Player.Instance.InventoryItems;
                foreach (var item in inv)
                {
                    if ((item.Id == ItemId.Quicksilver_Sash || item.Id == ItemId.Mercurial_Scimitar) && item.CanUseItem() && Player.Instance.CountEnemiesInRange(700) > 0)
                    {
                        if (Player.HasBuff("PoppyDiplomaticImmunity") || Player.HasBuff("MordekaiserChildrenOfTheGrave") || Player.HasBuff("FizzMarinerDoom") || Player.HasBuff("VladimirHemoplague") ||
                            Player.HasBuff("zedulttargetmark") || Player.HasBuff("AlZaharNetherGrasp") || Player.HasBuffOfType(BuffType.Suppression) || Player.HasBuffOfType(BuffType.Charm) || Player.HasBuffOfType(BuffType.Flee) || Player.HasBuffOfType(BuffType.Blind) ||
                            Player.HasBuffOfType(BuffType.Polymorph) || Player.HasBuffOfType(BuffType.Snare) || Player.HasBuffOfType(BuffType.Stun) || Player.HasBuffOfType(BuffType.Taunt))
                        {
                            Core.DelayAction(() => item.Cast(), 110);
                        }
                    }
                }
            }

            #endregion Qss

            #region Yomuu & Bortk

            var target = TargetSelector.GetTarget(550, DamageType.Physical); // 550 = Botrk.Range
            if (target != null)
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && UseYoumuu && Youmuu.IsOwned() && Youmuu.IsReady())
                {
                    Youmuu.Cast();
                }
                if (UseBotrk && Item.HasItem(Cutlass.Id) && Item.CanUseItem(Cutlass.Id) &&
                    Player.Instance.HealthPercent < MinHPBotrk &&
                    target.HealthPercent < EnemyMinHPBotrk)
                {
                    Item.UseItem(Cutlass.Id, target);
                }
                if (UseBotrk && Item.HasItem(Botrk.Id) && Item.CanUseItem(Botrk.Id) &&
                    Player.Instance.HealthPercent < MinHPBotrk &&
                    target.HealthPercent < EnemyMinHPBotrk)
                {
                    Botrk.Cast(target);
                }
            }

            #endregion Yomuu & Bortk
        }

        private static void OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if (!target.IsEnemy || target.IsZombie) return;
            if (UseTiamat)
            {
                InventorySlot[] inv = Player.Instance.InventoryItems;
                foreach (var item in inv)
                {
                    if ((item.Id == ItemId.Tiamat_Melee_Only || item.Id == ItemId.Ravenous_Hydra_Melee_Only || item.Id == ItemId.Titanic_Hydra) && item.CanUseItem() && Player.Instance.CountEnemiesInRange(400) > 0)
                    {
                        item.Cast();
                    }
                }
            }
        }
    }
}
