using __SCRIPTS;
using Unity.Cinemachine;
using UnityEngine;

public class SoloCameraTrigger : MonoBehaviour
{

	[SerializeField] CinemachineCamera arenaCamera;
	bool hasTriggered;
	public Life bossLife;

	void Start()
	{
		bossLife.OnDeathComplete += BossLifeOnOnDeathComplete;

		CameraSwitcher.I.SoloCamera(arenaCamera);
	}

	protected void OnTriggerEnter2D(Collider2D other)
	{
		if (hasTriggered) return;
		hasTriggered = true;
		var otherLife = other.GetComponent<Life>();
		if (otherLife == null) return;
		if (otherLife.IsDead()) return;
		if (!otherLife.player.IsMainPlayer()) return;
		CameraSwitcher.I.UnSoloCamera();


	}

	void BossLifeOnOnDeathComplete(Player arg1, bool arg2)
	{
		if (hasTriggered) return;
		CameraSwitcher.I.UnSoloCamera();
	}
}
