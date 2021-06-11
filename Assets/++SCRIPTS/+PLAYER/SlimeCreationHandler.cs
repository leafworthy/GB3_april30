using UnityEngine;

namespace _SCRIPTS
{
	public class SlimeCreationHandler : MonoBehaviour
	{
		[SerializeField] private GameObject ProjectilePrefab;
		[SerializeField] private GameObject ThrowPoint;
		private AnimationEvents animationEvents;
		private ConeAttackHandler attackHandler;

		private FaceDirection faceDirection;
		private UnitStats stats;

		private void Start()
		{

		}

		private void Awake()
		{
			stats = GetComponent<UnitStats>();
			faceDirection = GetComponent<FaceDirection>();
			animationEvents = GetComponentInChildren<AnimationEvents>();
			animationEvents.OnAttackHit += CreateSlime;
			attackHandler = GetComponent<ConeAttackHandler>();

		}

		private void CreateSlime(int obj)
		{
			var throwPoint=attackHandler.currentAttackTarget;
			var newProjectile = MAKER.Make(ProjectilePrefab, throwPoint);
			var projectileScript = newProjectile.GetComponent<SlimePool>();
			var directionMult = faceDirection.isFacingRight ? 1 : -1;
			projectileScript.Fire(directionMult);
		}


	}
}
