using __SCRIPTS._COMMON;
using __SCRIPTS._FX;
using __SCRIPTS._UNITS;
using UnityEngine;

namespace __SCRIPTS._ATTACKS
{
	public class Brock_FX : MonoBehaviour
	{
		private KunaiAttacks kunaiAttacks;

		public void OnEnable()
		{
			kunaiAttacks = GetComponent<KunaiAttacks>();
			kunaiAttacks.OnThrow += KunaiAttacks_OnThrow;
		}

		private void KunaiAttacks_OnThrow(Vector3 aimDir, Vector3 pos, float throwHeight, Life life, bool isAirThrow)
		{
			var newProjectile = Maker.Make(FX.Assets.kunaiPrefab, transform.position);
			var kunaiScript = newProjectile.GetComponent<Kunai>();
			kunaiScript.Throw(aimDir, pos, throwHeight, life, isAirThrow);
		}

		private void OnDisable()
		{
			kunaiAttacks.OnThrow -= KunaiAttacks_OnThrow;
		}
	}
}