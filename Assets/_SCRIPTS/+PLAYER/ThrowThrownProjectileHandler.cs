using UnityEngine;

namespace _SCRIPTS
{
	public class ThrowThrownProjectileHandler : MonoBehaviour
	{
		[SerializeField] private GameObject ProjectilePrefab;
		[SerializeField] private GameObject ThrowPoint;



			[SerializeField] private AnimationEvents animationEvents;
		public Vector3 throwVector;

		private FaceDirection faceDirection;
		private UnitStats stats;
		private BeanAttackHandler attack;
		private Vector3 aimDirection;

		private void Start()
		{
			attack = GetComponent<BeanAttackHandler>();
			attack.OnNadeAim += OnNadeAim;
		}

		private void OnNadeAim(Vector3 aimDir)
		{
			aimDirection = aimDir;
			var directionMult = aimDir.x > 0? 1 : -1;
			DRAW.trajectory(new Vector2(aimDirection.x, aimDirection.y), ThrowPoint.transform.position,
				30, 3.5f);
		}

		private void Awake()
		{
			stats = GetComponent<UnitStats>();
			faceDirection = GetComponent<FaceDirection>();
			animationEvents.OnThrow += Throw;
		}



		private void Throw()
		{
			Debug.Log("throw handled");
			var newProjectile = MAKER.Make(ProjectilePrefab, transform.position);
			var projectileScript = newProjectile.GetComponent<ThrownProjectile>();
			var directionMult = aimDirection.x > 0? 1 : -1;
			projectileScript.Throw(new Vector2(aimDirection.x, aimDirection.y), stats.isPlayer, transform.position.y,
				ThrowPoint.transform.position.y);
		}
	}
}
