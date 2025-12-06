using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace __SCRIPTS.Plugins._ISOSORT
{
	//[ExecuteInEditMode]
	[ExecuteInEditMode,Serializable]
	public class IsoSpriteSortingManager : SerializedMonoBehaviour
	{
		[ShowInInspector]private static readonly List<IsoSpriteSorting> fgSpriteList = new(1080);
		[ShowInInspector]private static readonly List<IsoSpriteSorting> floorSpriteList = new(1080);
		[ShowInInspector]private static readonly List<IsoSpriteSorting> staticSpriteList = new(1080);
		[ShowInInspector]private static readonly List<IsoSpriteSorting> currentlyVisibleStaticSpriteList = new(1080);

		[ShowInInspector]private static readonly List<IsoSpriteSorting> moveableSpriteList = new(1080);
		[ShowInInspector]private static readonly List<IsoSpriteSorting> currentlyVisibleMoveableSpriteList = new(1080);
		[ShowInInspector]private static readonly List<IsoSpriteSorting> sortedSprites = new(1080);

		public static void RegisterSprite(IsoSpriteSorting newSprite)
		{
			if (newSprite.registered) return;
			if (!newSprite.gameObject.activeInHierarchy) return;
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
				// In editor mode, treat all sprites as movable for dynamic sorting
				if (newSprite.isMovable || !Application.isPlaying)
					moveableSpriteList.Add(newSprite);
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
			for (var i = 0; i < the_count; i++)
			{
				var otherSprite = staticSpriteList[i];
				if (otherSprite == null) continue;
				if (otherSprite == newSprite) continue; // Don't compare with self

				if (CalculateBoundsIntersection(newSprite, otherSprite))
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
					// If compareResult == 0, no dependency needed (they're equal)
				}
			}
		}

		public static void UnregisterSprite(IsoSpriteSorting spriteToRemove)
		{
			if (spriteToRemove.registered)
			{
				if (spriteToRemove.renderBelowAll)
					floorSpriteList.Remove(spriteToRemove);
				else if (spriteToRemove.renderAboveAll)
					fgSpriteList.Remove(spriteToRemove);
				else
				{
					if (spriteToRemove.isMovable || !Application.isPlaying)
						moveableSpriteList.Remove(spriteToRemove);
					else
					{
						staticSpriteList.Remove(spriteToRemove);
						RemoveStaticDependencies(spriteToRemove);
					}
				}

				spriteToRemove.registered = false;
			}
		}

		private static void RemoveStaticDependencies(IsoSpriteSorting spriteToRemove)
		{
			for (var i = 0; i < spriteToRemove.inverseStaticDependencies.Count; i++)
			{
				var otherSprite = spriteToRemove.inverseStaticDependencies[i];
				if (otherSprite != null)
					otherSprite.staticDependencies.Remove(spriteToRemove);
			}

			for (var i = 0; i < spriteToRemove.staticDependencies.Count; i++)
			{
				var otherSprite = spriteToRemove.staticDependencies[i];
				if (otherSprite != null)
					otherSprite.inverseStaticDependencies.Remove(spriteToRemove);
			}

			spriteToRemove.inverseStaticDependencies.Clear();
			spriteToRemove.staticDependencies.Clear();
		}

		private void Update()
		{
			IsoSpriteSorting.UpdateSorters();
			UpdateSorting();

#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				// This line helps with editor refreshing
				UnityEditor.EditorUtility.SetDirty(this);
			}
#endif
		}

		[Button]
		public void UpdateSortingButton()
		{
			IsoSpriteSorting.UpdateSorters();
			UpdateSorting();
		}

		public static void UpdateSorting()
		{
			FilterListByVisibility(staticSpriteList, currentlyVisibleStaticSpriteList);
			FilterListByVisibility(moveableSpriteList, currentlyVisibleMoveableSpriteList);

			ClearMovingDependencies(currentlyVisibleStaticSpriteList);
			ClearMovingDependencies(currentlyVisibleMoveableSpriteList);

			AddMovingDependencies(currentlyVisibleMoveableSpriteList, currentlyVisibleStaticSpriteList);

			sortedSprites.Clear();
			TopologicalSort.Sort(currentlyVisibleStaticSpriteList, currentlyVisibleMoveableSpriteList, sortedSprites);
			SetSortOrderBasedOnListOrder(sortedSprites);
		}

		private static void AddMovingDependencies(List<IsoSpriteSorting> moveableList, List<IsoSpriteSorting> staticList)
		{
			var moveableCount = moveableList.Count;
			var staticCount = staticList.Count;

			// Moving sprites vs Static sprites
			for (var i = 0; i < moveableCount; i++)
			{
				var moveSprite = moveableList[i];
				if (moveSprite == null) continue; // FIXED: Changed from return to continue

				// Add Moving Dependencies to static sprites
				for (var j = 0; j < staticCount; j++)
				{
					var staticSprite = staticList[j];
					if (staticSprite == null) continue;

					if (CalculateBoundsIntersection(moveSprite, staticSprite))
					{
						var compareResult = IsoSpriteSorting.CompareIsoSorters(moveSprite, staticSprite);
						if (compareResult == -1)
							staticSprite.movingDependencies.Add(moveSprite);
						else if (compareResult == 1)
							moveSprite.movingDependencies.Add(staticSprite);
						// If compareResult == 0, no dependency needed
					}
				}
			}

			// Moving sprites vs Moving sprites - only compare each pair once
			for (var i = 0; i < moveableCount; i++)
			{
				var moveSprite1 = moveableList[i];
				if (moveSprite1 == null) continue; // FIXED: Changed from return to continue

				// Start at i+1 to avoid comparing the same pair twice and comparing with self
				for (var j = i + 1; j < moveableCount; j++)
				{
					var moveSprite2 = moveableList[j];
					if (moveSprite2 == null) continue; // FIXED: Changed from return to continue

					if (CalculateBoundsIntersection(moveSprite1, moveSprite2))
					{
						var compareResult = IsoSpriteSorting.CompareIsoSorters(moveSprite1, moveSprite2);
						if (compareResult == -1)
							moveSprite2.movingDependencies.Add(moveSprite1);
						else if (compareResult == 1)
							moveSprite1.movingDependencies.Add(moveSprite2);
						// If compareResult == 0, no dependency needed
					}
				}
			}
		}

		private static void ClearMovingDependencies(List<IsoSpriteSorting> sprites)
		{
			var count = sprites.Count;
			for (var i = 0; i < count; i++)
			{
				if (sprites[i] != null)
					sprites[i].movingDependencies.Clear();
			}
		}

		private static bool CalculateBoundsIntersection(IsoSpriteSorting sprite, IsoSpriteSorting otherSprite)
		{
			// Add null checks for safety
			if (sprite == null || otherSprite == null) return false;

			Bounds2D b1 = sprite.TheBounds;
			Bounds2D b2 = otherSprite.TheBounds;


			return b1.Intersects(b2);
		}

		private static void SetSortOrderBasedOnListOrder(List<IsoSpriteSorting> spriteList)
		{
			var orderCurrent = 420;
			var count = spriteList.Count;
			for (var i = 0; i < count; ++i)
			{
				if (spriteList[i] == null) continue;

				spriteList[i].RendererSortingOrder = orderCurrent;
				orderCurrent += 1 + spriteList[i].renderersToSort.Count;
			}
		}

		private static void SetSortOrderNegative(List<IsoSpriteSorting> spriteList)
		{
			var currentIndex = -10000;
			for (var i = 0; i < spriteList.Count; ++i)
			{
				if (spriteList[i] == null) continue;

				spriteList[i].RendererSortingOrder = spriteList[i].renderBelowSortingOrder + currentIndex + i;
				currentIndex += spriteList[i].renderersToSort.Count + 10;
			}
		}

		private static void SetSortOrderTop(List<IsoSpriteSorting> spriteList)
		{
			var currentIndex = 10000;
			for (var i = 0; i < spriteList.Count; ++i)
			{
				if (spriteList[i] == null) continue;

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
					bool isVisible = false;
					for (var j = 0; j < sprite.renderersToSort.Count; j++)
					{
						if (sprite.renderersToSort[j] == null) continue;
						if (sprite.renderersToSort[j].isVisible)
						{
							isVisible = true;
							break;
						}
					}

					if (isVisible)
						destinationList.Add(sprite);
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
}
