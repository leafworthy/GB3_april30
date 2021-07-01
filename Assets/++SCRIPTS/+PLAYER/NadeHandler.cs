using System.Collections.Generic;
using UnityEngine;

public class NadeHandler : MonoBehaviour
{
	[SerializeField] private GameObject ProjectilePrefab;
	[SerializeField] private GameObject ThrowPoint;


	[SerializeField] private AnimationEvents animationEvents;
	public Vector3 throwVector;

	private GameObject currentArrowHead;
	private DirectionHandler directionHandler;
	private UnitStats stats;
	private BeanAttackHandler attack;
	private Vector3 aimDirection;
	private Vector3 hitpoint;
	private int throwTime = 30;
	private float gravity = 3.5f;
	public GameObject circleSprite;
	private static List<GameObject> Circles = new List<GameObject>();

	public GameObject arrowHeadPrefab;

	private void Awake()
	{
		attack = GetComponent<BeanAttackHandler>();
		stats = GetComponent<UnitStats>();
		directionHandler = GetComponent<DirectionHandler>();
	}

	private void Start()
	{
		attack.OnNadeAim += OnNadeAim;
		animationEvents.OnThrow += Throw;
		currentArrowHead = MAKER.Make(arrowHeadPrefab, transform.position);
		currentArrowHead.SetActive(false);
	}

	private void OnNadeAim(Vector3 aimDir)
	{
		aimDirection = new Vector3(aimDir.x, aimDir.y + throwVector.y, 0);

		hitpoint = DrawTrajectory(aimDirection, attack.GetAimCenter(),
			throwTime, gravity);
	}

	private void Throw()
	{
		ClearCircles();
		Debug.Log("throw handled");
		var newProjectile = MAKER.Make(ProjectilePrefab, transform.position);
		var projectileScript = newProjectile.GetComponent<ThrownProjectile>();

		projectileScript.Throw(new Vector2(aimDirection.x, aimDirection.y), stats.isPlayer, hitpoint.y,
			ThrowPoint.transform.position.y, hitpoint, throwTime, attack);
	}

	private static void ClearCircles()
	{
		foreach (var circle in Circles) MAKER.Unmake(circle);
	}

	private Vector3 DrawTrajectory(Vector2 vel, Vector2 start, int foresight, float _gravity = 1)
	{
		if (Circles.Count > 0)
		{
			foreach (var circle in Circles) MAKER.Unmake(circle);
		}

		Debug.Log("DRAWINGDRAJECTORY");
		var lastP = start;
		var p = start;
		var chunk = 1;
		var circle1 = MAKER.Make(circleSprite, p);
		Circles.Add(circle1);
		var lookPos = Vector2.zero;
		if (vel != Vector2.zero)
		{
			for (var i = 0; i < foresight + 1; i++)
			{
				p = lastP;
				var circle = MAKER.Make(circleSprite, p);
				Circles.Add(circle);
				vel = new Vector2(vel.x, vel.y - _gravity * Time.fixedDeltaTime) * chunk;
				p += vel * chunk;

				if (i == foresight - 1) lookPos = lastP - p;
				lastP = p;
			}

			currentArrowHead.SetActive(true);
			currentArrowHead.transform.position = lastP;
			return lastP;


			//float angle = Mathf.Atan2 (lookPos.y, lookPos.x) * Mathf.Rad2Deg + 90f;


			//arrowHead.transform.localRotation = Quaternion.AngleAxis (angle, Vector3.forward);
		}
		else
			Debug.Log("NO vel");

		return Vector2.zero;
	}
}
