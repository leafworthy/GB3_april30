using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/PlayerAssets")]
public class CharacterPrefabAssets : ScriptableObject
{
	public enum Characters
	{
		Bean,
		Brock
	}

	public GameObject GangstaBeanPlayerPrefab;
	public GameObject BrockLeePlayerPrefab;
	public GameObject ToastEnemyPrefab;
	public GameObject ConeEnemyPrefab;
	public GameObject DonutEnemyPrefab;

}
