using System.Collections.Generic;
using GangstaBean.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace __SCRIPTS
{
	public class ProduceLootOnDead : MonoBehaviour
	{
		Life life => _life ??= GetComponentInChildren<Life>();
		Life _life;
		LootTable lootTable => _lootTable ?? ServiceLocator.Get<LootTable>();
		LootTable _lootTable;
		public GameObject customDropPoint;

		public List<LootType> lootTypes = new();
		public int amountOfLootDropped = 5;

		protected void Start()
		{
			life.OnDead += DebrisOnDeathOnDebris;
		}

		void DebrisOnDeathOnDebris(Attack attack)
		{
			for (var i = 0; i < amountOfLootDropped; i++) lootTable.DropLoot(GetPosition(), GetLootType(), 0, 1.5f,60);
		}

		LootType GetLootType() => lootTypes.Count == 0 ? LootType.Random : lootTypes[Random.Range(0, lootTypes.Count)];

		Vector3 GetPosition() => customDropPoint == null ? transform.position : customDropPoint.transform.position;

		[Button]
		public void AddCustomDropPoint()
		{
			if (customDropPoint != null) return;
			var go = new GameObject("Custom Drop Point");
			go.transform.SetParent(transform);
			go.transform.localPosition = Vector3.zero;
			customDropPoint = go;
		}
	}
}
