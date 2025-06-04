using UnityEngine;

namespace GangstaBean.Player
{
	public class PlayerIndicator : MonoBehaviour
	{
		public SpriteRenderer indicator;
		public SpriteRenderer indicatorB;

		private void Start()
		{
			HideIndicator();
		}

		public void ShowIndicator()
		{
			gameObject.SetActive(true);
		}

		public void SetColor(Color newColor)
		{
			indicator.color = newColor;
			indicatorB.color = newColor;
		}

		public void HideIndicator()
		{
			gameObject.SetActive(false);
		}
	}
}