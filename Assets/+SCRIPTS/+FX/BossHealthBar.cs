using __SCRIPTS;
using UnityEngine;

public class BossHealthBar : MonoBehaviour
{
	Life life => _life ??= GetComponent<Life>();
	Life _life;
	bool hasStarted;

	void Start()
	{
		Debug.Log("boss this started");
		if (life == null) return;
		life.OnDead += Life_OnDead;
		Services.hudManager.SetBossLifeHealthbarVisible(true);
		hasStarted = true;
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
		if (!hasStarted) return;
		Services.hudManager.SetBossLifeHealthbarVisible(false);
	}
}
