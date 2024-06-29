using UnityEngine;

namespace FunkyCode.SmartLighting2D.Scripts.Misc
{
	public class LightingMonoBehaviour : MonoBehaviour
	{
		public void DestroySelf()
		{
			if (Application.isPlaying)
			{
				Destroy(this.gameObject);
			}
				else
			{
				if (this != null && this.gameObject != null)
				{
					DestroyImmediate(this.gameObject);
				}
			}
		}
	}
}
