using __SCRIPTS;
using UnityEngine;

public class Fruit_SFX : MonoBehaviour
{
	Life life => _life ??= GetComponentInChildren<Life>();
	Life _life;
	NPC_AI npcAi => _npcAi ??= GetComponentInParent<NPC_AI>();
	NPC_AI _npcAi;

	void OnEnable()
	{
		life.OnDead += Life_OnDead;
		npcAi.OnRescued += NpcAi_OnRescued;
	}

	void NpcAi_OnRescued(NPC_AI obj)
	{
		Services.sfx.sounds.fruit_thank_you_sounds.PlayRandomAt(transform.position);
	}

	void OnDisable()
	{
		life.OnDead -= Life_OnDead;
	}

	void Life_OnDead(Attack obj)
	{
		Services.sfx.sounds.fruit_death_sounds.PlayRandomAt(transform.position);
	}
}
