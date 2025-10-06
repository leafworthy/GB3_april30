using __SCRIPTS;
using UnityEngine;

public class BossActivation : MonoBehaviour
{
	private bool isFinished;
	public GameObject Crimson;

	protected void OnTriggerEnter2D(Collider2D other)
	{
		var otherLife = other.GetComponent<Life>();
		if (otherLife == null) return;
		if (otherLife.IsDead()) return;
		if (!otherLife.Player.IsHuman()) return;
		if (isFinished) return;
		StartBossfight();
	}

	private void StartBossfight()
	{
		Crimson.gameObject.SetActive(true);
		isFinished = true;
	}
}
