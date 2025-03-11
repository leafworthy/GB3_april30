using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectMaker : MonoBehaviour
{
	//CREATOR DESTROYER
	private static ObjectMaker I;
	private static List<GameObject> allActiveUnits = new();

	private static Dictionary<RecycleGameObject, ObjectPool> pools = new();

	private GameObject containerContainer;
	public List<GameObject> ObjectsToPool = new();
	private int defaultPoolSize = 20;

	private void Awake()
	{
		if(I != null)
		{
			Destroy(gameObject);
			return;
		}
		I = this;
		containerContainer = new GameObject("Object Pools");
		containerContainer.transform.SetParent(I.transform);
		LevelGameScene.OnStart += PoolObjects;
		LevelGameScene.OnStop += DestroyAllUnits;
	}

	private void PoolObjects(SceneDefinition sceneDefinition)
	{
		foreach (var obj in ObjectsToPool)
		{

			Pool(obj, defaultPoolSize);

		}
	}

	public static void Pool(GameObject obj, int clones)
	{
		Debug.Log("pooling: " + obj.name + " with " + clones + " clones.");
		var currentPool = new List<GameObject>();
		for (var i = 0; i < clones; i++)
		{
			var newObj = Make(obj, Vector2.zero);
			currentPool.Add(newObj);
		}

		for (var b = 0; b < currentPool.Count; b++) Unmake(currentPool[b]);
	}

	public static void DestroyAllUnits(SceneDefinition sceneDefinition)
	{
		var tempList = allActiveUnits.ToList();
		foreach (var t in tempList)
		{
			Unmake(t);
		}

		allActiveUnits.Clear();
	}

	public static GameObject Make(GameObject prefab, Vector2 pos)
	{
		if (prefab == null) return null;
		GameObject instance;
		var recycledScript = prefab.GetComponent<RecycleGameObject>();
		if (recycledScript != null)
		{
			var pool = I.GetObjectPool(recycledScript);
			instance = pool.NextObject(pos).gameObject;
			instance.transform.SetParent(pool.transform);
		}
		else
		{
			instance = Instantiate(prefab);
			instance.transform.position = pos;
		}

		allActiveUnits.Add(instance);
		return instance;
	}

	public static void Unmake(GameObject gameObject, float deathTime = 0)
	{
		if (gameObject == null) return;
		if (I == null) return;
		var recyleGameObject = gameObject.GetComponent<RecycleGameObject>();
		if (deathTime != 0)
			I.StartCoroutine(I.WaitAndDestroy(gameObject, deathTime));
		else
		{
			if (recyleGameObject != null)
				recyleGameObject.DeactivateGameObject();
			else
			{
				allActiveUnits.Remove(gameObject);
				var comp = gameObject.AddComponent<RecycleGameObject>();
				comp.DeactivateGameObject();
			}
		}
	}

	private IEnumerator WaitAndDestroy(GameObject go, float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		Unmake(go);
	}

	private ObjectPool GetObjectPool(RecycleGameObject reference)
	{
		ObjectPool pool;

		if (pools.TryGetValue(reference, out var pool1))
			pool = pool1;
		else
		{
			var poolContainer = new GameObject(reference.gameObject.name + "_ObjectPool");
			poolContainer.transform.SetParent(I.containerContainer.transform);
			pool = poolContainer.AddComponent<ObjectPool>();
			pool.prefab = reference;
			pools.Add(reference, pool);
		}

		return pool;
	}

	public static GameObject Make(GameObject prefab) => Make(prefab, Vector2.zero);
}