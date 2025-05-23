﻿using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Light;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Components.LightTilemap2D.Types;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Misc;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Components.LightTilemap2D
{
	public class LightTilemapTransform
	{
		private bool update = true;

		public bool UpdateNeeded
		{
			get => update;
			set => update = value;
		}

		private Vector2 scale = Vector2.one;
		public Vector2 position = Vector2.one;
		public float rotation = 0;
		public Vector3 tilemapAnchor = Vector3.zero;
		public Vector3 tilemapCellSize = Vector3.zero;
		public Vector3 tilemapGapSize = Vector3.zero;

		public int sortingOrder = 0;
		public int sortingLayerID = 0;

		public void Update(LightTilemapCollider2D tilemapCollider2D)
		{
			Transform transform = tilemapCollider2D.transform;

			Vector2 position2D = LightingPosition.GetPosition2D(transform.position);
			Vector2 scale2D = transform.lossyScale;
			float rotation2D = transform.rotation.eulerAngles.z;

			update = false;

			if (scale != scale2D)
			{
				scale = scale2D;

				update = true;
			}

			if (position != position2D)
			{
				position = position2D;

				update = true;
			}

			if (rotation != rotation2D)
			{
				rotation = rotation2D;

				update = true;
			}

			if (tilemapCollider2D.mapType != MapType.SuperTilemapEditor)
			{
				Tilemap tilemap = GetTilemap(tilemapCollider2D.gameObject);

				if (tilemap != null)
				{
					if (tilemapAnchor != tilemap.tileAnchor)
					{
						tilemapAnchor = tilemap.tileAnchor;
						update = true;
					}
				}

				TilemapRenderer tilemapRenderer = GetTilemapRenderer(tilemapCollider2D.gameObject);

				if (tilemapRenderer != null)
				{
					int sortID = UnityEngine.SortingLayer.GetLayerValueFromID(tilemapRenderer.sortingLayerID);

					if (sortingLayerID != sortID)
					{
						sortingLayerID = sortID;
					}

					if (sortingOrder != tilemapRenderer.sortingOrder)
					{
						sortingOrder = tilemapRenderer.sortingOrder;
					}
				}

				Grid grid = GetGrid(tilemapCollider2D.gameObject);

				if (grid != null)
				{
					if(tilemapCellSize != grid.cellSize)
					{
						tilemapCellSize = grid.cellSize;

						update = true;
					}

					if (tilemapGapSize != grid.cellGap)
					{
						tilemapGapSize = grid.cellGap;

						update = true;
					}
				}
			}
		}

		public TilemapRenderer tilemapRenderer;

		public TilemapRenderer GetTilemapRenderer(GameObject gameObject)
		{
			if (tilemapRenderer == null)
			{
				tilemapRenderer = gameObject.GetComponent<TilemapRenderer>();
			}

			return(tilemapRenderer);
		}

		Tilemap tilemap = null;

		public Tilemap GetTilemap(GameObject gameObject)
		{
			if (tilemap == null)
			{
				tilemap = gameObject.GetComponent<Tilemap>();
			}
			return(tilemap);
		}

		Grid grid = null;

		public Grid GetGrid(GameObject gameObject)
		{
			if (grid == null)
			{
				Tilemap tilemap = GetTilemap(gameObject);

				if (tilemap != null)
				{
					grid = tilemap.layoutGrid;
				}
			}
			return(grid);
		}
	}
}
