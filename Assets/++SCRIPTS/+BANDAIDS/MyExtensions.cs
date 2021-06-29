using System.Collections.Generic;
using UnityEngine;

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

	public static  T GetRandom<T>(this List<T> list) where T:Object
	{
		if (list.Count == 0) return null;
		return list[(int)Random.Range(0, list.Count)];
	}

	public static void PlayRandom(this List<AudioClip> list)
	{
		AUDIO.PlaySound(list.GetRandom());
	}

}
