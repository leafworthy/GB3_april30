using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/LayerAssets")]
public class LevelAssets : ScriptableObject
{
	public GameObject StartingLevelPrefab;
	public LayerMask PlayerLayer;
	public LayerMask EnemyLayer;
	public LayerMask LandableLayer;
	public LayerMask LandedLayer;
	public LayerMask BuildingLayer;
	public LayerMask JumpingLayer;
	public LayerMask GroundedLayer;
	public LayerMask DoorLayer;
	public LayerMask JumpableLayer;
	public LayerMask ShootableNotWalkableLayer;
	public string Scene_GameSystems;
	public LayerMask EnemyUnwalkableLayers;
	public UnitStatsData DefaultUnitData;
	public LayerMask EnemyLayerOnLandable;
}