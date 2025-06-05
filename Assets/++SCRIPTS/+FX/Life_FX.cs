using System.Collections.Generic;
using System.Linq;
using __SCRIPTS.RisingText;
using UnityEngine;
using UnityEngine.UI;

namespace __SCRIPTS
{
	public class Life_FX : MonoBehaviour
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
		public GameObject healthBar;
		public bool BlockTint;

		public void OnEnable()
		{
			renderersToTint = GetComponentsInChildren<Renderer>().ToList();
			_life = GetComponentInParent<Life>();
			if (_life == null) return;
			_life.OnDamaged += Life_Damaged;
			_life.OnFractionChanged += DefenceOnDefenceChanged;
			_life.OnDying += DefenceOnDead;
			_life.OnPlayerSet += OnPlayerSet;
			if (_life.unitData.showLifeBar) return;
			if (healthBar != null) healthBar.SetActive(false);
		}

		private void OnPlayerSet(Player player)
		{
			if (!player.IsPlayer()) return;
			Debug.Log("on player set" + player.playerColor);
			foreach (var r in renderersToTint)
			{
				r.material.SetColor(ColorReplaceColor, player.playerColor);
			}
		}

		public void OnDisable()
		{
			if (_life == null) return;
			_life.OnDamaged -= Life_Damaged;
			_life.OnFractionChanged -= DefenceOnDefenceChanged;
			_life.OnDying -= DefenceOnDead;
			_life.OnPlayerSet -= OnPlayerSet;
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



		private void Life_Damaged(Attack attack)
		{

			CreateDamageRisingText(attack);
			if (attack.DestinationLife.DebrisType == DebrisType.none) return;
			StartTint(attack.color);
			SprayDebree(attack);
			MakeHitMark(attack);
		}

		private void MakeHitMark(Attack attack)
		{

			var hitList = ASSETS.FX.GetBulletHits(_life.DebrisType);

			if (hitList == null) return;

			var heightCorrectionForDepth = new Vector2(0, -1f);
			var hitMarkObject = ObjectMaker.I.Make(hitList.GetRandom(), (Vector2) attack.DestinationFloorPoint + heightCorrectionForDepth);

			var hitHeightScript = hitMarkObject.GetComponent<ThingWithHeight>();
			hitHeightScript.SetDistanceToGround(attack.DestinationHeight - heightCorrectionForDepth.y, false);
			Debug.DrawLine(attack.DestinationFloorPoint, attack.DestinationFloorPoint + new Vector2(0, attack.DestinationHeight), Color.magenta, 5);

			if (!(attack.Direction.x > 0))
			{

				var localScale = hitMarkObject.transform.localScale;
				hitMarkObject.transform.localScale = new Vector3(-Mathf.Abs(localScale.x), localScale.y, 0);
			}

			ObjectMaker.I.Unmake(hitMarkObject, 5);
			Debug.DrawLine(attack.DestinationFloorPoint, attack.DestinationFloorPoint + heightCorrectionForDepth, Color.black, 1f);

		}
		private void SprayDebree(Attack attack)
		{
			if (attack.IsPoison) return;
			MakeDebree(attack);
			if (_life.DebrisType != DebrisType.blood) return;
			CreateBloodSpray(attack);
		}

		private void CreateBloodSpray(Attack attack)
		{
			var blood = ObjectMaker.I.Make(ASSETS.FX.bloodspray.GetRandom(), attack.DestinationFloorPoint);
			if (attack.Direction.x < 0)
			{
				blood.transform.localScale = new Vector3(-blood.transform.localScale.x, blood.transform.localScale.y, 0);
			}
		}
		private void MakeDebree(Attack attack)
		{
			if (_life.DebrisType == DebrisType.none) return;
			var randAmount = Random.Range(2, 4);
			for (var j = 0; j < randAmount; j++)
			{
				//----->
				var forwardDebree = ObjectMaker.I.Make(ASSETS.FX.GetDebree(_life.DebrisType), attack.DestinationFloorPoint);

				forwardDebree.GetComponent<FallToFloor>().Fire(attack);
				ObjectMaker.I.Unmake(forwardDebree, 3);

				//<-----
				var flippedAttack = new Attack(_life, attack.OriginLife, attack.DamageAmount);
				var backwardDebree = ObjectMaker.I.Make(ASSETS.FX.GetDebree(_life.DebrisType), attack.DestinationFloorPoint);
				backwardDebree.GetComponent<FallToFloor>().Fire(flippedAttack);
				ObjectMaker.I.Unmake(backwardDebree, 3);

					var sprite = forwardDebree.GetComponentInChildren<SpriteRenderer>();
					if (sprite != null) sprite.color = DebreeTint;
					sprite = backwardDebree.GetComponentInChildren<SpriteRenderer>();
					if (sprite != null) sprite.color = DebreeTint;

			}

		}

		private void CreateDamageRisingText(Attack attack)
		{
			if (attack.DamageAmount <= 0) return;
			if (_life.cantDie) return;
			var roundedDamage = Mathf.Round(attack.DamageAmount);
			RisingTextCreator.CreateRisingText("-" + roundedDamage, attack.DestinationWithHeight, Color.red);
		}
		private void DefenceOnDefenceChanged(float newAmount)
		{
			UpdateBarFill();
		}

		private void DefenceOnDead(Player player, Life life)
		{
			_life.OnFractionChanged -= DefenceOnDefenceChanged;
			_life.OnDying -= DefenceOnDead;
		}

		private void UpdateGradient()
		{
			var time = _life == null ? targetFill : _life.GetFraction();
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

			if (!_life.unitData.showLifeBar) return;
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
	}
}
