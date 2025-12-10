using System.Collections.Generic;

namespace __SCRIPTS.Plugins._ISOSORT
{
	public static class TopologicalSort
	{
		static readonly Dictionary<int, bool> circularDepData = new();
		static readonly List<IsoSpriteSorting> circularDepStack = new(64);

		static readonly HashSet<int> visited = new();
		static readonly List<IsoSpriteSorting> allSprites = new(64);

		public static List<IsoSpriteSorting> Sort(List<IsoSpriteSorting> staticSprites, List<IsoSpriteSorting> movableSprites, List<IsoSpriteSorting> sorted)
		{
			allSprites.Clear();
			allSprites.AddRange(movableSprites);
			allSprites.AddRange(staticSprites);

			var allSpriteCount = allSprites.Count;

			for (var i = 0; i < 5; i++)
			{
				circularDepStack.Clear();
				circularDepData.Clear();
				var removedDependency = false;
				for (var j = 0; j < allSpriteCount; j++)
					if (RemoveCircularDependencies(allSprites[j], circularDepStack, circularDepData))
						removedDependency = true;
				if (!removedDependency) break;
			}

			visited.Clear();
			for (var i = 0; i < allSpriteCount; i++) Visit(allSprites[i], sorted, visited);

			return sorted;
		}

		static void Visit(IsoSpriteSorting item, List<IsoSpriteSorting> sorted, HashSet<int> visited)
		{
			var id = item.GetInstanceID();
			if (!visited.Contains(id))
			{
				visited.Add(id);

				var dependencies = item.VisibleMovingDependencies;
				var dcount = dependencies.Count;
				for (var i = 0; i < dcount; i++) Visit(dependencies[i], sorted, visited);
				dependencies = item.VisibleStaticDependencies;
				dcount = dependencies.Count;
				for (var i = 0; i < dcount; i++) Visit(dependencies[i], sorted, visited);

				sorted.Add(item);
			}
		}

		static bool RemoveCircularDependencies(IsoSpriteSorting item, List<IsoSpriteSorting> _circularDepStack, Dictionary<int, bool> _circularDepData)
		{
			_circularDepStack.Add(item);
			var removedDependency = false;

			var id = item.GetInstanceID();
			var alreadyVisited = _circularDepData.TryGetValue(id, out var inProcess);
			if (alreadyVisited)
			{
				if (inProcess)
				{
					RemoveCircularDependencyFromStack(_circularDepStack);
					removedDependency = true;
				}
			}
			else
			{
				_circularDepData[id] = true;

				var dependencies = item.VisibleMovingDependencies;
				for (var i = 0; i < dependencies.Count; i++)
					if (RemoveCircularDependencies(dependencies[i], _circularDepStack, _circularDepData))
						removedDependency = true;
				dependencies = item.VisibleStaticDependencies;
				for (var i = 0; i < dependencies.Count; i++)
					if (RemoveCircularDependencies(dependencies[i], _circularDepStack, _circularDepData))
						removedDependency = true;

				_circularDepData[id] = false;
			}

			_circularDepStack.RemoveAt(_circularDepStack.Count - 1);
			return removedDependency;
		}

		static void RemoveCircularDependencyFromStack(List<IsoSpriteSorting> _circularReferenceStack)
		{
			if (_circularReferenceStack.Count > 1)
			{
				var startingSorter = _circularReferenceStack[_circularReferenceStack.Count - 1];
				var repeatIndex = 0;
				for (var i = _circularReferenceStack.Count - 2; i >= 0; i--)
				{
					var sorter = _circularReferenceStack[i];
					if (sorter == startingSorter)
					{
						repeatIndex = i;
						break;
					}
				}

				var weakestDepIndex = -1;
				var longestDistance = float.MinValue;
				for (var i = repeatIndex; i < _circularReferenceStack.Count - 1; i++)
				{
					var sorter1a = _circularReferenceStack[i];
					var sorter2a = _circularReferenceStack[i + 1];
					if (sorter1a.sortType == IsoSpriteSorting.SortType.Point && sorter2a.sortType == IsoSpriteSorting.SortType.Point)
					{
						var dist = UnityEngine.Mathf.Abs(sorter1a.AsPoint.x - sorter2a.AsPoint.x);
						if (dist > longestDistance)
						{
							weakestDepIndex = i;
							longestDistance = dist;
						}
					}
				}

				if (weakestDepIndex == -1)
				{
					for (var i = repeatIndex; i < _circularReferenceStack.Count - 1; i++)
					{
						var sorter1a = _circularReferenceStack[i];
						var sorter2a = _circularReferenceStack[i + 1];
						var dist = UnityEngine.Mathf.Abs(sorter1a.AsPoint.x - sorter2a.AsPoint.x);
						if (dist > longestDistance)
						{
							weakestDepIndex = i;
							longestDistance = dist;
						}
					}
				}

				var sorter1 = _circularReferenceStack[weakestDepIndex];
				var sorter2 = _circularReferenceStack[weakestDepIndex + 1];
				sorter1.VisibleStaticDependencies.Remove(sorter2);
				sorter1.VisibleMovingDependencies.Remove(sorter2);
			}
		}
	}
}
