using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS
{
	public class LootOnExplode : MonoBehaviour
	{
		private ExplodeOnDeath explodeOnDeath  => _explodeOnDeath ??= GetComponentInChildren<ExplodeOnDeath>();
		private ExplodeOnDeath _explodeOnDeath;
		public List<LootType> lootTypes = new List<LootType>();
		public int amount = 5;

		private LootTable lootTable => _lootTable ?? ServiceLocator.Get<LootTable>();
		private LootTable _lootTable;
		
		protected void Start()
		{
			explodeOnDeath.OnExplode += ExplodeOnDeath_OnExplode;
		}

		private void ExplodeOnDeath_OnExplode()
		{
			for (int i = 0; i < amount; i++)
			{
				var lootType = lootTypes[Random.Range(0, lootTypes.Count)];
				lootTable.DropLoot(transform.position, lootType);
			}
			explodeOnDeath.OnExplode -= ExplodeOnDeath_OnExplode;
		}
	}
}