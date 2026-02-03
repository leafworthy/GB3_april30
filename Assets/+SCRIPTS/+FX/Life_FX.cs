using GangstaBean.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityUtils;
using Random = UnityEngine.Random;

namespace __SCRIPTS
{
	public class Life_FX : MonoBehaviour, INeedPlayer
	{
		IGetAttacked life => _life ??= GetComponentInParent<IGetAttacked>();
		IGetAttacked _life;
		Tinter tint => _tint ??= GetComponent<Tinter>();
		Tinter _tint;
		LineBar healthBar => _healthBar ??= GetComponentInChildren<LineBar>(true);
		LineBar _healthBar;

		const float showBarFraction = .9f;

		void Life_Shielded(Attack obj)
		{
			tint?.StartTint(Color.yellow);
		}

		public void SetPlayer(Player newPlayer)
		{
			if (life == null) return;
			life.OnDead += Defence_Dead;
			life.OnAttackHit += Life_AttackHit;
			life.OnShielded += Life_Shielded;
			life.OnFractionChanged += Life_FractionChanged;
			if (healthBar == null) return;
			healthBar.useGradientColor = true;
		}

		void Life_FractionChanged(float newFraction)
		{
			if (healthBar == null) return;
			if (newFraction is > showBarFraction or 0)
			{
				healthBar?.gameObject.SetActive(false);
				return;
			}

			healthBar?.gameObject.SetActive(true);
			healthBar?.UpdateBar(newFraction);
		}

		public void OnDisable()
		{
			if (life == null) return;
			life.OnDead -= Defence_Dead;
			life.OnAttackHit -= Life_AttackHit;
			life.OnShielded -= Life_Shielded;
		}

		void Life_AttackHit(Attack attack)
		{
			Debug.Log("life attack hit");
			tint?.StartTint(attack.TintColor);
			CreateDamageRisingText(attack);
			SprayDebree(attack);
			MakeHitMark(attack);
		}
		void MakeHitMark(Attack attack)
		{
			var hitList = Services.assetManager.FX.GetBulletHits(life.DebrisType);

			if (hitList == null) return;

			var heightCorrectionForDepth = new Vector2(0, -1f);
			var hitMarkObject = Services.objectMaker.Make(hitList.GetRandom(), attack.DestinationFloorPoint + heightCorrectionForDepth);

			var hitHeightScript = hitMarkObject.GetComponent<HeightAbility>();
			hitHeightScript.SetHeight(attack.DestinationHeight - heightCorrectionForDepth.y);

			if (!(attack.Direction.x > 0))
			{
				var localScale = hitMarkObject.transform.localScale;
				hitMarkObject.transform.localScale = new Vector3(-Mathf.Abs(localScale.x), localScale.y, 0);
			}

			Services.objectMaker.Unmake(hitMarkObject, 5);
		}

		void SprayDebree(Attack attack)
		{
			if (!attack.MakesDebree) return;
			MakeDebree(attack);
			if (life.DebrisType != DebrisType.blood) return;
			CreateBloodSpray(attack);
		}

		void CreateBloodSpray(Attack attack)
		{
			Debug.Log("bloodspray created");
			var blood = Services.objectMaker.Make(Services.assetManager.FX.bloodspray.GetRandom(), attack.DestinationFloorPoint);
			var bloodHeightScript = blood.GetComponent<HeightAbility>();
			bloodHeightScript.SetHeight(attack.DestinationHeight);
			if (attack.Direction.x < 0) blood.transform.localScale = new Vector3(-blood.transform.localScale.x, blood.transform.localScale.y, 0);
		}

		void MakeDebree(Attack attack)
		{
			if (life.DebrisType == DebrisType.none) return;
			var randAmount = Random.Range(2, 10);
			for (var j = 0; j < randAmount; j++)
			{
				var randomAngle = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)).normalized;
				//----->
				FireDebree(attack.Direction + randomAngle, attack.OriginHeight, 1);

				//<-----
				FireDebree(attack.FlippedDirection + randomAngle, attack.OriginHeight, 1);
			}
		}

		void FireDebree(Vector2 angle, float height, float verticalSpeed)
		{
			var forwardDebree = Services.objectMaker.Make(Services.assetManager.FX.GetDebree(life.DebrisType), transform.position);
			forwardDebree.GetComponent<MoveJumpAndRotateAbility>().Fire(angle, height, verticalSpeed);
			_tint?.TintDebree(forwardDebree);

			Services.objectMaker.Unmake(forwardDebree, 3);
		}

		void CreateDamageRisingText(Attack attack)
		{
			if (attack.DamageAmount <= 0) return;
			if (!life.CanTakeDamage()) return;
			var roundedDamage = Mathf.Round(attack.DamageAmount);
			Services.risingText.CreateRisingText("-" + roundedDamage, attack.DestinationWithHeight, Color.red);
		}

		void Defence_Dead(Attack __attack)
		{
			if(healthBar != null)healthBar.gameObject.SetActive(false);
			_life.OnDead -= Defence_Dead;
			_life.OnAttackHit -= Life_AttackHit;
			_life.OnShielded -= Life_Shielded;
		}
	}
}
