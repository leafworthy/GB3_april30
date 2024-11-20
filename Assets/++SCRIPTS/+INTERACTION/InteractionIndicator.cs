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
		interactable = GetComponentInChildren<PlayerInteractable>(true);
		interactable.OnPlayerEnters += PlayerEnters;
		interactable.OnPlayerExits += PlayerExits;
		interactable.OnSelected += OnSelected;
		interactable.OnDeselected += OnDeselect;
	}

	private void OnDeselect(Player player)
	{
		indicator.HideIndicator(player);

	}

	private void OnSelected(Player player)
	{ 
		indicator.ShowIndicator(player);
	}

	private void PlayerExits(Player player)
	{
		if (indicator == null) return;
		player.RemoveInteractable(interactable);
	}


	private void PlayerEnters(Player player)
	{
		if (indicator == null) return;
		if(player == null) return;
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