using System.Collections.Generic;
using UnityEngine;

public class NadeHandler : MonoBehaviour
{
	[SerializeField] private AnimationEvents animationEvents;

	private GameObject currentArrowHead;
	private BeanAttackHandler attack;
	private Vector2 pointA;
	private Vector2 pointB;
	private int throwTime = 30;
	private static List<GameObject> Circles = new List<GameObject>();

	private void Awake()
	{
		attack = GetComponent<BeanAttackHandler>();
	}

	private void Start()
	{
		attack.OnNadeAim += OnNadeAim;
		attack.OnNadeAimAt += OnNadeAimAt;
		animationEvents.OnThrow += Throw;
		currentArrowHead = MAKER.Make(ASSETS.FX.nadeTargetPrefab, transform.position);
		currentArrowHead.SetActive(false);
	}



	private void OnNadeAim(Vector2 aimDir)
	{
		pointB = attack.GetAimCenter() +  aimDir;
		pointA = attack.GetAimCenter();
		currentArrowHead.SetActive(true);
	}

	private void OnNadeAimAt(Vector2 targetPosition)
	{
		pointB = targetPosition;
		pointA = attack.GetAimCenter();
		currentArrowHead.SetActive(true);
	}

	private void Throw()
	{
		ClearCircles();
		currentArrowHead.SetActive(false);

		var newProjectile = MAKER.Make(ASSETS.FX.nadePrefab, transform.position);
		Circles.Add(newProjectile);
		var nadeThrower = newProjectile.GetComponent<NadeLaunchHandler>();
		var velocity = new Vector3((pointB.x - pointA.x) / throwTime,
			(pointB.y - pointA.y) / throwTime);
		nadeThrower.Launch(pointA, pointB, velocity, throwTime, attack);
	}

	private static void ClearCircles()
	{
		if (Circles.Count <= 0) return;
		foreach (var circle in Circles) MAKER.Unmake(circle);
	}
}
