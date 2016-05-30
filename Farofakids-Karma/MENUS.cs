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

namespace Farofakids_Karma
{
    internal class MENUS
    {
        private static Menu FarofakidsKarmaMenu, ComboMenu, HarassMenu, DrawingMenu, MiscMenu;

        public static void Initialize()
        {
            FarofakidsKarmaMenu = MainMenu.AddMenu("Farofakids Karma", "Farofakids-Karma");
            FarofakidsKarmaMenu.AddGroupLabel("Farofakids Karma");
            FarofakidsKarmaMenu.Add("URFMODE", new CheckBox("URF MODE: EVER W AND E"));

            // Combo Menu
            ComboMenu = FarofakidsKarmaMenu.AddSubMenu("Combo Features", "ComboFeatures");
            ComboMenu.AddGroupLabel("Combo Features");
            ComboMenu.Add("UseQCombo", new CheckBox("Use Q"));
            ComboMenu.Add("UseWCombo", new CheckBox("Use W"));
            ComboMenu.Add("UseRCombo", new CheckBox("Use R"));

            // Harass Menu
            HarassMenu = FarofakidsKarmaMenu.AddSubMenu("Harass Features", "HarassFeatures");
            HarassMenu.AddGroupLabel("Harass Features");
            HarassMenu.Add("UseQHarass", new CheckBox("Use Q"));
            HarassMenu.Add("UseWHarass", new CheckBox("Use W"));
            HarassMenu.Add("UseRHarass", new CheckBox("Use R"));
            HarassMenu.AddSeparator(1);
            HarassMenu.Add("HarassMana", new Slider("Mana Limiter at Mana %", 40));

            // Drawing Menu
            DrawingMenu = FarofakidsKarmaMenu.AddSubMenu("Drawing Features", "DrawingFeatures");
            DrawingMenu.AddGroupLabel("Drawing Features");
            DrawingMenu.Add("QRange", new CheckBox("Q range", false));
            DrawingMenu.Add("WRange", new CheckBox("W range", false));
            DrawingMenu.Add("ERange", new CheckBox("E range", false));
            DrawingMenu.Add("RRange", new CheckBox("R range", false));

            // Setting Menu
            MiscMenu = FarofakidsKarmaMenu.AddSubMenu("Settings", "Settings");
            MiscMenu.AddGroupLabel("Settings");
            MiscMenu.AddLabel("Interrupter");
            MiscMenu.Add("UseEDefense", new CheckBox("Use E For Defense"));
            MiscMenu.Add("InterruptSpells", new CheckBox("Interrupt spells"));

        }

        //combo
        public static bool UseQCombo { get { return ComboMenu["UseQCombo"].Cast<CheckBox>().CurrentValue; } }
        public static bool UseWCombo { get { return ComboMenu["UseWCombo"].Cast<CheckBox>().CurrentValue; } }
        public static bool UseRCombo { get { return ComboMenu["UseRCombo"].Cast<CheckBox>().CurrentValue; } }

        //harras
        public static bool UseQHarass { get { return HarassMenu["UseQHarass"].Cast<CheckBox>().CurrentValue; } }
        public static bool UseWHarass { get { return HarassMenu["UseWHarass"].Cast<CheckBox>().CurrentValue; } }
        public static bool UseRHarass { get { return HarassMenu["UseRHarass"].Cast<CheckBox>().CurrentValue; } }
        public static int HarassMana { get { return HarassMenu["HarassMana"].Cast<Slider>().CurrentValue; } }
        
        //draw
        public static bool QRange { get { return DrawingMenu["QRange"].Cast<CheckBox>().CurrentValue; } }
        public static bool WRange { get { return DrawingMenu["WRange"].Cast<CheckBox>().CurrentValue; } }
        public static bool ERange { get { return DrawingMenu["ERange"].Cast<CheckBox>().CurrentValue; } }
        public static bool RRange { get { return DrawingMenu["RRange"].Cast<CheckBox>().CurrentValue; } }
        
        //misc
        public static bool InterruptSpells { get { return MiscMenu["InterruptSpells"].Cast<CheckBox>().CurrentValue; } }
        public static bool UseEDefense { get { return MiscMenu["UseEDefense"].Cast<CheckBox>().CurrentValue; } }

    }
}
