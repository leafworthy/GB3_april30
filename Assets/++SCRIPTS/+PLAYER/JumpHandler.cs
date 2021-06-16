using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace _SCRIPTS
{
	public class JumpHandler : MonoBehaviour
	{
		public GameObject jumpObject;
		public GameObject shadowObject;
		public Vector3 jumpVector = new Vector3(0, 5, 0);

		public LayerMask jumpingLayer;
		public LayerMask landedLayer;
		public LayerMask groundedLayer;

		private Vector3 originalJumpObjectPosition;
		private Vector3 originalShadowObjectPosition;
		private bool isFalling;
		private bool isOnLandable;
		public bool isJumping;
		private bool isGrounded = true;
		public bool isDoneLanding = true;
		private bool jumpPressed;
		private Vector3 velocity;
		private float landingTime = .5f;

		public event Action OnJump;
		public event Action OnFall;
		public event Action OnLand;
		public event Action OnLandingStop;

		private IPlayerController remoteController;
		private Landable currentLandable;
		private MovementHandler movementHandler;
		private SortingGroup sortingGroup;
		private AnimationEvents animEvents;



		void Awake()
		{
			animEvents = GetComponentInChildren<AnimationEvents>();
			animEvents.OnLandingStart += LandingStart;
			animEvents.OnLandingStop += LandingStop;
			movementHandler = GetComponent<MovementHandler>();
			sortingGroup = GetComponent<SortingGroup>();

			remoteController = GetComponent<IPlayerController>();
			remoteController.OnJumpPress += JumpPress;
			remoteController.OnJumpRelease += JumpRelease;
			originalJumpObjectPosition = jumpObject.transform.localPosition;
			originalShadowObjectPosition = shadowObject.transform.localPosition;

		}

		private void LandingStop()
		{
			OnLandingStop?.Invoke();
			isDoneLanding = true;
		}

		private void LandingStart()
		{
			isDoneLanding = false;
		}

		private void JumpRelease()
		{
			jumpPressed = false;
		}

		private void JumpPress()
		{
			if (CanJump())
			{
				jumpPressed = true;
				Jump();
			}
		}

		private bool CanJump()
		{

			return movementHandler.CanMove() && !isJumping && !jumpPressed && isDoneLanding;
		}

		void FixedUpdate()
		{
			HandleLandable();
			if (isJumping)
			{
				HandleVelocity();
				HandleFalling();
				HandleLanding();
			}
		}

		private void HandleVelocity()
		{
			velocity -= GAME.Gravity * Time.fixedDeltaTime;
			jumpObject.transform.localPosition += velocity;
		}

		private void HandleLanding()
		{
			if (isFalling)
			{
				if (jumpObject.transform.localPosition.y
				    <= originalJumpObjectPosition.y + (IsAboveLandable() ? currentLandable.height : 0))
				{
					Land();
				}
			}
		}

		private void HandleFalling()
		{
			if (velocity.y < 0)
			{
				if (!isFalling)
				{
					isFalling = true;
					OnFall?.Invoke();
				}
			}
		}

		private void HandleLandable()
		{
			var raycastHit = IsAboveLandable();
			if (raycastHit.collider != null)
			{
				if (isJumping)
					currentLandable = raycastHit.collider.gameObject.GetComponent<Landable>();
				if (currentLandable != null)
				{
					shadowObject.transform.localPosition =
						new Vector3(shadowObject.transform.localPosition.x, currentLandable.height, 0);
					Debug.Log("up");
					sortingGroup.sortingOrder = 1;
				}
			}
			else
			{
				sortingGroup.sortingOrder = 0;
				if (!isJumping && isOnLandable)
				{
					FallFromLanded();
				}
				else
				{
					shadowObject.transform.localPosition = originalShadowObjectPosition;
				}
			}
		}

		private void FallFromLanded()
		{
			Debug.Log("fall from landed");

			isOnLandable = false;
			isFalling = true;
			isJumping = true;
			isGrounded = false;
			currentLandable = null;

			velocity = Vector3.zero;
			shadowObject.transform.localPosition = originalShadowObjectPosition;
			OnJump?.Invoke();
			OnFall?.Invoke();
		}

		private RaycastHit2D IsAboveLandable()
		{
		if(ASSETS.layers is null) return new RaycastHit2D();
			RaycastHit2D raycastHit = Physics2D.Raycast(transform.position,
				(Vector3.down), .01f,
				ASSETS.layers.LandableLayer);
			return raycastHit;
		}

		private void Jump()
		{
			Debug.Log("jump");
			isOnLandable = false;
			isDoneLanding = false;
			isJumping = true;
			isFalling = false;
			isGrounded = false;
			isDoneLanding = false;
			currentLandable = null;

			velocity = jumpVector;
			gameObject.layer = (int) Mathf.Log(jumpingLayer.value, 2);
			OnJump?.Invoke();
		}

		private void Land()
		{
			var raycastHit = IsAboveLandable();
			if (raycastHit.collider != null)
			{
				LandOnLandable(raycastHit);
			}
			else
			{
				LandOnGround();
			}

			isDoneLanding = false;
			Invoke("LandingStop", landingTime);
			Debug.Log("land");
			OnLand?.Invoke();
		}

		private void LandOnGround()
		{
			Debug.Log("land on ground");
			isOnLandable = false;
			isJumping = false;
			isFalling = false;
			isGrounded = true;
			isDoneLanding = false;

			sortingGroup.sortingOrder = 0;
			jumpObject.transform.localPosition = originalJumpObjectPosition;
			gameObject.layer = (int) Mathf.Log(groundedLayer.value, 2);
			currentLandable = null;
		}

		private void LandOnLandable(RaycastHit2D raycastHit)
		{
			Debug.Log("land on landable");
			isOnLandable = true;
			isJumping = false;
			isFalling = false;
			isGrounded = false;

			var height = raycastHit.collider.gameObject.GetComponent<Landable>().height;
			jumpObject.transform.localPosition = originalJumpObjectPosition + new Vector3(0, height, 0);
			gameObject.layer = (int) Mathf.Log(landedLayer.value, 2);

			sortingGroup.sortingOrder = 1;
		}
	}
}
