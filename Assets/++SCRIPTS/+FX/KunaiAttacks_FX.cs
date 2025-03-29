using __SCRIPTS.Projectiles;
using UnityEngine;

namespace __SCRIPTS
{
	public class KunaiAttacks_FX : MonoBehaviour
	{
		private PrimaryAttack_Kunai primaryAttackKunai;

		public void OnEnable()
		{
			primaryAttackKunai = GetComponent<PrimaryAttack_Kunai>();
			primaryAttackKunai.OnThrow += PrimaryAttackKunaiOnThrow;
		}

		private void PrimaryAttackKunaiOnThrow(Vector3 aimDir, Vector3 pos, float throwHeight, Life life, bool isAirThrow)
		{
		
			var newProjectile = ObjectMaker.I.Make(ASSETS.FX.kunaiPrefab, transform.position);
			var kunaiScript = newProjectile.GetComponent<Kunai>();
			kunaiScript.Throw(aimDir, pos, throwHeight, life, isAirThrow);
		}

		private void OnDisable()
		{
			primaryAttackKunai.OnThrow -= PrimaryAttackKunaiOnThrow;
		}
	}
}