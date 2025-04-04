using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace __SCRIPTS
{
	public static class ListExtensions
	{
		
	
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