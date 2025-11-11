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
		private float marginPercent = .2f;

		private void Update()
		{
			if (cam == null) cam = CursorManager.GetCamera();
			if (target == null)
			{
				var interaction = FindFirstObjectByType<ShoppeInteraction>();
				if (interaction != null) target = interaction.transform;
			}

			if (target == null || pointer == null || cam == null) return;

			var viewportPos = cam.WorldToViewportPoint(target.position);
			float leftMargin = marginPercent;
			var rightMargin = 1f - marginPercent;
			float bottomMargin = marginPercent;
			var topMargin = 1f - marginPercent;

			var isOffScreen = viewportPos.x < leftMargin || viewportPos.x > rightMargin || viewportPos.y < bottomMargin || viewportPos.y > topMargin;

			// Show/hide pointer
			pointerCanvasGroup.alpha = isOffScreen ? 1 : 0;
			if (!isOffScreen) return;

			// Calculate screen bounds in canvas space
			var canvasRect = (RectTransform) pointer.parent;
			var canvasWidth = canvasRect.rect.width;
			var canvasHeight = canvasRect.rect.height;

			// Get direction from screen center to target
			var screenCenter = new Vector2(canvasWidth / 2, canvasHeight / 2);
			var targetPosOnCanvas = new Vector2(viewportPos.x * canvasWidth, viewportPos.y * canvasHeight);

			var direction = (targetPosOnCanvas - screenCenter).normalized;

			// Calculate intersection with screen rectangle
			var halfWidth = canvasWidth / 2 + edgeOffset;
			var halfHeight = canvasHeight / 2 + edgeOffset;

			// Find where ray intersects rectangle borders
			var absX = Mathf.Abs(direction.x);
			var absY = Mathf.Abs(direction.y);

			var scaleX = halfWidth / (absX > 0.0001f ? absX : 0.0001f);
			var scaleY = halfHeight / (absY > 0.0001f ? absY : 0.0001f);

			// Take the smaller scale to ensure we hit the closest edge
			var scale = Mathf.Min(scaleX, scaleY);

			// Calculate position on screen edge
			var edgePosition = screenCenter + direction * scale;

			// Set arrow position
			pointer.position = edgePosition;

			// Set arrow rotation
			var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
			pointer.localRotation = Quaternion.Euler(0, 0, angle);
		}

		public void SetPlayer(Player newPlayer)
		{
			player = newPlayer;
			player.OnPlayerDies += Player_OnPlayerDies;
		}

		private void Player_OnPlayerDies(Player obj, bool b)
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
