using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS
{
	public class ObjectPool : MonoBehaviour
	{

		public RecycleGameObject prefab;
		static int entities = 0;

		public List<RecycleGameObject> poolInstances = new List<RecycleGameObject> ();

		[RuntimeInitializeOnLoadMethod]
		static void ResetStatics()
		{
			entities = 0;
		}
		RecycleGameObject CreateInstance (Vector3 pos)
		{
			entities++;

			var clone = Instantiate (prefab);
			clone.transform.position = pos;
			clone.name = prefab.name + entities;

			poolInstances.Add (clone);

			return clone;
		}

		public RecycleGameObject NextObject (Vector3 pos)
		{
			RecycleGameObject instance = null;
			for (int i = 0; i < poolInstances.Count; i++)
			{
				if (poolInstances[i] == null) continue;
				if (poolInstances[i].gameObject == null) continue;
				if (poolInstances[i].gameObject.activeSelf) continue;
				instance = poolInstances[i];
				instance.transform.position = pos;

			}

			if (instance != null) {
				instance.ActivateGameObject ();
				return instance;
			}

			instance = CreateInstance (pos);
			instance.ActivateGameObject ();


			return instance;

		}

	}
}
