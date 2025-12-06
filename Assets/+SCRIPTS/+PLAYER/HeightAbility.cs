using __SCRIPTS;
using __SCRIPTS.Plugins._ISOSORT;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class HeightAbility : MonoBehaviour, IHaveHeight
{

	private Vector2 Height;
	public GameObject HeightObject;

	private void OnValidate()
	{
		if (HeightObject == null) HeightObject = transform.Find("HeightObject")?.gameObject;
		if (HeightObject == null) HeightObject = transform.Find("Height Object")?.gameObject;
	}

	public IHaveHeight SetHeight(float height)
	{
		Height.y = height;
		HeightObject.transform.localPosition = Height;
		return this;
	}

	public float GetHeight() => Height.y;
}
