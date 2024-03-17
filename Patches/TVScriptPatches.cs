using System.Reflection;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

namespace BestestTVModPlugin
{
    // Token: 0x02000004 RID: 4
    [HarmonyPatch(typeof(TVScript))]
    internal class TVScriptPatches
    {
        [HarmonyPatch(typeof(StartOfRound), "Start")]
        [HarmonyPostfix]
        public static void SetTVIndex()
        {
            TVIndex = 0;// TVScriptPatches.TVIndex % VideoManager.Videos.Count - 0;
        }
        // Token: 0x06000006 RID: 6 RVA: 0x000021D1 File Offset: 0x000003D1
        [HarmonyPatch(typeof(TVScript), "Update")]
        [HarmonyPrefix]
        public static bool Update(TVScript __instance)
        {
            if (currentVideoPlayer == null)
            {
                currentVideoPlayer = __instance.GetComponent<VideoPlayer>();
                if (VideoManager.Videos.Count > 0)
                {
                   // TVScriptPatches.PrepareVideo(__instance, 0);
                }
            }
            return false;
        }

        // Token: 0x06000007 RID: 7 RVA: 0x00002210 File Offset: 0x00000410
        [HarmonyPatch(typeof(TVScript), "TurnTVOnOff")]
        [HarmonyPrefix]
        public static bool TurnTVOnOff(bool on, TVScript __instance)
        {
            //int num2 = (int)TVScriptPatches.currentClipProperty.GetValue(__instance);
            int num2 = TVIndex;
            __instance.tvOn = on;
            bool flag = __instance.video.source != VideoSource.Url || __instance.video.url == "" || nextVideoPlayer.url == "" || currentVideoPlayer.url == "";
            bool flag2 = flag;
            if (flag2)
            {
                //if (ConfigManager.tvSkipsAfterOffOn.Value)
                //{
                //        num2 = (int)TVScriptPatches.currentClipProperty.GetValue(__instance) + 0;
                //        TVScriptPatches.TVIndex = num2;
                //}
                __instance.video.aspectRatio = ConfigManager.TvScalingOption.Value;
                //__instance.video.renderMode = ConfigManager.tvRenderMode.Value;
                Object.Destroy(nextVideoPlayer);
                nextVideoPlayer = __instance.gameObject.AddComponent<VideoPlayer>();
                nextVideoPlayer.clip = null;
                currentVideoPlayer.clip = null;
                __instance.video.clip = null;
                __instance.tvSFX.clip = null;
                BestestTVModPlugin.Log.LogInfo($"file://{BestestTVModPlugin.filePaths[TVIndex]}");
                __instance.video.url = $"file://{BestestTVModPlugin.filePaths[TVIndex]}";
                __instance.video.source = VideoSource.Url;
                __instance.video.controlledAudioTrackCount = 1;
                __instance.video.audioOutputMode = VideoAudioOutputMode.AudioSource;
                __instance.video.SetTargetAudioSource(0, __instance.tvSFX);
                __instance.video.Prepare();
                //if (!on && ConfigManager.tvSkipsAfterOffOn.Value)
                //{
                //    PrepareVideo(__instance); 
                //}
                //TVScriptPatches.nextVideoPlayer.Stop();
                //TVScriptPatches.currentVideoPlayer.Stop();
                __instance.video.Stop();
                __instance.tvSFX.Stop();
            }
            if (on)
            {
                BestestTVModPlugin.Log.LogInfo("Turning on TV");
                SetTVScreenMaterial(__instance, true);
                if (on && ConfigManager.TvSkipsAfterOffOn.Value)// && tvHasPlayedBefore)
                {
                    //num2 = (num2 + 1) % VideoManager.Videos.Count;
                   // TVScriptPatches.currentTimeProperty.SetValue(__instance, 0f);
                   // TVScriptPatches.currentClipProperty.SetValue(__instance, num2);
                    bool flag10 = num2 >= BestestTVModPlugin.filePaths.Length - 1;
                    if (flag10)
                    {
                        num2 = 0;
                    }
                    else
                    {
                        num2++;
                    }
                    TVIndex = num2;
                }
                __instance.tvSFX.Play();
                __instance.video.Play();
                __instance.tvSFX.PlayOneShot(__instance.switchTVOn);
                WalkieTalkie.TransmitOneShotAudio(__instance.tvSFX, __instance.switchTVOn, 1f);
            }
            else
            {
                if (!ConfigManager.TvOnAlways.Value)
                {
                    BestestTVModPlugin.Log.LogInfo("Turning on TV");
                    SetTVScreenMaterial(__instance, false);
                    __instance.tvSFX.Stop();
                    __instance.video.Stop();
                    __instance.tvSFX.PlayOneShot(__instance.switchTVOn);
                    WalkieTalkie.TransmitOneShotAudio(__instance.tvSFX, __instance.switchTVOff, 1f);
                }
                else
                {
                    BestestTVModPlugin.Log.LogInfo("Turning on TV");
                    SetTVScreenMaterial(__instance, true);
                    //if (ConfigManager.tvSkipsAfterOffOn.Value)
                    //{
                    //    num2 = (num2 + 1) % VideoManager.Videos.Count;
                    //    TVScriptPatches.currentTimeProperty.SetValue(__instance, 0f);
                    //    TVScriptPatches.currentClipProperty.SetValue(__instance, num2);
                    //}
                    if (!on && ConfigManager.TvSkipsAfterOffOn.Value)
                    {
                        //num2 = (num2 + 1) % VideoManager.Videos.Count;
                        // TVScriptPatches.currentTimeProperty.SetValue(__instance, 0f);
                        // TVScriptPatches.currentClipProperty.SetValue(__instance, num2);
                        bool flag10 = num2 >= BestestTVModPlugin.filePaths.Length - 1;
                        if (flag10)
                        {
                            num2 = 0;
                        }
                        else
                        {
                            num2++;
                        }
                        TVIndex = num2;
                    }
                    __instance.tvSFX.Play();
                    __instance.video.Play();
                    __instance.tvSFX.PlayOneShot(__instance.switchTVOn);
                    WalkieTalkie.TransmitOneShotAudio(__instance.tvSFX, __instance.switchTVOn, 1f);
                }
            }
            return false;
        }

        public static void SetTVScreenMaterial(TVScript __instance, bool b)
        {
            MethodInfo method = __instance.GetType().GetMethod("SetTVScreenMaterial", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(__instance, new object[]
            {
                b
            });
            if (!ConfigManager.TvLightEnabled.Value)
            {
                __instance.tvLight.enabled = false;
            }
        }

        // Token: 0x06000008 RID: 8 RVA: 0x00002308 File Offset: 0x00000508
        [HarmonyPatch(typeof(TVScript), "TVFinishedClip")]
        [HarmonyPrefix]
        public static bool TVFinishedClip(TVScript __instance, VideoPlayer _)
        {
            if (!__instance.tvOn || GameNetworkManager.Instance.localPlayerController.isInsideFactory || !ConfigManager.TvPlaysSequentially.Value)//(!__instance.tvOn || GameNetworkManager.Instance.localPlayerController.isInsideFactory)
            {
                return false;
            }
            BestestTVModPlugin.Log.LogInfo("TVFinishedClip");
            int num2 = (int)currentClipProperty.GetValue(__instance);
            if (VideoManager.Videos.Count > 0)
            {
                num2 = (num2 + 1) % VideoManager.Videos.Count;
                TVIndex = num2;
            }
            currentTimeProperty.SetValue(__instance, 0f);
            currentClipProperty.SetValue(__instance, num2);
            WhatItDo(__instance);
            return false;
        }

        // Token: 0x06000009 RID: 9 RVA: 0x0000239C File Offset: 0x0000059C
        private static void PrepareVideo(TVScript __instance, int num2 = -1)
        {
            if (num2 == -1)
            {
                num2 = (int)currentClipProperty.GetValue(__instance) + 1;
                TVIndex = num2;
            }
            if (nextVideoPlayer != null && nextVideoPlayer.gameObject.activeInHierarchy)
            {
                Object.Destroy(nextVideoPlayer);
            }
            nextVideoPlayer = __instance.gameObject.AddComponent<VideoPlayer>();
            nextVideoPlayer.playOnAwake = false;
            nextVideoPlayer.isLooping = false;
            nextVideoPlayer.source = VideoSource.Url;
            nextVideoPlayer.controlledAudioTrackCount = 1;
            nextVideoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            nextVideoPlayer.SetTargetAudioSource(0, __instance.tvSFX);
            nextVideoPlayer.url = $"file://{BestestTVModPlugin.filePaths[num2 % VideoManager.Videos.Count]}";
            nextVideoPlayer.Prepare();
            nextVideoPlayer.prepareCompleted += delegate (VideoPlayer source)
            {
                BestestTVModPlugin.Log.LogInfo("Prepared next video!");
            };
        }

        private static void WhatItDo(TVScript __instance, int num2 = -1)
        {
            //if (num2 == -1)
            //{
            //    num2 = (int)TVScriptPatches.currentClipProperty.GetValue(__instance) + 1;
            //    TVScriptPatches.TVIndex = num2;
            //}
            __instance.video.aspectRatio = ConfigManager.TvScalingOption.Value;
            Object.Destroy(nextVideoPlayer);
            nextVideoPlayer = __instance.gameObject.AddComponent<VideoPlayer>();
            nextVideoPlayer.clip = null;
            currentVideoPlayer.clip = null;
            __instance.video.clip = null;
            __instance.tvSFX.clip = null;
            BestestTVModPlugin.Log.LogInfo($"file://{BestestTVModPlugin.filePaths[TVIndex + 1]}");
            __instance.video.url = $"file://{BestestTVModPlugin.filePaths[TVIndex + 1]}";
            __instance.video.source = VideoSource.Url;
            __instance.video.controlledAudioTrackCount = 1;
            __instance.video.audioOutputMode = VideoAudioOutputMode.AudioSource;
            __instance.video.SetTargetAudioSource(0, __instance.tvSFX);
            __instance.video.Prepare();
            //if (!on && ConfigManager.tvSkipsAfterOffOn.Value)
            //{
            //    PrepareVideo(__instance); 
            //}
            nextVideoPlayer.Stop();
            currentVideoPlayer.Stop();
            __instance.video.Stop();
            __instance.tvSFX.Stop();
            __instance.video.Play();
            __instance.tvSFX.Play();
        }

        // Token: 0x0600000A RID: 10 RVA: 0x000024A0 File Offset: 0x000006A0
        
        // LethalTVManager.Patches.LethalTVController
        // Token: 0x0600000A RID: 10 RVA: 0x00002438 File Offset: 0x00000638
        [HarmonyPatch(typeof(PlayerControllerB), "Update")]
        [HarmonyPostfix]
        public static void GetTVInput(PlayerControllerB __instance)
        {
            bool flag = !__instance.IsOwner || !__instance.isPlayerControlled || __instance.inTerminalMenu || __instance.isTypingChat || __instance.isPlayerDead;
            if (!flag)
            {
                InteractTrigger hoveringOverTrigger = __instance.hoveringOverTrigger;
                bool flag2 = hoveringOverTrigger == null;
                object obj;
                if (flag2)
                {
                    obj = null;
                }
                else
                {
                    Transform parent = hoveringOverTrigger.transform.parent;
                    obj = ((parent != null) ? parent.gameObject : null);
                }
                bool flag3 = false;
                GameObject gameObject = (GameObject)obj;
                bool flag4 = gameObject != null;
                if (flag4)
                {
                    bool flag5 = gameObject.name.Contains("Television");
                    if (flag5)
                    {
                        flag3 = true;
                    }
                }
                bool flag6 = flag3;
                if (flag6)
                {
                    VideoPlayer componentInChildren = gameObject.GetComponentInChildren<VideoPlayer>();
                    AudioSource component = gameObject.transform.Find("TVAudio").GetComponent<AudioSource>();
                    double num = componentInChildren.time;
                    int num2 = TVIndex;
                    float num3 = Mouse.current.scroll.ReadValue().y;
                    float num4 = component.volume;
                    bool flag7 = componentInChildren != null;
                    if (flag7)
                    {
                        bool wasPressedThisFrame = Keyboard.current.leftBracketKey.wasPressedThisFrame;
                        if (wasPressedThisFrame && ConfigManager.EnableSeeking.Value)
                        {
                            num -= 15.0;
                            bool flag8 = num < 0.0;
                            if (flag8)
                            {
                                componentInChildren.time = 0.0;
                                BestestTVModPlugin.Log.LogInfo("AdjustTime: " + 0f.ToString());
                            }
                            else
                            {
                                componentInChildren.time = num;
                                BestestTVModPlugin.Log.LogInfo("AdjustTime: " + num.ToString());
                            }
                        }
                        bool wasPressedThisFrame2 = Keyboard.current.rightBracketKey.wasPressedThisFrame;
                        if (wasPressedThisFrame2 && ConfigManager.EnableSeeking.Value)
                        {
                            num += 15.0;
                            componentInChildren.time = num;
                            BestestTVModPlugin.Log.LogInfo("AdjustTime: " + num.ToString());
                        }
                        bool wasPressedThisFrame3 = Keyboard.current.commaKey.wasPressedThisFrame;
                        if (wasPressedThisFrame3 && ConfigManager.EnableChannels.Value && !ConfigManager.RestrictChannels.Value)// && !ConfigManager.tvSkipsAfterOffOn.Value && !ConfigManager.tvPlaysSequentially.Value)
                        {
                            bool flag9 = num2 > 0;
                            if (flag9)
                            {
                                num2--;
                                //num2 = (num2 - 1) % VideoManager.Videos.Count;
                            }
                            else
                            {
                                num2 = BestestTVModPlugin.filePaths.Length - 1;
                            }
                        }
                        bool wasPressedThisFrame4 = Keyboard.current.periodKey.wasPressedThisFrame;
                        if (wasPressedThisFrame4 && ConfigManager.EnableChannels.Value && !ConfigManager.RestrictChannels.Value)// && !ConfigManager.tvSkipsAfterOffOn.Value && !ConfigManager.tvPlaysSequentially.Value)
                        {
                            bool flag10 = num2 >= BestestTVModPlugin.filePaths.Length - 1;
                            if (flag10)
                            {
                                num2 = 0;
                            }
                            else
                            {
                                num2++;
                               // num2 = (num2 + 1) % VideoManager.Videos.Count;
                            }
                        }
                        Transform parent2 = componentInChildren.transform.parent;
                        InteractTrigger interactTrigger = (parent2 != null) ? parent2.GetComponentInChildren<InteractTrigger>() : null;
                        bool flag11 = interactTrigger == null;
                        if (flag11)
                        {
                            BestestTVModPlugin.Log.LogInfo("Television trigger missing!");
                        }
                        string Seek = $"Seek: [[][]]\n{num:0.###}";
                        string Volume = $"\nVolume: [-][+]\n{num4:0.##}";
                        string Channels = $"\nChannels: [,][.]\n{num2 + 1}";
                        if (!ConfigManager.HideHoverTip.Value)
                        {
                            if (ConfigManager.EnableSeeking.Value && ConfigManager.EnableChannels.Value && ConfigManager.MouseWheelVolume.Value)
                                interactTrigger.hoverTip = $"{Seek}{Volume}{Channels}";
                            if (ConfigManager.EnableSeeking.Value && ConfigManager.EnableChannels.Value && !ConfigManager.MouseWheelVolume.Value)
                                interactTrigger.hoverTip = $"{Seek}{Channels}";
                            if (ConfigManager.EnableSeeking.Value && !ConfigManager.EnableChannels.Value && ConfigManager.MouseWheelVolume.Value)
                                interactTrigger.hoverTip = $"{Seek}{Volume}";
                            if (!ConfigManager.EnableSeeking.Value && ConfigManager.EnableChannels.Value && ConfigManager.MouseWheelVolume.Value)
                                interactTrigger.hoverTip = $"{Volume}{Channels}";
                            if (!ConfigManager.EnableSeeking.Value && !ConfigManager.EnableChannels.Value && ConfigManager.MouseWheelVolume.Value)
                                interactTrigger.hoverTip = $"{Volume}";
                            if (ConfigManager.EnableSeeking.Value && !ConfigManager.EnableChannels.Value && !ConfigManager.MouseWheelVolume.Value)
                                interactTrigger.hoverTip = $"{Seek}";
                            if (!ConfigManager.EnableSeeking.Value && ConfigManager.EnableChannels.Value && !ConfigManager.MouseWheelVolume.Value)
                                interactTrigger.hoverTip = $"{Channels}";
                        }
                        bool flag12 = num2 != TVIndex;
                        if (flag12 && ConfigManager.EnableChannels.Value)
                        {
                            currentVideoPlayer.Stop();
                            TVIndex = num2;
                            componentInChildren.time = 0.0;
                            componentInChildren.url = $"file://{BestestTVModPlugin.filePaths[TVIndex]}";
                            currentClipProperty.SetValue(__instance, num2);
                            //TVScriptPatches.currentTimeProperty.SetValue(__instance, 0f);
                            //TVScriptPatches.currentVideoPlayer.url = "file://" + BestestTVModPlugin.filePaths[num2 % VideoManager.Videos.Count];
                            //TVScriptPatches.nextVideoPlayer.url = "file://" + BestestTVModPlugin.filePaths[VideoManager.Videos.Count];
                            BestestTVModPlugin.Log.LogInfo($"AdjustMediaFile: {BestestTVModPlugin.filePaths[TVIndex]}");
                        }
                        bool flag13 = num3 != 0f;
                        if (flag13 && ConfigManager.MouseWheelVolume.Value)
                        {
                            num3 /= 6000f;
                            num4 = Mathf.Clamp(num4 + num3, 0f, 1f);
                            component.volume = num4;
                            BestestTVModPlugin.Log.LogInfo($"Changed volume: {component.volume}");
                        }
                    }
                }
            }
        }


        // Token: 0x0600000B RID: 11 RVA: 0x000027C0 File Offset: 0x000009C0
        [HarmonyPatch(typeof(TVScript), "__initializeVariables")]
        [HarmonyPostfix]
        public static void SetTelevisionHoverTip(TVScript __instance)
        {
            Transform parent = __instance.transform.parent;
            InteractTrigger interactTrigger = (parent != null) ? parent.GetComponentInChildren<InteractTrigger>() : null;
            bool flag = interactTrigger == null;
            if (flag || !ConfigManager.EnableSeeking.Value && !ConfigManager.EnableChannels.Value && !ConfigManager.MouseWheelVolume.Value)
            {
                BestestTVModPlugin.Log.LogInfo("Television trigger missing!");
            }
            else
            {
                //interactTrigger.hoverTip = "Seek: [[][]]\nVolume: [-][+]\nChannels: [,][.]\n";
            }
        }
        // Token: 0x04000007 RID: 7
        private static readonly FieldInfo currentClipProperty = typeof(TVScript).GetField("currentClip", BindingFlags.Instance | BindingFlags.NonPublic);

        // Token: 0x04000008 RID: 8
        private static readonly FieldInfo currentTimeProperty = typeof(TVScript).GetField("currentClipTime", BindingFlags.Instance | BindingFlags.NonPublic);

        public static bool doweneedtodestroy = false;

        // Token: 0x0400000F RID: 15
        private static VideoPlayer currentVideoPlayer;

        // Token: 0x04000010 RID: 16
        private static VideoPlayer nextVideoPlayer;
        
        public Light tvLight;
        
        public static int TVIndex;
    }
}
