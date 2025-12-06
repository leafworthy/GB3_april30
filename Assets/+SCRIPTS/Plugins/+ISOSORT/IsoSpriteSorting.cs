using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace __SCRIPTS.Plugins._ISOSORT
{
	[ExecuteInEditMode, Serializable]
	public class IsoSpriteSorting : SerializedMonoBehaviour
	{
		private const float MATH_EPSILON = 0.0001f;

		// Configuration
		public bool isMovable;
		public bool renderBelowAll;
		public bool renderAboveAll;
		public int renderBelowSortingOrder;
		public Collider2D sortingBounds;

		public enum SortType { Point, Line }
		public SortType sortType = SortType.Point;

		public Vector3 SorterPositionOffset;
		public Vector3 SorterPositionOffset2;
		public List<Renderer> renderersToSort = new();

		// Runtime state
		[NonSerialized] public bool registered = false;
		[NonSerialized] public bool forceSort;
		[NonSerialized] public int cachedInstanceID;

		// Dependency lists for topological sorting
		[NonSerialized] public readonly List<IsoSpriteSorting> staticDependencies = new(1080);
		[NonSerialized] public readonly List<IsoSpriteSorting> inverseStaticDependencies = new(1080);
		[NonSerialized] public readonly List<IsoSpriteSorting> movingDependencies = new(1080);
		[NonSerialized] private readonly List<IsoSpriteSorting> visibleStaticDependencies = new(1080);

		// Cache
		private Vector3 cachedPoint1;
		private Vector3 cachedPoint2;
		private float cachedSortingHeight;
		private Bounds2D cachedBounds;
		private int lastFrameUpdated = -1;
		private int visibleStaticLastRefreshFrame = -1;
		private bool hasRenderers;
		private Transform t;

		private static IsoSpriteSorting[] isoSorters = new IsoSpriteSorting[300];

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void ResetStatics()
		{
			isoSorters = new IsoSpriteSorting[300];
		}

		public List<IsoSpriteSorting> VisibleStaticDependencies
		{
			get
			{
				if (visibleStaticLastRefreshFrame < Time.frameCount)
				{
					IsoSpriteSortingManager.FilterListByVisibility(staticDependencies, visibleStaticDependencies);
					visibleStaticLastRefreshFrame = Time.frameCount;
				}
				return visibleStaticDependencies;
			}
		}

		public List<IsoSpriteSorting> VisibleMovingDependencies => movingDependencies;

		public static void UpdateSorters()
		{
#if UNITY_EDITOR
			if (Application.isPlaying) return;
			isoSorters = (IsoSpriteSorting[])FindObjectsByType(typeof(IsoSpriteSorting), FindObjectsInactive.Include, FindObjectsSortMode.None);
			foreach (var sorter in isoSorters)
			{
				sorter.Setup();
				sorter.forceSort = true;
			}
			IsoSpriteSortingManager.UpdateSorting();
			SceneView.RepaintAll();
#endif
		}

		private void Awake()
		{
			t = transform;
			cachedInstanceID = GetInstanceID();
		}

		private void OnEnable()
		{
			Setup();
		}

		private void OnDisable()
		{
			Unregister();
		}

		public void Setup()
		{
			t = transform;
			cachedInstanceID = GetInstanceID();
			GetRenderers();
			EnsureCache(force: true);
			IsoSpriteSortingManager.RegisterSprite(this);
		}

		public void SetupStaticCache()
		{
			EnsureCache(force: true);
		}

		/// <summary>
		/// Optimized cache update: Static objects cache once, movable per-frame, editor always updates
		/// FIXED: Now uses TransformPoint to properly handle scale and rotation
		/// </summary>
		public void EnsureCache(bool force = false)
		{
			// Static objects in play mode: cache once and never update
			if (!force && Application.isPlaying && !isMovable && lastFrameUpdated > 0)
				return;

			// Already updated this frame in play mode: skip
			if (!force && Application.isPlaying && Time.frameCount == lastFrameUpdated)
				return;

			// In editor mode, always update to support live editing
			lastFrameUpdated = Time.frameCount;

			if (t == null)
			{
				t = transform;
				if (t == null)
				{
					cachedPoint1 = SorterPositionOffset;
					cachedPoint2 = SorterPositionOffset2;
					cachedSortingHeight = SorterPositionOffset.y;
					Debug.LogWarning($"IsoSpriteSorting: Transform is null on {gameObject?.name ?? "unknown"}", this);
					return;
				}
			}

			// FIXED: Use TransformPoint instead of simple addition to handle scale and rotation
			cachedPoint1 = t.TransformPoint(SorterPositionOffset);

			if (sortType == SortType.Line)
			{
				cachedPoint2 = t.TransformPoint(SorterPositionOffset2);
				cachedSortingHeight = (cachedPoint1.y + cachedPoint2.y) * 0.5f;
			}
			else
			{
				cachedPoint2 = cachedPoint1;
				cachedSortingHeight = cachedPoint1.y;
			}

			// Update bounds
			if (sortingBounds != null)
			{
				cachedBounds = new Bounds2D(sortingBounds.bounds);
			}
			else if (renderersToSort.Count > 0 && renderersToSort[0] != null)
			{
				cachedBounds = new Bounds2D(renderersToSort[0].bounds);
			}
			else
			{
				cachedBounds = new Bounds2D(new Bounds(cachedPoint1, Vector3.one));
			}
		}

		public void GetRenderers()
		{
			if (hasRenderers) return;
			hasRenderers = true;

			RemoveNulls();
			var tempRenderersToSort = GetComponentsInChildren<SpriteRenderer>(true);
			foreach (var spriteRenderer in tempRenderersToSort)
			{
				if (spriteRenderer.CompareTag("DontSort")) continue;
				if (!renderersToSort.Contains(spriteRenderer))
					renderersToSort.Add(spriteRenderer);
			}
		}

		public void RemoveNulls()
		{
			for (var i = renderersToSort.Count - 1; i >= 0; i--)
			{
				if (renderersToSort[i] == null)
					renderersToSort.RemoveAt(i);
			}
		}

		// ----------------------------------------------------------------------
		//  COMPARISON METHODS
		// ----------------------------------------------------------------------

		public static int CompairIsoSortersBasic(IsoSpriteSorting sprite1, IsoSpriteSorting sprite2)
		{
			sprite1.EnsureCache();
			sprite2.EnsureCache();
			return sprite2.cachedSortingHeight.CompareTo(sprite1.cachedSortingHeight);
		}

		/// <summary>
		/// Main comparison function. Returns -1 if sprite1 is behind sprite2 (renders first).
		/// </summary>
		public static int CompareIsoSorters(IsoSpriteSorting s1, IsoSpriteSorting s2)
		{
			// Ensure both caches are up to date
			s1.EnsureCache();
			s2.EnsureCache();

			// Cache locals for performance
			Vector3 s1p1 = s1.cachedPoint1;
			Vector3 s1p2 = s1.cachedPoint2;
			Vector3 s2p1 = s2.cachedPoint1;
			Vector3 s2p2 = s2.cachedPoint2;

			bool s1IsLine = s1.sortType == SortType.Line;
			bool s2IsLine = s2.sortType == SortType.Line;

			// Point vs Point
			if (!s1IsLine && !s2IsLine)
			{
				if (Mathf.Abs(s1p1.y - s2p1.y) > MATH_EPSILON)
					return s2p1.y.CompareTo(s1p1.y); // Higher Y = further back = renders first

				return ResolveTie(s1, s2);
			}

			// Line vs Line
			if (s1IsLine && s2IsLine)
			{
				return CompareLineAndLine(
					s1p1, s1p2, s1.cachedSortingHeight, s1,
					s2p1, s2p2, s2.cachedSortingHeight, s2
				);
			}

			// Mixed: Point vs Line
			if (!s1IsLine) // s1 is Point, s2 is Line
				return ComparePointAndLine(s1p1, s2p1, s2p2, s1, s2);

			// s1 is Line, s2 is Point (inverted)
			return -ComparePointAndLine(s2p1, s1p1, s1p2, s2, s1);
		}

		public static int CompareIsoSortersBelow(IsoSpriteSorting sprite1, IsoSpriteSorting sprite2)
		{
			if (sprite1.renderBelowSortingOrder == sprite2.renderBelowSortingOrder)
				return 0;
			return sprite1.renderBelowSortingOrder > sprite2.renderBelowSortingOrder ? 1 : -1;
		}

		/// <summary>
		/// Compare a point against a line segment.
		/// Returns -1 if point is behind line (renders first), 1 if in front (renders after).
		/// For long lines, we compare the point's Y against the line's Y at the point's X position.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int ComparePointAndLine(Vector3 P, Vector3 A, Vector3 B, IsoSpriteSorting sP, IsoSpriteSorting sLine)
		{
			// Get line's X bounds
			float lineMinX = Mathf.Min(A.x, B.x);
			float lineMaxX = Mathf.Max(A.x, B.x);
			float lineWidth = lineMaxX - lineMinX;

			// If line is vertical (or nearly vertical), use its average Y
			if (lineWidth < MATH_EPSILON)
			{
				float lineAvgY = (A.y + B.y) * 0.5f;
				float yDiffVertical = P.y - lineAvgY;

				if (Mathf.Abs(yDiffVertical) > MATH_EPSILON)
					return yDiffVertical > 0 ? -1 : 1;

				return ResolveTie(sP, sLine);
			}

			// For non-vertical lines: find the Y value of the line at the point's X position
			float yOnLine;

			// If point's X is within the line's X range, interpolate
			if (P.x >= lineMinX - MATH_EPSILON && P.x <= lineMaxX + MATH_EPSILON)
			{
				yOnLine = GetYAtXSafe(A, B, P.x);
			}
			else
			{
				// Point is outside line's X range - use the nearest endpoint
				if (P.x < lineMinX)
				{
					// Use the leftmost point's Y
					yOnLine = A.x < B.x ? A.y : B.y;
				}
				else
				{
					// Use the rightmost point's Y
					yOnLine = A.x > B.x ? A.y : B.y;
				}
			}

			// Compare point's Y against the line's Y at this X position
			float yDiff = P.y - yOnLine;

			if (Mathf.Abs(yDiff) > MATH_EPSILON)
			{
				// Point has higher Y = behind line = renders first
				return yDiff > 0 ? -1 : 1;
			}

			// Y values are the same - use cross product as tiebreaker
			float cross = (B.x - A.x) * (P.y - A.y) - (B.y - A.y) * (P.x - A.x);

			if (Mathf.Abs(cross) < MATH_EPSILON)
				return ResolveTie(sP, sLine);

			return cross < 0 ? -1 : 1;
		}

		/// <summary>
		/// Compare two line segments. Handles vertical lines and X-overlap cases robustly.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int CompareLineAndLine(
			Vector3 a1, Vector3 a2, float aHeight, IsoSpriteSorting sA,
			Vector3 b1, Vector3 b2, float bHeight, IsoSpriteSorting sB)
		{
			// Calculate X bounds for both lines (used in multiple cases)
			float aMinX = Mathf.Min(a1.x, a2.x);
			float aMaxX = Mathf.Max(a1.x, a2.x);
			float bMinX = Mathf.Min(b1.x, b2.x);
			float bMaxX = Mathf.Max(b1.x, b2.x);

			float widthA = aMaxX - aMinX;
			float widthB = bMaxX - bMinX;

			bool aIsVertical = widthA < MATH_EPSILON;
			bool bIsVertical = widthB < MATH_EPSILON;

			// CASE 1: Both vertical - compare by average Y
			if (aIsVertical && bIsVertical)
			{
				if (Mathf.Abs(aHeight - bHeight) > MATH_EPSILON)
					return bHeight.CompareTo(aHeight);
				return ResolveTie(sA, sB);
			}

			// CASE 2: Only A is vertical
			if (aIsVertical)
			{
				// Check if A's X is within B's X range
				if (a1.x < bMinX - MATH_EPSILON || a1.x > bMaxX + MATH_EPSILON)
				{
					// A is outside B's X range - use average heights
					if (Mathf.Abs(aHeight - bHeight) > MATH_EPSILON)
						return bHeight.CompareTo(aHeight);
					return ResolveTie(sA, sB);
				}

				// A's X is within B's range - compare A's center Y against B's Y at A's X
				float aMinY = Mathf.Min(a1.y, a2.y);
				float aMaxY = Mathf.Max(a1.y, a2.y);
				float aCenterY = (aMinY + aMaxY) * 0.5f;
				float yOnB = GetYAtXSafe(b1, b2, a1.x);

				if (Mathf.Abs(aCenterY - yOnB) < MATH_EPSILON)
					return ResolveTie(sA, sB);

				return aCenterY > yOnB ? -1 : 1;
			}

			// CASE 3: Only B is vertical
			if (bIsVertical)
			{
				// Check if B's X is within A's X range
				if (b1.x < aMinX - MATH_EPSILON || b1.x > aMaxX + MATH_EPSILON)
				{
					// B is outside A's X range - use average heights
					if (Mathf.Abs(aHeight - bHeight) > MATH_EPSILON)
						return bHeight.CompareTo(aHeight);
					return ResolveTie(sA, sB);
				}

				// B's X is within A's range - compare B's center Y against A's Y at B's X
				float bMinY = Mathf.Min(b1.y, b2.y);
				float bMaxY = Mathf.Max(b1.y, b2.y);
				float bCenterY = (bMinY + bMaxY) * 0.5f;
				float yOnA = GetYAtXSafe(a1, a2, b1.x);

				if (Mathf.Abs(yOnA - bCenterY) < MATH_EPSILON)
					return ResolveTie(sA, sB);

				return yOnA > bCenterY ? -1 : 1;
			}

			// CASE 4: Both lines are slanted - use X-overlap method
			float overlapMin = Mathf.Max(aMinX, bMinX);
			float overlapMax = Mathf.Min(aMaxX, bMaxX);
			float overlapWidth = overlapMax - overlapMin;

			// No X-overlap: use average height
			if (overlapWidth <= MATH_EPSILON)
			{
				if (Mathf.Abs(aHeight - bHeight) > MATH_EPSILON)
					return bHeight.CompareTo(aHeight);
				return ResolveTie(sA, sB);
			}

			// Compare Y at the center of overlap
			float midX = overlapMin + overlapWidth * 0.5f;
			float yA = GetYAtXSafe(a1, a2, midX);
			float yB = GetYAtXSafe(b1, b2, midX);

			if (Mathf.Abs(yA - yB) < MATH_EPSILON)
				return ResolveTie(sA, sB);

			// Higher Y at overlap = further back = renders first
			return yA > yB ? -1 : 1;
		}

		/// <summary>
		/// Get Y coordinate of a line at given X. Safe version handles near-vertical lines.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static float GetYAtXSafe(Vector3 p1, Vector3 p2, float x)
		{
			float run = p2.x - p1.x;

			// Vertical or near-vertical: return average Y
			if (Mathf.Abs(run) < MATH_EPSILON)
				return (p1.y + p2.y) * 0.5f;

			// Linear interpolation
			float t = (x - p1.x) / run;
			return p1.y + (p2.y - p1.y) * t;
		}

		/// <summary>
		/// Deterministic tie-breaker using instance ID to prevent flickering.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int ResolveTie(IsoSpriteSorting s1, IsoSpriteSorting s2)
		{
			if (s1.cachedInstanceID == s2.cachedInstanceID) return 0;
			return s1.cachedInstanceID < s2.cachedInstanceID ? -1 : 1;
		}

		// ----------------------------------------------------------------------
		//  PROPERTIES
		// ----------------------------------------------------------------------

		public Vector3 AsPoint
		{
			get
			{
				EnsureCache();
				return sortType == SortType.Line
					? (cachedPoint1 + cachedPoint2) * 0.5f
					: cachedPoint1;
			}
		}

		public int RendererSortingOrder
		{
			get
			{
				if (renderersToSort.Count > 0)
					return renderersToSort[0].sortingOrder;
				return 0;
			}
			set
			{
				for (var j = 0; j < renderersToSort.Count; ++j)
				{
					if (renderersToSort[j] == null) continue;
					renderersToSort[j].sortingOrder = value + j;
				}
			}
		}

		public Bounds2D TheBounds
		{
			get
			{
				EnsureCache();
				return cachedBounds;
			}
		}

		// ----------------------------------------------------------------------
		//  LIFECYCLE
		// ----------------------------------------------------------------------

		private void OnDestroy()
		{
			if (Application.isPlaying) Unregister();
		}

		private void Unregister()
		{
			IsoSpriteSortingManager.UnregisterSprite(this);
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			if (t == null) t = transform;

			if (t != null)
			{
				GetRenderers();
				forceSort = true;
				IsoSpriteSortingManager.RegisterSprite(this);
			}
		}

		private void OnTransformChildrenChanged()
		{
			if (t == null) t = transform;
			GetRenderers();
			forceSort = true;
		}

		private Vector3 lastPosition;
		private Quaternion lastRotation;
		private Vector3 lastScale;
		private float lastGizmosUpdateTime;

		private void OnDrawGizmosSelected()
		{
			if (!Application.isPlaying)
			{
				if (t == null) t = transform;
				forceSort = true;
				EnsureCache(force: true);
				IsoSpriteSortingManager.UpdateSorting();
			}
		}

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying && t != null && Time.realtimeSinceStartup - lastGizmosUpdateTime > 0.1f)
			{
				var hasChanged = t.position != lastPosition || t.rotation != lastRotation || t.localScale != lastScale;

				if (hasChanged)
				{
					lastPosition = t.position;
					lastRotation = t.rotation;
					lastScale = t.localScale;

					forceSort = true;
					EnsureCache(force: true);
					lastGizmosUpdateTime = Time.realtimeSinceStartup;
				}
			}
		}
#endif
	}
}
