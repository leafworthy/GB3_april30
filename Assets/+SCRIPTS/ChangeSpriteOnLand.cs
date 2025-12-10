using __SCRIPTS;
using Sirenix.OdinInspector;
using UnityEngine;

public class ChangeSpriteOnLand : MonoBehaviour
{
	public SpriteRenderer spriteRenderer => _spriteRenderer ??= GetComponentInChildren<SpriteRenderer>();
	private SpriteRenderer _spriteRenderer;
	private JumpAndRotateAbility JumpAndRotateAbility  => jumpAndRotateAbility ??= GetComponent<JumpAndRotateAbility>();
	private JumpAndRotateAbility jumpAndRotateAbility;
	private HideRevealObjects hideRevealObjects  => _hideRevealObjects ??= GetComponentInChildren<HideRevealObjects>();
	private HideRevealObjects _hideRevealObjects;

	private void Start()
	{
		JumpAndRotateAbility.OnResting += JumpAndRotateAbilityOnResting;
	}

	[Button]
	private void JumpAndRotateAbilityOnResting(Vector2 obj)
	{
		hideRevealObjects.SetRandom();
	}
}
