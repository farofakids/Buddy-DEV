using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberHidesStaticFromOuterClass
namespace Farofakids_Swain
{

    public static class Config
    {
        private const string MenuName = "Farofakids-Swain";

        private static readonly Menu Menu;

        static Config()
        {
            // Initialize the menu
            Menu = MainMenu.AddMenu(MenuName, MenuName.ToLower());
            Menu.AddGroupLabel("Welcome to this Farofakids-Swain!");

            // Initialize the modes
            Modes.Initialize();
        }

        public static void Initialize()
        {
        }

        public static class Modes
        {
            private static readonly Menu Menu, MenuL, MenuM;

            static Modes()
            {
                // Initialize the menu
                Menu = Config.Menu.AddSubMenu("Modes");
                MenuL = Config.Menu.AddSubMenu("Lane/Jungle Clear");
                MenuM = Config.Menu.AddSubMenu("Misc/Drawing");

                // Initialize all modes
                // Combo
                Combo.Initialize();
                Menu.AddSeparator();
                // Harass
                Harass.Initialize();
                LaneClear.Initialize();
                Misc.Initialize();
                Menu.AddSeparator();
                Drawing.Initialize();
            }

            public static void Initialize()
            {
            }

            public static class Combo
            {
                private static readonly CheckBox _useQ;
                private static readonly CheckBox _useW;
                private static readonly CheckBox _useE;
                private static readonly CheckBox _useR;
                private static readonly CheckBox _useZhonya;
                private static readonly Slider _useZhonyaheal;

                public static bool UseQ
                {
                    get { return _useQ.CurrentValue; }
                }
                public static bool UseW
                {
                    get { return _useW.CurrentValue; }
                }
                public static bool UseE
                {
                    get { return _useE.CurrentValue; }
                }
                public static bool UseR
                {
                    get { return _useR.CurrentValue; }
                }
                public static bool UseZhonya
                {
                    get { return _useZhonya.CurrentValue; }
                }
                public static int UseZhonyaheal
                {
                    get { return _useZhonyaheal.CurrentValue; }
                }



                static Combo()
                {
                    // Initialize the menu values
                    Menu.AddGroupLabel("Combo");
                    _useQ = Menu.Add("comboUseQ", new CheckBox("Use Q"));
                    _useW = Menu.Add("comboUseW", new CheckBox("Use W"));
                    _useE = Menu.Add("comboUseE", new CheckBox("Use E"));
                    _useR = Menu.Add("comboUseR", new CheckBox("Use R", false)); // Default false
                    _useZhonya = Menu.Add("C_MockingSwain", new CheckBox("Use Zhonya while Ult"));
                    _useZhonyaheal = Menu.Add("C_MockingSwainSlider", new Slider("Zhonya ult at Health (%)", 30));
                    
                }

                public static void Initialize()
                {
                }
            }

            public static class Harass
            {
                public static bool UseQ
                {
                    get { return Menu["harassUseQ"].Cast<CheckBox>().CurrentValue; }
                }
                public static bool UseW
                {
                    get { return Menu["harassUseW"].Cast<CheckBox>().CurrentValue; }
                }
                public static bool UseE
                {
                    get { return Menu["harassUseE"].Cast<CheckBox>().CurrentValue; }
                }
                public static bool UseR
                {
                    get { return Menu["harassUseR"].Cast<CheckBox>().CurrentValue; }
                }
                public static int Mana
                {
                    get { return Menu["harassMana"].Cast<Slider>().CurrentValue; }
                }
                public static bool UseQauto
                {
                    get { return Menu["H_AutoQ"].Cast<CheckBox>().CurrentValue; }
                }
                public static bool UseEauto
                {
                    get { return Menu["H_AutoE"].Cast<CheckBox>().CurrentValue; }
                }
                public static int ManaAuto
                {
                    get { return Menu["H_ESlinder"].Cast<Slider>().CurrentValue; }
                }
                static Harass()
                {
                    Menu.AddGroupLabel("Harass");
                    Menu.Add("harassUseQ", new CheckBox("Use Q"));
                    Menu.Add("harassUseW", new CheckBox("Use W"));
                    Menu.Add("harassUseE", new CheckBox("Use E"));
                    Menu.Add("harassMana", new Slider("Maximum mana usage in percent ({0}%)", 40));
                    Menu.Add("H_AutoQ", new CheckBox("Auto-Q enemies"));
                    Menu.Add("H_AutoE", new CheckBox("Auto-E enemies"));
                    Menu.Add("H_ESlinder", new Slider("Auto Q/E Minimum Mana", 70));
                }

                public static void Initialize()
                {
                }
            }

            public static class LaneClear
            {
                public static bool UseR
                {
                    get { return MenuL["LaneClearUseR"].Cast<CheckBox>().CurrentValue; }
                }
                public static int Mana
                {
                    get { return MenuL["LaneClearMana"].Cast<Slider>().CurrentValue; }
                }
                public static int MinNumberR
                {
                    get { return MenuL["laneNumR"].Cast<Slider>().CurrentValue; }
                }

                static LaneClear()
                {
                    MenuL.AddGroupLabel("Lane/Jungle Clear");
                    MenuL.Add("LaneClearUseR", new CheckBox("Use R"));
                    MenuL.Add("laneNumR", new Slider("Minion number for R", 3, 1, 10));
                    MenuL.Add("LaneClearMana", new Slider("Maximum mana usage in percent ({0}%)", 70));
                }

                public static void Initialize()
                {
                }
            }

            public static class Misc
            {
                public static bool UseWint
                {
                    get { return MenuM["UseWint"].Cast<CheckBox>().CurrentValue; }
                }

                static Misc()
                {
                    MenuM.AddGroupLabel("Miscellaneous");
                    MenuM.Add("UseWint", new CheckBox("Use W to interrupt dangerous spells"));
                    
                }

                public static void Initialize()
                {
                }
            }

            public static class Drawing
            {
                private static readonly CheckBox _drawQ;
                private static readonly CheckBox _drawW;
                private static readonly CheckBox _drawE;
                private static readonly CheckBox _drawR;
                private static readonly CheckBox _healthbar;

                public static bool DrawQ
                {
                    get { return _drawQ.CurrentValue; }
                }
                public static bool DrawW
                {
                    get { return _drawW.CurrentValue; }
                }
                public static bool DrawE
                {
                    get { return _drawE.CurrentValue; }
                }
                public static bool DrawR
                {
                    get { return _drawR.CurrentValue; }
                }
                public static bool IndicatorHealthbar
                {
                    get { return _healthbar.CurrentValue; }
                }

                static Drawing()
                {
                    MenuM.AddGroupLabel("Spell ranges");
                    _drawQ = MenuM.Add("drawQ", new CheckBox("Q range", false));
                    _drawW = MenuM.Add("drawW", new CheckBox("W range", false));
                    _drawE = MenuM.Add("drawE", new CheckBox("E range", false));
                    _drawR = MenuM.Add("drawR", new CheckBox("R range", false));
                    _healthbar = MenuM.Add("healthbar", new CheckBox("Healthbar overlay"));


                }

                public static void Initialize()
                {
                }
            }

        }


    }
}
