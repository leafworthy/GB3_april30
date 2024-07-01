using System.Collections.Generic;
using System.Linq;
using __SCRIPTS._ATTACKS;
using __SCRIPTS._PLAYER;
using __SCRIPTS._UNITS;
using UnityEngine;
using UnityEngine.UI;

namespace __SCRIPTS._FX
{
	public class Life_FX : MonoBehaviour
	{
		[Header("Color Options")]
		public Image slowBarImage;
		public Image fastBarImage;
		public bool HideWhenFull;
		private List<Renderer> renderersToTint = new List<Renderer>();
		private Color materialTintColor;
		private const float tintFadeSpeed = 6f;
		private static readonly int Tint = Shader.PropertyToID("_Tint");
		public enum ColorMode
		{
			Single,
			Gradient
		}
		public ColorMode colorMode;
		public Color slowBarColor = Color.white;
		public Gradient barGradient = new Gradient();
	 

		private float targetFill = 0.0f;
		private float smoothingFactor = .25f;
		private Life _life;
		private Color PlayerColor;

		protected virtual void Start()
		{
		
			_life = GetComponentInParent<Life>();
			if (_life == null) return;
			
		}

		public void OnEnable()
		{
			renderersToTint = GetComponentsInChildren<Renderer>().ToList();
			_life = GetComponent<Life>();
			if (_life == null) return;
			_life.OnDamaged += Life_Damaged;
			_life.OnFractionChanged += DefenceOnDefenceChanged;
			_life.OnDying += DefenceOnDead;
		}

		public void StartTint(Color tintColor)
		{
			materialTintColor = new Color();
			materialTintColor = tintColor;
			foreach (var r in renderersToTint)
			{
				r.material.SetColor(Tint, materialTintColor);
			}
		}

		private void Life_Damaged(Attack attack)
		{
			StartTint(attack.color);
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
		void UpdateGradient()
		{
			var time = (_life == null) ? targetFill : _life.GetFraction();
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
			if (_life != null)
			{
			
				targetFill = _life.GetFraction();
				if (HideWhenFull && (targetFill > .9f))
				{
					gameObject.SetActive(false);
				}
				else
				{
					gameObject.SetActive(true);
				}
			}

			if (slowBarImage != null)
			{
				slowBarImage.fillAmount = Mathf.Lerp(slowBarImage.fillAmount, targetFill, smoothingFactor);
			}
			if (fastBarImage != null)
			{
				fastBarImage.fillAmount = targetFill;
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

		public void UpdateColor(Color targetColor)
		{
			if (colorMode != ColorMode.Single || slowBarImage == null)
				return;
			slowBarColor = targetColor;
			slowBarImage.color = slowBarColor;
		}

		public void UpdateColor(Gradient targetGradient)
		{
			// If the color is not set to gradient, then return.
			if (colorMode != ColorMode.Gradient || slowBarImage == null)
				return;

			barGradient = targetGradient;
			UpdateGradient();
		}

		#endregion
	}
}
