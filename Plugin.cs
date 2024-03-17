using System.IO;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace BestestTVModPlugin
{
    // Token: 0x02000003 RID: 3
    [BepInPlugin("DeathWrench.BestestTelevisionMod", "BestestTelevisionMod", "1.0.0")]
    public class BestestTVModPlugin : BaseUnityPlugin
    {
        // Token: 0x06000002 RID: 2 RVA: 0x0000205C File Offset: 0x0000025C
        private void Awake()
        {
            ConfigManager.Init(Config);
            instance = this;
            pluginpath = $"{Paths.PluginPath}{Path.DirectorySeparatorChar}Television Videos";
            if (Directory.Exists(pluginpath))
            {
                files = Directory.GetFiles(pluginpath);
            }
            else
            {
                files = null;
            }
            Debug.Log(files);
            int num = 0;
            int num2 = 0;
            foreach (string text in files)
            {
                num++;
                bool flag3 = text != null && text.Contains(".mp4");
                if (flag3)
                {
                    num2++;
                }
            }
            filePaths = new string[num2];
            int num3 = 0;
            for (int j = 0; j < num; j++)
            {
                bool flag4 = files[j] != null && files[j].Contains(".mp4");
                if (flag4)
                {
                    filePaths[num3] = files[j];
                    Log.LogInfo($"Loaded file: {filePaths[num3]}");
                    num3++;
                }
            }
            Log = Logger;
            Harmony.PatchAll();
            VideoManager.Load();
        }

        // Token: 0x04000004 RID: 4
        private static readonly Harmony Harmony = new Harmony("DeathWrench.BestTelevisionMod");

        // Token: 0x04000005 RID: 5
        public static string[] filePaths;

        // Token: 0x04000006 RID: 6
        private string[] files;

        // Token: 0x04000007 RID: 7
        public static BestestTVModPlugin instance;

        // Token: 0x04000008 RID: 8
        public static ManualLogSource Log = new ManualLogSource("TVLoader");

        // Token: 0x04000009 RID: 9
        private string pluginpath;
    }
}
