using UnityEngine;
using System.Collections.Generic;

public class ThrowAimFX : MonoBehaviour
{
	private Vector2 pointA;
	private Vector2 pointB;

	private List<GameObject> _trajectoryMarkersContainer = new List<GameObject>();
	private float _height = 8f;
	private int _numberOfMarkers = 10;

	private Camera mainCamera;
	private BeanAttackHandler attackHandler;
	private GameObject currentArrowHead;
	private bool isAiming;

	private void Start()
	{
		attackHandler = GetComponent<BeanAttackHandler>();
		attackHandler.OnNadeAim += OnAim;
		attackHandler.OnNadeAimAt += OnAimAt;
		attackHandler.OnNadeAimStop += OnAimStop;

		mainCamera = Camera.main;
		SpawnCircles();

		pointA = attackHandler.GetAimCenter();
		pointB = mainCamera.ScreenToWorldPoint(Input.mousePosition);

		currentArrowHead = MAKER.Make(ASSETS.FX.nadeTargetPrefab, pointB);
		currentArrowHead.SetActive(false);
	}

	private void SpawnCircles()
	{
		for (var i = 0; i < _numberOfMarkers; i++)
			_trajectoryMarkersContainer.Add(MAKER.Make(ASSETS.FX.trajectoryMarkerPrefab));
	}

	private  void ClearCircles()
	{
		if (_trajectoryMarkersContainer.Count <= 0) return;
		foreach (var marker in _trajectoryMarkersContainer) MAKER.Unmake(marker);
	}

	private void OnAimStop()
	{
		isAiming = false;
		currentArrowHead.SetActive(false);
		ClearCircles();
	}

	private void OnAim(Vector2 obj)
	{
		currentArrowHead.SetActive(true);
		pointA = attackHandler.GetAimCenter();
		isAiming = true;
		SpawnCircles();
	}

	private void OnAimAt(Vector2 obj)
	{
		currentArrowHead.SetActive(true);
		isAiming = true;
		SpawnCircles();

	}

	private void FixedUpdate()
	{
		if (!isAiming) return;
		pointA = attackHandler.GetAimCenter();
		pointB = mainCamera.ScreenToWorldPoint(Input.mousePosition);
		currentArrowHead.transform.position = pointB;
		for (float i = 1; i <= _numberOfMarkers; i++)
		{
			var currentPosition = SampleParabola(pointA, pointB, _height, i / (float) _numberOfMarkers);
			_trajectoryMarkersContainer[(int) i - 1].transform.position =
				new Vector3(currentPosition.x, currentPosition.y, 0);

			var nextPosition = SampleParabola(pointA, pointB, _height, (i + 1) / (float) _numberOfMarkers);
			var angleInR = Mathf.Atan2(nextPosition.y - currentPosition.y, nextPosition.x - currentPosition.x);
			_trajectoryMarkersContainer[(int) i - 1].transform.eulerAngles =
				new Vector3(0, 0, Mathf.Rad2Deg * angleInR - 90);
		}
	}


	private static Vector3 SampleParabola(Vector3 start, Vector3 end, float height, float t)
	{
		var parabolicT = t * 2 - 1;
		if (Mathf.Abs(start.y - end.y) < 0.1f)
		{
			var travelDirection = end - start;
			var result = start + t * travelDirection;
			result.y += (-parabolicT * parabolicT + 1) * height;
			return result;
		}
		else
		{
			var travelDirection = end - start;
			var levelDirection = end - new Vector3(start.x, end.y, start.z);
			var right = Vector3.Cross(travelDirection, levelDirection);
			var up = Vector3.Cross(right, levelDirection);
			if (end.y > start.y) up = -up;
			var result = start + t * travelDirection;
			result += (-parabolicT * parabolicT + 1) * height * up.normalized;
			return result;
		}
	}
}
