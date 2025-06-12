using System;
using UnityEngine;

namespace __SCRIPTS.UpgradeS
{
	[Serializable]
	public class Upgrade : MonoBehaviour
	{
		public int level = 0;
		public int cost = 0;
		public virtual string GetDescription => "No description";

		public virtual void CauseEffect(Player player)
		{
		}

		public void UpgradeLevel()
		{
			level++;

		}

		public int Cost => (int)Mathf.Round(cost+ (level*100));

		public int Level => level;

		public virtual string GetName() => "";

		public virtual void ResetLevel()
		{
			level = 0;
		}

		public void ResetUpgrade()
		{
			ResetLevel();
		}
	}
}