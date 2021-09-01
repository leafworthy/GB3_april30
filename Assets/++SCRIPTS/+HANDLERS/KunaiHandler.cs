using UnityEngine;

public class KunaiHandler : MonoBehaviour
{
	[SerializeField] private GameObject AirThrowPoint;
	[SerializeField] private GameObject ProjectilePrefab;
	[SerializeField] private GameObject ThrowPoint;
	private AnimationEvents animationEvents;

	private DirectionHandler directionHandler;
	private UnitStats stats;
	private Vector3 aimDir;
	private BrockAttackHandler attackHandler;

	private void Awake()
	{
		attackHandler = GetComponent<BrockAttackHandler>();
		attackHandler.OnAim += OnAim;
		stats = GetComponent<UnitStats>();
		directionHandler = GetComponent<DirectionHandler>();
		animationEvents = GetComponentInChildren<AnimationEvents>();
		animationEvents.OnThrow += Throw;
		animationEvents.OnAirThrow += AirThrow;
	}

	private void OnAim(Vector3 newAimDir)
	{
		aimDir = newAimDir;
	}

	private void AirThrow()
	{
		var spawnPoint = transform.position ;
		var directionMult = directionHandler.isFacingRight ? 1 : -1;
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
			AirThrowPoint.transform.position.y, true, stats.player);
	}

	private void Throw()
	{
		Debug.Log("throw handled");
		var newProjectile = MAKER.Make(ProjectilePrefab, transform.position);
		var projectileScript = newProjectile.GetComponent<Projectile>();
		var directionMult = directionHandler.isFacingRight ? 1 : -1;
		projectileScript.Fire(aimDir, stats.isPlayer, transform.position.y, ThrowPoint.transform.position.y, false, stats.player);
	}
}
