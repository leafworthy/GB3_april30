using UnityEngine;

public class Damageable : MonoBehaviour
{
	private Life life;

	[SerializeField]private HideRevealObjects damageStates;

	public void Start()
	{
		life = GetComponentInChildren<Life>();
		if (life == null) return;
		life.OnDamaged += Object_OnDamaged;
		life.OnDead += Object_OnDead;
	}

	private void Object_OnDead(Player player)
	{
		SetDamageState();
	}


	private void Object_OnDamaged(Attack attack)
	{
		if (life.IsDead()) return;
		SetDamageState();
	}

	private void SetDamageState()
	{
		if (damageStates.objectsToReveal.Count <= 0) return;
		var index = damageStates.objectsToReveal.Count;
		var currentDamageLevel = Mathf.FloorToInt((1 - life.GetFraction()) * index);
		if (life.Health >= life.HealthMax)
		{
			damageStates.Set(0);
		}
		else
		{
			damageStates.Set(currentDamageLevel);
		}
	}


}