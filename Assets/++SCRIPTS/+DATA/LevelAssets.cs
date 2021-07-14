using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/LayerAssets")]
public class LevelAssets : ScriptableObject
{

	public GameObject StartingLevelPrefab;
	public LayerMask PlayerLayer;
	public LayerMask EnemyLayer;
	public LayerMask LandableLayer;
	public LayerMask PlayerDashLayer;
	public LayerMask BuildingLayer;
}
