using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

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
	private void Refresh()
	{
		GatherTransforms();
		Set(revealedObjectIndex);
	}

	[Button]
	public void SetToCurrent()
	{
		Set(revealedObjectIndex);
	}

	private void GatherTransforms()
	{
		objectsToReveal.Clear();

		foreach (Transform child in transform)
		{
			if (child.parent != this.transform) continue;
				objectsToReveal.Add(child.gameObject);
		}
	}

	

	public GameObject Set(int objectIndex)
	{
		if (objectsToReveal.Count <= 0) return null;
		if (objectIndex >= objectsToReveal.Count)
		{
			objectIndex = objectsToReveal.Count - 1;
		}
		revealedObjectIndex = objectIndex;
		foreach (var obj in objectsToReveal) obj.SetActive(false);

		if (isAdditive)
		{
			for (int i = 0; i <= objectIndex; i++)
			{
				objectsToReveal[i].SetActive(true);
			}
		}
		else
		{
			objectsToReveal[revealedObjectIndex].SetActive(true);
		}

		if (objectIndex >= objectsToReveal.Count) return objectsToReveal[^1];
		return objectsToReveal[objectIndex];
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