using System.Collections.Generic;
using System.Linq;
using GangstaBean.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace __SCRIPTS
{
	[ExecuteAlways]
	public class Life_FX : MonoBehaviour, IPoolable, INeedPlayer
	{
		public Color DebreeTint = Color.red;

		public enum ColorMode
		{
			Single,
			Gradient
		}

		private List<Renderer> renderersToTint = new();

		private Color materialTintColor;
		private Player player;
		private const float tintFadeSpeed = 6f;

		private static readonly int ColorReplaceColorA = Shader.PropertyToID("_NewColorA");
		private static readonly int ColorReplaceColorB = Shader.PropertyToID("_NewColorB");
		private static readonly int Tint = Shader.PropertyToID("_Tint");

		public void StartTint(Color tintColor)
		{
			Debug.Log("tint started with color " + tintColor);
			if (BlockTint) return;
			materialTintColor = tintColor;
			renderersToTint = GetComponentsInChildren<Renderer>().ToList();
			if (renderersToTint == null) return;
			foreach (var r in renderersToTint)
			{
				Debug.Log("tinting renderer " + r.name);
				r.material.SetColor(Tint, materialTintColor);
			}
		}

		public ColorMode colorMode;
		public Color slowBarColor = Color.white;
		public Gradient barGradient = new();

		private float targetFill;
		private float smoothingFactor = .25f;
		private IGetAttacked _life;
		private IGetAttacked life => _life ??= GetComponentInParent<IGetAttacked>();
		private ICanAttack _attack;
		private ICanAttack attack => _attack ??= GetComponentInParent<ICanAttack>();
		private HealthBar healthBar => _healthBar ??= InitHealthBar();
		[SerializeField] private HealthBar _healthBar;

		private HealthBar InitHealthBar()
		{
			if (!isBoss) return GetComponentInChildren<HealthBar>();
			Services.hudManager.SetBossLifeHealthbarVisible(true);
			return Services.hudManager.GetBossLifeHealthbar();

		}

		public bool BlockTint;
		public bool isBoss;

		public void Start()
		{
			if (life == null) return;
			life.OnFractionChanged += DefenceOnDefenceChanged;
			life.OnDead += DefenceOnDead;
			life.OnAttackHit += Life_AttackHit;
			life.OnShielded += Life_Shielded;
			if (healthBar != null) healthBar.gameObject.SetActive(false);
		}

		private void TintSprite(GameObject debreeObject)
		{
			var spriteToTint = debreeObject.GetComponentInChildren<SpriteRenderer>();
			if (spriteToTint != null)
				spriteToTint.color = DebreeTint;
		}

		private void Life_Shielded(Attack obj)
		{
			StartTint(Color.yellow);
		}

		public void SetPlayer(Player newPlayer)
		{
			if (!newPlayer.IsHuman()) return;
			PlayerTint(newPlayer.playerColor);
		}

		private void PlayerTint(Color color)
		{
			renderersToTint = GetComponentsInChildren<Renderer>().ToList();
			foreach (var r in renderersToTint)
			{
				r.material.SetColor(ColorReplaceColorA, color);
				r.material.SetColor(ColorReplaceColorB, color);
			}
		}

		public void OnDisable()
		{
			if (life == null) return;
			life.OnFractionChanged -= DefenceOnDefenceChanged;
			life.OnDead -= DefenceOnDead;
			life.OnAttackHit -= Life_AttackHit;
			life.OnShielded -= Life_Shielded;
		}

		private void Life_AttackHit(Attack attack)
		{
			Debug.Log("stats fx offence hit, color " + attack.TintColor);
			StartTint(attack.TintColor);
			CreateDamageRisingText(attack);
			SprayDebree(attack);
			MakeHitMark(attack);
		}

		[Button]
		private void Life_AttackHit()
		{
			var attack = Attack.Create(_attack, life).WithDamage(10).WithOriginPoint((Vector2) transform.position + Vector2.right)
			                   .WithDestinationPoint(transform.position).WithDestinationHeight(15);
			StartTint(attack.TintColor);
			CreateDamageRisingText(attack);
			SprayDebree(attack);
			MakeHitMark(attack);
		}

		private void MakeHitMark(Attack attack)
		{
			var hitList = Services.assetManager.FX.GetBulletHits(life.debrisType);

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
			if (life.debrisType != DebrisType.blood) return;
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
			if (life.debrisType == DebrisType.none) return;
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

		public void ExplodeDebreeEverywhere(float explosionSize, int min = 5, int max = 10)
		{
			if (life.debrisType == DebrisType.none) return;
			var randAmount = Random.Range(min, max);
			for (var j = 0; j < randAmount; j++)
			{
				var randomAngle = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
				FireDebree(randomAngle, 0, explosionSize);
			}
		}

		private void FireDebree(Vector2 angle, float height, float verticalSpeed)
		{
			var forwardDebree = Services.objectMaker.Make(Services.assetManager.FX.GetDebree(life.debrisType), transform.position);
			forwardDebree.GetComponent<MoveJumpAndRotateAbility>().Fire(angle, height, verticalSpeed);
			TintSprite(forwardDebree);
			Services.objectMaker.Unmake(forwardDebree, 3);
		}

		private void CreateDamageRisingText(Attack attack)
		{
			if (attack.DamageAmount <= 0) return;
			if (!life.CanTakeDamage()) return;
			var roundedDamage = Mathf.Round(attack.DamageAmount);
			Services.risingText.CreateRisingText("-" + roundedDamage, attack.DestinationWithHeight, Color.red);
		}

		private void DefenceOnDefenceChanged(float newAmount)
		{
			UpdateBarFill();
		}

		private void DefenceOnDead(Attack attack)
		{
			_life.OnFractionChanged -= DefenceOnDefenceChanged;
			_life.OnDead -= DefenceOnDead;
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
		}

		public void OnPoolDespawn()
		{
			if (_life == null) return;
			_life.OnFractionChanged -= DefenceOnDefenceChanged;
			_life.OnDead -= DefenceOnDead;
		}
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
}
