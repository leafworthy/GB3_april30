using UnityEngine;

public class Armor 
{
	private int Amount;
	private HideRevealObjects DefenceStates;

	public Armor(int amount, HideRevealObjects defenceStates)
	{
		Amount = amount;
		DefenceStates = defenceStates;
	}
	public void Defend(Attack attack)
	{
		if (Amount <= 0) return;
		attack.DamageAmount = 0;
		Damage();
	}

	public void Repair()
	{
		Amount += 2;
		Refresh();
	}

	private void Damage()
	{
		Amount--;
		Amount = Mathf.Max(0, Amount);
		Refresh();
	}

	private void Refresh()
	{
		if (DefenceStates == null) return;
		if (Amount >= DefenceStates.objectsToReveal.Count - 1)
		{
			DefenceStates.Set(DefenceStates.objectsToReveal.Count - 1);
		}
		else
		{
			DefenceStates.Set(Amount);
		}
	}
}