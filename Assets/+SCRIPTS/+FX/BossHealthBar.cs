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
		life.OnFractionChanged += Life_OnFractionChanged;
		Services.hudManager.bossHealthbar.UpdateBar(1);
	}

	void Life_OnFractionChanged(float newFraction)
	{
		Services.hudManager.bossHealthbar.UpdateBar(newFraction);
	}

	void Life_OnDead(Attack attack)
	{
		Services.hudManager.SetBossLifeHealthbarVisible(false);
	}

	void OnDisable()
	{
		Services.hudManager.SetBossLifeHealthbarVisible(false);
	}
}
