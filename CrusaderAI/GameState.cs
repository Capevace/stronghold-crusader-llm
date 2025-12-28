using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using CrusaderDE;
using UnityEngine;
using System.IO;
using System.Text;
using SimpleJSON;

namespace CrusaderAI
{
    public static class Jsonifiers
    {
        public static Dictionary<string, object> ToDict(this GameState state)
        {
            if (state == null) return null;
            return new Dictionary<string, object>
            {
                { "Difficulty", state.Difficulty.ToString() },
                { "Mode", state.Mode.ToString() },
                { "Player", state.Player.ToDict() },
                { "Mission", state.Mission.ToDict() },
                { "Map", state.Map.ToDict() },
            };
        }

        public static Dictionary<string, object> ToDict(this Player player)
        {
            return new Dictionary<string, object>
            {
                { "ID", player.ID },
                { "Name", player.Name },
            };
        }

        public static Dictionary<string, object> ToDict(this Mission mission)
        {
            return new Dictionary<string, object>
            {
                { "Text", mission.Text },
            };
        }
        
        public static Dictionary<string, object> ToDict(this Map map)
        {
            return new Dictionary<string, object>
            {
                { "Size", map.Size },
            };
        }

        public static Dictionary<string, object> ToDict(this MapTile tile)
        {
            return new Dictionary<string, object>
            {
                { "Position", tile.Position.ToDict() },
                { "WorldPosition", tile.WorldPosition.ToDict() },
                { "Height", tile.Height },
                { "BuildingHeight", tile.BuildingHeight },
                { "FileIndex", tile.FileIndex },
                { "ImageIndex", tile.ImageIndex },
            };
        }

        public static Dictionary<string, object> ToDict(this Vector2Int v)
        {
            return new Dictionary<string, object>
            {
                { "x", v.x },
                { "y", v.y },
            };
        }
    }

    public class GameStateCache
    {
        public static GameStateCache Instance { get; } = new GameStateCache();

        public GameState State;
        
        private float _lastWriteTime = 0f;
        private const float WriteInterval = 2.0f;
        
        private GameStateCache()
        {
            this.State = GameState.Initial();
        }
        
        public void WriteStateToFile()
        {
            if (Time.time < _lastWriteTime + WriteInterval)
            {
                return;
            }
            
            _lastWriteTime = Time.time;
            
            try
            {
                var json = JSONEncoder.Encode(this.State.ToDict());

                string filePath = Path.Combine(Paths.GameRootPath, "GameState.json");
                File.WriteAllText(filePath, json);

                Plugin.Log.LogInfo("Successfully wrote GameState.json");
                
                WriteTilesToCsv(this.State.Map);
            } catch (Exception ex)
            {
                Plugin.Log.LogError("Failed to write game state to file.");
                Plugin.Log.LogError(ex.ToString());
            }
        }
        
        private void WriteTilesToCsv(Map map)
        {
            if (map.Tiles == null) return;
        
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("PositionX,PositionY,WorldPositionX,WorldPositionY,Height,BuildingHeight,FileIndex,ImageIndex");
        
                for (int x = 0; x < map.Tiles.GetLength(0); x++)
                {
                    for (int y = 0; y < map.Tiles.GetLength(1); y++)
                    {
                        var tile = map.Tiles[x, y];

                        if (
                            tile.Position.x == 0
                            && tile.Position.y == 0
                            && tile.WorldPosition.x == 0
                            && tile.WorldPosition.y == 0
                            && tile.Height == 0
                            && tile.BuildingHeight == 0
                            && tile.FileIndex == 0
                            && tile.ImageIndex == 0
                        )
                        {
                            continue;
                        }
                        
                        var line = string.Format(System.Globalization.CultureInfo.InvariantCulture, 
                            "{0},{1},{2},{3},{4},{5},{6},{7}",
                            tile.Position.x,
                            tile.Position.y,
                            tile.WorldPosition.x,
                            tile.WorldPosition.y,
                            tile.Height,
                            tile.BuildingHeight,
                            tile.FileIndex,
                            tile.ImageIndex);
                        
                        sb.AppendLine(line);
                    }
                }
        
                string csvFilePath = Path.Combine(Paths.GameRootPath, "Tiles.csv");
                File.WriteAllText(csvFilePath, sb.ToString());
                Plugin.Log.LogInfo("Successfully wrote Tiles.csv");
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError("Failed to write tiles to CSV.");
                Plugin.Log.LogError(ex.ToString());
            }
        }
    }
    
    public class GameState
    {
        public Enums.GameDifficulty Difficulty;
        public Enums.GameModes Mode;
        
        public Player Player;
        public Mission Mission;
        public Map Map;
        
        public static GameState Initial()
        {
            var playState = GameData.Instance.lastGameState;
            var gameData = GameData.Instance;
            var gameMap = GameMap.instance;
            var scoreData = EngineInterface.GetScoreData();
            var multiplayerScoreData = EngineInterface.GetMPScoreData();
            
            return new GameState(playState, gameData, gameMap, scoreData, multiplayerScoreData);
        }
        
        public GameState(
            EngineInterface.PlayState playState,
            GameData gameData,
            GameMap gameMap,
            EngineInterface.ScoreData scoreData,
            EngineInterface.MPScoreData multiplayerScoreData
        )
        {
            this.Difficulty = gameData.difficulty_level;
            this.Mode = gameData.mapType;
            
            this.Player = new Player
            {
                ID = gameData.playerID,
                Name = gameData.playerID.ToString()
            };

            this.Mission = new Mission
            {
                Text = gameData.utf8MissionText
            };

            this.Map = new Map(gameMap);
            
            // gameMap.getRadarTexture()
            //     EditorDirector.instance.loadSaveGame();
            //     MainViewModel.Instance.HUDRoot.
                    
                    
        }
    }

    public struct Player
    {
        public int ID;
        public string Name;
    }

    public struct Mission
    {
        public string Text;
    }

    public struct Map
    {
        public int Size;
        public MapTile[,] Tiles;
        
        public Map(GameMap map)
        {
            this.Size = map.getMapTileSize() * 2;
            this.Tiles = new MapTile[this.Size, this.Size];
            
            for (int x = 0; x < this.Size; x++)
            {
                for (int y = 0; y < this.Size; y++)
                {
                    var tile = map.getMapTile(x, y);
                    
                    if (tile != null)
                    {
                        this.Tiles[x, y] = new MapTile(tile);
                    }
                    else
                    {
                        this.Tiles[x, y] = new MapTile();
                    }
                }
            }
        }
        
        public MapTile GetTile(int x, int y)
        {
            if (x < 0 || x >= this.Size || y < 0 || y >= this.Size)
            {
                throw new ArgumentOutOfRangeException($"Coordinates ({x}, {y}) are out of bounds for the map size {this.Size}.");
            }
            
            return this.Tiles[x, y];
        }
        
        public MapTile SetTile(int x, int y, MapTile tile)
        {
            if (x < 0 || x >= this.Size || y < 0 || y >= this.Size)
            {
                throw new ArgumentOutOfRangeException($"Coordinates ({x}, {y}) are out of bounds for the map size {this.Size}.");
            }
            
            this.Tiles[x, y] = tile;
            return tile;
        }
    }

    public struct MapTile
    {
        public Vector2Int Position;
        public Vector2Int WorldPosition;
        
        public float Height;
        public float BuildingHeight;
        
        public int FileIndex;
        public int ImageIndex;

        public MapTile(GameMapTile tile)
        {
            this.Position = new Vector2Int(tile.column, tile.row);
            this.WorldPosition = new Vector2Int(tile.gameMapX, tile.gameMapY);

            this.Height = tile.height;
            this.BuildingHeight = tile.buildingHeight;

            this.FileIndex = 0;
            this.ImageIndex = 0;
        }
    }
}