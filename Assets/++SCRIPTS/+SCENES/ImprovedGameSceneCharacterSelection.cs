using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace __SCRIPTS
{
    /// <summary>
    /// Improved character selection system using simplified UI navigation
    /// Eliminates timing issues and provides reliable left/right navigation
    /// </summary>
    public class ImprovedGameSceneCharacterSelection : GameScene
    {
        [SerializeField] private List<CharacterButton> Buttons = new();
        [SerializeField] private HideRevealObjects titlePressStart;

        private bool playersAllChosen;
        private bool isListening;
        private List<Player> playersBeingListenedTo = new();

        // Events
        public event Action OnSelectCharacter;
        public event Action OnDeselectCharacter;
        public event Action OnPlayerMoveLeft;
        public event Action OnPlayerMoveRight;
        public event Action OnPlayerUnjoins;
        public event Action OnPlayerStartsSelecting;
        public event Action OnTryToStartGame;

        protected void OnEnable()
        {
            CleanUp();
            Players.I.OnPlayerJoins += PlayerStartsSelecting;

            // Handle players that joined before this scene was active
            foreach (var player in Players.I.AllJoinedPlayers)
            {
                PlayerStartsSelectingFromMainMenu(player);
            }

            foreach (var button in Buttons)
            {
                button.SetPlayerColors();
            }

            isActive = true;
        }

        private void OnDisable()
        {
            Players.I.OnPlayerJoins -= PlayerStartsSelecting;
            CleanUp();
        }

        private void CleanUp()
        {
            // Stop listening to all players
            var tempList = playersBeingListenedTo.ToList();
            foreach (var player in tempList)
            {
                StopListeningToPlayer(player);
            }

            foreach (var button in Buttons)
            {
                button.CleanUp();
            }

            HideGoGoGo();
            playersAllChosen = false;
            playersBeingListenedTo.Clear();
        }

        private void PlayerStartsSelectingFromMainMenu(Player player)
        {
            if (playersBeingListenedTo.Contains(player)) return;

            SetupPlayerForSelection(player);
            HideGoGoGo();
        }

        private void PlayerStartsSelecting(Player player)
        {
            if (playersBeingListenedTo.Contains(player)) return;

            SetupPlayerForSelection(player);
            OnPlayerStartsSelecting?.Invoke();
            HideGoGoGo();
        }

        private void SetupPlayerForSelection(Player player)
        {
            player.SetState(Player.State.SelectingCharacter);
            player.CurrentButton = Buttons[0];
            player.CurrentButton.HighlightButton(player);
            ListenToPlayer(player);
        }

        // Store unjoined player delegates separately
        private Dictionary<Player, System.Action> unjoinedPlayerDelegates = new Dictionary<Player, System.Action>();

        private void PlayerUnjoins(Player player)
        {
            if (!playersBeingListenedTo.Contains(player)) return;

            player.SetState(Player.State.Unjoined);
            player.CurrentButton.UnHighlightButton(player);
            player.CurrentButton = null;
            OnPlayerUnjoins?.Invoke();
            StopListeningToPlayer(player);

            // Set up rejoining capability with properly stored delegate
            if (player.GetComponent<ImprovedPlayerController>() != null)
            {
                var improvedController = player.GetComponent<ImprovedPlayerController>();
              //  var rejoinDelegate = () => OnUnjoinedPlayerPressSelect(player);
               // unjoinedPlayerDelegates[player] = rejoinDelegate;
               // improvedController.UINavigation.OnSelect += rejoinDelegate;
            }

            Players.I.AllJoinedPlayers.Remove(player);
        }

        private void OnUnjoinedPlayerPressSelect(Player player)
        {
            // Remove the temporary event subscription using stored delegate
            if (player.GetComponent<ImprovedPlayerController>() != null && unjoinedPlayerDelegates.TryGetValue(player, out var storedDelegate))
            {
                var improvedController = player.GetComponent<ImprovedPlayerController>();
                improvedController.UINavigation.OnSelect -= storedDelegate;
                unjoinedPlayerDelegates.Remove(player);
            }

            // Add player back to AllJoinedPlayers when rejoining
            if (!Players.I.AllJoinedPlayers.Contains(player))
            {
                Players.I.AllJoinedPlayers.Add(player);
            }

            PlayerStartsSelecting(player);
        }

        // Store delegate references for proper unsubscription
        private Dictionary<Player, PlayerEventHandlers> playerEventHandlers = new Dictionary<Player, PlayerEventHandlers>();

        private class PlayerEventHandlers
        {
            public System.Action OnSelectHandler;
            public System.Action OnCancelHandler;
            public System.Action OnLeftHandler;
            public System.Action OnRightHandler;
            public System.Action<NewControlButton> OnSelectHandlerOld;
            public System.Action<NewControlButton> OnCancelHandlerOld;
            public System.Action<IControlAxis> OnLeftHandlerOld;
            public System.Action<IControlAxis> OnRightHandlerOld;
        }

        private void ListenToPlayer(Player player)
        {
            // Create event handlers for this player
            var handlers = new PlayerEventHandlers();
            handlers.OnSelectHandler = () => PlayerPressSelect(player);
            handlers.OnCancelHandler = () => PlayerPressCancel(player);
            handlers.OnLeftHandler = () => PlayerMoveLeft(player);
            handlers.OnRightHandler = () => PlayerMoveRight(player);
            handlers.OnSelectHandlerOld = (btn) => PlayerPressSelect(player);
            handlers.OnCancelHandlerOld = (btn) => PlayerPressCancel(player);
            handlers.OnLeftHandlerOld = (axis) => PlayerMoveLeft(player);
            handlers.OnRightHandlerOld = (axis) => PlayerMoveRight(player);

            // Store handlers for later unsubscription
            playerEventHandlers[player] = handlers;

            // Check if player has improved controller
            var improvedController = player.GetComponent<ImprovedPlayerController>();
            if (improvedController != null && improvedController.UINavigation != null)
            {
                // Use improved navigation system
                improvedController.UINavigation.OnSelect += handlers.OnSelectHandler;
                improvedController.UINavigation.OnCancel += handlers.OnCancelHandler;
                improvedController.UINavigation.OnNavigateLeft += handlers.OnLeftHandler;
                improvedController.UINavigation.OnNavigateRight += handlers.OnRightHandler;
            }
            else
            {
                // Fallback to old system if needed
                var oldController = player.GetComponent<PlayerController>();
                if (oldController != null)
                {
                    oldController.Select.OnPress += handlers.OnSelectHandlerOld;
                    oldController.Cancel.OnPress += handlers.OnCancelHandlerOld;
                    oldController.UIAxis.OnLeft += handlers.OnLeftHandlerOld;
                    oldController.UIAxis.OnRight += handlers.OnRightHandlerOld;
                }
            }

            playersBeingListenedTo.Add(player);
        }

        private void StopListeningToPlayer(Player player)
        {
            if (!playerEventHandlers.TryGetValue(player, out var handlers))
            {
                Debug.LogWarning($"No event handlers found for player {player.playerIndex}");
                playersBeingListenedTo.Remove(player);
                return;
            }

            // Check if player has improved controller
            var improvedController = player.GetComponent<ImprovedPlayerController>();
            if (improvedController != null && improvedController.UINavigation != null)
            {
                // Remove improved navigation subscriptions using stored handlers
                improvedController.UINavigation.OnSelect -= handlers.OnSelectHandler;
                improvedController.UINavigation.OnCancel -= handlers.OnCancelHandler;
                improvedController.UINavigation.OnNavigateLeft -= handlers.OnLeftHandler;
                improvedController.UINavigation.OnNavigateRight -= handlers.OnRightHandler;
            }
            else
            {
                // Fallback to old system cleanup using stored handlers
                var oldController = player.GetComponent<PlayerController>();
                if (oldController != null)
                {
                    oldController.Select.OnPress -= handlers.OnSelectHandlerOld;
                    oldController.Cancel.OnPress -= handlers.OnCancelHandlerOld;
                    oldController.UIAxis.OnLeft -= handlers.OnLeftHandlerOld;
                    oldController.UIAxis.OnRight -= handlers.OnRightHandlerOld;
                }
            }

            // Clean up stored handlers
            playerEventHandlers.Remove(player);
            playersBeingListenedTo.Remove(player);
        }

        private void PlayerMoveLeft(Player player)
        {
            if (!isActive || player.state != Player.State.SelectingCharacter) return;

            OnPlayerMoveLeft?.Invoke();
            MovePlayerSelection(player, false);
        }

        private void PlayerMoveRight(Player player)
        {
            if (!isActive || player.state != Player.State.SelectingCharacter) return;

            OnPlayerMoveRight?.Invoke();
            MovePlayerSelection(player, true);
        }

        private void MovePlayerSelection(Player player, bool toRight)
        {
            player.CurrentButton.UnHighlightButton(player);

            if (toRight)
            {
                player.buttonIndex = (player.buttonIndex + 1) % Buttons.Count;
            }
            else
            {
                player.buttonIndex = (player.buttonIndex - 1 + Buttons.Count) % Buttons.Count;
            }

            player.CurrentButton = Buttons[player.buttonIndex];
            player.CurrentButton.HighlightButton(player);

            // Play sound effect
            SFX.I.sounds.charSelect_move_sounds.PlayRandom();
        }

        private void PlayerPressSelect(Player player)
        {
            if (!isActive) return;

            switch (player.state)
            {
                case Player.State.Selected:
                    TryToStartGame(player);
                    break;
                case Player.State.SelectingCharacter:
                    SelectCharacter(player);
                    break;
                case Player.State.Unjoined:
                    PlayerStartsSelecting(player);
                    break;
            }
        }

        private void PlayerPressCancel(Player player)
        {
            switch (player.state)
            {
                case Player.State.Selected:
                    DeselectCharacter(player);
                    break;
                case Player.State.SelectingCharacter:
                    PlayerUnjoins(player);
                    break;
            }
        }

        private void SelectCharacter(Player player)
        {
            OnSelectCharacter?.Invoke();
            player.CurrentButton.SelectCharacter(player);
            player.CurrentCharacter = player.CurrentButton.character;
            player.SetState(Player.State.Selected);
            CheckIfPlayersAllSelected();

            SFX.I.sounds.charSelect_select_sounds.PlayRandom();
        }

        private void DeselectCharacter(Player player)
        {
            OnDeselectCharacter?.Invoke();
            player.CurrentButton.DeselectCharacter(player);
            player.CurrentCharacter = player.CurrentButton.character;
            player.SetState(Player.State.SelectingCharacter);
            CheckIfPlayersAllSelected();

            SFX.I.sounds.charSelect_deselect_sounds.PlayRandom();
        }

        private void TryToStartGame(Player player)
        {
            CheckIfPlayersAllSelected();
            if (!playersAllChosen) return;

            OnTryToStartGame?.Invoke();
            titlePressStart.gameObject.SetActive(false);
            CleanUp();
            isActive = false;
            Players.I.OnPlayerJoins -= PlayerStartsSelecting;
            LevelManager.I.StartGame();
        }

        private void CheckIfPlayersAllSelected()
        {
            var playersStillSelecting = Players.I.AllJoinedPlayers
                .Where(t => t.state == Player.State.SelectingCharacter).ToList();

            if (playersStillSelecting.Count > 0)
            {
                HideGoGoGo();
                return;
            }

            ShowGoGoGo();
        }

        private void ShowGoGoGo()
        {
            titlePressStart.Set(1);
            playersAllChosen = true;
        }

        private void HideGoGoGo()
        {
            playersAllChosen = false;
            titlePressStart.Set(0);
        }
    }
}
