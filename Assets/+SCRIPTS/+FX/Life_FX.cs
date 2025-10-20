using System.Collections.Generic;
using System.Linq;
using GangstaBean.Core;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace __SCRIPTS
{
	public class Life_FX : MonoBehaviour, IPoolable, INeedPlayer
	{
		public Color DebreeTint = Color.white;
		private List<Renderer> renderersToTint = new();
		private Color materialTintColor;
		private const float tintFadeSpeed = 6f;
		private static readonly int ColorReplaceColorA = Shader.PropertyToID("_NewColorA");
		private static readonly int ColorReplaceColorB = Shader.PropertyToID("_NewColorB");
		private static readonly int Tint = Shader.PropertyToID("_Tint");

		public enum ColorMode
		{
			Single,
			Gradient
		}

		public ColorMode colorMode;
		public Color slowBarColor = Color.white;
		public Gradient barGradient = new();

		private float targetFill;
		private float smoothingFactor = .25f;
		private Life _life;
		private Life life => _life ??= GetComponentInParent<Life>();
		private HealthBar healthBar;
		public bool BlockTint;
		public bool isBoss = false;

		public void Start()
		{
			if (!isBoss) healthBar = GetComponentInChildren<HealthBar>();
			else
			{
				Debug.Log("got here bruh FX");
				healthBar = Services.hudManager.GetBossLifeHealthbar();
				Services.hudManager.SetBossLifeHealthbarVisible(true);
			}
			renderersToTint = GetComponentsInChildren<Renderer>().ToList();
			if (life == null) return;
			life.OnFractionChanged += DefenceOnDefenceChanged;
			life.OnDying += DefenceOnDying;
			life.OnAttackHit += Life_AttackHit;
			life.OnShielded += Life_Shielded;
			if (life.showLifeBar) return;
			if (healthBar != null) healthBar.gameObject.SetActive(false);
		}

		private void Life_Shielded(Attack obj)
		{
			StartTint(Color.yellow);
		}

		public void SetPlayer(Player player)
		{
			if (!player.IsHuman()) return;

			renderersToTint = GetComponentsInChildren<Renderer>().ToList();
			foreach (var r in renderersToTint)
			{
				r.material.SetColor(ColorReplaceColorA, player.playerColor);
				r.material.SetColor(ColorReplaceColorB, player.playerColor);
			}
		}

		public void OnDisable()
		{
			if (life == null) return;
			life.OnFractionChanged -= DefenceOnDefenceChanged;
			life.OnDying -= DefenceOnDying;
			life.OnAttackHit -= Life_AttackHit;
			life.OnShielded -= Life_Shielded;
		}

		public void StartTint(Color tintColor)
		{
			if (BlockTint) return;
			materialTintColor = new Color();
			materialTintColor = tintColor;
			foreach (var r in renderersToTint)
			{
				r.material.SetColor(Tint, materialTintColor);
			}
		}

		private void Life_AttackHit(Attack attack)
		{
			Debug.Log("life fx attack hit, color " + attack.TintColor);
			StartTint(attack.TintColor);
			CreateDamageRisingText(attack);
			SprayDebree(attack);
			MakeHitMark(attack);
		}

		private void MakeHitMark(Attack attack)
		{
			var hitList = Services.assetManager.FX.GetBulletHits(life.DebrisType);

			if (hitList == null) return;

			var heightCorrectionForDepth = new Vector2(0, -1f);
			var hitMarkObject = Services.objectMaker.Make(hitList.GetRandom(), attack.DestinationFloorPoint + heightCorrectionForDepth);

			var hitHeightScript = hitMarkObject.GetComponent<ThingWithHeight>();
			hitHeightScript.SetDistanceToGround(attack.DestinationHeight - heightCorrectionForDepth.y, false);
			Debug.DrawLine(attack.DestinationFloorPoint, attack.DestinationFloorPoint + new Vector2(0, attack.DestinationHeight), Color.magenta, 5);

			if (!(attack.Direction.x > 0))
			{
				var localScale = hitMarkObject.transform.localScale;
				hitMarkObject.transform.localScale = new Vector3(-Mathf.Abs(localScale.x), localScale.y, 0);
			}

			Services.objectMaker.Unmake(hitMarkObject, 5);
			Debug.DrawLine(attack.DestinationFloorPoint, attack.DestinationFloorPoint + heightCorrectionForDepth, Color.black, 1f);
		}

		private void SprayDebree(Attack attack)
		{
			if (attack.MakesDebree) return;
			MakeDebree(attack);
			if (life.DebrisType != DebrisType.blood) return;
			CreateBloodSpray(attack);
		}

		private void CreateBloodSpray(Attack attack)
		{
			var blood = Services.objectMaker.Make(Services.assetManager.FX.bloodspray.GetRandom(), attack.DestinationFloorPoint);
			var bloodHeightScript = blood.GetComponent<ThingWithHeight>();
			bloodHeightScript.SetDistanceToGround(attack.DestinationHeight, false);
			if (attack.Direction.x < 0) blood.transform.localScale = new Vector3(-blood.transform.localScale.x, blood.transform.localScale.y, 0);
		}

		private void MakeDebree(Attack attack)
		{
			if (life.DebrisType == DebrisType.none) return;
			var randAmount = Random.Range(2, 4);
			for (var j = 0; j < randAmount; j++)
			{
				//----->
				var forwardDebree = Services.objectMaker.Make(Services.assetManager.FX.GetDebree(life.DebrisType), attack.DestinationFloorPoint);
				forwardDebree.GetComponent<IDebree>().Fire(attack).TintSprite(DebreeTint);
				Services.objectMaker.Unmake(forwardDebree, 3);

				//<-----
				var flippedAttack = Attack.GetFlippedAttack(attack);
				var backwardDebree = Services.objectMaker.Make(Services.assetManager.FX.GetDebree(life.DebrisType), flippedAttack.DestinationFloorPoint);
				backwardDebree.GetComponent<IDebree>().Fire(flippedAttack).TintSprite(DebreeTint);
				Services.objectMaker.Unmake(backwardDebree, 3);
			}
		}

		public void ExplodeDebreeEverywhere(float explosionSize, int min = 5, int max = 10)
		{
			if (life.DebrisType == DebrisType.none) return;
			var randAmount = Random.Range(min, max);
			for (var j = 0; j < randAmount; j++)
			{
				//----->
				var forwardDebree = Services.objectMaker.Make(Services.assetManager.FX.GetDebree(life.DebrisType), transform.position);
				forwardDebree.GetComponent<IDebree>().Explode(explosionSize).TintSprite(DebreeTint);
				Services.objectMaker.Unmake(forwardDebree, 3);

				//<-----
				var backwardDebree = Services.objectMaker.Make(Services.assetManager.FX.GetDebree(life.DebrisType), transform.position);
				backwardDebree.GetComponent<IDebree>().Explode(explosionSize).TintSprite(DebreeTint);
				Services.objectMaker.Unmake(backwardDebree, 3);
			}
		}

		private void CreateDamageRisingText(Attack attack)
		{
			if (attack.DamageAmount <= 0) return;
			if (!life.IsNotInvincible) return;
			var roundedDamage = Mathf.Round(attack.DamageAmount);
			Services.risingText.CreateRisingText("-" + roundedDamage, attack.DestinationWithHeight, Color.red);
		}

		private void DefenceOnDefenceChanged(float newAmount)
		{
			UpdateBarFill();
		}

		private void DefenceOnDying(Attack attack)
		{
			_life.OnFractionChanged -= DefenceOnDefenceChanged;
			_life.OnDying -= DefenceOnDying;
			if (!isBoss) return;
			Services.hudManager.SetBossLifeHealthbarVisible(false);
		}

		private void UpdateGradient()
		{
			var time = life == null ? targetFill : life.GetFraction();
			if (float.IsNaN(time))
			{
				this.enabled = false;
				return;
			}
			if (colorMode == ColorMode.Gradient)
				healthBar.FastBar.color = barGradient.Evaluate(time);
		}

		#region PUBLIC FUNCTIONS


		private void UpdateBarFill()
		{
			if (_life == null) return;
			if (healthBar == null) return;

			if (!life.showLifeBar) return;
			targetFill = _life.GetFraction();
			if (targetFill > .9f || targetFill <= 0)
			{
				if(!isBoss) healthBar.gameObject.SetActive(false);
			}
			else
			{
				healthBar.gameObject.SetActive(true);
				if (healthBar.SlowBar != null) healthBar.SlowBar.fillAmount = Mathf.Lerp(healthBar.SlowBar.fillAmount, targetFill, smoothingFactor);
				if (healthBar.FastBar != null) healthBar.FastBar.fillAmount = targetFill;
			}
		}

		private void Update()
		{
			UpdateColor(slowBarColor);
			UpdateColor(barGradient);
			UpdateBarFill();
			FadeOutTintAlpha();
		}

		private void FadeOutTintAlpha()
		{
			if (!(materialTintColor.a > 0)) return;
			materialTintColor.a = Mathf.Clamp01(materialTintColor.a - tintFadeSpeed * Time.deltaTime);
			foreach (var r in renderersToTint)
			{
				r.material.SetColor(Tint, materialTintColor);
			}
		}

		private void UpdateColor(Color targetColor)
		{
			if (colorMode != ColorMode.Single || healthBar == null || healthBar?.SlowBar == null)
				return;
			slowBarColor = targetColor;
			healthBar.SlowBar.color = slowBarColor;
		}

		private void UpdateColor(Gradient targetGradient)
		{
			if (colorMode != ColorMode.Gradient || healthBar == null || healthBar?.SlowBar == null)
				return;

			barGradient = targetGradient;
			UpdateGradient();
		}

		#endregion

		public void OnPoolSpawn()
		{
			// Reset tint to no tint (transparent) when spawning from pool
			materialTintColor = new Color(1, 1, 1, 1);
			renderersToTint = GetComponentsInChildren<Renderer>().ToList();
			foreach (var r in renderersToTint)
			{
				if (r != null && r.material != null) r.material.SetColor(Tint, materialTintColor);
			}
		}

		public void OnPoolDespawn()
		{
			// Clean up event subscriptions when despawning
			if (_life == null) return;
			_life.OnFractionChanged -= DefenceOnDefenceChanged;
			_life.OnDying -= DefenceOnDying;
		}
	}


	public interface IRotate
	{
		IRotate RotateToDirection(Vector2 direction, GameObject rotationObject);
		IRotate SetRotationRate(float i);
		IRotate SetFreezeRotation(bool freeze);
	}
	public interface IDebree
	{
		IDebree Fire(Attack attack);
		IDebree Fire(Vector2 angle, float height);
		IDebree Explode(float explosionSize);
		IDebree TintSprite(Color debreeTint);

		IDebree SetDistanceToGround(float height);

		IDebree FireFlipped(Attack attack);
	}
}
