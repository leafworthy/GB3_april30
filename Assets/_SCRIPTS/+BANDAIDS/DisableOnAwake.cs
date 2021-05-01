using UnityEngine;

namespace _SCRIPTS
{
	public class DisableOnAwake : MonoBehaviour
	{
		void Awake()
		{
			gameObject.SetActive(false);
		}
	}
}
