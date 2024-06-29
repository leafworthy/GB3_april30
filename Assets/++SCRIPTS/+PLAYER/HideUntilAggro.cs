using UnityEngine;

public class Hider:MonoBehaviour
{



	private void DisableAllColliders()
	{
		var moreColliders = GetComponentsInChildren<Collider2D>();
		foreach (var col in moreColliders)
		{
			if(!col.gameObject.CompareTag("DontDisable"))
			{
				col.enabled = false;
			}
		}
	}

	private void EnableAllColliders()
	{
		var moreColliders = GetComponentsInChildren<Collider2D>();
		foreach (var col in moreColliders) col.enabled = true;
	}

	private void Hide()
	{
		DisableAllColliders();
	}

	private void Unhide()
	{
		EnableAllColliders();
	}
}