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
using SharpDX;

namespace Damage_Indicator
{
    class Program
    {
        public static Menu menu;

        private static readonly AIHeroClient _Player = ObjectManager.Player;


        public static Spell.Active Q = new Spell.Active(SpellSlot.Q);
        public static Spell.Active W = new Spell.Active(SpellSlot.W);
        public static Spell.Active E = new Spell.Active(SpellSlot.E);
        public static Spell.Active R = new Spell.Active(SpellSlot.R);

        private static bool Dind
        {
            get { return menu["Dind"].Cast<CheckBox>().CurrentValue; }
        }

        private static void Main()
        {
            Loading.OnLoadingComplete += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs args)
        {
            Hacks.RenderWatermark = false;
            Chat.Print("Damage Indicator Loaded Succesfully", Color.DodgerBlue);
            OnMenuLoad();
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        private static readonly float _barLength = 104;
        private static readonly float _xOffset = 2;
        private static readonly float _yOffset = 9;

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (_Player.IsDead)
                return;
            if (!Dind) return;

            foreach (var aiHeroClient in EntityManager.Heroes.Enemies)
            {
                if (!aiHeroClient.IsHPBarRendered || !aiHeroClient.VisibleOnScreen) continue;
                var barPos = aiHeroClient.HPBarPosition;
                var damage = getComboDamage(aiHeroClient);
                var percentHealthAfterDamage = Math.Max(0, aiHeroClient.Health - damage) / aiHeroClient.MaxHealth;
                var yPos = barPos.Y + _yOffset;
                var xPos = barPos.X + _xOffset;
                var xPosDamage = barPos.X + _xOffset + _barLength * percentHealthAfterDamage;
                var xPosCurrentHp = barPos.X + _xOffset + _barLength * aiHeroClient.Health / aiHeroClient.MaxHealth;
                {
                    Line.DrawLine(System.Drawing.Color.Green, 9f,
                    new Vector2(xPosCurrentHp + (damage > percentHealthAfterDamage ? percentHealthAfterDamage : damage) - 2, yPos),
                    new Vector2(xPosDamage + (damage > percentHealthAfterDamage ? percentHealthAfterDamage : damage) + 2, yPos));

                    Line.DrawLine(System.Drawing.Color.DarkOrange, 9f,
                    new Vector2(xPosDamage + (damage > percentHealthAfterDamage ? percentHealthAfterDamage : damage) - 2, yPos),
                    new Vector2(xPosDamage + (damage > percentHealthAfterDamage ? percentHealthAfterDamage : damage) + 2, yPos));
                }
            }
        }

        private static Item HasHydra()
        {

            var hydraId =
                ObjectManager.Player.InventoryItems.FirstOrDefault(
                    it => it.Id == ItemId.Ravenous_Hydra_Melee_Only || it.Id == ItemId.Tiamat_Melee_Only);
            if (hydraId != null)
            {
                return new Item(hydraId.Id, 300);
            }
            return null;
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
                          "ITEMS:" + Environment.NewLine +
                          "Ravenous Hydra" + Environment.NewLine +
                          "Ignite SOON" + Environment.NewLine + Environment.NewLine +
                          "Not work 100% with champions, containing passive in their abilities" + Environment.NewLine +
                          "only use to have a base of their damage" + Environment.NewLine);
        }

        private static float getComboDamage(Obj_AI_Base enemy)
        {
            if (enemy != null)
            {
                float damage = 0;
                damage = damage + ObjectManager.Player.GetAutoAttackDamage(enemy);
                if (HasHydra() != null) damage = damage + _Player.GetAutoAttackDamage(enemy) * 0.7f;
                if (Q.IsReady()) damage = damage + ObjectManager.Player.GetSpellDamage(enemy, SpellSlot.Q);
                if (W.IsReady()) damage = damage + ObjectManager.Player.GetSpellDamage(enemy, SpellSlot.W);
                if (E.IsReady()) damage = damage + ObjectManager.Player.GetSpellDamage(enemy, SpellSlot.E);
                if (R.IsReady()) damage = damage + ObjectManager.Player.GetSpellDamage(enemy, SpellSlot.R);
                return damage;
            }
            return 0;
        }

    }
}
