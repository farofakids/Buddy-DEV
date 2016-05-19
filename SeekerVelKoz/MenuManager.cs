using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace SeekerVelKoz
{
    internal class MenuManager
    {
        // Create Main Segments
        public static Menu SeekerVelKozMenu, ComboMenu, HarassMenu, JungleMenu, LaneClearMenu, LastHitMenu, KillStealMenu, DrawingMenu, SettingMenu;

        public static void Initialize()
        {
            // Addon Menu
            SeekerVelKozMenu = MainMenu.AddMenu("Seeker Vel'Koz", "SeekerVelKoz");
            SeekerVelKozMenu.AddGroupLabel("Seeker Vel'Koz");

            // Combo Menu
            ComboMenu = SeekerVelKozMenu.AddSubMenu("Combo Features", "ComboFeatures");
            ComboMenu.AddGroupLabel("Combo Features");
            ComboMenu.AddLabel("Independent boxes for Spells:");
            ComboMenu.Add("Qcombo", new CheckBox("Use Q"));
            ComboMenu.Add("Wcombo", new CheckBox("Use W"));
            ComboMenu.Add("Ecombo", new CheckBox("Use E"));
            ComboMenu.Add("Rcombo", new CheckBox("Use R"));
            ComboMenu.AddSeparator(1);
            ComboMenu.Add("Wlimit", new CheckBox("Only W if knocked back", false));
            ComboMenu.Add("Elimit", new CheckBox("Only E if slowed", false));
            ComboMenu.Add("Rcool", new CheckBox("Only R if other spells on Cooldown"));
            ComboMenu.Add("Rlimit", new Slider("Use R when Enemies in range >=", 4, 1, 5));

            // Harass Menu
            HarassMenu = SeekerVelKozMenu.AddSubMenu("Harass Features", "HarassFeatures");
            HarassMenu.AddGroupLabel("Harass Features");
            HarassMenu.AddLabel("Independent boxes for Spells:");
            HarassMenu.Add("Qharass", new CheckBox("Use Q"));
            HarassMenu.Add("Eharass", new CheckBox("Use E", false));
            HarassMenu.AddSeparator(1);
            HarassMenu.Add("Mharass", new Slider("Mana Limiter at Mana %", 25));

            // Jungle Menu
            JungleMenu = SeekerVelKozMenu.AddSubMenu("Jungle Features", "JungleFeatures");
            JungleMenu.AddGroupLabel("Jungle Features");
            JungleMenu.AddLabel("Independent boxes for Spells:");
            JungleMenu.Add("Qjungle", new CheckBox("Use Q"));
            JungleMenu.Add("Wjungle", new CheckBox("Use W"));
            JungleMenu.Add("Ejungle", new CheckBox("Use E"));
            JungleMenu.AddSeparator(1);
            JungleMenu.Add("Mjungle", new Slider("Mana Limiter at Mana %", 25));

            // LaneClear Menu
            LaneClearMenu = SeekerVelKozMenu.AddSubMenu("Lane Clear Features", "LaneClearFeatures");
            LaneClearMenu.AddGroupLabel("Lane Clear Features");
            LaneClearMenu.AddLabel("Independent boxes for Spells:");
            LaneClearMenu.Add("Qlanec", new CheckBox("Use Q", false));
            LaneClearMenu.Add("Wlanec", new CheckBox("Use W", false));
            LaneClearMenu.Add("Elanec", new CheckBox("Use E", false));
            LaneClearMenu.AddSeparator(1);
            LaneClearMenu.Add("Mlanec", new Slider("Mana Limiter at Mana %", 25));

            // LastHit Menu
            LastHitMenu = SeekerVelKozMenu.AddSubMenu("Last Hit Features", "LastHitFeatures");
            LastHitMenu.AddGroupLabel("Last Hit Features");
            LastHitMenu.AddLabel("Independent boxes for Spells:");
            LastHitMenu.Add("Qlasthit", new CheckBox("Use Q", false));
            LastHitMenu.Add("Wlasthit", new CheckBox("Use W", false));
            LastHitMenu.Add("Elasthit", new CheckBox("Use E", false));
            LastHitMenu.AddSeparator(1);
            LastHitMenu.Add("Mlasthit", new Slider("Mana Limiter at Mana %", 25));

            // Kill Steal Menu
            KillStealMenu = SeekerVelKozMenu.AddSubMenu("KS Features", "KSFeatures");
            KillStealMenu.AddGroupLabel("Kill Steal Features");
            KillStealMenu.Add("Uks", new CheckBox("KS Mode"));
            KillStealMenu.AddSeparator(1);
            KillStealMenu.AddLabel("Independent boxes for Spells:");
            KillStealMenu.Add("Qks", new CheckBox("Use Q in KS"));
            KillStealMenu.Add("Wks", new CheckBox("Use W in KS", false));
            KillStealMenu.Add("Eks", new CheckBox("Use E in KS", false));
            KillStealMenu.Add("Rks", new CheckBox("Use R in KS"));
            KillStealMenu.Add("Kslimit", new Slider("Use R when Enemies in range >=", 2, 1, 5));

            // Drawing Menu
            DrawingMenu = SeekerVelKozMenu.AddSubMenu("Drawing Features", "DrawingFeatures");
            DrawingMenu.AddGroupLabel("Drawing Features");
            DrawingMenu.Add("Udraw", new CheckBox("Draw Mode"));
            DrawingMenu.AddSeparator(1);
            DrawingMenu.AddLabel("Independent boxes for Spells:");
            DrawingMenu.Add("QWdraw", new CheckBox("Draw Q & W"));
            DrawingMenu.Add("Edraw", new CheckBox("Draw E"));
            DrawingMenu.Add("Rdraw", new CheckBox("Draw R"));
            DrawingMenu.AddSeparator(1);
            DrawingMenu.AddLabel("Skin Designer");
            DrawingMenu.Add("Udesign", new CheckBox("Draw Skin Design"));
            DrawingMenu.Add("Sdesign", new Slider("Skin Designer: ", 0, 0, 2));

            // Setting Menu
            SettingMenu = SeekerVelKozMenu.AddSubMenu("Settings", "Settings");
            SettingMenu.AddGroupLabel("Settings");
            SettingMenu.AddLabel("Automatic Leveler");
            SettingMenu.Add("Ulevel", new CheckBox("Auto Leveler"));
            SettingMenu.AddSeparator(1);
            SettingMenu.AddLabel("Ultimate Follower");
            SettingMenu.Add("Uultimate", new CheckBox("Use Ult Follower"));
            SettingMenu.AddLabel("Automatic Tear Stacker");
            SettingMenu.Add("Ustack", new CheckBox("Stack Mode"));
            SettingMenu.AddLabel("Interrupter");
            SettingMenu.Add("Uinterrupt", new CheckBox("Interrupt Mode"));
            SettingMenu.Add("Einterrupt", new CheckBox("Use E to interrupt"));
            SettingMenu.AddSeparator(1);
            SettingMenu.AddLabel("Gap Closer");
            SettingMenu.Add("Ugapc", new CheckBox("Gap Closer Mode"));
            SettingMenu.Add("Egapc", new CheckBox("Use E to gapclose"));
        }

        // Assign Global Checks+
        public static bool ComboUseQ { get { return ComboMenu["Qcombo"].Cast<CheckBox>().CurrentValue; } }
        public static bool ComboUseW { get { return ComboMenu["Wcombo"].Cast<CheckBox>().CurrentValue; } }
        public static bool ComboUseE { get { return ComboMenu["Ecombo"].Cast<CheckBox>().CurrentValue; } }
        public static bool ComboUseR { get { return ComboMenu["Rcombo"].Cast<CheckBox>().CurrentValue; } }
        public static bool ComboWKnock { get { return ComboMenu["Wlimit"].Cast<CheckBox>().CurrentValue; } }
        public static bool ComboESlow { get { return ComboMenu["Elimit"].Cast<CheckBox>().CurrentValue; } }
        public static bool ComboRCooldown { get { return ComboMenu["Rcool"].Cast<CheckBox>().CurrentValue; } }
        public static int ComboRLimiter { get { return ComboMenu["Rlimit"].Cast<Slider>().CurrentValue; } }

        public static bool HarassUseQ { get { return HarassMenu["Qharass"].Cast<CheckBox>().CurrentValue; } }
        public static bool HarassUseE { get { return HarassMenu["Eharass"].Cast<CheckBox>().CurrentValue; } }
        public static int HarassMana { get { return HarassMenu["Mharass"].Cast<Slider>().CurrentValue; } }

        public static bool JungleUseQ { get { return JungleMenu["Qjungle"].Cast<CheckBox>().CurrentValue; } }
        public static bool JungleUseW { get { return JungleMenu["Wjungle"].Cast<CheckBox>().CurrentValue; } }
        public static bool JungleUseE { get { return JungleMenu["Ejungle"].Cast<CheckBox>().CurrentValue; } }
        public static int JungleMana { get { return JungleMenu["Mjungle"].Cast<Slider>().CurrentValue; } }

        public static bool LaneClearUseQ { get { return LaneClearMenu["Qlanec"].Cast<CheckBox>().CurrentValue; } }
        public static bool LaneClearUseW { get { return LaneClearMenu["Wlanec"].Cast<CheckBox>().CurrentValue; } }
        public static bool LaneClearUseE { get { return LaneClearMenu["Elanec"].Cast<CheckBox>().CurrentValue; } }
        public static int LaneClearMana { get { return LaneClearMenu["Mlanec"].Cast<Slider>().CurrentValue; } }

        public static bool LastHitUseQ { get { return LastHitMenu["Qlasthit"].Cast<CheckBox>().CurrentValue; } }
        public static bool LastHitUseW { get { return LastHitMenu["Wlasthit"].Cast<CheckBox>().CurrentValue; } }
        public static bool LastHitUseE { get { return LastHitMenu["Elasthit"].Cast<CheckBox>().CurrentValue; } }
        public static int LastHitMana { get { return LastHitMenu["Mlasthit"].Cast<Slider>().CurrentValue; } }

        public static bool KsMode { get { return KillStealMenu["Uks"].Cast<CheckBox>().CurrentValue; } }
        public static bool KsUseQ { get { return KillStealMenu["Qks"].Cast<CheckBox>().CurrentValue; } }
        public static bool KsUseW { get { return KillStealMenu["Wks"].Cast<CheckBox>().CurrentValue; } }
        public static bool KsUseE { get { return KillStealMenu["Eks"].Cast<CheckBox>().CurrentValue; } }
        public static bool KsUseR { get { return KillStealMenu["Rks"].Cast<CheckBox>().CurrentValue; } }
        public static int KsUltLimiter { get { return KillStealMenu["Kslimit"].Cast<Slider>().CurrentValue; } }

        public static bool DrawerMode { get { return DrawingMenu["Udraw"].Cast<CheckBox>().CurrentValue; } }
        public static bool DrawQw { get { return DrawingMenu["QWdraw"].Cast<CheckBox>().CurrentValue; } }
        public static bool DrawE { get { return DrawingMenu["Edraw"].Cast<CheckBox>().CurrentValue; } }
        public static bool DrawR { get { return DrawingMenu["Rdraw"].Cast<CheckBox>().CurrentValue; } }
        public static bool DesignerMode { get { return DrawingMenu["Udesign"].Cast<CheckBox>().CurrentValue; } }
        public static int DesignerSkin { get { return DrawingMenu["Sdesign"].Cast<Slider>().CurrentValue; } }

        public static bool LevelerMode { get { return SettingMenu["Ulevel"].Cast<CheckBox>().CurrentValue; } }
        public static bool StackMode { get { return SettingMenu["Ustack"].Cast<CheckBox>().CurrentValue; } }
        public static bool UltimateFollower { get { return SettingMenu["Uultimate"].Cast<CheckBox>().CurrentValue; } }

        public static bool InterrupterMode { get { return SettingMenu["Uinterrupt"].Cast<CheckBox>().CurrentValue; } }
        public static bool InterrupterUseE { get { return SettingMenu["Einterrupt"].Cast<CheckBox>().CurrentValue; } }
        public static bool GapCloserMode { get { return SettingMenu["Ugapc"].Cast<CheckBox>().CurrentValue; } }
        public static bool GapCloserUseE { get { return SettingMenu["Egapc"].Cast<CheckBox>().CurrentValue; } }
    }
}
