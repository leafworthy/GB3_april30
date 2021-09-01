using UnityEngine;

public class Attack
{
	public Attack(Vector2 damageOrigin, Vector2 damagePosition, float damageAmount = 0,  bool isPoison = false, STUNNER.StunLength stunlength =
		              STUNNER.StunLength.Normal, bool shakes = true, Player _owner = null)
	{
		DamageOrigin = damageOrigin;
		DamagePosition = damagePosition;
		DamageAmount = damageAmount;
		Stunlength = stunlength;
		Shakes = shakes;
		IsPoison = isPoison;
		if (_owner == null)
		{
			_owner = PLAYERS.GetEnemyPlayer();
		}
		Owner = _owner;
	}

	public Vector2 DamageDirection => DamagePosition - DamageOrigin;

	public float DamageAmount;
	public Vector2 DamagePosition;
	public bool IsPoison;
	public STUNNER.StunLength Stunlength;
	public bool Shakes;
	public Vector2 DamageOrigin;
	public Player Owner;
	public Vector3 HitPosition;
}
