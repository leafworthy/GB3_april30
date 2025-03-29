using TMPro;
using UnityEngine;

namespace __SCRIPTS.UpgradeS
{
	public class UpgradeSelectButton : MonoBehaviour
	{
		public Upgrade upgrade;
		public GameObject highlight;
		public GameObject redHighlight;
		//[SerializeField] private GameObject selectionGraphic;

		public UpgradeSelectButton buttonToRight;
		public UpgradeSelectButton buttonToLeft;

		public TextMeshProUGUI CostText;
		public TextMeshProUGUI LevelText;

		private void Update()
		{
			RefreshText();
		}

		public void RefreshText()
		{
			CostText.text = upgrade.Cost.ToString();
			LevelText.text = upgrade.Level.ToString();
		}

		public void Highlight()
		{
			highlight.SetActive(true);
			redHighlight.SetActive(false);
		}

		public void Unhighlight()
		{
			highlight.SetActive(false);
			redHighlight.SetActive(false);
		}

		public void RedHighlight()
		{
			highlight.SetActive(false);
			redHighlight.SetActive(true);
		}

		public void Select()
		{
			//selectionGraphic.SetActive(true);
		}

		public void Deselect()
		{
			//selectionGraphic.SetActive(false);
		}
	}
}