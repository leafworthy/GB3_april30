using System.Collections.Generic;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class ProduceLootOnDead : MonoBehaviour
	{
		IGetAttacked life => _life ??= GetComponent<IGetAttacked>();
		IGetAttacked _life;
		LootTable lootTable => _lootTable ?? ServiceLocator.Get<LootTable>();
		LootTable _lootTable;

		public List<LootType> lootTypes = new();
		public int amountOfLootDropped = 5;

		protected void Start()
		{
			life.OnDead += DebrisOnDeathOnDebris;
		}

		void DebrisOnDeathOnDebris(Attack attack)
		{
			for (var i = 0; i < amountOfLootDropped; i++)
			{
				if (lootTypes.Count == 0)
				{
					lootTable.DropLoot(transform.position);
					continue;
				}

				var lootType = lootTypes[Random.Range(0, lootTypes.Count)];
				lootTable.DropLoot(transform.position, lootType);
			}
		}
	}
}
