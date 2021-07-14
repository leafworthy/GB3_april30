using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAKER : Singleton<MAKER>
{
	//CREATOR DESTROYER

	private static List<GameObject> allUnits = new List<GameObject> ();

	private static Dictionary<RecycleGameObject, ObjectPool> pools = new Dictionary<RecycleGameObject, ObjectPool> ();

	public static void Pool (GameObject obj, int clones)
	{
		List<GameObject> currentPool = new List<GameObject> ();
		for (int i = 0; i < clones; i++) {
			GameObject newObj = Make (obj, Vector2.zero);
			currentPool.Add (newObj);
		}
		for (int b = 0; b < currentPool.Count; b++) {
			MAKER.Unmake (currentPool [b]);
		}
	}


	public static void DestroyAllUnits ()
	{
		foreach (var t in allUnits)
		{
			Destroy (t);
		}
		allUnits.Clear();
	}


	public static GameObject Make (GameObject prefab, Vector2 pos)
	{

		GameObject instance = null;
		RecycleGameObject recycledScript = prefab.GetComponent<RecycleGameObject> ();
		if (recycledScript != null) {

			var pool = GetObjectPool (recycledScript);
			if (pool == null) {
				Debug.Break ();
			}
			instance = pool.NextObject (pos).gameObject;
			if (instance == null) {
				Debug.Break ();
			}
			instance.transform.SetParent (pool.transform);

		} else {

			instance = GameObject.Instantiate (prefab);
			instance.transform.position = pos;

		}


		allUnits.Add (instance);
		return instance;
	}


	public static void Unmake (GameObject gameObject, float deathTime = 0)
	{
		if (gameObject == null) return;
		var recyleGameObject = gameObject.GetComponent<RecycleGameObject> ();
		if (deathTime != 0) {
			I.StartCoroutine (I.WaitAndDestroy (gameObject, deathTime));
		} else {

			if (recyleGameObject != null) {
				recyleGameObject.DeactivateGameObject ();
			} else {
				allUnits.Remove (gameObject);
				var comp = gameObject.AddComponent<RecycleGameObject>();
				comp.DeactivateGameObject();
			}

		}

	}

	public IEnumerator WaitAndDestroy (GameObject go, float waitTime)
	{
		yield return new WaitForSeconds (waitTime);
		Unmake (go, 0);
	}

	private static ObjectPool GetObjectPool (RecycleGameObject reference)
	{

		ObjectPool pool = null;


		if (pools.ContainsKey (reference)) {
			pool = pools [reference];
		} else {
			var poolContainer = new GameObject (reference.gameObject.name + "ObjectPool");
			pool = poolContainer.AddComponent<ObjectPool> ();
			pool.prefab = reference;
			pools.Add (reference, pool);
		}


		return pool;
	}


	public static GameObject Make(GameObject prefab)
	{
		return Make(prefab, Vector2.zero);
	}
}
