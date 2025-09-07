using __SCRIPTS.Plugins._ISOSORT;
using UnityEngine;

public class ThingWithHeight : MonoBehaviour
{

	private float DistanceToGround;

	public GameObject ShadowObject;
	public GameObject JumpObject;
	public IsoSpriteSorting Sorting;
	private bool lands;

	protected virtual void Start()
	{
		if (Sorting == null) Sorting = GetComponent<IsoSpriteSorting>();
	}

	protected virtual void FixedUpdate()
	{
		if (!lands) return;
		UpdateGraphics();
	}

	private void UpdateGraphics()
	{
		SetDistanceToGround(DistanceToGround);
	}


	public void SetDistanceToGround(float height, bool _lands = true)
	{
		lands = _lands;
		DistanceToGround = height;
		JumpObject.transform.localPosition = new Vector3(0, DistanceToGround, 0);
	}

	public float GetDistanceToGround() => DistanceToGround;
}
