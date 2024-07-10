using UnityEngine;
using UnityEngine.Serialization;

public class InteractionIndicator : MonoBehaviour
{
	[FormerlySerializedAs("interactionOffset")] public Vector3 indicatorOffset;
	private PlayerInteractable interactable;
	private PlayerIndicator indicator;

	private void Start()
	{
		indicator = MakeIndicator();
		interactable = GetComponent<PlayerInteractable>();
		interactable.OnPlayerEnters += PlayerEnters;
		interactable.OnPlayerExits += PlayerExits;
		interactable.OnSelected += OnSelected;
		interactable.OnDeselected += OnDeselect;
	}

	private void OnDeselect(Player player)
	{
		indicator.HideIndicator(player);

		Debug.Log("Indicator hide on Deselect: " + interactable.name);
	}

	private void OnSelected(Player player)
	{ 
		indicator.ShowIndicator(player);
		Debug.Log("Indicator show on Select: " + interactable.name);
	}

	private void PlayerExits(Player player)
	{
		if (indicator == null) return;
		Debug.Log("Player Exits " + interactable.name);
		player.RemoveInteractable(interactable);
	}


	private void PlayerEnters(Player player)
	{
		if (indicator == null) return;
		Debug.Log("Player Enters " + interactable.name);
		SetIndicatorColor(player);
		player.AddInteractable(interactable);
	}

	private void SetIndicatorColor(Player player)
	{
		indicator.SetColor(player, player.color);
	}

	private PlayerIndicator MakeIndicator()
	{
		var indicatorObj = Maker.Make(FX.Assets.indicatorPrefab, transform.position+ indicatorOffset);
		indicatorObj.transform.SetParent(transform);
		return indicatorObj.GetComponentInChildren<PlayerIndicator>();
	}
}