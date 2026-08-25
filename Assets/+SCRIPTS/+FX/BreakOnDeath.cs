using __SCRIPTS;
using UnityEngine;

public class BreakOnDeath : MonoBehaviour
{
	Life life => _life ??= GetComponentInChildren<Life>();
	Life _life;
	HideRevealObjects breakable => _breakable ??= GetComponentInChildren<HideRevealObjects>();
	HideRevealObjects _breakable;

	void Start()
	{
		life.OnDead += Life_OnDead;
		breakable?.Set(0);
	}

	void Life_OnDead(Attack obj)
	{
		breakable?.Set(1);
	}
}
