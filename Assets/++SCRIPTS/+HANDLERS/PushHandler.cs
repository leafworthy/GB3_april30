using UnityEngine;

public class PushHandler : MonoBehaviour
{
	private DefenceHandler health;
	private MovementHandler movementHandler;
	private const float pushMultiplier = .01f;


	private void Awake()
	{
		movementHandler = GetComponent<MovementHandler>();
		health = GetComponent<DefenceHandler>();
		health.OnDamaged += Health_DamagePush;

	}

	public void Push(Vector2 direction, float speed)
	{
		var tempVel = new Vector2(direction.x * speed, direction.y * speed);
		movementHandler.AddVelocity(tempVel);
	}

	private void Health_DamagePush(Attack attack)
	{
		Push(attack.DamageDirection, attack.DamageAmount * pushMultiplier);
	}

}
