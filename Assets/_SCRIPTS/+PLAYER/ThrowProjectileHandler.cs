using UnityEngine;

namespace _SCRIPTS
{
	public class ThrowProjectileHandler : MonoBehaviour
	{
		[SerializeField] private GameObject AirThrowPoint;
		[SerializeField] private GameObject ProjectilePrefab;
		[SerializeField] private GameObject ThrowPoint;
		private AnimationEvents animationEvents;

		private FaceDirection faceDirection;
		private UnitStats stats;

		private void Awake()
		{
			stats = GetComponent<UnitStats>();
			faceDirection = GetComponent<FaceDirection>();
			animationEvents = GetComponentInChildren<AnimationEvents>();
			animationEvents.OnThrow += Throw;
			animationEvents.OnAirThrow += AirThrow;
		}

		private void AirThrow()
		{
			var spawnPoint = transform.position ;
			var directionMult = faceDirection.isFacingRight ? 1 : -1;
			CreateProjectile(new Vector3(directionMult, -.5f, 0));
			CreateProjectile(new Vector3(directionMult, -1f, 0));
			CreateProjectile(new Vector3(directionMult, -.75f, 0));
		}

		private void CreateProjectile(Vector3 direction)
		{
			var newProjectile = MAKER.Make(ProjectilePrefab, transform.position);

			var projectileScript = newProjectile.GetComponent<Projectile>();
			projectileScript.HeightObject.transform.position = AirThrowPoint.transform.position;

			projectileScript.Fire(direction, stats.isPlayer, transform.position.y,
				AirThrowPoint.transform.position.y);
		}

		private void Throw()
		{
			Debug.Log("throw handled");
			var newProjectile = MAKER.Make(ProjectilePrefab, transform.position);
			var projectileScript = newProjectile.GetComponent<Projectile>();
			var directionMult = faceDirection.isFacingRight ? 1 : -1;
			projectileScript.Fire(new Vector3(directionMult, 0, 0), stats.isPlayer, transform.position.y, ThrowPoint.transform.position.y);
		}
	}
}
