using __SCRIPTS.Cursor;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
    public class OffScreenPointer : MonoBehaviour, INeedPlayer
    {
        public Transform target;
        public RectTransform pointer;
        public Camera cam;
        public float edgeOffset = 30f;
        public CanvasGroup pointerCanvasGroup;
        private Player player;

        void Update()
        {
            if (cam == null) cam = CursorManager.GetCamera();
            if (target == null)
            {
                var interaction = FindFirstObjectByType<ShoppeInteraction>();
                if (interaction != null) target = interaction.transform;
            }
            if (target == null || pointer == null || cam == null) return;

            // Check if target is off screen
            Vector3 viewportPos = cam.WorldToViewportPoint(target.position);
            bool isOffScreen = viewportPos.x < 0 || viewportPos.x > 1 ||
                               viewportPos.y < 0 || viewportPos.y > 1 ||
                               viewportPos.z < 0;

            // Show/hide pointer
            pointerCanvasGroup.alpha = isOffScreen ? 1 : 0;
            if (!isOffScreen) return;

            // Handle behind-camera case by inverting direction
            if (viewportPos.z < 0)
            {
                viewportPos.x = -viewportPos.x;
                viewportPos.y = -viewportPos.y;
            }

            // Calculate screen bounds in canvas space
            RectTransform canvasRect = (RectTransform)pointer.parent;
            float canvasWidth = canvasRect.rect.width;
            float canvasHeight = canvasRect.rect.height;

            // Get direction from screen center to target
            Vector2 screenCenter = new Vector2(canvasWidth/2, canvasHeight/2);
            Vector2 targetPosOnCanvas = new Vector2(
                (viewportPos.x * canvasWidth),
                (viewportPos.y * canvasHeight)
            );

            Vector2 direction = (targetPosOnCanvas - screenCenter).normalized;

            // Calculate intersection with screen rectangle
            float halfWidth = (canvasWidth/2) + edgeOffset;
            float halfHeight = (canvasHeight/2) + edgeOffset;

            // Find where ray intersects rectangle borders
            float absX = Mathf.Abs(direction.x);
            float absY = Mathf.Abs(direction.y);

            float scaleX = halfWidth / (absX > 0.0001f ? absX : 0.0001f);
            float scaleY = halfHeight / (absY > 0.0001f ? absY : 0.0001f);

            // Take the smaller scale to ensure we hit the closest edge
            float scale = Mathf.Min(scaleX, scaleY);

            // Calculate position on screen edge
            Vector2 edgePosition = screenCenter + direction * scale;

            // Set arrow position
            pointer.position = edgePosition;

            // Set arrow rotation
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            pointer.localRotation = Quaternion.Euler(0, 0, angle);
        }

        public void SetPlayer(Player _player)
        {
	         player = _player;
	         player.OnPlayerDies += Player_OnPlayerDies;

        }

        private void Player_OnPlayerDies(Player obj)
        {
	        enabled = false;

	        pointer?.gameObject.SetActive(false);

	        player.OnPlayerDies -= Player_OnPlayerDies;
        }

        private void OnDestroy()
        {
	        if (player != null)
		        player.OnPlayerDies -= Player_OnPlayerDies;
        }
    }
}
