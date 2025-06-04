using System;
using System.Collections.Generic;
using UnityEngine;

namespace GangstaBean.ZombieSpawn._ZOMBIESPAWN
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