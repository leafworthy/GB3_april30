using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace __SCRIPTS
{
	//REQUIRES levelManager
	public class ObjectMaker : ServiceUser, IService
	{
		//CREATOR DESTROYER
		private  List<GameObject> allActiveUnits = new();

		private  Dictionary<RecycleGameObject, ObjectPool> pools = new();

		private GameObject containerContainer;
		public List<GameObject> ObjectsToPool = new();
		private int defaultPoolSize = 20;


		public void StartService()
		{
			if (containerContainer == null)
			{
				containerContainer = new GameObject("Object Pools");
				containerContainer.transform.SetParent(transform);
			}

			levelManager.OnStartLevel += PoolObjects;
			levelManager.OnStopLevel += DestroyAllUnits;
		}
		public void OnDisable()
		{
			Debug.Log("object maker disabled");
			DestroyAllUnits(null);
			if (levelManager == null) return;
			levelManager.OnStartLevel -= PoolObjects;
			levelManager.OnStopLevel -= DestroyAllUnits;
		}



		private void PoolObjects(GameLevel gameLevel)
		{
			Debug.Log("poolin");
			foreach (var obj in ObjectsToPool)
			{

				Pool(obj, defaultPoolSize);

			}
		}

		public  void Pool(GameObject obj, int clones)
		{
			var currentPool = new List<GameObject>();
			for (var i = 0; i < clones; i++)
			{
				var newObj = Make(obj, Vector2.zero);
				currentPool.Add(newObj);
			}

			for (var b = 0; b < currentPool.Count; b++) Unmake(currentPool[b]);
		}

		public  void DestroyAllUnits(GameLevel gameLevel)
		{
			var tempList = allActiveUnits.ToList();
			foreach (var t in tempList)
			{
				Unmake(t);
			}

			allActiveUnits.Clear();
		}

		public GameObject Make(GameObject prefab, Vector2 pos)
		{
			if (prefab == null) return null;
			GameObject instance;
			var recycledScript = prefab.GetComponent<RecycleGameObject>();
			if (recycledScript == null)
			{
				return Instantiate(prefab, pos, Quaternion.identity);
			}

				var pool = GetObjectPool(recycledScript);
				instance = pool.NextObject(pos).gameObject;
				instance.transform.SetParent(pool.transform);


			allActiveUnits.Add(instance);
			return instance;
		}

		public void Unmake(GameObject _gameObject, float deathTime = 0)
		{
			if (_gameObject == null) return;
			var recyleGameObject = _gameObject.GetComponent<RecycleGameObject>();
			if (deathTime != 0)
				StartCoroutine(WaitAndDestroy(_gameObject, deathTime));
			else
			{
				if (recyleGameObject != null)
					recyleGameObject.DeactivateGameObject();
				else
				{

					var comp = _gameObject.AddComponent<RecycleGameObject>();
					comp.DeactivateGameObject();
				}

				allActiveUnits.Remove(_gameObject);
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
				poolContainer.transform.SetParent(containerContainer.transform);
				pool = poolContainer.AddComponent<ObjectPool>();
				pool.prefab = reference;
				pools.Add(reference, pool);
			}

			return pool;
		}

		public GameObject Make(GameObject prefab) =>   Make(prefab, Vector2.zero);

	}
}
