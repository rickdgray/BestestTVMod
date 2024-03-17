using BepInEx.Configuration;
using UnityEngine.Video;

namespace BestestTVModPlugin
{
    public class ConfigManager
    {
        public static ConfigManager Instance { get; private set; }

        public static void Init(ConfigFile config)
        {
            Instance = new ConfigManager(config);
        }

        public static ConfigEntry<bool> TvOnAlways { get; set; }
        public static ConfigEntry<bool> TvPlaysSequentially { get; set; }
        public static ConfigEntry<bool> TvSkipsAfterOffOn { get; set; }
        public static ConfigEntry<bool> EnableSeeking { get; set; }
        public static ConfigEntry<bool> EnableChannels { get; set; }
        public static ConfigEntry<bool> MouseWheelVolume { get; set; }
        public static ConfigEntry<bool> HideHoverTip { get; set; }
        public static ConfigEntry<bool> RestrictChannels { get; set; }
        public static ConfigEntry<bool> TvLightEnabled { get; set; }
        public static ConfigEntry<string> MediaFolder { get; set; }
        public static ConfigEntry<VideoAspectRatio> TvScalingOption { get; set; }
        public static ConfigFile ConfigFile { get; private set; }
        private ConfigManager(ConfigFile cfg)
        {
            ConfigFile = cfg;
            TvScalingOption = cfg.Bind("Options", "Aspect Ratio", VideoAspectRatio.FitVertically, "Available choices:\nNoScaling\nFitVertically\nFitHorizontally\nFitInside\nFitOutside\nStretch}");
            TvLightEnabled = cfg.Bind("Options", "Television Lights", true, "Do lights cast from television? If using Scaleable Television set this to false.");
            TvOnAlways = cfg.Bind("Options", "TV Always On", false, "Should the TV stay on after it's been turned on?\n Warning: TV Skips After Off On will skip twice as much with this enabled");
            TvPlaysSequentially = cfg.Bind("Options", "TV Plays Sequentially", true, "Play videos in order or loop?\n");
            TvSkipsAfterOffOn = cfg.Bind("Options", "TV Skips After Off On", false, "Should what is currently playing be skipped after the television is turned off and back on again?\n Warning: Minor UI bug where current channel will be + 1 of what it actually is if Enable Channels is checked");
            EnableSeeking = cfg.Bind("Options", "Enable Seeking", true, "Use brackets to fast forward or rewind?");
            EnableChannels = cfg.Bind("Options", "Enable Channels", true, "Use comma or period to skip videos?");
            MouseWheelVolume = cfg.Bind("Options", "Mouse Wheel Volume", true, "Should the mouse wheel control the volume?");
            HideHoverTip = cfg.Bind("Options", "Hide Hovertips", false, "Hide the controls when hovering over the TV");
            RestrictChannels = cfg.Bind("Options", "Restrict Channels", false, "Disable the channel controls, but keep the UI, unless Hide Hovertips is also checked");
        }
    }
}