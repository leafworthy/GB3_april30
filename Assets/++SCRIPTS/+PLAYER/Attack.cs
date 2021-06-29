using UnityEngine;

public class Attack
{
	public Attack(Vector2 damageOrigin, Vector2 damagePosition, float damageAmount = 0,  bool isPoison = false, HITSTUN.StunLength stunlength =
		              HITSTUN.StunLength.Normal, bool shakes = true)
	{
		DamageOrigin = damageOrigin;
		DamagePosition = damagePosition;
		DamageAmount = damageAmount;
		Stunlength = stunlength;
		Shakes = shakes;
		IsPoison = isPoison;
	}

	public Vector2 DamageDirection
	{
		get { return DamagePosition - DamageOrigin; }
	}

	public float DamageAmount;
	public Vector2 DamagePosition;
	public bool IsPoison;
	public HITSTUN.StunLength Stunlength;
	public bool Shakes;
	public Vector2 DamageOrigin;
}
