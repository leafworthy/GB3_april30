using System.Collections.Generic;
using UnityEngine;

namespace _SCRIPTS
{
	public static class MyExtensions
	{

		public static void Shuffle<T>(this List<T> list)
		{
			List<T> shuffledList = new List<T>();
			for (int i = 0; i < list.Count; i++)
			{
				T temp = list[i];
				int randomIndex = UnityEngine.Random.Range(i, list.Count-1);
				list[i] = list[randomIndex];
				list[randomIndex] = temp;
			}

			list = shuffledList;
		}

		public static T GetRandom<T>(this List<T> list)
		{
			return list[Random.Range(0, list.Count)];
		}

	}
}
