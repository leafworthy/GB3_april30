using UnityEngine;

namespace __SCRIPTS
{
	[CreateAssetMenu(menuName = "My Assets/LayerAssets")]
	public class LevelAssets : ScriptableObject
	{
		public LayerMask PlayerLayer;
		public LayerMask EnemyLayer;
		public LayerMask LandedLayer;
		public LayerMask BuildingLayer;
		public LayerMask JumpingLayer;
		public LayerMask GroundedLayer;
		public LayerMask DoorLayer;
		public LayerMask ShootableNotWalkableLayer;

		public LayerMask EnemyUnwalkableLayers;
		public UnitStatsDatabase StatsDatabase;
	}
}
