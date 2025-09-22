using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using LBoL.Base;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace LbolDreamStartOfTurnMod
{
    [BepInPlugin(LbolDreamStartOfTurnMod.PInfo.GUID, LbolDreamStartOfTurnMod.PInfo.Name, LbolDreamStartOfTurnMod.PInfo.version)]
    [BepInDependency(AddWatermark.API.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInProcess("LBoL.exe")]
    public class BepinexPlugin : BaseUnityPlugin
    {
        //The Unique mod ID of the mod.
        //If defined, this is also the ID used by the Act 1 boss.
        //WARNING: It is mandatory to rename it to avoid issues.
        public static string modUniqueID = "DreamStartOfTurnMod";

        private static readonly Harmony harmony = LbolDreamStartOfTurnMod.PInfo.harmony;

        internal static BepInEx.Logging.ManualLogSource log;


        void Awake()
        {
            log = Logger;
            // very important. Without this the entry point MonoBehaviour gets destroyed
            DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.HideAndDontSave;

            log.LogInfo("Running Harmony patches for LbolDreamStartOfTurnMod");
            harmony.PatchAll();
            log.LogInfo("Harmony patches for LbolDreamStartOfTurnMod run");

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(AddWatermark.API.GUID))
            { 
                WatermarkWrapper.ActivateWatermark();
            }
        }

        void OnDestroy()
        {
            if (harmony != null)
            { 
                harmony.UnpatchSelf();
            }
        }
    }
}
