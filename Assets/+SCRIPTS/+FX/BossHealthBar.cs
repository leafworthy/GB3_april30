using __SCRIPTS;
using UnityEngine;

public class BossHealthBar : MonoBehaviour
{
	Life life => _life ??= GetComponent<Life>();
	Life _life;

	void Start()
	{
		if (life == null) return;
		life.OnDead += Life_OnDead;
		Services.hudManager.SetBossLifeHealthbarVisible(true);
	}

	void Life_OnDead(Attack attack)
	{
		Services.hudManager.SetBossLifeHealthbarVisible(false);
	}
}
