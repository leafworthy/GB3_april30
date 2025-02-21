
using UnityEngine;

[ExecuteInEditMode]
public class DoorDefence:MonoBehaviour
{
	public int defence;
	public HideRevealObjects defenceStates;
	public virtual void Defend(Attack attack)
	{
		if (defence <= 0) return;
		attack.DamageAmount = 0;
		Damage();
	}

	public void Repair()
	{
		defence += 2;
		Refresh();
	}

	public void Damage()
	{
		defence--;
		defence = Mathf.Max(0, defence);
		Refresh();
	}

	
	private void Refresh()
	{
		defenceStates ??= GetComponent<HideRevealObjects>();
		if (defence >= defenceStates.objectsToReveal.Count - 1)
		{
			defenceStates.Set(defenceStates.objectsToReveal.Count-1);
		}
		else
		{
			defenceStates.Set(defence);
		}
	}
}