using __SCRIPTS;
using Sirenix.OdinInspector;
using UnityEngine;

public class ChangeSpriteOnLand : MonoBehaviour
{
	public SpriteRenderer spriteRenderer => _spriteRenderer ??= GetComponentInChildren<SpriteRenderer>();
	SpriteRenderer _spriteRenderer;
	JumpAndRotateAbility JumpAndRotateAbility => jumpAndRotateAbility ??= GetComponent<JumpAndRotateAbility>();
	JumpAndRotateAbility jumpAndRotateAbility;
	HideRevealObjects hideRevealObjects => _hideRevealObjects ??= GetComponentInChildren<HideRevealObjects>();
	HideRevealObjects _hideRevealObjects;

	void Start()
	{
		JumpAndRotateAbility.OnResting += JumpAndRotateAbilityOnResting;
	}

	[Button]
	void JumpAndRotateAbilityOnResting(Vector2 obj)
	{
		hideRevealObjects.SetRandom();
	}
}