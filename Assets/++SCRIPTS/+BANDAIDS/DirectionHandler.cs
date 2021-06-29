using UnityEngine;

public class DirectionHandler: MonoBehaviour
{
	private MovementHandler movementHandler;
	public Transform animScaleObject;
	public bool isFacingRight = true;
	public Transform FlipBackObject;

	private void Awake()
	{
		movementHandler = GetComponent<MovementHandler>();
		movementHandler.OnMoveDirectionChange += FaceCorrectDirection;
	}

	private void FaceCorrectDirection(bool faceRight)
	{
		if (isFacingRight == faceRight) return;
		isFacingRight = faceRight;
		if (faceRight)
		{
			FlipToRight();
			FlipBackToRight();
		}
		else
		{
			FlipToLeft();
			FlipBackToLeft();
		}
	}

	private void FlipToRight()
	{
		var localScale = animScaleObject.localScale;
		var currentScale = localScale;

		currentScale.x = Mathf.Abs(localScale.x);
		localScale = currentScale;
		animScaleObject.localScale = localScale;
	}

	private void FlipBackToRight()
	{
		if (FlipBackObject == null) return;
		var localScale = FlipBackObject.localScale;
		var currentScale = localScale;

		currentScale.x = Mathf.Abs(localScale.x);
		localScale = currentScale;
		FlipBackObject.localScale = localScale;
	}

	private void FlipToLeft()
	{
		var localScale = animScaleObject.localScale;
		var currentScale = localScale;

		currentScale.x = -Mathf.Abs(localScale.x);
		localScale = currentScale;
		animScaleObject.localScale = localScale;
	}

	private void FlipBackToLeft()
	{
		if (FlipBackObject == null) return;
		var localScale = FlipBackObject.localScale;
		var currentScale = localScale;

		currentScale.x = -Mathf.Abs(localScale.x);
		localScale = currentScale;
		FlipBackObject.localScale = localScale;
	}
}
