using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using Eromancer;
using Eromancer.Animation;
using Eromancer.GameDB;
using UnityEngine.Networking;
/*

    Classes are pulled from dnspy decompiling the game
    you can search the classes/methods pretty easily, c# decompiles nicely

    https://github.com/BepInEx/HarmonyX/wiki
    https://harmony.pardeike.net/articles/intro.html
    https://docs.bepinex.dev/articles/user_guide/installation/index.html

*/




    [BepInPlugin("org.bepinex.plugins.onyxplugin", "Onyx Plug-in", "1.0.0.0")]
    [BepInProcess("pure-onyx.exe")]
    public class Plugin : BaseUnityPlugin
    {

        //inject BepinWex
        private readonly Harmony harmony = new Harmony("org.bepinex.plugins.onyxplugin");

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin org.bepinex.plugins.onyxplugin is loaded!");
            harmony.PatchAll( typeof(Sex_Scene_Patch) );
        }

        
        class Sex_Scene_Patch
        {

            //static variables can be accessed by patch methods
            static string animationName = "";
            static float timer = 0.0f;

            public static void sendCommand(string name){


                //dev version
                //var request = UnityWebRequest.Post("http://localhost:7105/Edi/Play/"+name+"?seek=0", "");
                //release version
                var request = UnityWebRequest.Post("http://localhost:5000/Edi/Play/"+name+"?seek=0", "");
                 request.SetRequestHeader("accept", "*/*");
                request.SendWebRequest();
            }
             public static void stopCommand(){
                var request = UnityWebRequest.Post("http://localhost:5000/Edi/Stop/", "");
                request.SetRequestHeader("accept", "*/*");
                request.SendWebRequest();
             }




            [HarmonyPatch(typeof(Animator3D),"PlayAnimation")]
            [HarmonyPrefix]
            static bool PlayAnimation(string animationSlot, ref float playbackSpeed, bool clearAnimationQueue, Dictionary<string, EromancerAnimation> ___animations)
            {
                animationName = animationSlot;
                
                //H animations are distinguished by actors (A0,A1,A2)
                //A0 is in every scene
                if(animationSlot.Contains("A0") ){


                    //  restricts the playback speed, seems to just cause jittery movements
                    //  playbackSpeed = 1f;

                    //scale the animation by the playback speed
                    //animationLength = ___animations[animationSlot].Length / playbackSpeed;


                    //FileLog.Log("animationSlot: " + animationSlot + " playbackSpeed: " + playbackSpeed.ToString() + " Length: " +  ___animations[animationSlot].Length.ToString());
                    
                    
                    //logs the output to do most the work of setting up the csv
                    //need to process the file so the timer lines up with the video and duplicate animations are removed
                    int currTime = (int)(timer*100);

                    //add the speed to the end of the name to distinguish for edi
                    //this value should be passed to /edi/play/{animName}
                    string animName = animationSlot +"_"+ ((int)(___animations[animationSlot].Length * 100)).ToString();

                    //log to Desktop/harmony.log.txt animation timings(for definition.csv)
                    //FileLog.Log("---" + currTime.ToString() + ",gallery,True");
                    //FileLog.Log(animName + ",onyx_gallery,"+ currTime.ToString() + ",");


                    sendCommand(animName);
                    /* Example output target(after processing)
                    OnyxVsWraxeLift_A0_S0_45,1,120227,120227,gallery,True
                    OnyxVsWraxeLift_A0_H0_30,1,197219,197219,gallery,True
                    OnyxVsWraxeLift_A0_H1_9,1,287330,287330,gallery,True
                    */
                }
                return true;
            }

            //simple timer for animation logging
            [HarmonyPatch(typeof(Animator3D),"Update")]
            [HarmonyPrefix]
            static bool upd()
            {
                timer += Time.deltaTime;
                return true;
            }

            //runs after a sexscene sends device stop signal
            [HarmonyPatch(typeof(SexScenePlayer),"PostScene")]
            [HarmonyPrefix]
            static bool PostScene( SexScenePlayer __instance )
            {
                stopCommand();
                return true;
            }
            
            /*
            [HarmonyPatch(typeof(SexScenePlayer),"PreScene")]
            [HarmonyPrefix]
            static bool PreScene( SexScenePlayer __instance )
            {
                //runs when a scene starts to play
                //pretty good place to start a game object

                
                if (buttplugObj == null){
                    buttplugObj = new GameObject("ButtController");
                    buttServer= buttplugObj.AddComponent<StartServerProcessAndScan>() as StartServerProcessAndScan;
                }
                
                return true;
            }

            //runs when animations are loaded, outputs all animation names for a level/gallery
            HarmonyPatch(typeof(Animator3D),"LoadAnimation")]
            [HarmonyPrefix]
            static bool Prefix(EromancerAnimation animation, string slotName)
            {
               
                if (animation.Clip != null){
                    FileLog.Log(animation.Clip.name);
                }
                return true;
            }

            */
            
        }
    }

    


