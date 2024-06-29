using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[Serializable,ExecuteInEditMode]
public class HideRevealList : MonoBehaviour
{
	[SerializeField] public List<GOList> goListsToReveal = new List<GOList>();
	[Range(0, 5),SerializeField]  protected int revealedObjectIndex;

	private void Start()
	{
		Refresh();
	}

	[Button]
	private void Refresh()
	{
		SetActiveObject(revealedObjectIndex);
	}

	public void Set(int _revealedObjectIndex)
	{
		revealedObjectIndex = _revealedObjectIndex;
		Refresh();
	}

	private void SetActiveObject(int objectIndex)
	{
		if (objectIndex > goListsToReveal.Count - 1) return;
		for (var i = 0; i < goListsToReveal.Count; i++)
		{
			var list = goListsToReveal[i];
			if (i == objectIndex)
			{
				foreach (var obj in list.list)
				{
					if(obj!= null) obj.SetActive(true);
				}
			}
			else
			{
				foreach (var obj in list.list)
				{
					if (obj != null) obj.SetActive(false);
				}
			}
		}

	}


}

[Serializable]
public class GOList
{
	public List<GameObject> list;

}
