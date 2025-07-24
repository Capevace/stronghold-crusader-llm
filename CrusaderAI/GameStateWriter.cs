using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using SimpleJSON;
using UnityEngine;

namespace CrusaderAI
{
    public static class GameStateWriter
    {
        private static float _lastWriteTime = 0f;
        private const float WriteInterval = 2.0f;

        private static Dictionary<string, object> ObjectToDict(object obj)
        {
            if (obj == null) return null;
            var dict = new Dictionary<string, object>();
            var fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var field in fields)
            {
                dict[field.Name] = field.GetValue(obj);
            }
            return dict;
        }

        public static void WritePlayStateToFile(EngineInterface.PlayState state)
        {
            if (Time.time < _lastWriteTime + WriteInterval)
            {
                return;
            }

            _lastWriteTime = Time.time;

            try
            {
                var dict = ObjectToDict(state);
                string jsonString = JSONEncoder.Encode(dict);

                string filePath = Path.Combine(Paths.GameRootPath, "PlayState.json");
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError("Error writing play state to JSON: " + ex.ToString());
            }
        }

        public static void WriteScoreStateToFile(EngineInterface.ScoreData data)
        {
            if (Time.time < _lastWriteTime + WriteInterval)
            {
                return;
            }

            _lastWriteTime = Time.time;

            try
            {
                var dict = ObjectToDict(data);
                string jsonString = JSONEncoder.Encode(dict);
                
                string filePath = Path.Combine(Paths.GameRootPath, "ScoreState.json");
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError("Error writing score state to JSON: " + ex.ToString());
            }
        }
    }
}