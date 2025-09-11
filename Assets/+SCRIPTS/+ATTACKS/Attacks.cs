using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class Attacks : MonoBehaviour, INeedPlayer, IActivity
	{
		private Life _attacker;
		protected Life life => _attacker?? GetComponent<Life>();
		public virtual string AbilityName => "Generic-Attack";





		public virtual void SetPlayer(Player _player)
		{
		}
	}
}
