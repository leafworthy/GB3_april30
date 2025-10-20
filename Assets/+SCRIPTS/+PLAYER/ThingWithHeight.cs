using __SCRIPTS.Plugins._ISOSORT;
using UnityEngine;
using UnityEngine.Serialization;

public class ThingWithHeight : MonoBehaviour
{

	private Vector2 DistanceToGround;
	[FormerlySerializedAs("JumpObject")] public GameObject HeightObject;
	private bool lands;

	protected virtual void FixedUpdate()
	{
		if (!lands) return;
		UpdateGraphics();
	}

	private void UpdateGraphics()
	{
		SetDistanceToGround(DistanceToGround.y);
	}


	public void SetDistanceToGround(float height, bool _lands = true)
	{
		lands = _lands;
		DistanceToGround.y = height;
		HeightObject.transform.localPosition = DistanceToGround;
	}

	public float GetDistanceToGround() => DistanceToGround.y;
}
