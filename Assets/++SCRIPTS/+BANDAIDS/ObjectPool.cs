using System.Collections.Generic;
using UnityEngine;

namespace _SCRIPTS
{
	public class ObjectPool : MonoBehaviour
	{

		public RecycleGameObject prefab;
		static int entities = 0;

		public List<RecycleGameObject> poolInstances = new List<RecycleGameObject> ();

		private RecycleGameObject CreateInstance (Vector3 pos)
		{
			ObjectPool.entities++;

			var clone = GameObject.Instantiate (prefab);
			clone.transform.position = pos;
			clone.name = prefab.name + ObjectPool.entities.ToString ();

			poolInstances.Add (clone);

			return clone;
		}

		public RecycleGameObject NextObject (Vector3 pos)
		{
			RecycleGameObject instance = null;
			for (int i = 0; i < poolInstances.Count; i++) {
				if (poolInstances[i] != null)
				{
					if (poolInstances[i].gameObject != null)
					{
						if (!poolInstances[i].gameObject.activeSelf)
						{
							instance = poolInstances[i];
							instance.transform.position = pos;
						}
					}
				}

			}

			if (instance != null) {
				instance.Restart ();
				return instance;
			} else {
				instance = CreateInstance (pos);
				instance.Restart ();
			}


			return instance;

		}

	}
}
