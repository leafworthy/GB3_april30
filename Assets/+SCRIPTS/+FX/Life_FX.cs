using System.Collections.Generic;
using System.Linq;
using GangstaBean.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace __SCRIPTS
{
	[ExecuteAlways]
	public class PlayerTint
	{
		private List<Renderer> renderersToTint;
		public Color DebreeTint = Color.white;
		private Color materialTintColor;
		private Player player;
		private const float tintFadeSpeed = 6f;
		public bool BlockTint;

		public PlayerTint(Color color, GameObject go)
		{
			renderersToTint = go.GetComponentsInChildren<Renderer>().ToList();
			foreach (var r in renderersToTint)
			{
				r.material.SetColor(ColorReplaceColorA, color);
				r.material.SetColor(ColorReplaceColorB, color);
			}
		}

		private static readonly int ColorReplaceColorA = Shader.PropertyToID("_NewColorA");
		private static readonly int ColorReplaceColorB = Shader.PropertyToID("_NewColorB");
		private static readonly int Tint = Shader.PropertyToID("_Tint");

		public void TintSprite(GameObject debreeObject)
		{
			var spriteToTint = debreeObject.GetComponentInChildren<SpriteRenderer>();
			if (spriteToTint != null)
				spriteToTint.color = DebreeTint;
		}

		public void FadeOutTintAlpha()
		{
			if (!(materialTintColor.a > 0)) return;
			materialTintColor.a = Mathf.Clamp01(materialTintColor.a - tintFadeSpeed * Time.deltaTime);
			foreach (var r in renderersToTint)
			{
				r.material.SetColor(Tint, materialTintColor);
			}
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


	}

	[ExecuteAlways]
	public class Life_FX : MonoBehaviour, IPoolable, INeedPlayer
	{
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
		public bool isBoss;

		private PlayerTint playerTint;

		public void Start()
		{
			if (!isBoss) healthBar = GetComponentInChildren<HealthBar>();
			else
			{
				Debug.Log("got here bruh FX");
				healthBar = Services.hudManager.GetBossLifeHealthbar();
				Services.hudManager.SetBossLifeHealthbarVisible(true);
			}

			playerTint = new PlayerTint(Color.white, gameObject);
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
			playerTint?.StartTint(Color.yellow);
		}

		public void SetPlayer(Player player)
		{
			if (!player.IsHuman()) return;
			playerTint = new PlayerTint(player.playerColor, gameObject);
		}

		public void OnDisable()
		{
			if (life == null) return;
			life.OnFractionChanged -= DefenceOnDefenceChanged;
			life.OnDying -= DefenceOnDying;
			life.OnAttackHit -= Life_AttackHit;
			life.OnShielded -= Life_Shielded;
		}

		private void Life_AttackHit(Attack attack)
		{
			Debug.Log("life fx attack hit, color " + attack.TintColor);
			playerTint?.StartTint(attack.TintColor);
			CreateDamageRisingText(attack);
			SprayDebree(attack);
			MakeHitMark(attack);
		}

		[Button]
		private void Life_AttackHit()
		{
			var attack = Attack.Create(life, life).WithDamage(10).WithOriginPoint((Vector2)transform.position+Vector2.right).WithDestinationPoint(transform.position).WithDestinationHeight(15);
			playerTint?.StartTint(attack.TintColor);
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

			var hitHeightScript = hitMarkObject.GetComponent<HeightAbility>();
			hitHeightScript.SetHeight(attack.DestinationHeight - heightCorrectionForDepth.y);

			if (!(attack.Direction.x > 0))
			{
				var localScale = hitMarkObject.transform.localScale;
				hitMarkObject.transform.localScale = new Vector3(-Mathf.Abs(localScale.x), localScale.y, 0);
			}

			Services.objectMaker.Unmake(hitMarkObject, 5);
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
			var bloodHeightScript = blood.GetComponent<HeightAbility>();
			bloodHeightScript.SetHeight(attack.DestinationHeight);
			if (attack.Direction.x < 0) blood.transform.localScale = new Vector3(-blood.transform.localScale.x, blood.transform.localScale.y, 0);
		}

		private void MakeDebree(Attack attack)
		{
			if (life.DebrisType == DebrisType.none) return;
			var randAmount = Random.Range(2, 4);
			for (var j = 0; j < randAmount; j++)
			{
				//----->
				FireDebree(attack.Direction, attack.OriginHeight, 0);

				//<-----
				FireDebree(attack.FlippedDirection, attack.OriginHeight, 0);
			}
		}

		public void ExplodeDebreeEverywhere(float explosionSize, int min = 5, int max = 10)
		{
			if (life.DebrisType == DebrisType.none) return;
			var randAmount = Random.Range(min, max);
			for (var j = 0; j < randAmount; j++)
			{
				var randomAngle = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
				FireDebree(randomAngle, 0, explosionSize);
			}
		}


		private void FireDebree(Vector2 angle, float height, float verticalSpeed)
		{
			var forwardDebree = Services.objectMaker.Make(Services.assetManager.FX.GetDebree(life.DebrisType), transform.position);
			forwardDebree.GetComponent<IDebree>().Fire(angle, height, verticalSpeed);
			playerTint?.TintSprite(forwardDebree);
			Services.objectMaker.Unmake(forwardDebree, 3);
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
				enabled = false;
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
			if (targetFill is > .9f or <= 0)
			{
				if (!isBoss) healthBar.gameObject.SetActive(false);
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
			playerTint?.FadeOutTintAlpha();
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

		}

		public void OnPoolDespawn()
		{
			if (_life == null) return;
			_life.OnFractionChanged -= DefenceOnDefenceChanged;
			_life.OnDying -= DefenceOnDying;
		}

		public void StartTint(Color pickupTintColor) => playerTint?.StartTint(pickupTintColor);
	}

	public interface IRotate
	{
		IRotate RotateToDirection(Vector2 direction, GameObject rotationObject);
		IRotate SetRotationRate(float i);
		IRotate SetFreezeRotation(bool freeze);
	}

	public interface IHaveHeight
	{
		IHaveHeight SetHeight(float height);
		float GetHeight();
	}

	public interface IDebree : IHaveHeight
	{
		IDebree Fire(Vector2 shootAngle, float height, float verticalSpeed = 0, float pushSpeed = 40);
	}
}
