using UnityEngine;

namespace _SCRIPTS
{
	public interface IAttackHandler
	{
		bool CanAttack(Vector3 getPosition);
		void Disable();
	}
}