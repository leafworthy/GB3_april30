using UnityEngine;

namespace __SCRIPTS
{
	public class EffectSurface : MonoBehaviour
	{
		public enum SurfaceAngle
		{
			Horizontal,
			Vertical,
			DiagonalFacingLeft,
			DiagonalFacingRight
		}

		public SurfaceAngle surfaceAngle;
	}
}
