using UnityEngine;

namespace _SCRIPTS
{
	public class FaceDirection: MonoBehaviour
	{
		private MovementHandler movementHandler;
		public Transform animScaleObject;
		private float flipTolerance = .1f;
		public bool isFacingRight = true;

		private void Awake()
		{
			movementHandler = GetComponent<MovementHandler>();
			movementHandler.OnMoveDirectionChange += FaceCorrectDirection;
		}

		private void FaceCorrectDirection(bool faceRight)
		{
			isFacingRight = faceRight;
			if (faceRight)
			{
				FlipToRight();
			}
			else
			{
				FlipToLeft();
			}
		}

		private void FlipToRight()
		{
			var currentScale = animScaleObject.localScale;
			currentScale.x = Mathf.Abs(animScaleObject.localScale.x);
			animScaleObject.localScale = currentScale;
		}

		private void FlipToLeft()
		{
			var currentScale = animScaleObject.localScale;
			currentScale.x = -Mathf.Abs(animScaleObject.localScale.x);
			animScaleObject.localScale = currentScale;
		}
	}
}
