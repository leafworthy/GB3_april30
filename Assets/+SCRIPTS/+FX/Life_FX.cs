using System;
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
		public Image slowBarImage;
		public Image fastBarImage;
		public Color DebreeTint = Color.white;
		private List<Renderer> renderersToTint = new();
		private Color materialTintColor;
		private const float tintFadeSpeed = 6f;
		private static readonly int ColorReplaceColor = Shader.PropertyToID("_NewColorA");
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
		public GameObject healthBar;
		public bool BlockTint;

		public void Start()
		{
			renderersToTint = GetComponentsInChildren<Renderer>().ToList();
			if (life == null) return;
			life.OnDamaged += LifeDamaged;
			life.OnFractionChanged += DefenceOnDefenceChanged;
			life.OnDying += DefenceOnDying;
			if (life.showLifeBar) return;
			if (healthBar != null) healthBar.SetActive(false);
		}

		public void SetPlayer(Player player)
		{
			Debug.Log("player set in life fx");
			if (!player.IsPlayer())
			{
				Debug.Log("not a player");
				return;
			}

			renderersToTint = GetComponentsInChildren<Renderer>().ToList();
			Debug.Log("on player set" + player.playerColor);
			foreach (var r in renderersToTint)
			{
				r.material.SetColor(ColorReplaceColor, player.playerColor);
			}
		}

		public void OnDisable()
		{
			if (life == null) return;
			life.OnDamaged -= LifeDamaged;
			life.OnFractionChanged -= DefenceOnDefenceChanged;
			life.OnDying -= DefenceOnDying;
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

		private void LifeDamaged(Attack attack)
		{
			if (life.IsShielded)
				StartTint(Color.yellow);
			else
			{
				StartTint(attack.color);
				CreateDamageRisingText(attack);
				SprayDebree(attack);
				MakeHitMark(attack);
			}

			if (attack.DestinationLife.DebrisType == DebrisType.none) return;
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
			if (attack.IsPoison) return;
			MakeDebree(attack);
			if (life.DebrisType != DebrisType.blood) return;
			CreateBloodSpray(attack);
		}

		private void CreateBloodSpray(Attack attack)
		{
			var blood = Services.objectMaker.Make(Services.assetManager.FX.bloodspray.GetRandom(), attack.DestinationFloorPoint);
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

				forwardDebree.GetComponent<FallToFloor>().Fire(attack);
				Services.objectMaker.Unmake(forwardDebree, 3);

				//<-----
				var flippedAttack = new Attack(life, attack.OriginLife, attack.DamageAmount);
				var backwardDebree = Services.objectMaker.Make(Services.assetManager.FX.GetDebree(life.DebrisType), attack.DestinationFloorPoint);
				backwardDebree.GetComponent<FallToFloor>().Fire(flippedAttack);
				Services.objectMaker.Unmake(backwardDebree, 3);

				var sprite = forwardDebree.GetComponentInChildren<SpriteRenderer>();
				if (sprite != null) sprite.color = DebreeTint;
				sprite = backwardDebree.GetComponentInChildren<SpriteRenderer>();
				if (sprite != null) sprite.color = DebreeTint;
			}
		}

		private void CreateDamageRisingText(Attack attack)
		{
			if (attack.DamageAmount <= 0) return;
			if (!life.CanBeAttacked) return;
			var roundedDamage = Mathf.Round(attack.DamageAmount);
			Services.risingText.CreateRisingText("-" + roundedDamage, attack.DestinationWithHeight, Color.red);
		}

		private void DefenceOnDefenceChanged(float newAmount)
		{
			UpdateBarFill();
		}

		private void DefenceOnDying(Player player, Life life1)
		{
			_life.OnFractionChanged -= DefenceOnDefenceChanged;
			_life.OnDying -= DefenceOnDying;
		}

		private void UpdateGradient()
		{
			var time = _life == null ? targetFill : life.GetFraction();
			if (colorMode == ColorMode.Gradient)
				fastBarImage.color = barGradient.Evaluate(time);
		}

		#region PUBLIC FUNCTIONS

		public void UpdateBar(float currentValue, float maxValue)
		{
			if (slowBarImage == null)
				return;
			targetFill = currentValue / maxValue;
			UpdateGradient();
			UpdateBarFill();
		}

		private void UpdateBarFill()
		{
			if (_life == null) return;
			if (healthBar == null) return;

			if (!life.showLifeBar) return;
			targetFill = _life.GetFraction();
			if (targetFill > .9f || targetFill <= 0)
				healthBar.SetActive(false);
			else
			{
				healthBar.SetActive(true);
				if (slowBarImage != null) slowBarImage.fillAmount = Mathf.Lerp(slowBarImage.fillAmount, targetFill, smoothingFactor);
				if (fastBarImage != null) fastBarImage.fillAmount = targetFill;
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
			if (colorMode != ColorMode.Single || slowBarImage == null)
				return;
			slowBarColor = targetColor;
			slowBarImage.color = slowBarColor;
		}

		private void UpdateColor(Gradient targetGradient)
		{
			if (colorMode != ColorMode.Gradient || slowBarImage == null)
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
			_life.OnDamaged -= LifeDamaged;
			_life.OnFractionChanged -= DefenceOnDefenceChanged;
			_life.OnDying -= DefenceOnDying;
		}
	}
}
