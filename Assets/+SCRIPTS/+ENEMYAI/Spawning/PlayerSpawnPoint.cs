using System.Collections.Generic;
using UnityEngine;


	public class PlayerSpawnPoint : MonoBehaviour
	{
		private void Awake()
		{
			foreach (var c in GetComponentsInChildren<SpriteRenderer>())
			{
				c.enabled = false;
			}
		}
	}
