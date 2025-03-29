using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS
{
	public class CharacterSelectIndicators : MonoBehaviour
	{
		[SerializeField] private HideRevealObjects p1indicator;
		[SerializeField] private HideRevealObjects p2indicator;
		[SerializeField] private HideRevealObjects p3indicator;
		[SerializeField] private HideRevealObjects p4indicator;

		private List<HideRevealObjects> indicators = new();

		public void SetColors()
		{
			p1indicator.SetPlayerColor(Players.I.playerPresets[0].playerColor);
			p2indicator.SetPlayerColor(Players.I.playerPresets[1].playerColor);
			p3indicator.SetPlayerColor(Players.I.playerPresets[2].playerColor);
			p4indicator.SetPlayerColor(Players.I.playerPresets[3].playerColor);
		
		}

		public void Set(int index, int state)
		{
			switch (index)
			{
				case 0:
					p1indicator.Set(state);
					break;
				case 1:
					p2indicator.Set(state);
					break;
				case 2:
					p3indicator.Set(state);
					break;
				case 3:
					p4indicator.Set(state);
					break;
			}

			SetColors();
		}

		public void SetAllUnhighlighted()
		{
			p1indicator.Set(0);
			p2indicator.Set(0);
			p3indicator.Set(0);
			p4indicator.Set(0);
		}
	}
}