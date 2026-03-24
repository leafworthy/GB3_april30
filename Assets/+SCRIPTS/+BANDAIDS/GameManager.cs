using UnityEngine;

namespace __SCRIPTS
{
	public class GameManager : MonoBehaviour
	{

		protected void Start()
		{
			DontDestroyOnLoad(gameObject);
		}


	}
}
