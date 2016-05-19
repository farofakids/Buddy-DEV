using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Farofakids_Velkoz
{
    internal class MENUS
    {
        public static Menu FarofakidsVelkozMenu, ComboMenu, HarassMenu, DrawingMenu, MiscMenu;

        public static void Initialize()
        {
            FarofakidsVelkozMenu = MainMenu.AddMenu("Farofakids VelKoz", "Farofakids-Velkoz");
            FarofakidsVelkozMenu.AddGroupLabel("Farofakids VelKoz");

            // Combo Menu
            ComboMenu = FarofakidsVelkozMenu.AddSubMenu("Combo Features", "ComboFeatures");
            ComboMenu.AddGroupLabel("Combo Features");
            ComboMenu.Add("UseQCombo", new CheckBox("Use Q"));
            ComboMenu.Add("UseWCombo", new CheckBox("Use W"));
            ComboMenu.Add("UseECombo", new CheckBox("Use E"));
            ComboMenu.Add("UseRCombo", new CheckBox("Use R"));

            // Harass Menu
            HarassMenu = FarofakidsVelkozMenu.AddSubMenu("Harass Features", "HarassFeatures");
            HarassMenu.AddGroupLabel("Harass Features");
            HarassMenu.Add("UseQHarass", new CheckBox("Use Q"));
            HarassMenu.Add("UseWHarass", new CheckBox("Use W", false));
            HarassMenu.Add("UseEHarass", new CheckBox("Use E", false));
            HarassMenu.AddSeparator(1);
            HarassMenu.Add("HarassMana", new Slider("Mana Limiter at Mana %", 25));

            // Drawing Menu
            DrawingMenu = FarofakidsVelkozMenu.AddSubMenu("Drawing Features", "DrawingFeatures");
            DrawingMenu.AddGroupLabel("Drawing Features");
            DrawingMenu.Add("QRange", new CheckBox("Q range", false));
            DrawingMenu.Add("WRange", new CheckBox("W range", false));
            DrawingMenu.Add("ERange", new CheckBox("E range", false));
            DrawingMenu.Add("RRange", new CheckBox("R range", false));

            // Setting Menu
            MiscMenu = FarofakidsVelkozMenu.AddSubMenu("Settings", "Settings");
            MiscMenu.AddGroupLabel("Settings");
            MiscMenu.AddLabel("Interrupter");
            MiscMenu.Add("InterruptSpells", new CheckBox("Interrupt spells"));

        }

        //combo
        public static bool UseQCombo { get { return ComboMenu["UseQCombo"].Cast<CheckBox>().CurrentValue; } }
        public static bool UseWCombo { get { return ComboMenu["UseWCombo"].Cast<CheckBox>().CurrentValue; } }
        public static bool UseECombo { get { return ComboMenu["UseECombo"].Cast<CheckBox>().CurrentValue; } }
        public static bool UseRCombo { get { return ComboMenu["UseRCombo"].Cast<CheckBox>().CurrentValue; } }

        //harras
        public static bool UseQHarass { get { return HarassMenu["UseQHarass"].Cast<CheckBox>().CurrentValue; } }
        public static bool UseWHarass { get { return HarassMenu["UseWHarass"].Cast<CheckBox>().CurrentValue; } }
        public static bool UseEHarass { get { return HarassMenu["UseEHarass"].Cast<CheckBox>().CurrentValue; } }
        public static int HarassMana { get { return HarassMenu["HarassMana"].Cast<Slider>().CurrentValue; } }

        //draw
        public static bool QRange { get { return DrawingMenu["QRange"].Cast<CheckBox>().CurrentValue; } }
        public static bool WRange { get { return DrawingMenu["WRange"].Cast<CheckBox>().CurrentValue; } }
        public static bool ERange { get { return DrawingMenu["ERange"].Cast<CheckBox>().CurrentValue; } }
        public static bool RRange { get { return DrawingMenu["RRange"].Cast<CheckBox>().CurrentValue; } }
        
        //misc
        public static bool InterruptSpells { get { return MiscMenu["InterruptSpells"].Cast<CheckBox>().CurrentValue; } }

    }
}
