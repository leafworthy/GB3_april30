using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class RecycleGameObject : MonoBehaviour
{
	private List<IRecyclable> RecyclableComponents = new List<IRecyclable>();
	public void ActivateGameObject()
	{
		RecyclableComponents = GetComponentsInChildren<IRecyclable>().ToList();
		gameObject.SetActive(true);
		foreach (var recyclableComponent in RecyclableComponents)
		{
			recyclableComponent.Recycle();
		}
	}

	public void DeactivateGameObject()
	{

		foreach (var recyclableComponent in RecyclableComponents)
		{
			recyclableComponent.Breakdown();
		}
		gameObject.SetActive(false);
	}
}

public interface IRecyclable
{
	void Recycle();
	void Breakdown();
}
