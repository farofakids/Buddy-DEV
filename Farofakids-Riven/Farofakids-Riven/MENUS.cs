using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Enumerations;

namespace Farofakids_Riven
{
    internal class MENUS
    {
        private static Menu FarofakidsRivenMenu, ComboMenu, HarassMenu, ItemsMenu, MiscMenu, DrawingMenu;

        public static void Initialize()
        {
            FarofakidsRivenMenu = MainMenu.AddMenu("Farofakids Riven", "FarofakidsRivenMenu");
            FarofakidsRivenMenu.AddGroupLabel("Farofakids Riven");

            // Combo
            ComboMenu = FarofakidsRivenMenu.AddSubMenu("Combo Features", "ComboFeatures");
            ComboMenu.AddGroupLabel("Combo Features");
            ComboMenu.Add("Burst", new KeyBind("Use Burst Combo", false, KeyBind.BindTypes.HoldActive, "T".ToCharArray()[0]));

            //HarassMenu
            HarassMenu = FarofakidsRivenMenu.AddSubMenu("Harass Features", "HarassFeatures");
            HarassMenu.AddGroupLabel("Harass Features");
            HarassMenu.Add("FastHarass", new KeyBind("Use FastHarass Harras", false, KeyBind.BindTypes.HoldActive, 'Y'/*.ToCharArray()[0]*/));

            //Misc
            MiscMenu = FarofakidsRivenMenu.AddSubMenu("Misc Features", "MiscFeatures");
            MiscMenu.AddGroupLabel("Misc Features");
            MiscMenu.Add("Winterrupt", new CheckBox("W interrupt"));
            MiscMenu.Add("AutoShield", new CheckBox("Auto Cast E"));
            MiscMenu.Add("Shield", new CheckBox("Auto Cast E While LastHit"));

            //items
            ItemsMenu = FarofakidsRivenMenu.AddSubMenu("Items Features", "ItemsFeatures");
            ItemsMenu.AddGroupLabel("Items Features");
            ItemsMenu.Add("Youmu", new CheckBox("Use Youmu When E", false));
            ItemsMenu.Add("FirstHydra", new CheckBox("Flash Burst Hydra Cast before W", false));

            // Drawing Menu
            DrawingMenu = FarofakidsRivenMenu.AddSubMenu("Drawing Features", "DrawingFeatures");
            DrawingMenu.AddGroupLabel("Drawing Features");
            DrawingMenu.Add("QRange", new CheckBox("Q range", false));
            DrawingMenu.Add("WRange", new CheckBox("W range", false));
            DrawingMenu.Add("ERange", new CheckBox("E range", false));
            DrawingMenu.Add("RRange", new CheckBox("R range", false));

            Chat.Print("Farofakids-Riven: Loaded", System.Drawing.Color.Red);

        }

        //Combo
        public static bool Burst { get { return ComboMenu["Burst"].Cast<KeyBind>().CurrentValue; } }

        //Harass
        public static bool FastHarass { get { return HarassMenu["FastHarass"].Cast<KeyBind>().CurrentValue; } }

        //Items 
        public static bool Youmu { get { return ItemsMenu["Youmu"].Cast<CheckBox>().CurrentValue; } }
        public static bool FirstHydra { get { return ItemsMenu["FirstHydra"].Cast<CheckBox>().CurrentValue; } }

        //Misc
        public static bool Winterrupt { get { return MiscMenu["Winterrupt"].Cast<CheckBox>().CurrentValue; } }
        public static bool AutoShield { get { return MiscMenu["AutoShield"].Cast<CheckBox>().CurrentValue; } }
        public static bool Shield { get { return MiscMenu["Shield"].Cast<CheckBox>().CurrentValue; } }

        //draw
        public static bool QRange { get { return DrawingMenu["QRange"].Cast<CheckBox>().CurrentValue; } }
        public static bool WRange { get { return DrawingMenu["WRange"].Cast<CheckBox>().CurrentValue; } }
        public static bool ERange { get { return DrawingMenu["ERange"].Cast<CheckBox>().CurrentValue; } }
        public static bool RRange { get { return DrawingMenu["RRange"].Cast<CheckBox>().CurrentValue; } }

    }
}
