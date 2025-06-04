using UnityEngine;

namespace GangstaBean.UI
{
	public class CharacterSelectButton : MonoBehaviour
	{
		public Character character;
		[SerializeField] private GameObject highlight;
		[SerializeField] private GameObject selectionGraphic;

		public CharacterSelectButton buttonToRight;
		public CharacterSelectButton buttonToLeft;



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
			selectionGraphic.SetActive(true);
		}

		public void Deselect()
		{
			selectionGraphic.SetActive(false);
		}
	}
}