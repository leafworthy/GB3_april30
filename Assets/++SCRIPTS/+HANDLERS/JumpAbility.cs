using System;
using UnityEngine;
using UnityEngine.Serialization;

public class JumpAbility : MonoBehaviour
{
	private Body body;
	public bool isResting;
	private float verticalVelocity;

	public static readonly string VerbName = "jumping";

	public event Action<Vector2> OnLand;
	public event Action<Vector2> OnResting;
	public event Action<Vector2> OnJump;

	private bool isOverLandable;
	[FormerlySerializedAs("isInAir")] public bool IsJumping;
	private ThingWithHeight thing;
	private float currentLandableHeight;
	private bool isActive = true;
	private bool isJumping;
	public Action<Vector2> OnFall;
	public Action OnBounce;
	private bool initiated;
	private float minBounceVelocity = 1000;
	private float bounceVelocityDragFactor = .2f;

	public void SetActive(bool active)
	{
		isActive = active;
	}

	public void Jump(float startingHeight = 0, float jumpSpeed = 2, float minBounce = 1)
	{
		minBounceVelocity = minBounce;
		IsJumping = true;
		isResting = false;
		OnJump?.Invoke(transform.position+ new Vector3(0,startingHeight,0));
		
		verticalVelocity = jumpSpeed;
		
		thing = GetComponent<ThingWithHeight>();
		thing.SetDistanceToGround(startingHeight);
		
		body = GetComponent<Body>();
		if (body == null) return;
		body.ChangeLayer(Body.BodyLayer.jumping);
	}

	private void Start()
	{
		Init();
	}

	private void Init()
	{
		if (initiated) return;
		initiated = true;
		thing = GetComponent<ThingWithHeight>();
		thing.OnFallFromLandable += FallFromHeight;
		body = GetComponent<Body>();
	}

	public void FallFromHeight(float fallHeight)
	{
		Init();
	
	
		if (body != null)
		{
			if (body.ShadowObject != null) body.ShadowObject.transform.localPosition = new Vector3(0,
				thing.GetCurrentLandableHeight(),0);
		}

		thing.SetDistanceToGround(fallHeight);
		IsJumping = true;
		isResting = false;
		OnFall?.Invoke(transform.position);
		
	}
	


	protected void FixedUpdate()
	{
		if (Game_GlobalVariables.IsPaused) return;
		if (isResting) return;
		if (!isActive) return;
		if (!IsJumping) return;
		

		Fly();
		
	}

	

	private void Fly()
	{
		
		currentLandableHeight = thing.GetCurrentLandableHeight();
		verticalVelocity -= (Game_GlobalVariables.Gravity.y) * Time.fixedDeltaTime;
		if ((thing.GetDistanceToGround() + verticalVelocity <= currentLandableHeight) && (verticalVelocity < 0))
		{
			Land();
		}
		else
		{
			thing.canLand = verticalVelocity < 0;
			thing.SetDistanceToGround(thing.GetDistanceToGround() + verticalVelocity);
		}
	}


	private void Land()
	{
		if (Mathf.Abs(verticalVelocity) > minBounceVelocity)
		{
			Bounce();
			return;
		}
		IsJumping = false;
		OnLand?.Invoke(transform.position+new Vector3(0, currentLandableHeight,0));
		thing.canLand = false;
		thing.SetDistanceToGround(currentLandableHeight);

		if (body != null) body.ChangeLayer(thing.isOverLandable ?  Body.BodyLayer.landed : Body.BodyLayer.grounded);

		verticalVelocity = 0;
	}

	private void Bounce()
	{
		verticalVelocity *= -1;
		var velocity = bounceVelocityDragFactor;
		verticalVelocity *= velocity;
		OnBounce?.Invoke();
	}


	public void StartResting()
	{
		isResting = true;
		OnResting?.Invoke(transform.position);
		if (body == null) return;
		body.legs.Stop("Landing");
		body.arms.Stop(VerbName);
		thing.canLand = false;
	}

}