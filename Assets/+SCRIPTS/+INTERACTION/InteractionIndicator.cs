using UnityEngine;
using UnityEngine.Serialization;

namespace __SCRIPTS
{
	public class InteractionIndicator : ServiceUser
	{
		private static readonly int HitTrigger = Animator.StringToHash("HitTrigger");
		private static readonly int IsFinished = Animator.StringToHash("IsFinished");
		private bool isFinished;
		[FormerlySerializedAs("interactionOffset")] public Vector3 indicatorOffset;
		private PlayerInteractable interactable;
		private PlayerIndicator indicator;
		public Vector3 interactionPoint;
		private Animator animator;

		private void OnEnable()
		{

			interactable = GetComponentInChildren<PlayerInteractable>(true);
			interactable.OnPlayerEnters += PlayerEnters;
			interactable.OnPlayerExits += PlayerExits;
			interactable.OnSelected += OnSelected;
			interactable.OnDeselected += OnDeselect;
			interactable.OnActionPress += OnActionPress;
			interactable.OnPlayerFinishes += OnPlayerFinishes;
		}

		private void Start()
		{
			indicator = MakeIndicator();
			animator = indicator.GetComponentInChildren<Animator>();
		}

		private void OnDisable()
		{
			if(interactable == null) return;
			interactable.OnPlayerEnters -= PlayerEnters;
			interactable.OnPlayerExits -= PlayerExits;
			interactable.OnSelected -= OnSelected;
			interactable.OnDeselected -= OnDeselect;
			interactable.OnActionPress -= OnActionPress;
			interactable.OnPlayerFinishes -= OnPlayerFinishes;
		}

		private void OnPlayerFinishes(Player player)
		{
			isFinished = true;
			animator?.SetBool(IsFinished, true);
			PlayerExits(player);
		}

		private void OnActionPress(Player player)
		{
			if(isFinished) return;
			animator?.SetTrigger(HitTrigger);
		}

		private void OnDeselect(Player player)
		{
			if (isFinished) return;
			indicator.HideIndicator();

		}

		private void OnSelected(Player player)
		{
			if (isFinished) return;
			indicator.ShowIndicator();
		}

		private void PlayerExits(Player player)
		{
			if (indicator == null) return;
			var selector = player.SpawnedPlayerGO.GetComponent<InteractableSelector>();
			selector.RemoveInteractable(interactable);
		}


		private void PlayerEnters(Player player)
		{
			if (isFinished) return;
			if (indicator == null) return;
			if(player == null) return;
			SetIndicatorColor(player);
			var selector = player.SpawnedPlayerGO.GetComponent<InteractableSelector>();
			selector.RemoveInteractable(interactable);
			selector.AddInteractable(interactable);
		}

		private void SetIndicatorColor(Player player)
		{
			indicator.SetColor(player.playerColor);

		}

		private PlayerIndicator MakeIndicator()
		{
			var indicatorObj = objectMaker.Make( assets.FX.indicatorPrefab, transform.position+ indicatorOffset);
			indicatorObj.transform.SetParent(transform);
			return indicatorObj.GetComponentInChildren<PlayerIndicator>();
		}
	}
}
