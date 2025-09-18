using Sirenix.OdinInspector;
using UnityEngine;

public class SorterPoint : MonoBehaviour
{
	public int sorterPosition;

	// This class is just a placeholder for the editor script to work with.
	// It can be extended with additional functionality as needed.

	[Button]
	public static void SortScene()
	{
		Debug.Log("scene sorted");
		var sorterPoints = FindObjectsByType<SorterPoint>(FindObjectsSortMode.None);
		foreach (var sorterPoint in sorterPoints)
		{
			if (sorterPoint == null || sorterPoint.transform == null)
			{
				Debug.LogWarning("SorterPoint or its transform is null.", sorterPoint);
				continue;
			}

			var renderers = sorterPoint.GetComponentsInChildren<SpriteRenderer>();
			if (renderers == null || renderers.Length == 0) continue;
			foreach (var renderer in renderers)
			{
				if (renderer == null)
				{
					Debug.LogWarning("Renderer is null.", sorterPoint);
					continue;
				}

				var sorterModifier = renderer.GetComponent<SorterPointDepthModifier>();
				if (sorterModifier != null)
				{
					renderer.sortingOrder += sorterPoint.sorterPosition+sorterModifier.SorterPointDepthModification - 1000;
				}
				else
				{
					renderer.sortingOrder = sorterPoint.sorterPosition - 1000;
				}
			}
		}
	}
}
