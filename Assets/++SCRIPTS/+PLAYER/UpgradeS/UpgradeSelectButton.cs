using TMPro;
using UnityEngine;

namespace UpgradeS
{
	public class UpgradeSelectButton : MonoBehaviour
	{
		public Upgrade upgrade;
		[SerializeField] private GameObject highlight;
		//[SerializeField] private GameObject selectionGraphic;

		public UpgradeSelectButton buttonToRight;
		public UpgradeSelectButton buttonToLeft;

		public TextMeshProUGUI CostText;
		public TextMeshProUGUI LevelText;

		private void Start()
		{
			if(upgrade == null)
			{
				CostText.text = "EXIT";
				return;
			}

			RefreshText();
		}

		private void RefreshText()
		{
			CostText.text = upgrade.GetCost().ToString();
			LevelText.text = upgrade.GetLevel().ToString();
		}

		public void Highlight()
		{
			highlight.SetActive(true);
		}

		public void Unhighlight()
		{
			highlight.SetActive(false);
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