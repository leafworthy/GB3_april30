using System.Collections.Generic;
using GangstaBean.Objects.Projectiles;
using UnityEngine;

namespace GangstaBean.Effects
{
	public class NadeAbility_FX : MonoBehaviour
	{

		private NadeAttack nade;
		private Body body;
		private GameObject currentArrowHead;
		private List<GameObject> _trajectoryMarkersContainer = new List<GameObject>();
		private bool isShowingAiming;
		private const int _numberOfMarkers = 10;
		private const float _throwHeight = 8f;
		private void OnEnable()
		{
			nade = GetComponent<NadeAttack>();
			body = GetComponent<Body>();

			nade.OnThrow += Nade_OnThrow;
			nade.OnShowAiming += Nade_OnShowAiming;
			nade.OnHideAiming += Nade_OnHideAiming;
			nade.OnAimAt += Nade_OnAimAt;

			Nade_OnHideAiming();
			SpawnTrajectoryMarkers();

		}

		private void OnDisable()
		{
			_trajectoryMarkersContainer.Clear();
			nade.OnThrow -= Nade_OnThrow;
			nade.OnShowAiming -= Nade_OnShowAiming;
			nade.OnHideAiming -= Nade_OnHideAiming;
			nade.OnAimAt -= Nade_OnAimAt;
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
			if (!isShowingAiming) return;
			isShowingAiming = false;
			Debug.Log("hide aiming plz");
			if (currentArrowHead == null) return;
			currentArrowHead.SetActive(false);
			if (_trajectoryMarkersContainer.Count <= 0) return;
			foreach (var marker in _trajectoryMarkersContainer)
			{
				marker.SetActive(false);
			}
		}



		private void Nade_OnShowAiming()
		{
			if (isShowingAiming) return;
			isShowingAiming = true;
			Debug.Log("show aiming plz");
			currentArrowHead.SetActive(true);
			if (_trajectoryMarkersContainer.Count <= 0)
			{
				SpawnTrajectoryMarkers();
				return;
			}
			foreach (var marker in _trajectoryMarkersContainer) marker.SetActive(true);

		}

		private void SpawnTrajectoryMarkers()
		{
			currentArrowHead = ObjectMaker.I.Make(ASSETS.FX.nadeTargetPrefab);
			currentArrowHead.SetActive(false);
			for (var i = 0; i < _numberOfMarkers; i++)
			{
				_trajectoryMarkersContainer.Add(ObjectMaker.I.Make(ASSETS.FX.trajectoryMarkerPrefab));
			}
		}


		private void Nade_OnThrow(Vector2 startPoint, Vector2 velocity, float time, Player player)
		{
			var newProjectile = ObjectMaker.I.Make(ASSETS.FX.nadePrefab, body.AimCenter.transform.position);
			var nadeThrower = newProjectile.GetComponent<Nade>();
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
