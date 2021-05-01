using UnityEngine;

namespace _SCRIPTS
{
	[CreateAssetMenu(menuName = "My Assets/PlayerAssets")]
	public class PlayerAssets : ScriptableObject
	{
		public enum Characters
		{
			Bean,
			Brock
		}

		public GameObject GangstaBeanPlayerPrefab;
		public GameObject BrockLeePlayerPrefab;

	}
}
