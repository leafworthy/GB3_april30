using UnityEngine;

public class AimAbility : MonoBehaviour
{
	protected Life life;
	protected Player owner;
	protected Body body;
	[HideInInspector] public Vector2 AimDir;
	public const float aimDistanceFactor = 100;
	private float minMagnitude = .3f;

	public virtual void Start()
	{
		body = GetComponent<Body>();
		
		life = GetComponent<Life>();
		if (life == null) return;
		owner = life.player;
		if (owner == null) return;
		if (owner.isUsingMouse) return;
		owner.Controller.AimAxis.OnChange += AimerOnAim;
	}

	private void OnDisable()
	{
		if (owner == null) return;
		if (owner.isUsingMouse) return;
		owner.Controller.AimAxis.OnChange -= AimerOnAim;
	
	}

	private void AimerOnAim(IControlAxis controlAxis, Vector2 aimDir)
	{
		if (GlobalManager.IsPaused) return;
		if (owner.isDead()) return;
			
		if (aimDir.magnitude > minMagnitude)
			AimDir = aimDir;
		
	
	}

	protected virtual void Update()
	{
		if (GlobalManager.IsPaused) return;
		rotateAimObjects(GetAimDir());
		DrawShootableLine();
	}

	public RaycastHit2D CheckRaycastHit(Vector3 targetDirection)
	{
		var raycastHit = Physics2D.Raycast(body.FootPoint.transform.position, targetDirection.normalized, life.AttackRange,
			ASSETS.LevelAssets.BuildingLayer);

		return raycastHit;
	}

	private void rotateAimObjects(Vector2 direction)
	{
		var rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		foreach (var obj in body.RotateWithAim)
		{
			if (obj == null) continue;
			obj.transform.eulerAngles = new Vector3(0, 0, rotation);
		}
	}

	private void DrawShootableLine()
	{
		var hitPoint = CheckRaycastHit(AimDir);
		if (hitPoint.collider != null)
		{
			Debug.DrawLine(body.FootPoint.transform.position, hitPoint.point, Color.red);
		}
		else
		{
			Debug.DrawLine(body.FootPoint.transform.position, GetAimPoint(), Color.green);

		}
	}
	public Vector2 GetAimPoint(float multiplier = 1)
	{
		if (owner.isUsingMouse)
		{
			var mousePos = CursorManager.GetMousePosition();
			if (Vector2.Distance(body.AimCenter.transform.position, mousePos) < life.AttackRange * multiplier)
			{
				return CursorManager.GetMousePosition();
			}
			else
			{
				return (Vector2) body.AimCenter.transform.position + AimDir.normalized * life.AttackRange;
			}
		}

		return (Vector2) body.AimCenter.transform.position + AimDir * aimDistanceFactor;
	}

	public virtual Vector3 GetAimDir()
	{
		if(owner.isUsingMouse)
		{
			return CursorManager.GetMousePosition() - body.AimCenter.transform.position;
		}
		return AimDir;
	}
}