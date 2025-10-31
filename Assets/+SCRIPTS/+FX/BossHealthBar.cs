using __SCRIPTS;
using UnityEngine;

public class BossHealthBar : MonoBehaviour
{
	private BasicHealth life => _life ??= GetComponent<BasicHealth>();
	private BasicHealth _life;
	private void Start()
	{
		if (life == null) return;
		life.OnDead += Life_OnDead;
		Services.hudManager.SetBossLifeHealthbarVisible(true);
	}

	private void Life_OnDead(Attack attack)
	{
		Services.hudManager.SetBossLifeHealthbarVisible(false);
	}
}
