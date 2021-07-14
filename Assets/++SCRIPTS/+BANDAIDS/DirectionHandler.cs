using UnityEngine;

public class DirectionHandler : MonoBehaviour
{
	private MovementHandler movementHandler;
	public Transform animScaleObject;
	public bool isFacingRight = true;

	private void Start()
	{
		movementHandler = GetComponent<MovementHandler>();
		movementHandler.OnMoveDirectionChange += FaceCorrectDirection;
		movementHandler.OnStopAllMovement += CleanUp;
	}

	private void CleanUp()
	{
		movementHandler.OnMoveDirectionChange -= FaceCorrectDirection;
		movementHandler.OnStopAllMovement -= CleanUp;
	}

	private void FaceCorrectDirection(bool faceRight)
	{
		if (isFacingRight == faceRight) return;
		isFacingRight = faceRight;
		if (faceRight)
			FlipToRight();
		else
			FlipToLeft();
	}

	private void FlipToRight()
	{
		var localScale = animScaleObject.localScale;
		var currentScale = localScale;

		currentScale.x = Mathf.Abs(localScale.x);
		localScale = currentScale;
		animScaleObject.localScale = localScale;
	}


	private void FlipToLeft()
	{
		var localScale = animScaleObject.localScale;
		var currentScale = localScale;

		currentScale.x = -Mathf.Abs(localScale.x);
		localScale = currentScale;
		animScaleObject.localScale = localScale;
	}
}
