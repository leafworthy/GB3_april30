﻿using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Components.LightTilemap2D.Types
{
	[System.Serializable]
    public class Hexagon : Base {

		private Tilemap tilemap2D;

		public override MapType TilemapType() {
			return(MapType.UnityHexagon);
		}

        public override void Initialize() {
			base.Initialize();

			if (UpdateProperties() == false) {
				return;
			}

			MapTiles.Clear();

			tilemap2D = properties.tilemap;

			ITilemap tilemap = (ITilemap) FormatterServices.GetUninitializedObject(typeof(ITilemap));
			typeof(ITilemap).GetField("m_Tilemap", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(tilemap, tilemap2D);

			foreach (Vector3Int position in tilemap2D.cellBounds.allPositionsWithin) {
				TileData tileData = new TileData();

				TileBase tilebase = tilemap2D.GetTile(position);

				if (tilebase != null) {
					tilebase.GetTileData(position, tilemap, ref tileData);

					LightTile lightTile = new LightTile();
					lightTile.gridPosition = new Vector3Int(position.x, position.y, 0);
					lightTile.SetSprite(tileData.sprite);
					lightTile.GetPhysicsShapePolygons();

					MapTiles.Add(lightTile);
				}
			}

			chunkManager.Update(MapTiles, this);
		}

		public override Vector2 TileWorldPosition(LightTile tile) {
			Vector2 resultPosition = properties.transform.position;

            Vector2 tilePosition = new Vector2(tile.gridPosition.x + tile.gridPosition.y / 2, tile.gridPosition.y);
            tilePosition.x += properties.cellAnchor.x;
            tilePosition.y += properties.cellAnchor.y;

            tilePosition.x = tilePosition.x + tilePosition.y * -0.5f;
            tilePosition.y = tilePosition.y * 0.75f;

            tilePosition.x *= properties.transform.lossyScale.x;
            tilePosition.y *= properties.transform.lossyScale.y;

            resultPosition += tilePosition;

            return(resultPosition);
		}

		public override float TileWorldRotation(LightTile tile) {
			float worldRotation = tilemap2D.transform.eulerAngles.z;

			return(worldRotation);
		}

		public override Vector2 TileWorldScale() {
          	Vector2 scale = new Vector2();

            scale.x = properties.cellSize.x * properties.transform.lossyScale.x;
            scale.y = properties.cellSize.y * properties.transform.lossyScale.y;

			return(scale);
        }
    }
}
