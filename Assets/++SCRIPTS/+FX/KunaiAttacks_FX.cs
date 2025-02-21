using UnityEngine;

public class KunaiAttacks_FX : MonoBehaviour
{
	private KunaiAttack kunaiAttack;

	public void OnEnable()
	{
		kunaiAttack = GetComponent<KunaiAttack>();
		kunaiAttack.OnThrow += KunaiAttackOnThrow;
	}

	private void KunaiAttackOnThrow(Vector3 aimDir, Vector3 pos, float throwHeight, Life life, bool isAirThrow)
	{
		
		var newProjectile = ObjectMaker.Make(FX.Assets.kunaiPrefab, transform.position);
		var kunaiScript = newProjectile.GetComponent<Kunai>();
		kunaiScript.Throw(aimDir, pos, throwHeight, life, isAirThrow);
	}

	private void OnDisable()
	{
		kunaiAttack.OnThrow -= KunaiAttackOnThrow;
	}
}