using UnityEngine;

public class RemoveOnDie : MonoBehaviour
{
	private Life life => GetComponent<Life>();

	private void OnEnable()
	{
		life.OnDead += Life_OnDead;
	}

	private void OnDisable()
	{
		life.OnDead -= Life_OnDead;
	}

	private void Life_OnDead(Player player)
	{
		Maker.Unmake(gameObject);
	}
}