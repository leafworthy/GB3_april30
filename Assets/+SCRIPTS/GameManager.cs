using UnityEngine;

namespace __SCRIPTS
{
	public class GameManager : Singleton<GameManager>
	{
		protected override void OnEnable()
		{
			base.OnEnable();
			Debug.Log("GameManager started");
			DontDestroyOnLoad(gameObject);
		}
	}
}
