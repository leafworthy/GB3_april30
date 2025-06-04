using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace GangstaBean.Utilities
{
	public static class ListExtensions
	{
		public static void Shuffle<T>(this IList<T> list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				int randomIndex = Random.Range(i, list.Count);
				Swap(list, randomIndex, i);
			}
		}

		public static void Swap<T>(this IList<T> list, int firstIndex, int secondIndex)
		{
			if (list == null)
				throw new ArgumentNullException(nameof(list));

			if (list.Count < 2)
				throw new ArgumentException("List count should be at least 2 for a swap.");

			T firstValue = list[firstIndex];

			list[firstIndex] = list[secondIndex];
			list[secondIndex] = firstValue;
		}
		public static void RemoveNullEntries<T>(this IList<T> list) where T : class
		{
			for (int i = list.Count - 1; i >= 0; i--)
				if (Equals(list[i], null))
					list.RemoveAt(i);
		}

		public static void RemoveDefaultValues<T>(this IList<T> list)
		{
			for (int i = list.Count - 1; i >= 0; i--)
				if (Equals(default(T), list[i]))
					list.RemoveAt(i);
		}
		public static void AddMany<T>(this List<T> list, params T[] elements)
		{
			list.AddRange(elements);
		}

		public static  T GetRandom<T>(this List<T> list) where T:UnityEngine.Object
		{
			return list.Count == 0 ? null : list[(int)Random.Range(0, list.Count)];
		}

		public static void PlayRandomAt(this List<AudioClip> list, Vector3 position)
		{
			SFX.I.PlayRandomAt( list, position);

		}

		public static void PlayRandom(this List<AudioClip> list)
		{
			SFX.I.PlayRandom(list.GetRandom());

		}

	}
}
