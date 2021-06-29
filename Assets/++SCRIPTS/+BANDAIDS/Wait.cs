using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wait : Singleton<Wait>
{
	private static IEnumerator ForCoroutine(float seconds, System.Action thenDoThis)
	{
		yield return new WaitForSeconds(seconds);
		thenDoThis();
	}
	public static void For(float seconds, System.Action thenDoThis)
	{
		I.StartCoroutine(ForCoroutine(seconds, thenDoThis));
	}

	private static IEnumerator ActionSequenceCoroutine(float secondsLongestWait, List<System.Action> thenDoThese)
	{
		int totalActions = thenDoThese.Count;
		for (int i = 0; i < totalActions; i++)
		{
			thenDoThese[i]();
			yield return new WaitForSeconds(Random.Range(0f, secondsLongestWait));
		}
		
	}

	public static void ActionSequence(float secondsLongestWait, List<System.Action> thenDoThese)
	{
		I.StartCoroutine(ActionSequenceCoroutine(secondsLongestWait, thenDoThese));
	}

}
