using System;
using UnityEngine;

namespace UpgradeS
{
	[Serializable]
	public class Upgrade : MonoBehaviour
	{
		protected int level = 1;
		public virtual string GetDescription => "No description";

		public virtual void CauseEffect(Player player)
		{
		}

		public virtual void UpgradeLevel()
		{
			level++;
		}

		public virtual int GetCost() => 0;

		public int GetLevel() => level;

		public virtual string GetName() => "";

		public virtual void ResetLevel()
		{
			level = 1;
		}
	}
}