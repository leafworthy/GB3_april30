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
				GatherTransforms();
				SetActiveObject(revealedObjectIndex);
			}
		}

		[Button()]
		private void Refresh()
		{
			GatherTransforms();
			SetActiveObject(revealedObjectIndex);
		}


		public void GatherTransforms()
		{
			if (objectsToReveal.Count > 0)
			{
				if (!LockTransforms)
				{
					objectsToReveal.Clear();
					foreach (Transform child in transform)
						if (child != transform)
							objectsToReveal.Add(child.gameObject);
				}
			}
			else
			{
				foreach (Transform child in transform)
					if (child != transform)
						objectsToReveal.Add(child.gameObject);
			}

			SetActiveObject(revealedObjectIndex);
		}

		public void SetActiveObject(int objectIndex)
		{
			revealedObjectIndex = objectIndex;
			foreach (var obj in objectsToReveal) obj.SetActive(false);

			objectsToReveal[revealedObjectIndex].SetActive(true);
		}
	}
}
