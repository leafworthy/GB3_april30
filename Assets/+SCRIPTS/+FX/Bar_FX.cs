using UnityEngine;
using UnityEngine.UI;

namespace __SCRIPTS
{
	[ExecuteAlways]
	public class Bar_FX : MonoBehaviour
	{
		public Image slowBarImage;
		public Image fastBarImage;

		public enum ColorMode
		{
			Single,
			Gradient
		}

		public ColorMode colorMode;
		public Color slowBarColor = Color.white;
		public Gradient barGradient = new();

		public float targetFill;
		private float smoothingFactor = .25f;
		private Life _life;

		private void UpdateGradient()
		{

			if (colorMode == ColorMode.Gradient)
			{
				var time = _life == null ? targetFill : _life.GetFraction();
				fastBarImage.color = barGradient.Evaluate(time);
			}
		}

		#region PUBLIC FUNCTIONS

		public void UpdateBar(float currentValue, float maxValue)
		{
			UpdateBar(currentValue / maxValue);
		}

		public void UpdateBar(float fraction)
		{
			if (slowBarImage == null)
				return;
			targetFill = fraction;
			UpdateGradient();
			UpdateBarFill();
		}

		private void UpdateBarFill()
		{



			if( (slowBarImage != null) && targetFill < slowBarImage.fillAmount)
			{
				if (slowBarImage != null) slowBarImage.fillAmount = Mathf.Lerp(slowBarImage.fillAmount, targetFill, smoothingFactor);
				if (fastBarImage != null) fastBarImage.fillAmount = targetFill;
			}
			else
			{
				if (slowBarImage != null) fastBarImage.fillAmount = Mathf.Lerp(slowBarImage.fillAmount, targetFill, smoothingFactor);
				if (fastBarImage != null) slowBarImage.fillAmount = targetFill;
			}

		}

		private void Update()
		{
			UpdateColor(slowBarColor);
			UpdateColor(barGradient);
			UpdateBarFill();
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
