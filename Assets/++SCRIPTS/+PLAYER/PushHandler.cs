using UnityEngine;

public class PushHandler : MonoBehaviour
{
	private AnimationEvents animEvents;
	private DefenceHandler health;
	private MovementHandler movementHandler;
	private Vector2 pushVelocity;
	private bool isOn;
	private DashHandler dashHandler;
	private Vector2 currentPushDir;
	private float currentPushSpeed;
	private const float pushMultiplier = .01f;


	private void Awake()
	{
		movementHandler = GetComponent<MovementHandler>();
		health = GetComponent<DefenceHandler>();
		health.OnDamaged += Health_DamagePush;

		isOn = true;
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
