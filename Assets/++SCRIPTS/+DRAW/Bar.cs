using UnityEngine;
using UnityEngine.UI;

namespace _SCRIPTS
{
	[ExecuteInEditMode]
	public class Bar : MonoBehaviour
	{
		[Header("Color Options")]
		public Image slowBarImage;
		public Image fastBarImage;

		public enum ColorMode
		{
			Single,
			Gradient
		}
		public ColorMode colorMode;
		public Color slowBarColor = Color.white;
		public Gradient barGradient = new Gradient();
		[Space]
		[Header("Fraction")]
		public float currentFraction = 1.0f;

		private float maxValue = 0.0f;
		private float currentValue = 0.0f;
		private float targetFill = 0.0f;
		private float smoothingFactor = .25f;
		private DefenceHandler defenceHandler;
		private Color PlayerColor;

		protected virtual void Start()
		{
			var color = GetComponentInParent<IPlayerController>();
			if (color != null)
			{
				PlayerColor = color.GetPlayerColor();
				fastBarImage.color = PlayerColor;
			}
			defenceHandler = GetComponentInParent<DefenceHandler>();
			defenceHandler.OnFractionChanged += DefenceOnDefenceChanged;
			defenceHandler.OnDead += DefenceOnDead;
		}

		private void DefenceOnDefenceChanged(float newAmount)
		{
			currentFraction = newAmount;
			UpdateBarFill();
		}

		private void DefenceOnDead()
		{
			gameObject.SetActive(false);
		}
		void UpdateGradient()
		{
			if (colorMode == ColorMode.Gradient)
				fastBarImage.color = barGradient.Evaluate(currentFraction);
		}

		#region PUBLIC FUNCTIONS

		public void UpdateBar(float currentValue, float maxValue)
		{
			if (slowBarImage == null)
				return;

			currentFraction = currentValue / maxValue;

			if (currentFraction < 0 || currentFraction > 1)
				currentFraction = currentFraction < 0 ? 0 : 1;


			this.maxValue = maxValue;
			this.currentValue = currentValue;

			UpdateGradient();
			UpdateBarFill();
		}



		private void UpdateBarFill()
		{
			targetFill = currentFraction;
			if (slowBarImage != null)
			{
				slowBarImage.fillAmount = Mathf.Lerp(slowBarImage.fillAmount, targetFill, smoothingFactor);
			}
			if (fastBarImage != null)
			{
				fastBarImage.fillAmount = targetFill;
			}
		}

		public void AddToBar(float add)
		{
			UpdateBar(currentValue + add, maxValue);
		}

		private void Update()
		{
			UpdateColor(slowBarColor);
			UpdateColor(barGradient);
			UpdateBarFill();
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
