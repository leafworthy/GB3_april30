using System.Collections.Generic;
using UnityEngine;

namespace _PLUGINS
{
	[ExecuteInEditMode]
	public class IsoSpriteSortingManager : MonoBehaviour
	{
		private static List<IsoSpriteSorting> floorSpriteList = new List<IsoSpriteSorting>(64);
		private static List<IsoSpriteSorting> staticSpriteList = new List<IsoSpriteSorting>(256);
		private static List<IsoSpriteSorting> currentlyVisibleStaticSpriteList = new List<IsoSpriteSorting>();

		private static List<IsoSpriteSorting> moveableSpriteList = new List<IsoSpriteSorting>(64);
		private static List<IsoSpriteSorting> currentlyVisibleMoveableSpriteList = new List<IsoSpriteSorting>();

		public static void RegisterSprite(IsoSpriteSorting newSprite)
		{
			if (newSprite.renderBelowAll)
			{
				if (!floorSpriteList.Contains(newSprite)) floorSpriteList.Add(newSprite);
				SortListSimple(floorSpriteList);
				SetSortOrderNegative(floorSpriteList);
			}
			else
			{
				if (newSprite.isMovable)
				{
					if (!moveableSpriteList.Contains(newSprite)) moveableSpriteList.Add(newSprite);
				}
				else
				{
					if (!staticSpriteList.Contains(newSprite)) staticSpriteList.Add(newSprite);
					SetupStaticDependencies(newSprite);
				}
			}
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
		}

		private static void RemoveStaticDependencies(IsoSpriteSorting spriteToRemove)
		{
			foreach (var otherSprite in spriteToRemove.inverseStaticDependencies)
				otherSprite.staticDependencies.Remove(spriteToRemove);
		}
#if UNITY_EDITOR
		public void SortScene()
		{
			var isoSorters = FindObjectsOfType(typeof(IsoSpriteSorting)) as IsoSpriteSorting[];
			for (var i = 0; i < isoSorters.Length; i++) isoSorters[i].Setup();

			UpdateSorting();
			for (var i = 0; i < isoSorters.Length; i++) isoSorters[i].Unregister();

			UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager
				.GetActiveScene());
		}
#endif
		private void Update()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying) SortScene();
#endif
			UpdateSorting();
		}

		private static List<IsoSpriteSorting> sortedSprites = new List<IsoSpriteSorting>(2000);

		public static void UpdateSorting()
		{
			FilterListByVisibility(staticSpriteList, currentlyVisibleStaticSpriteList);
			FilterListByVisibility(moveableSpriteList, currentlyVisibleMoveableSpriteList);

			ClearMovingDependencies(currentlyVisibleStaticSpriteList);
			ClearMovingDependencies(currentlyVisibleMoveableSpriteList);

			AddMovingDependenciesToStaticSprites(currentlyVisibleMoveableSpriteList, currentlyVisibleStaticSpriteList);
			AddMovingDependenciesToMovingSprites(currentlyVisibleMoveableSpriteList);

			sortedSprites.Clear();
			TopologicalSort.Sort(currentlyVisibleStaticSpriteList, currentlyVisibleMoveableSpriteList, sortedSprites);
			SetSortOrderBasedOnListOrder(sortedSprites);
		}

		private static void AddMovingDependenciesToStaticSprites(List<IsoSpriteSorting> moveableList,
		                                                         List<IsoSpriteSorting> staticList)
		{
			for (var i = 0; i < moveableList.Count; i++)
			{
				var moveSprite = moveableList[i];
				for (var j = 0; j < staticList.Count; j++)
				{
					var staticSprite = staticList[j];
					if (CalculateBoundsIntersection(moveSprite, staticSprite))
					{
						var compareResult = IsoSpriteSorting.CompareIsoSorters(moveSprite, staticSprite);
						if (compareResult == -1)
							staticSprite.movingDependencies.Add(moveSprite);
						else if (compareResult == 1) moveSprite.movingDependencies.Add(staticSprite);
					}
				}
			}
		}

		private static void AddMovingDependenciesToMovingSprites(List<IsoSpriteSorting> moveableList)
		{
			for (var i = 0; i < moveableList.Count; i++)
			{
				var sprite1 = moveableList[i];
				for (var j = 0; j < moveableList.Count; j++)
				{
					var sprite2 = moveableList[j];
					if (CalculateBoundsIntersection(sprite1, sprite2))
					{
						var compareResult = IsoSpriteSorting.CompareIsoSorters(sprite1, sprite2);
						if (compareResult == -1) sprite2.movingDependencies.Add(sprite1);
					}
				}
			}
		}

		private static void ClearMovingDependencies(List<IsoSpriteSorting> sprites)
		{
			for (var i = 0; i < sprites.Count; i++) sprites[i].movingDependencies.Clear();
		}

		private static bool CalculateBoundsIntersection(IsoSpriteSorting sprite, IsoSpriteSorting otherSprite)
		{
			return sprite.TheBounds.Intersects(otherSprite.TheBounds);
		}

		private static void SetSortOrderBasedOnListOrder(List<IsoSpriteSorting> spriteList)
		{
			var orderCurrent = 0;
			for (var i = 0; i < spriteList.Count; ++i)
			{
				spriteList[i].RendererSortingOrder = orderCurrent;
				orderCurrent += spriteList[i].renderersToSort.Length;
			}
		}

		private static void SetSortOrderNegative(List<IsoSpriteSorting> spriteList)
		{
			var startOrder = -spriteList.Count - 1;
			for (var i = 0; i < spriteList.Count; ++i) spriteList[i].RendererSortingOrder = startOrder + i;
		}

		public static void FilterListByVisibility(List<IsoSpriteSorting> fullList,
		                                          List<IsoSpriteSorting> destinationList)
		{
			destinationList.Clear();
			for (var i = 0; i < fullList.Count; i++)
			{
				var sprite = fullList[i];
				if (sprite.forceSort)
				{
					destinationList.Add(sprite);
					sprite.forceSort = false;
				}
				else
				{
					if (sprite.renderersToSort != null)
					{
						for (var j = 0; j < sprite.renderersToSort.Length; j++)
						{
							if (sprite.renderersToSort[j] is null) break;
							if (sprite.renderersToSort[j].isVisible)
							{
								destinationList.Add(sprite);
								break;
							}
						}
					} // THIS LINE
				}
			}
		}

		private static void SortListSimple(List<IsoSpriteSorting> list)
		{
			list.Sort((a, b) =>
			{
				if (!a || !b)
					return 0;
				else
					return IsoSpriteSorting.CompareIsoSorters(a, b);
			});
		}
	}
}
