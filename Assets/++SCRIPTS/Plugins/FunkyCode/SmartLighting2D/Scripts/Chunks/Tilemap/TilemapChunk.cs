using System.Collections.Generic;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Components.LightTilemap2D;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Components.LightTilemap2D.Types;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings;
using UnityEngine;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Chunks.Tilemap
{
    public class TilemapManager
    {
        public LightTile[] display;

        static public int ChunkSize = 10;
        public List<LightTile>[,] maps;

        private int distplayCount = 0;
        private List<LightTile> tiles;
        private Base tilemapCollider;

        private bool initialized = false;

        void Initialize() {
            if (initialized == false) {
                initialized = true;

                ChunkSize = Lighting2D.ProjectSettings.chunks.chunkSize;

                display = new LightTile[1000];
                maps = new List<LightTile>[100, 100];
            }
        }

        public int GetTiles(Rect worldRect) {
            distplayCount = 0;

            if (Lighting2D.ProjectSettings.chunks.enabled) {
                GenerateChunks(worldRect);
            } else {
                GenerateSimple(worldRect);
            }

            return(distplayCount);
        }

        public void Update(List<LightTile> tiles, Base tilemapCollider) {
            this.tiles = tiles;

            Initialize();

            this.tilemapCollider = tilemapCollider;

            if (Lighting2D.ProjectSettings.chunks.enabled == false) {
                return;
            }

            for(int x = 0; x < 100; x++) {
                for(int y = 0; y < 100; y++) {
                    maps[x, y] = new List<LightTile>();
                }
            }

            foreach(LightTile tile in tiles) {
                Vector2 tilePosition = tile.GetWorldPosition(tilemapCollider);
                Vector2Int chunkPosition = Transform(tilePosition);

                maps[chunkPosition.x, chunkPosition.y].Add(tile);
             }
        }

        private void GenerateChunks(Rect worldRect) {
            Initialize();

            Vector2 p0 = new Vector2(worldRect.x, worldRect.y);
            Vector2 p1 = new Vector2(worldRect.x + worldRect.width, worldRect.y + worldRect.height);

            Vector2Int tp0 = Transform(p0);
            Vector2Int tp1 = Transform(p1);

            if (tp0.x > tp1.x) {
                tp1.x += 100;
            }

            if (tp0.y > tp1.y) {
               tp1.y += 100;
            }

            for(int x = tp0.x; x <= tp1.x; x++) {
                for(int y = tp0.y; y <= tp1.y; y++) {
                    int tx = x % 100;
                    int ty = y % 100;

                    if (maps[tx, ty] == null) {
                        continue;
                    }

                    foreach(LightTile tile in maps[tx, ty]) {
                        if (distplayCount < 1000) {
                            display[distplayCount] = tile;

                            distplayCount ++;
                        }
                    }
                }
            }
        }

        private void GenerateSimple(Rect worldRect) {
            if (tiles == null) {
                return;
            }

            if (tilemapCollider == null) {
                return;
            }

            foreach(LightTile tile in tiles) {
                if (distplayCount < 1000) {
                    Vector2 tilePosition = tile.GetWorldPosition(tilemapCollider);

                    if (worldRect.Contains(tilePosition)) {
                        display[distplayCount] = tile;

                        distplayCount ++;
                    }

                } else {
                    UnityEngine.Debug.LogWarning("Smart Lighting 2D: Tiles cache overflow");
                }
            }
        }

        static public Vector2Int Transform(Vector2 position) {
            float tx = (position.x / ChunkSize);
            float ty = (position.y / ChunkSize);

            int txInt = (int)tx;
            int tyInt = (int)ty;

            if (txInt < 0) {
                txInt = txInt + 1000;
            }

            if (tyInt < 0) {
                tyInt = tyInt + 1000;
            }

            txInt = txInt % 100;
            tyInt = tyInt % 100;

            return(new Vector2Int(txInt, tyInt));
        }

        static public Vector2Int TransformBounds(Vector2 position) {
            float tx = (position.x / ChunkSize);
            float ty = (position.y / ChunkSize);

            return(new Vector2Int(Mathf.FloorToInt(tx), Mathf.FloorToInt(ty)));
        }
    }
}
