using UnityEngine;

public class SlimeCreationHandler : MonoBehaviour
{
	[SerializeField] private GameObject ProjectilePrefab;
	[SerializeField] private GameObject ThrowPoint;
	private AnimationEvents animationEvents;
	private ConeAttackHandler attackHandler;

	private DirectionHandler directionHandler;
	private UnitStats stats;

	private void Start()
	{

	}

	private void Awake()
	{
		stats = GetComponent<UnitStats>();
		directionHandler = GetComponent<DirectionHandler>();
		animationEvents = GetComponentInChildren<AnimationEvents>();
		animationEvents.OnAttackHit += CreateSlime;
		attackHandler = GetComponent<ConeAttackHandler>();

	}

	private void CreateSlime(int obj)
	{
		var throwPoint=attackHandler.currentAttackTarget;
		var newProjectile = MAKER.Make(ProjectilePrefab, throwPoint);
		var projectileScript = newProjectile.GetComponent<SlimePool>();
		var directionMult = directionHandler.isFacingRight ? 1 : -1;
		projectileScript.Fire(directionMult);
	}


}
