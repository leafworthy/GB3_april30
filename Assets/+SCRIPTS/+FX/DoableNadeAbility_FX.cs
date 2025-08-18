using System.Collections.Generic;
using __SCRIPTS.Projectiles;
using UnityEngine;

namespace __SCRIPTS
{
	[DisallowMultipleComponent, RequireComponent(typeof(NadeAttack))]
	public class DoableNadeAbility_FX : ServiceUser
	{
		private NadeAttack nade;
		private Body body;
		private GameObject currentArrowHead;
		private List<GameObject> _trajectoryMarkersContainer = new();
		private bool isShowingAiming;
		private const int _numberOfMarkers = 10;
		private const float _throwHeight = 8f;

		private void Start()
		{
			nade = GetComponent<NadeAttack>();
			body = GetComponent<Body>();

			nade.OnThrow += Nade_OnThrow;
			nade.OnShowAiming += Nade_OnShowAiming;
			nade.OnHideAiming += Nade_OnHideAiming;
			nade.OnAimAt += Nade_OnAimAt;
			nade.OnAimInDirection += Nade_OnAimInDirection;

			SpawnTrajectoryMarkers();
			Nade_OnHideAiming();
		}

		private void OnDisable()
		{
			_trajectoryMarkersContainer.Clear();
			if (nade != null)
			{
				nade.OnThrow -= Nade_OnThrow;
				nade.OnShowAiming -= Nade_OnShowAiming;
				nade.OnHideAiming -= Nade_OnHideAiming;
				nade.OnAimAt -= Nade_OnAimAt;
				nade.OnAimInDirection -= Nade_OnAimInDirection;
			}
		}

		private void Nade_OnAimInDirection(Vector2 startPoint, Vector2 endPoint)
		{
			PlaceMarkers(startPoint, endPoint);
		}

		private void Nade_OnAimAt(Vector2 startPoint, Vector2 endPoint)
		{
			PlaceMarkers(startPoint, endPoint);
		}

		private void Nade_OnHideAiming()
		{
			if (currentArrowHead != null) currentArrowHead.SetActive(false);
			if (_trajectoryMarkersContainer.Count <= 0) return;
			foreach (var marker in _trajectoryMarkersContainer)
			{
				if (marker != null) marker.SetActive(false);
			}
		}

		private void Nade_OnShowAiming()
		{
			if (currentArrowHead != null) currentArrowHead.SetActive(true);
			if (_trajectoryMarkersContainer.Count <= 0)
			{
				SpawnTrajectoryMarkers();
				return;
			}

			foreach (var marker in _trajectoryMarkersContainer)
			{
				if (marker != null) marker.SetActive(true);
			}
		}

		private void SpawnTrajectoryMarkers()
		{
			if (AssetManager.FX.nadeTargetPrefab == null) return;

			if (AssetManager.FX.trajectoryMarkerPrefab == null) return;

			currentArrowHead = objectMaker.Make(AssetManager.FX.nadeTargetPrefab);
			if (currentArrowHead != null) currentArrowHead.SetActive(true);

			for (var i = 0; i < _numberOfMarkers; i++)
			{
				var marker = objectMaker.Make(AssetManager.FX.trajectoryMarkerPrefab);
				if (marker != null)
				{
					_trajectoryMarkersContainer.Add(marker);
					marker.SetActive(true);
				}
			}
		}

		private void Nade_OnThrow(Vector2 startPoint, Vector2 velocity, float time, Player player)
		{
			if (AssetManager.FX.nadePrefab == null) return;

			var newProjectile = objectMaker.Make(AssetManager.FX.nadePrefab, body.AimCenter.transform.position);
			if (newProjectile == null) return;

			var nadeThrower = newProjectile.GetComponent<Nade>();
			if (nadeThrower == null) return;

			nadeThrower.Launch(startPoint, velocity, time, player);
		}

		private void PlaceMarkers(Vector2 pointA, Vector2 pointB)
		{
			currentArrowHead.transform.position = pointB;
			Nade_OnShowAiming();
			for (float i = 1; i <= _numberOfMarkers; i++)
			{
				var currentPosition = SampleParabola(pointA, pointB, _throwHeight, i / _numberOfMarkers);
				_trajectoryMarkersContainer[(int) i - 1].transform.position = new Vector3(currentPosition.x, currentPosition.y, 0);

				var nextPosition = SampleParabola(pointA, pointB, _throwHeight, (i + 1) / _numberOfMarkers);
				var angleInR = Mathf.Atan2(nextPosition.y - currentPosition.y, nextPosition.x - currentPosition.x);
				_trajectoryMarkersContainer[(int) i - 1].transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * angleInR - 90);
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
}
