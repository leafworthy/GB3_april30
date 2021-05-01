using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace _SCRIPTS
{
	public class HideRevealObjects : MonoBehaviour
	{
		[SerializeField] public List<GameObject> objectsToReveal = new List<GameObject>();
		[Range(0, 20)] [SerializeField] private int revealedObjectIndex;
		public bool LockTransforms = false;
		[SerializeField] private bool SetActiveObjectOnUpdate = true;


		private void Awake()
		{
			if (SetActiveObjectOnUpdate)
			{
				GatherAllTransforms();
				SetActiveObject(revealedObjectIndex);
			}
		}

		public void SetRevealedObject(int i)
		{
			revealedObjectIndex = i;
		}

		[Button()]
		public void GatherAllTransforms()
		{
			if (objectsToReveal.Count > 0)
			{
				if (!LockTransforms)
				{
					objectsToReveal.Clear();
					foreach (UnityEngine.Transform child in transform)
					{
						if (child != transform)
						{
							objectsToReveal.Add(child.gameObject);
						}
					}
				}

			}
			else
			{
				foreach (UnityEngine.Transform child in transform)
				{
					if (child != transform)
					{
						objectsToReveal.Add(child.gameObject);
					}
				}

			}

			SetActiveObject(revealedObjectIndex);

		}

		[Button()]
		public void SetActiveObject(int objectIndex)
		{
			revealedObjectIndex = objectIndex;
			foreach (GameObject obj in objectsToReveal)
			{
				obj.SetActive(false);
			}

			objectsToReveal[revealedObjectIndex].SetActive(true);
		}
	}
}
