using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
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

	
		public static void AddMany<T>(this List<T> list, params T[] elements)
		{
			list.AddRange(elements);
		}
	
	public static  T GetRandom<T>(this List<T> list) where T:UnityEngine.Object
	{
		return list.Count == 0 ? null : list[(int)Random.Range(0, list.Count)];
	}

	public static void PlayRandom(this List<AudioClip> list)
	{
		Audio.PlaySound(list.GetRandom());
	}

	public static void RemoveNulls<T>(this IList<T> list) where T : Object
	{
		for (int i = list.Count - 1; i >= 0; i--)
			if (list[i] == null)
				list.RemoveAt(i);
	}

	public static void RemoveDefaultValues<T>(this IList<T> list)
	{
		for (int i = list.Count - 1; i >= 0; i--)
			if (Equals(default(T), list[i]))
				list.RemoveAt(i);
	}
}
