using UnityEngine;

namespace _SCRIPTS
{
	[CreateAssetMenu(menuName = "My Assets/LayerAssets")]
	public class LayerAssets : ScriptableObject
	{

		public LayerMask PlayerLayer;
		public LayerMask EnemyLayer;
		public LayerMask LandableLayer;
	}
}
