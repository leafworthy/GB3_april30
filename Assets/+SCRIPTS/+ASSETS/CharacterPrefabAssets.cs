using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS
{
	[CreateAssetMenu(menuName = "My Assets/PlayerAssets")]
	public class CharacterPrefabAssets : ScriptableObject
	{
		public GameObject followMousePrefab;
		public GameObject followStickPrefab;
		public GameObject GangstaBeanPlayerPrefab;
		public GameObject BrockLeePlayerPrefab;
		public GameObject TMatoPlayerPrefab;
		public GameObject ToastEnemyPrefab;
		[SerializeField]public List<Material> ToastEnemyMaterials = new List<Material>();
		public GameObject ConeEnemyPrefab;
		public List<Material> ConeEnemyMaterials = new List<Material>();
		public GameObject DonutEnemyPrefab;
		public List<Material> DonutEnemyMaterials = new List<Material>();
		public GameObject CornEnemyPrefab;
		public List<Material> CornEnemyMaterials = new List<Material>();
		public List<GameObject> FruitEnemyPrefabs;
		public List<Material> FruitEnemyMaterials = new List<Material>();

		public List<Material> GetCharacterPalettes(EnemySpawner.EnemyType type)
		{
			switch (type)
			{
				case EnemySpawner.EnemyType.Toast:
					return ToastEnemyMaterials;
				case EnemySpawner.EnemyType.Cone:
					return ConeEnemyMaterials;
				case EnemySpawner.EnemyType.Donut:
					return DonutEnemyMaterials;
				case EnemySpawner.EnemyType.Corn:
					return CornEnemyMaterials;
				case EnemySpawner.EnemyType.Fruit:
					return FruitEnemyMaterials;
				default:
					Debug.Log("WTF");
					return null;
			}
		}
	}
}
