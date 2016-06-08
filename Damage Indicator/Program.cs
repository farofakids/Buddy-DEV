using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Rendering;
using EloBuddy.SDK.Enumerations;
using SharpDX;

namespace Damage_Indicator
{
    class Program
    {
        public static Menu menu;

        public static Spell.Active QA, WA, EA, RA;

        public static Item Hydra = new Item((int)ItemId.Ravenous_Hydra_Melee_Only, 400);
        public static Item Tiamat = new Item((int)ItemId.Tiamat_Melee_Only, 400);
        public static Item BOTRK = new Item((int)ItemId.Blade_of_the_Ruined_King, 450);
        public static Item Cutl = new Item((int)ItemId.Bilgewater_Cutlass, 450);
        public static Item Sheen = new Item((int)ItemId.Sheen);
        public static Item TriForce = new Item((int)ItemId.Trinity_Force);

        public static Spell.Targeted IGNITE;

        public static bool Dind
        {
            get { return menu["Dind"].Cast<CheckBox>().CurrentValue; }
        }

        private static void Main()
        {
            Loading.OnLoadingComplete += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs args)
        {
            QA = new Spell.Active(SpellSlot.Q);
            WA = new Spell.Active(SpellSlot.W);
            EA = new Spell.Active(SpellSlot.E);
            RA = new Spell.Active(SpellSlot.R);

             
            IGNITE = new Spell.Targeted(SpellSlot.Unknown, 0);

            Chat.Print("Damage Indicator Loaded Succesfully", Color.DodgerBlue);
            OnMenuLoad();
        }

        private static void OnMenuLoad()
        {
            menu = MainMenu.AddMenu("Damage Indicator", "DamageIndicator");
            menu.AddGroupLabel("Draw");
            menu.Add("Dind", new CheckBox("Draw Damage Indicator"));
            menu.AddSeparator(150);
            menu.AddLabel("SUPORTED:" + Environment.NewLine +
                          "Spells Damages" + Environment.NewLine +
                          "Auto Attack Damages" + Environment.NewLine +
                          Environment.NewLine +
                          "SUMMONERS:" + Environment.NewLine +
                          "Ignite" + Environment.NewLine + Environment.NewLine +
                          "ITEMS:" + Environment.NewLine +
                          "Ravenous Hydra" + Environment.NewLine +
                          "Tiamat" + Environment.NewLine +
                          "Blade of the Ruined King" + Environment.NewLine +
                          "Bilgewater Cutlass" + Environment.NewLine +
                          "Sheen" + Environment.NewLine +
                          "TriForce" + Environment.NewLine + Environment.NewLine +
                          "Not work 100% with champions, containing passive in their abilities" + Environment.NewLine +
                          "only use to have a base of their damage" + Environment.NewLine);

            DamageIndicator.DamageToUnit = getComboDamage;
        }

        private static float getComboDamage(Obj_AI_Base enemy)
        {
            if (enemy != null)
            {
                var damage = 0d;
                
                if (Hydra.IsReady() && Hydra.IsOwned()) damage = damage + Player.Instance.GetItemDamage(enemy, ItemId.Ravenous_Hydra_Melee_Only);
                if (Tiamat.IsReady() && Tiamat.IsOwned()) damage = damage + Player.Instance.GetItemDamage(enemy, ItemId.Ravenous_Hydra_Melee_Only);
                if (BOTRK.IsReady() && BOTRK.IsOwned()) damage = damage + Player.Instance.GetItemDamage(enemy, ItemId.Blade_of_the_Ruined_King);
                if (Cutl.IsReady() && Cutl.IsOwned()) damage = damage + Player.Instance.GetItemDamage(enemy, ItemId.Bilgewater_Cutlass);
                if (Sheen.IsReady() && Sheen.IsOwned()) damage = damage + Player.Instance.GetAutoAttackDamage(enemy) + Player.Instance.BaseAttackDamage * 2;
                if (TriForce.IsReady() && TriForce.IsOwned()) damage = damage + Player.Instance.GetAutoAttackDamage(enemy) + Player.Instance.BaseAttackDamage * 2;

                if (IGNITE.IsReady()) damage += Player.Instance.GetSummonerSpellDamage(enemy, DamageLibrary.SummonerSpells.Ignite);

                if (Player.Instance.ChampionName == "Riven")
                {
                    float passivenhan = 0;
                    if (Player.Instance.Level >= 18) { passivenhan = 0.5f; }
                    else if (Player.Instance.Level >= 15) { passivenhan = 0.45f; }
                    else if (Player.Instance.Level >= 12) { passivenhan = 0.4f; }
                    else if (Player.Instance.Level >= 9) { passivenhan = 0.35f; }
                    else if (Player.Instance.Level >= 6) { passivenhan = 0.3f; }
                    else if (Player.Instance.Level >= 3) { passivenhan = 0.25f; }
                    else { passivenhan = 0.2f; }
                    if (WA.IsReady()) damage = damage + Player.Instance.GetSpellDamage(enemy, SpellSlot.W);
                    if (QA.IsReady())
                    {
                        var qnhan = 2;//4 - QStack;
                        damage = damage + Player.Instance.GetSpellDamage(enemy, SpellSlot.Q) * qnhan + (float)Player.Instance.GetAutoAttackDamage(enemy) * qnhan * (1 + passivenhan);
                    }
                    damage = damage + (float)Player.Instance.GetAutoAttackDamage(enemy) * (1 + passivenhan);
                    if (RA.IsReady())
                    {
                        return (float)damage * 1.2f + Player.Instance.GetSpellDamage(enemy, SpellSlot.R);
                    }
                    return (float)damage;
                }

                else
                {
                damage = damage += Player.Instance.GetAutoAttackDamage(enemy, true);
                if (QA.IsReady()) damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.Q);
                if (WA.IsReady()) damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.W);
                if (EA.IsReady()) damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.E);
                if (RA.IsReady()) damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.R);
                    return (float)damage;
                }
            }
            return 0;
        }

    }
}