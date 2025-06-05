using System;
using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS._ZOMBIESPAWN
{
	[Serializable]
	public class ZombieWave
	{
		public List<GameObject> ZombiesToSpawn = new List<GameObject>();
		public float TimeBetweenSpawns;
	
		//public ZombieWave(List<GameObject> zombiesToSpawn, float timeBetweenSpawns)
		//{
		//	ZombiesToSpawn = zombiesToSpawn;
		//	TimeBetweenSpawns = timeBetweenSpawns;
		//}
	}
}