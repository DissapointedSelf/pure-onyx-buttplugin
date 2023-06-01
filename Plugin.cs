using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using Eromancer;
using Eromancer.Animation;
using Eromancer.GameDB;
using Buttplug;
using Buttplug.Client.Connectors.WebsocketConnector;
using Buttplug.Client;


    [BepInPlugin("org.bepinex.plugins.onyxplugin", "Onyx Plug-in", "1.0.0.0")]
    [BepInProcess("pure-onyx.exe")]
    //[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {

        private readonly Harmony harmony = new Harmony("org.bepinex.plugins.onyxplugin");

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin org.bepinex.plugins.onyxplugin is loaded!");
            harmony.PatchAll( typeof(Sex_Scene_Patch) );
        }

        
        class Sex_Scene_Patch
        {

            static string animationName = "";
            static float animationLength = 0;

            static GameObject buttplugObj;
            static StartServerProcessAndScan buttServer;

            //any faster and bluetooth will break
            //sometimes you need 0.5f
            static float waitTime = 0.2f;
            static float timer = 0.0f;


            //[HarmonyPatch(typeof(SexScenePlayer),"PostScene")]
            [HarmonyPatch(typeof(Animator3D),"LoadAnimation")]
            [HarmonyPrefix]
            static bool Prefix(EromancerAnimation animation, string slotName)
            {
               
                if (animation.Clip != null){
                    FileLog.Log(animation.Clip.name);
                }
                return true;
            }


            [HarmonyPatch(typeof(Animator3D),"PlayAnimation")]
            [HarmonyPrefix]
            static bool PlayAnimation(string animationSlot, float playbackSpeed, bool clearAnimationQueue, Dictionary<string, EromancerAnimation> ___animations)
            {
                animationName = animationSlot;
                
                //player animations have A0 and A1 so just selecting A0 will get all them
                if(animationSlot.Contains("A0") ){
                    
                    //scale the animation by the playback speed
                    animationLength = ___animations[animationSlot].Length / playbackSpeed;

                    //FileLog.Log("animationSlot: " + animationSlot + " playbackSpeed: " + playbackSpeed.ToString() + " Length: " +  ___animations[animationSlot].Length.ToString());
                }
                return true;
            }

            [HarmonyPatch(typeof(Animator3D),"Update")]
            [HarmonyPrefix]
            static bool upd( Dictionary<string, EromancerAnimation> ___animations, float ___currentAnimationRemainingTime, Animator3D __instance )
            {

                if (___currentAnimationRemainingTime < 5f){
                    if(__instance.CurrentAnimationName.Contains("A0") ){
                        int percent = (int)(___currentAnimationRemainingTime / animationLength * 10 );
                        //FileLog.Log("Anim_update CurrentAnimationName: "+ __instance.CurrentAnimationName +" Animation remaining time:" +  ___currentAnimationRemainingTime.ToString() + " animation Length: " + animationLength.ToString());
                        //FileLog.Log("Anim_update CurrentAnimationName: "+ __instance.CurrentAnimationName +" Animation percent: " + percent.ToString());

                        if (timer > waitTime){
                            buttServer.UpdateDevices( ___currentAnimationRemainingTime / animationLength );
                            timer = 0;
                        }

                        timer += Time.deltaTime;
                    
                    }
                }
                return true;
            }
            
            
            [HarmonyPatch(typeof(SexScenePlayer),"PreScene")]
            [HarmonyPrefix]
            static bool PreScene( SexScenePlayer __instance )
            {
                if (buttplugObj == null){
                    buttplugObj = new GameObject("ButtController");
                    buttServer= buttplugObj.AddComponent<StartServerProcessAndScan>() as StartServerProcessAndScan;
                }
                return true;
            }
            /*
            [HarmonyPatch(typeof(SexScenePlayer),"PostScene")]
            [HarmonyPrefix]
            static bool PostScene( SexScenePlayer __instance )
            {

                Destroy(buttplugObj);
         
                return true;
            }
            */
            
        }
    }

    


