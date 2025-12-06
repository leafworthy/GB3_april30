using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace __SCRIPTS.Plugins._ISOSORT
{
	[ExecuteAlways, Serializable]
	public class IsoSpriteSortingManager : SerializedMonoBehaviour
	{
		#if UNITY_EDITOR
		private static float _lastEditorUpdateTime;
		private const float EDITOR_UPDATE_INTERVAL = 0.2f; // Update 5x per second in editor (slower)
		#endif

		[ShowInInspector] private static readonly List<IsoSpriteSorting> fgSpriteList = new(2048);
		[ShowInInspector] private static readonly List<IsoSpriteSorting> floorSpriteList = new(2048);
		[ShowInInspector] private static readonly List<IsoSpriteSorting> staticSpriteList = new(2049);
		[ShowInInspector] private static readonly List<IsoSpriteSorting> currentlyVisibleStaticSpriteList = new(2048);

		[ShowInInspector] private static readonly List<IsoSpriteSorting> moveableSpriteList = new(2048);
		[ShowInInspector] private static readonly List<IsoSpriteSorting> currentlyVisibleMoveableSpriteList = new(2048);
		[ShowInInspector] private static readonly List<IsoSpriteSorting> sortedSprites = new(2048);

		// Track which sprites moved this frame
		private static readonly List<IsoSpriteSorting> movedSprites = new(256);

		// Spatial partitioning grid for faster intersection checks
		private static SpatialGrid spatialGrid = new SpatialGrid(50f); // 50 unit cell size
		private static bool useSpatialhashing = true;

		// Cache for visibility checks
		private static int lastVisibilityFrame = -1;
		private static bool visibilityChanged = false;

		public static void RegisterSprite(IsoSpriteSorting newSprite)
		{
			if (newSprite.registered) return;

			if (newSprite.renderBelowAll)
			{
				floorSpriteList.Add(newSprite);
				SortListBelow(floorSpriteList);
				SetSortOrderNegative(floorSpriteList);
			}
			else if (newSprite.renderAboveAll)
			{
				if (!fgSpriteList.Contains(newSprite)) fgSpriteList.Add(newSprite);
				SortListSimple(fgSpriteList);
				SetSortOrderTop(fgSpriteList);
			}
			else
			{
				if (newSprite.isMovable || !Application.isPlaying)
				{
					moveableSpriteList.Add(newSprite);
					newSprite.InitializePosition();
				}
				else
				{
					staticSpriteList.Add(newSprite);
					newSprite.SetupStaticCache();
					SetupStaticDependencies(newSprite);
				}
			}

			newSprite.registered = true;
		}

		private static void SetupStaticDependencies(IsoSpriteSorting newSprite)
		{
			var the_count = staticSpriteList.Count;

			if (useSpatialhashing && the_count > 100)
			{
				// Use spatial hashing for large sprite counts
				var nearbySprites = spatialGrid.GetNearby(newSprite.TheBounds);
				foreach (var otherSprite in nearbySprites)
				{
					if (otherSprite == newSprite) continue;
					if (!staticSpriteList.Contains(otherSprite)) continue;

					if (CalculateBoundsIntersection(newSprite, otherSprite))
					{
						AddStaticDependency(newSprite, otherSprite);
					}
				}
				spatialGrid.Add(newSprite);
			}
			else
			{
				// Brute force for small counts
				for (var i = 0; i < the_count; i++)
				{
					var otherSprite = staticSpriteList[i];
					if (CalculateBoundsIntersection(newSprite, otherSprite))
					{
						AddStaticDependency(newSprite, otherSprite);
					}
				}
			}
		}

		private static void AddStaticDependency(IsoSpriteSorting newSprite, IsoSpriteSorting otherSprite)
		{
			var compareResult = IsoSpriteSorting.CompareIsoSorters(newSprite, otherSprite);
			if (compareResult == -1)
			{
				otherSprite.staticDependencies.Add(newSprite);
				newSprite.inverseStaticDependencies.Add(otherSprite);
			}
			else if (compareResult == 1)
			{
				newSprite.staticDependencies.Add(otherSprite);
				otherSprite.inverseStaticDependencies.Add(newSprite);
			}
		}

		public static void UnregisterSprite(IsoSpriteSorting spriteToRemove)
		{
			if (!spriteToRemove.registered) return;

			if (spriteToRemove.renderBelowAll)
			{
				floorSpriteList.Remove(spriteToRemove);
			}
			else if (spriteToRemove.renderAboveAll)
			{
				fgSpriteList.Remove(spriteToRemove);
			}
			else
			{
				if (spriteToRemove.isMovable || !Application.isPlaying)
				{
					moveableSpriteList.Remove(spriteToRemove);
				}
				else
				{
					staticSpriteList.Remove(spriteToRemove);
					spatialGrid.Remove(spriteToRemove);
					RemoveStaticDependencies(spriteToRemove);
				}
			}

			spriteToRemove.registered = false;
		}

		private static void RemoveStaticDependencies(IsoSpriteSorting spriteToRemove)
		{
			for (var i = 0; i < spriteToRemove.inverseStaticDependencies.Count; i++)
			{
				var otherSprite = spriteToRemove.inverseStaticDependencies[i];
				otherSprite.staticDependencies.Remove(spriteToRemove);
			}

			spriteToRemove.inverseStaticDependencies.Clear();
			spriteToRemove.staticDependencies.Clear();
		}

		private void Update()
		{
			#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				// Only update 5 times per second in editor mode
				float currentTime = (float)UnityEditor.EditorApplication.timeSinceStartup;
				if ((currentTime - _lastEditorUpdateTime) < EDITOR_UPDATE_INTERVAL)
					return;

				_lastEditorUpdateTime = currentTime;
			}
			#endif

			// Skip UpdateSorters call in play mode - it's editor-only
			#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				IsoSpriteSorting.UpdateSorters();
			}
			#endif

			// Track movement for moveable sprites
			UpdateMovementTracking();

			// Only update if something changed
			if (movedSprites.Count > 0 || visibilityChanged || !Application.isPlaying)
			{
				UpdateSorting();
			}
		}

		[Button]
		public void UpdateSortingButton()
		{
			IsoSpriteSorting.UpdateSorters();
			UpdateSorting();
		}

		private static void UpdateMovementTracking()
		{
			movedSprites.Clear();

			// Only check moveable sprites that are registered
			var count = moveableSpriteList.Count;
			for (var i = 0; i < count; i++)
			{
				var sprite = moveableSpriteList[i];
				if (sprite == null) continue;

				sprite.CheckForMovement();
				if (sprite.HasMoved())
				{
					movedSprites.Add(sprite);
				}
			}
		}

		public static void UpdateSorting()
		{
			// Filter visibility - track if it changed
			int prevStaticCount = currentlyVisibleStaticSpriteList.Count;
			int prevMoveableCount = currentlyVisibleMoveableSpriteList.Count;

			FilterListByVisibility(staticSpriteList, currentlyVisibleStaticSpriteList);
			FilterListByVisibility(moveableSpriteList, currentlyVisibleMoveableSpriteList);

			visibilityChanged = (prevStaticCount != currentlyVisibleStaticSpriteList.Count) ||
			                    (prevMoveableCount != currentlyVisibleMoveableSpriteList.Count);
			lastVisibilityFrame = Time.frameCount;

			// Only clear and recalculate if something changed
			if (movedSprites.Count > 0 || visibilityChanged || !Application.isPlaying)
			{
				ClearMovingDependencies(currentlyVisibleStaticSpriteList);
				ClearMovingDependencies(currentlyVisibleMoveableSpriteList);
				AddMovingDependencies(currentlyVisibleMoveableSpriteList, currentlyVisibleStaticSpriteList);
			}

			// Sort and set order
			sortedSprites.Clear();
			TopologicalSort.Sort(currentlyVisibleStaticSpriteList, currentlyVisibleMoveableSpriteList, sortedSprites);
			SetSortOrderBasedOnListOrder(sortedSprites);

			// Clear moved flags after processing
			for (var i = 0; i < movedSprites.Count; i++)
			{
				movedSprites[i].ClearMovedFlag();
			}
		}

		private static void AddMovingDependencies(List<IsoSpriteSorting> moveableList, List<IsoSpriteSorting> staticList)
		{
			var moveableCount = moveableList.Count;
			var staticCount = staticList.Count;

			// Use spatial hashing for large sprite counts
			if (useSpatialhashing && (moveableCount * staticCount) > 10000)
			{
				AddMovingDependenciesWithSpatialHash(moveableList, staticList);
				return;
			}

			// Standard brute force for smaller counts
			for (var i = 0; i < moveableCount; i++)
			{
				var moveSprite1 = moveableList[i];
				if (moveSprite1 == null) continue;

				var moveBounds = moveSprite1.TheBounds;

				// Check against static sprites
				for (var j = 0; j < staticCount; j++)
				{
					var staticSprite = staticList[j];
					if (moveBounds.Intersects(staticSprite.TheBounds))
					{
						var compareResult = IsoSpriteSorting.CompareIsoSorters(moveSprite1, staticSprite);
						if (compareResult == -1)
							staticSprite.movingDependencies.Add(moveSprite1);
						else if (compareResult == 1)
							moveSprite1.movingDependencies.Add(staticSprite);
					}
				}

				// Check against other moveable sprites
				for (var j = i + 1; j < moveableCount; j++)
				{
					var moveSprite2 = moveableList[j];
					if (moveSprite2 == null) continue;

					if (moveBounds.Intersects(moveSprite2.TheBounds))
					{
						var compareResult = IsoSpriteSorting.CompareIsoSorters(moveSprite1, moveSprite2);
						if (compareResult == -1)
							moveSprite2.movingDependencies.Add(moveSprite1);
						else if (compareResult == 1)
							moveSprite1.movingDependencies.Add(moveSprite2);
					}
				}
			}
		}

		private static void AddMovingDependenciesWithSpatialHash(List<IsoSpriteSorting> moveableList, List<IsoSpriteSorting> staticList)
		{
			// Update spatial grid with current static sprites
			spatialGrid.Clear();
			foreach (var sprite in staticList)
			{
				if (sprite != null) spatialGrid.Add(sprite);
			}

			var moveableCount = moveableList.Count;

			for (var i = 0; i < moveableCount; i++)
			{
				var moveSprite1 = moveableList[i];
				if (moveSprite1 == null) continue;

				var moveBounds = moveSprite1.TheBounds;

				// Only check nearby static sprites
				var nearbyStatic = spatialGrid.GetNearby(moveBounds);
				foreach (var staticSprite in nearbyStatic)
				{
					if (moveBounds.Intersects(staticSprite.TheBounds))
					{
						var compareResult = IsoSpriteSorting.CompareIsoSorters(moveSprite1, staticSprite);
						if (compareResult == -1)
							staticSprite.movingDependencies.Add(moveSprite1);
						else if (compareResult == 1)
							moveSprite1.movingDependencies.Add(staticSprite);
					}
				}

				// Check against other moveable sprites
				for (var j = i + 1; j < moveableCount; j++)
				{
					var moveSprite2 = moveableList[j];
					if (moveSprite2 == null) continue;

					if (moveBounds.Intersects(moveSprite2.TheBounds))
					{
						var compareResult = IsoSpriteSorting.CompareIsoSorters(moveSprite1, moveSprite2);
						if (compareResult == -1)
							moveSprite2.movingDependencies.Add(moveSprite1);
						else if (compareResult == 1)
							moveSprite1.movingDependencies.Add(moveSprite2);
					}
				}
			}
		}

		private static void ClearMovingDependencies(List<IsoSpriteSorting> sprites)
		{
			var count = sprites.Count;
			for (var i = 0; i < count; i++)
				sprites[i].movingDependencies.Clear();
		}

		private static bool CalculateBoundsIntersection(IsoSpriteSorting sprite, IsoSpriteSorting otherSprite) =>
			sprite.TheBounds.Intersects(otherSprite.TheBounds);

		private static void SetSortOrderBasedOnListOrder(List<IsoSpriteSorting> spriteList)
		{
			var orderCurrent = 420;
			var count = spriteList.Count;
			for (var i = 0; i < count; ++i)
			{
				spriteList[i].RendererSortingOrder = orderCurrent;
				orderCurrent += 10 + spriteList[i].renderersToSort.Count;
			}
		}

		private static void SetSortOrderNegative(List<IsoSpriteSorting> spriteList)
		{
			var currentIndex = -10000;
			for (var i = 0; i < spriteList.Count; ++i)
			{
				spriteList[i].RendererSortingOrder = spriteList[i].renderBelowSortingOrder + currentIndex + i;
				currentIndex += spriteList[i].renderersToSort.Count + 10;
			}
		}

		private static void SetSortOrderTop(List<IsoSpriteSorting> spriteList)
		{
			var currentIndex = 10000;
			for (var i = 0; i < spriteList.Count; ++i)
			{
				spriteList[i].RendererSortingOrder = currentIndex + i + spriteList[i].renderBelowSortingOrder;
				currentIndex += spriteList[i].renderersToSort.Count + 10;
			}
		}

		public static void FilterListByVisibility(List<IsoSpriteSorting> fullList, List<IsoSpriteSorting> destinationList)
		{
			destinationList.Clear();
			var count = fullList.Count;
			for (var i = 0; i < count; i++)
			{
				var sprite = fullList[i];
				if (sprite == null) continue;

				if (sprite.forceSort)
				{
					destinationList.Add(sprite);
					sprite.forceSort = false;
				}
				else
				{
					var rendererCount = sprite.renderersToSort.Count;
					for (var j = 0; j < rendererCount; j++)
					{
						var renderer = sprite.renderersToSort[j];
						if (renderer == null) continue;
						if (renderer.isVisible)
						{
							destinationList.Add(sprite);
							break;
						}
					}
				}
			}
		}

		private static void SortListSimple(List<IsoSpriteSorting> list)
		{
			list.Sort((a, b) =>
			{
				if (!a || !b) return 0;
				return IsoSpriteSorting.CompareIsoSorters(a, b);
			});
		}

		private static void SortListBelow(List<IsoSpriteSorting> list)
		{
			list.Sort((a, b) =>
			{
				if (!a || !b) return 0;
				return IsoSpriteSorting.CompareIsoSortersBelow(a, b);
			});
		}
	}

	// Spatial grid for fast collision detection
	public class SpatialGrid
	{
		private Dictionary<(int, int), List<IsoSpriteSorting>> grid = new Dictionary<(int, int), List<IsoSpriteSorting>>();
		private float cellSize;
		private List<IsoSpriteSorting> tempResults = new List<IsoSpriteSorting>(256);

		public SpatialGrid(float cellSize)
		{
			this.cellSize = cellSize;
		}

		public void Add(IsoSpriteSorting sprite)
		{
			var bounds = sprite.TheBounds;
			var cells = GetCellsForBounds(bounds);

			foreach (var cell in cells)
			{
				if (!grid.ContainsKey(cell))
					grid[cell] = new List<IsoSpriteSorting>();

				if (!grid[cell].Contains(sprite))
					grid[cell].Add(sprite);
			}
		}

		public void Remove(IsoSpriteSorting sprite)
		{
			var bounds = sprite.TheBounds;
			var cells = GetCellsForBounds(bounds);

			foreach (var cell in cells)
			{
				if (grid.ContainsKey(cell))
					grid[cell].Remove(sprite);
			}
		}

		public List<IsoSpriteSorting> GetNearby(Bounds2D bounds)
		{
			tempResults.Clear();
			var cells = GetCellsForBounds(bounds);

			foreach (var cell in cells)
			{
				if (grid.ContainsKey(cell))
				{
					foreach (var sprite in grid[cell])
					{
						if (!tempResults.Contains(sprite))
							tempResults.Add(sprite);
					}
				}
			}

			return tempResults;
		}

		public void Clear()
		{
			foreach (var list in grid.Values)
			{
				list.Clear();
			}
			grid.Clear();
		}

		private List<(int, int)> GetCellsForBounds(Bounds2D bounds)
		{
			var cells = new List<(int, int)>();

			int minX = Mathf.FloorToInt(bounds.minX / cellSize);
			int maxX = Mathf.FloorToInt(bounds.maxX / cellSize);
			int minY = Mathf.FloorToInt(bounds.minY / cellSize);
			int maxY = Mathf.FloorToInt(bounds.maxY / cellSize);

			for (int x = minX; x <= maxX; x++)
			{
				for (int y = minY; y <= maxY; y++)
				{
					cells.Add((x, y));
				}
			}

			return cells;
		}
	}
}
