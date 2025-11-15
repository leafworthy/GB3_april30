using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace __SCRIPTS.Plugins._ISOSORT
{
	[ExecuteAlways, Serializable]
	public class IsoSpriteSortingManager : SerializedMonoBehaviour
	{
		[ShowInInspector] private static readonly List<IsoSpriteSorting> staticSpriteList = new(2049);
		[ShowInInspector] private static readonly List<IsoSpriteSorting> currentlyVisibleStaticSpriteList = new(2048);
		[ShowInInspector] private static readonly List<IsoSpriteSorting> moveableSpriteList = new(2048);
		[ShowInInspector] private static readonly List<IsoSpriteSorting> currentlyVisibleMoveableSpriteList = new(2048);
		[ShowInInspector] private static readonly List<IsoSpriteSorting> sortedSprites = new(2048);

		public static void RegisterSprite(IsoSpriteSorting newSprite)
		{
			if (newSprite.registered || !newSprite.gameObject.activeInHierarchy) return;
			newSprite.registered = true;

			if (newSprite.isMovable || !Application.isPlaying) moveableSpriteList.Add(newSprite);
			else
			{
				staticSpriteList.Add(newSprite);
				newSprite.SetupStaticCache();
				SetupStaticDependencies(newSprite);
			}

		}

		private static void SetupStaticDependencies(IsoSpriteSorting newSprite)
		{
			var the_count = staticSpriteList.Count;
			for (var i = 0; i < the_count; i++)
			{
				if (staticSpriteList[i] == null) continue;
				var otherSprite = staticSpriteList[i];
				if (!BoundsIntersect(newSprite, otherSprite)) continue;
				switch (IsoSpriteSorting.CompareIsoSorters(newSprite, otherSprite))
				{
					case -1:
						otherSprite.staticDependencies.Add(newSprite);
						newSprite.inverseStaticDependencies.Add(otherSprite);
						break;
					case 1:
						newSprite.staticDependencies.Add(otherSprite);
						otherSprite.inverseStaticDependencies.Add(newSprite);
						break;
				}
			}
		}

		public static void UnregisterSprite(IsoSpriteSorting spriteToRemove)
		{
			if (!spriteToRemove.registered) return;
			spriteToRemove.registered = false;
			if (spriteToRemove.isMovable)
				moveableSpriteList.Remove(spriteToRemove);
			else
			{
				staticSpriteList.Remove(spriteToRemove);
				RemoveStaticDependencies(spriteToRemove);
			}

		}

		private static void RemoveStaticDependencies(IsoSpriteSorting spriteToRemove)
		{
			foreach (var otherSprite in spriteToRemove.inverseStaticDependencies)
			{
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

			//Add Moving Dependencies to static sprites
			for (var i = 0; i < moveableCount; i++)
			{
				if (moveableList[i] == null) continue;
				var moveSprite1 = moveableList[i];
				for (var j = 0; j < staticCount; j++)
				{
					if (staticList[j] == null) continue;
					var staticSprite = staticList[j];
					if (!BoundsIntersect(moveSprite1, staticSprite)) continue;
					if (IsoSpriteSorting.CompareIsoSorters(moveSprite1, staticSprite) == -1) staticSprite.movingDependencies.Add(moveSprite1);
					else if (IsoSpriteSorting.CompareIsoSorters(moveSprite1, staticSprite) == 1) moveSprite1.movingDependencies.Add(staticSprite);
				}

				//Add Moving Dependencies to Moving Sprites
				for (var j = 0; j < moveableCount; j++)
				{
					if (moveableList[j] == null) continue;
					var moveSprite2 = moveableList[j];
					if (!BoundsIntersect(moveSprite1, moveSprite2)) continue;
					if (IsoSpriteSorting.CompareIsoSorters(moveSprite1, moveSprite2) == -1) moveSprite2.movingDependencies.Add(moveSprite1);
				}
			}
		}

		private static void ClearMovingDependencies(List<IsoSpriteSorting> sprites)
		{
			var count = sprites.Count;
			for (var i = 0; i < count; i++) sprites[i].movingDependencies.Clear();
		}

		private static bool BoundsIntersect(IsoSpriteSorting sprite, IsoSpriteSorting otherSprite) =>
			sprite.TheBounds.Intersects(otherSprite.TheBounds);

		private static void SetSortOrderBasedOnListOrder(List<IsoSpriteSorting> spriteList)
		{
			var orderCurrent = 420; //was 420
			foreach (var t in spriteList)
			{
				t.RendererSortingOrder = orderCurrent;
				orderCurrent += 10 + t.RenderersToSortCount; // was 1 +
			}
		}

		public static void FilterListByVisibility(List<IsoSpriteSorting> fullList, List<IsoSpriteSorting> destinationList)
		{
			destinationList.Clear();
			var count = fullList.Count;
			for (var i = 0; i < count; i++)
			{
				var sprite = fullList[i];
				if (sprite.forceSort)
				{
					destinationList.Add(sprite);
					sprite.forceSort = false;
				}
				else
				{
					if (sprite.VisibleRenderersToSort) destinationList.Add(sprite);
				}
			}
		}
	}
}
