using UnityEngine;

public class GridButton : MenuButton
{
	public string buttonName = "Unnamed";
	public GridButton buttonLeft;
	public GridButton buttonRight;
	public GridButton buttonUp;
	public GridButton buttonDown;
	public float cashCost = 50;
	
	public GameObject indicator;
	public override void Highlight()
	{
		base.Highlight();
		indicator.SetActive(true);
	}

	public override void UnHighlight()
	{
		base.UnHighlight();
		indicator.SetActive(false);
	}
}