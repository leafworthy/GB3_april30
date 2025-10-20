using __SCRIPTS;
using Sirenix.OdinInspector;
using UnityEngine;

public class ChangeSpriteOnLand : MonoBehaviour
{
	public SpriteRenderer spriteRenderer => _spriteRenderer ??= GetComponentInChildren<SpriteRenderer>();
	private SpriteRenderer _spriteRenderer;
	private SimpleJumpAbility simpleJumpAbility  => _simpleJumpAbility ??= GetComponent<SimpleJumpAbility>();
	private SimpleJumpAbility _simpleJumpAbility;
	private HideRevealObjects hideRevealObjects  => _hideRevealObjects ??= GetComponentInChildren<HideRevealObjects>();
	private HideRevealObjects _hideRevealObjects;

	private void Start()
	{
		simpleJumpAbility.OnResting += SimpleJumpAbilityOnResting;
	}

	[Button]
	private void SimpleJumpAbilityOnResting(Vector2 obj)
	{
		Debug.Log("Landed, changing sprite");
		hideRevealObjects.SetRandom();
	}
}
