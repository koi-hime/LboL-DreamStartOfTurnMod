using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using LBoL.Base;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace DreamStartOfTurnMod_TopDeck
{
    /// <summary>
    /// Bepinex Plugin
    /// </summary>
    [BepInPlugin(PInfo.GUID, PInfo.Name, PInfo.version)]
    [BepInDependency(AddWatermark.API.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInProcess("LBoL.exe")]
    public class BepinexPlugin : BaseUnityPlugin
    {
        /// <summary>
        ///  The Unique mod ID of the mod.
        ///  If defined, this is also the ID used by the Act 1 boss.
        ///  WARNING: It is mandatory to rename it to avoid issues.
        /// </summary>
        public static string modUniqueID = "DreamStartOfTurn_TopDeck";

        private static readonly Harmony harmony = PInfo.harmony;

        internal static BepInEx.Logging.ManualLogSource log;


        void Awake()
        {
            log = Logger;
            // very important. Without this the entry point MonoBehaviour gets destroyed
            DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.HideAndDontSave;

            log.LogInfo("Running Harmony patches for DreamStartOfTurnMod_TopDeck");
            harmony.PatchAll();
            log.LogInfo("Harmony patches for DreamStartOfTurnMod_TopDeck run");

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
