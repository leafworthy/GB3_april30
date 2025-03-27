using UnityEngine;

public class DirectionPointer:MonoBehaviour
{
	

Vector3 screenPos;
Vector2 onScreenPos;
float max;
Camera camera;

void Start()
{
	camera = Camera.main;
}

void Update()
{
	screenPos = camera.WorldToViewportPoint(transform.position); //get viewport positions

	if (screenPos.x >= 0 && screenPos.x <= 1 && screenPos.y >= 0 && screenPos.y <= 1)
	{
		Debug.Log("already on screen, don't bother with the rest!");
		return;
	}

	onScreenPos = new Vector2(screenPos.x - 0.5f, screenPos.y - 0.5f) * 2; //2D version, new mapping
	max = Mathf.Max(Mathf.Abs(onScreenPos.x), Mathf.Abs(onScreenPos.y)); //get largest offset
	onScreenPos = (onScreenPos / (max * 2)) + new Vector2(0.5f, 0.5f); //undo mapping
	Debug.Log(onScreenPos);
}
}