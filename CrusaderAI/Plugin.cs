using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace CrusaderAI
{
    [BepInPlugin("me.mateffy.CrusaderAI", "CrusaderAI", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;
            try
            {
                Log.LogInfo("CrusaderAI is loading...");
                var harmony = new Harmony("me.mateffy.CrusaderAI.patch");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                Log.LogInfo("CrusaderAI has been loaded and Harmony patches applied successfully!");
            }
            catch (Exception ex)
            {
                Log.LogError("Failed to apply Harmony patches for CrusaderAI.");
                Log.LogError(ex.ToString());
            }
        }
    }


    [HarmonyPatch(typeof(EngineInterface), "CopyPlayStateStruct")]
    public static class CopyPlayStateStruct_Patch
    {
        public static void Postfix(EngineInterface.PlayState __result)
        {
            if (__result != null)
            {
                GameStateWriter.WritePlayStateToFile(__result);
            }
        }
    }
    
    [HarmonyPatch(typeof(EngineInterface), "GetScoreData")]
    public static class GetScoreData_Patch
    {
        public static void Postfix(EngineInterface.ScoreData __result)
        {
            if (__result != null)
            {
                // GameStateWriter.WriteScoreStateToFile(__result);
            }
        }
    }
    
    // Example: Patching a game's update loop to add your own logic
    // NOTE: You will need to find a real class and method to patch. 
    // "Director" or "GameManager" are common names. I'm using "Director" as an example.
    [HarmonyPatch(typeof(Director), "Update")] // Replace "Director" with a real class from the game
    public static class Director_Update_Patch
    {
        // Postfix runs after the original method
        public static void Postfix() 
        {
            // Check if a key is pressed to trigger your function call
            if (UnityInput.Current.GetKeyDown(KeyCode.F8))
            {
                Plugin.Log.LogInfo("F8 pressed, calling DLL_SetDebugMode(1)...");

                // EngineInterface.DLL_MapAction()
                var members = Platform_Multiplayer.Instance.gameMembers;

                var ids = new List<int>();
                
                foreach (var member in members)
                {
                    if (member != null && member.playerID > 0)
                    {
                        ids.Add(member.playerID);
                    }
                }
                
                
                Platform_Multiplayer.Instance.SendLobbyChatMessage("Deine Mudder");
                

                Plugin.Log.LogInfo("DLL function called successfully!");
            }
        }
    }
    
    [HarmonyPatch(typeof(GameMap), "processTestMap")]
    public static class GameMap_processTestMap_Patch
    {
        // public void processTestMap(
        //     short[] sourceMap,
        //     int numElements,
        //     EngineInterface.PlayState state,
        //     byte[] radarMap)
        // {
        //     if (numElements < 0)
        
        // We're interested in accessing sourceMap, numElements and radarMap
        public static void Postfix(short[] sourceMap, int numElements, byte[] radarMap)
        {
            if (sourceMap == null || numElements <= 0)
            {
                return;
            }

            Plugin.Log.LogInfo($"--- Processing Frame with {numElements} elements ---");

            // Loop through each valid record in the sourceMap
            for (int i = 0; i < numElements; i++)
            {
                // The starting index for the current record
                int recordIndex = i * 21;

                // Extract the basic information you're interested in
                short recordType = sourceMap[recordIndex + 0];
                short x = sourceMap[recordIndex + 1];
                short y = sourceMap[recordIndex + 2];

                // Use a switch to handle different types of records
                switch (recordType)
                {
                    case 0: // This is a ground tile update
                        short tileFile = sourceMap[recordIndex + 3];
                        short tileImage = sourceMap[recordIndex + 4];
                        
                        // var tile = GameStateCache.Instance.State.Mas, y, tile);
                        
                        // Plugin.Log.LogInfo($"TILE: Coords=({x}, {y}), SpriteFile={tileFile}, SpriteImage={tileImage}");
                        break;

                    // case 2: // This is a "Chimp" (unit) update
                    //     short unitType = sourceMap[recordIndex + 3];
                    //     short unitImage = sourceMap[recordIndex + 4]; // This is the animation frame
                    //     int unitId = sourceMap[recordIndex + 10];
                    //     Plugin.Log.LogInfo($"UNIT: Coords=({x}, {y}), Type={unitType}, ID={unitId}");
                    //     break;
                
                    // You could add more cases here for other types like projectiles (3) or buildings (4)
                }
            }
                        
            GameStateCache.Instance.WriteStateToFile();
        }
    }
}
