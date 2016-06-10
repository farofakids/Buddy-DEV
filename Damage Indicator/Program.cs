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
        /*public static Spell.Skillshot QS, WS, ES, RS;
        public static Spell.Targeted QT, WT, ET, RT;
        public static Spell.Chargeable QC, WC, EC, RC;*/

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

            /*QS = new Spell.Skillshot(SpellSlot.Q, 0, SkillShotType.Linear);
            WS = new Spell.Skillshot(SpellSlot.W, 0, SkillShotType.Linear);
            ES = new Spell.Skillshot(SpellSlot.E, 0, SkillShotType.Linear);
            RS = new Spell.Skillshot(SpellSlot.R, 0, SkillShotType.Linear);

            QT = new Spell.Targeted(SpellSlot.Q, 0);
            WT = new Spell.Targeted(SpellSlot.W, 0);
            ET = new Spell.Targeted(SpellSlot.E, 0);
            RT = new Spell.Targeted(SpellSlot.R, 0);

            QC = new Spell.Chargeable(SpellSlot.Q, 0, 0, 0);
            WC = new Spell.Chargeable(SpellSlot.W, 0, 0, 0);
            EC = new Spell.Chargeable(SpellSlot.E, 0, 0, 0);
            RC = new Spell.Chargeable(SpellSlot.R, 0, 0, 0);*/

            IGNITE = new Spell.Targeted(SpellSlot.Unknown, 0);

            Chat.Print("Damage Indicator Loaded Succesfully", Color.DodgerBlue);
            OnMenuLoad();
        }

        private static void OnMenuLoad()
        {
            menu = MainMenu.AddMenu("Damage Indicator", "Damage Indicator");
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

                damage = damage += Player.Instance.GetAutoAttackDamage(enemy, true);

                if (Hydra.IsReady() && Hydra.IsOwned()) damage = damage + Player.Instance.GetItemDamage(enemy, ItemId.Ravenous_Hydra_Melee_Only);
                if (Tiamat.IsReady() && Tiamat.IsOwned()) damage = damage + Player.Instance.GetItemDamage(enemy, ItemId.Ravenous_Hydra_Melee_Only);
                if (BOTRK.IsReady() && BOTRK.IsOwned()) damage = damage + Player.Instance.GetItemDamage(enemy, ItemId.Blade_of_the_Ruined_King);
                if (Cutl.IsReady() && Cutl.IsOwned()) damage = damage + Player.Instance.GetItemDamage(enemy, ItemId.Bilgewater_Cutlass);
                if (Sheen.IsReady() && Sheen.IsOwned()) damage = damage + Player.Instance.GetAutoAttackDamage(enemy) + Player.Instance.BaseAttackDamage * 2;
                if (TriForce.IsReady() && TriForce.IsOwned()) damage = damage + Player.Instance.GetAutoAttackDamage(enemy) + Player.Instance.BaseAttackDamage * 2;

                if (IGNITE.IsReady()) damage += Player.Instance.GetSummonerSpellDamage(enemy, DamageLibrary.SummonerSpells.Ignite);

                if (QA.IsReady()) damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.Q);
                if (WA.IsReady()) damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.W);
                if (EA.IsReady()) damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.E);
                if (RA.IsReady()) damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.R);

                //if (QS.IsReady()) damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.Q);
                //if (WS.IsReady()) damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.W);
                //if (ES.IsReady()) damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.E);
                //if (RS.IsReady()) damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.R);

                //if (QT.IsReady()) damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.Q);
                //if (WT.IsReady()) damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.W);
                //if (ET.IsReady()) damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.E);
                //if (RT.IsReady()) damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.R);

                //if (QC.IsReady()) damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.Q);
                //if (WC.IsReady()) damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.W);
                //if (EC.IsReady()) damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.E);
                //if (RC.IsReady()) damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.R);
                return (float)damage;
            }
            return 0;
        }

    }
}