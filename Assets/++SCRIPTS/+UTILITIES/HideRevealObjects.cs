using System;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

namespace GangstaBean.Utilities
{
	[ExecuteInEditMode]
	public class HideRevealObjects : MonoBehaviour
	{
		[SerializeField] public List<GameObject> objectsToReveal = new();
		[Range(0, 20), SerializeField] private int revealedObjectIndex;
		public bool isAdditive;
		private Color playerColor;

		private void Awake()
		{
			Set(revealedObjectIndex);
		}

		[Button]
		public void Set()
		{
			Set(revealedObjectIndex);
		}

		[Button]
		public void GetChildGameObjects()
		{
			objectsToReveal.Clear();
			foreach (Transform child in transform)
			{
				objectsToReveal.Add(child.gameObject);
			}

			Set(revealedObjectIndex); // Ensure the correct object is set after getting children
		}


		public void Set(int objectIndex)
		{
			if (objectsToReveal.Count <= 0) return;
			if (objectIndex >= objectsToReveal.Count) return;
			if (objectIndex >= objectsToReveal.Count) objectIndex = objectsToReveal.Count - 1;
			revealedObjectIndex = objectIndex;
			foreach (var obj in objectsToReveal)
			{
				obj.SetActive(false);
			}

			if (isAdditive)
				for (var i = 0; i <= objectIndex; i++)
					objectsToReveal[i].SetActive(true);
			else
				objectsToReveal[revealedObjectIndex].SetActive(true);
		}

		public void SetPlayerColor(Color color)
		{
			playerColor = color;
			var sprite = objectsToReveal[revealedObjectIndex].GetComponent<SpriteRenderer>();
			if (sprite != null)
			{
				var c1 = playerColor;
				c1.a = sprite.color.a;
				sprite.color = c1;
			}
		}
	}
}