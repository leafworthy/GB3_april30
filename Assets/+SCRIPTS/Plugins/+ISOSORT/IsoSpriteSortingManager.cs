using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace __SCRIPTS.Plugins._ISOSORT
{
	[ExecuteAlways, Serializable]
	public class IsoSpriteSortingManager : SerializedMonoBehaviour
	{
		[ShowInInspector] private static readonly List<IsoSpriteSorting> fgSpriteList = new(256);
		[ShowInInspector] private static readonly List<IsoSpriteSorting> floorSpriteList = new(256);
		[ShowInInspector] private static readonly List<IsoSpriteSorting> staticSpriteList = new(256);
		[ShowInInspector] private static readonly List<IsoSpriteSorting> currentlyVisibleStaticSpriteList = new(256);

		[ShowInInspector] private static readonly List<IsoSpriteSorting> moveableSpriteList = new(256);
		[ShowInInspector] private static readonly List<IsoSpriteSorting> currentlyVisibleMoveableSpriteList = new(256);
		[ShowInInspector] private static readonly List<IsoSpriteSorting> sortedSprites = new(256);

		public static void RegisterSprite(IsoSpriteSorting newSprite)
		{
			if (newSprite.registered)
			{
				return;
			}
			//if (!newSprite.gameObject.activeInHierarchy) return;
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
				}
			}
		}

		public static void UnregisterSprite(IsoSpriteSorting spriteToRemove)
		{
			if (spriteToRemove.registered)
			{
				if (spriteToRemove.renderBelowAll)
					floorSpriteList.Remove(spriteToRemove);
				else
				{
					if (spriteToRemove.isMovable)
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
				otherSprite.staticDependencies.Remove(spriteToRemove);
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
				CleanupNullReferences(); // Add this
				// This line helps with editor refreshing
				UnityEditor.EditorUtility.SetDirty(this);
			}
#endif
		}

		[Button]
		public void UpdateSortingButton()
		{
			IsoSpriteSorting.UpdateSorters();
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
			for (var i = 0; i < moveableCount; i++)
			{
				var moveSprite1 = moveableList[i];
				if (moveSprite1 == null) return;
				//Add Moving Dependencies to static sprites
				for (var j = 0; j < staticCount; j++)
				{
					var staticSprite = staticList[j];
					if (CalculateBoundsIntersection(moveSprite1, staticSprite))
					{
						var compareResult = IsoSpriteSorting.CompareIsoSorters(moveSprite1, staticSprite);
						if (compareResult == -1)
							staticSprite.movingDependencies.Add(moveSprite1);
						else if (compareResult == 1) moveSprite1.movingDependencies.Add(staticSprite);
					}
				}

				//Add Moving Dependencies to Moving Sprites
				for (var j = 0; j < moveableCount; j++)
				{
					var moveSprite2 = moveableList[j];
					if (moveSprite2 == null) return;
					if (CalculateBoundsIntersection(moveSprite1, moveSprite2))
					{
						var compareResult = IsoSpriteSorting.CompareIsoSorters(moveSprite1, moveSprite2);
						if (compareResult == -1) moveSprite2.movingDependencies.Add(moveSprite1);
					}
				}
			}
		}

		private static void ClearMovingDependencies(List<IsoSpriteSorting> sprites)
		{
			var count = sprites.Count;
			for (var i = 0; i < count; i++) sprites[i].movingDependencies.Clear();
		}

		private static bool CalculateBoundsIntersection(IsoSpriteSorting sprite, IsoSpriteSorting otherSprite) =>
			sprite.TheBounds.Intersects(otherSprite.TheBounds);

		private static void SetSortOrderBasedOnListOrder(List<IsoSpriteSorting> spriteList)
		{
			var orderCurrent = 420;//was 420
			var count = spriteList.Count;
			for (var i = 0; i < count; ++i)
			{
				spriteList[i].RendererSortingOrder = orderCurrent;
				orderCurrent += 10 + spriteList[i].renderersToSort.Count; // was 1 +
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
			Debug.LogWarning("rener above all");
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
					for (var j = 0; j < sprite.renderersToSort.Count; j++)
					{
						if (sprite.renderersToSort[j] == null) continue;
						if (sprite.renderersToSort[j].isVisible)
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

		[Button]
		public static void CleanupNullReferences()
		{
			fgSpriteList.RemoveAll(sprite => sprite == null);
			floorSpriteList.RemoveAll(sprite => sprite == null);
			staticSpriteList.RemoveAll(sprite => sprite == null);
			moveableSpriteList.RemoveAll(sprite => sprite == null);
		}
	}
}
