using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace __SCRIPTS.Plugins._ISOSORT
{
	[ExecuteAlways, Serializable]
	public class IsoSpriteSorting : SerializedMonoBehaviour
	{
		public bool isBelowZeroSortingOrder;
		public bool isMovable;
		public bool renderBelowAll;

		[NonSerialized] public bool registered = false;
		[NonSerialized] public bool forceSort;

		public Collider2D sortingBounds;

		[NonSerialized] public readonly List<IsoSpriteSorting> staticDependencies = new(2048);
		[NonSerialized] public readonly List<IsoSpriteSorting> inverseStaticDependencies = new(2048);
		[NonSerialized] public readonly List<IsoSpriteSorting> movingDependencies = new(2048);
		[NonSerialized] private readonly List<IsoSpriteSorting> visibleStaticDependencies = new(2048);

		public int renderBelowSortingOrder;
		private int visibleStaticLastRefreshFrame;

		private static IsoSpriteSorting[] isoSorters = new IsoSpriteSorting[8];

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void ResetStatics()
		{
			isoSorters = new IsoSpriteSorting[8];
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

		public enum SortType
		{
			Point,
			Line
		}

		public SortType sortType = SortType.Point;

		public Vector3 SorterPositionOffset;
		public Vector3 SorterPositionOffset2;
		public List<Renderer> renderersToSort = new();
		public bool renderAboveAll;

		private Bounds2D cachedBounds;
		private int lastBoundsCalculatedFrame;
		private bool hasRenderers;
		private Transform t;

		public void SetupStaticCache()
		{
			RefreshBounds();
			RefreshPoint1();
			RefreshPoint2();
		}

		private void RefreshBounds()
		{
			if (renderersToSort.Count <= 0) return;
			if (sortingBounds != null)
			{
				cachedBounds = new Bounds2D(sortingBounds.bounds);
				return;
			}

			if (renderersToSort[0] == null) GetRenderers();

			if (renderersToSort[0] == null) return;
			cachedBounds = new Bounds2D(renderersToSort[0].bounds);
		}

		private void RefreshPoint1()
		{
			if (t == null)
			{
				if (transform == null) return;
				t = transform;
			}

			if (t != null)
				cachedPoint1 = SorterPositionOffset + t.position;
			else
			{
				// Fallback if transform is still null
				cachedPoint1 = SorterPositionOffset;
			}
		}

		private void RefreshPoint2()
		{
			if (t == null) t = transform;

			if (t != null)
				cachedPoint2 = SorterPositionOffset2 + t.position;
			else
			{
				// Fallback if transform is still null
				cachedPoint2 = SorterPositionOffset2;
			}
		}

		private int lastPoint1CalculatedFrame;
		private Vector2 cachedPoint1;

		private Vector3 SortingPoint1
		{
			get
			{
				if (isMovable || !Application.isPlaying)
				{
					var frameCount = Time.frameCount;
					if (frameCount != lastPoint1CalculatedFrame)
					{
						lastPoint1CalculatedFrame = frameCount;
						RefreshPoint1();
					}
				}

				return cachedPoint1;
			}
		}

		private int lastPoint2CalculatedFrame;
		private Vector2 cachedPoint2;

		private Vector3 SortingPoint2
		{
			get
			{
				if (isMovable || !Application.isPlaying)
				{
					var frameCount = Time.frameCount;
					if (frameCount != lastPoint2CalculatedFrame)
					{
						lastPoint2CalculatedFrame = frameCount;
						RefreshPoint2();
					}
				}

				return cachedPoint2;
			}
		}

		public Vector3 AsPoint
		{
			get
			{
				if (sortType == SortType.Line)
					return (SortingPoint1 + SortingPoint2) / 2;
				return SortingPoint1;
			}
		}

		private float SortingLineCenterHeight
		{
			get
			{
				if (sortType == SortType.Line)
					return (SortingPoint1.y + SortingPoint2.y) / 2;
				return SortingPoint1.y;
			}
		}
#if ENABLE_UNITYEDITOR
		[MenuItem("Tools/Update Sorters")]
#endif
		public static void UpdateSorters()
		{
#if UNITY_EDITOR
			if (Application.isPlaying) return;
			isoSorters = (IsoSpriteSorting[]) FindObjectsByType(typeof(IsoSpriteSorting), FindObjectsInactive.Include, FindObjectsSortMode.None);
			foreach (var t1 in isoSorters)
			{
				t1.Setup();
				t1.forceSort = true; // Force sorting update for all sprites
			}

			IsoSpriteSortingManager.UpdateSorting();
			SceneView.RepaintAll();
#endif
		}

		private void Awake()
		{
			t = transform; //This needs to be here AND in the setup function
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
			t = transform; //This needs to be here AND in the Awake function
			GetRenderers();
			IsoSpriteSortingManager.RegisterSprite(this);
		}
#if UNITY_EDITOR
		private void OnValidate()
		{
			// Make sure transform reference is set
			if (t == null) t = transform;

			// Only proceed if we have a valid transform
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

		private void OnDrawGizmosSelected()
		{
			return;
			// Force refresh in Scene view when object is selected
			if (!Application.isPlaying)
			{
				if (t == null) t = transform;

				forceSort = true;
				RefreshPoint1();
				RefreshPoint2();
				RefreshBounds();

				IsoSpriteSortingManager.UpdateSorting();
			}
		}

		private Vector3 lastPosition;
		private Quaternion lastRotation;
		private Vector3 lastScale;

		private void OnDrawGizmos()
		{
			// Check if transform has changed - limit frequency to prevent editor freezing
			if (!Application.isPlaying && t != null && Time.realtimeSinceStartup - lastGizmosUpdateTime > 0.1f)
			{
				var hasChanged = t.position != lastPosition || t.rotation != lastRotation || t.localScale != lastScale;

				if (hasChanged)
				{
					lastPosition = t.position;
					lastRotation = t.rotation;
					lastScale = t.localScale;

					forceSort = true;
					RefreshPoint1();
					RefreshPoint2();
					RefreshBounds();
					// Don't call UpdateSorting and RepaintAll from OnDrawGizmos as it can cause infinite loops
					lastGizmosUpdateTime = Time.realtimeSinceStartup;
				}
			}
		}

		private float lastGizmosUpdateTime;
#endif

		public void GetRenderers()
		{
			if (hasRenderers && Application.isPlaying) return;
			hasRenderers = true;

			RemoveNulls();
			var tempRenderersToSort = GetComponentsInChildren<SpriteRenderer>(true);
			foreach (var spriteRenderer in tempRenderersToSort)
			{
				if (spriteRenderer.CompareTag("DontSort")) continue;
				if (!renderersToSort.Contains(spriteRenderer)) renderersToSort.Add(spriteRenderer);
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

		public static int CompairIsoSortersBasic(IsoSpriteSorting sprite1, IsoSpriteSorting sprite2)
		{
			var y1 = sprite1.sortType == SortType.Point ? sprite1.SortingPoint1.y : sprite1.SortingLineCenterHeight;
			var y2 = sprite2.sortType == SortType.Point ? sprite2.SortingPoint1.y : sprite2.SortingLineCenterHeight;
			return y2.CompareTo(y1);
		}

		//A result of -1 means sprite1 is above sprite2 in physical space
		public static int CompareIsoSorters(IsoSpriteSorting sprite1, IsoSpriteSorting sprite2)
		{
			if (sprite1.sortType == SortType.Point && sprite2.sortType == SortType.Point)
				return sprite2.SortingPoint1.y.CompareTo(sprite1.SortingPoint1.y);
			if (sprite1.sortType == SortType.Line && sprite2.sortType == SortType.Line)
				return CompareLineAndLine(sprite1, sprite2);
			if (sprite1.sortType == SortType.Point && sprite2.sortType == SortType.Line)
				return ComparePointAndLine(sprite1.SortingPoint1, sprite2);
			if (sprite1.sortType == SortType.Line && sprite2.sortType == SortType.Point)
				return -ComparePointAndLine(sprite2.SortingPoint1, sprite1);
			return 0;
		}

		public static int CompareIsoSortersBelow(IsoSpriteSorting sprite1, IsoSpriteSorting sprite2)
		{
			if (sprite1.renderBelowSortingOrder == sprite2.renderBelowSortingOrder) return 1; //return 0
			return sprite1.renderBelowSortingOrder > sprite2.renderBelowSortingOrder ? 1 : -1;
		}

		private static int CompareLineAndLine(IsoSpriteSorting line1, IsoSpriteSorting line2)
		{
			Vector2 line1Point1 = line1.SortingPoint1;
			Vector2 line1Point2 = line1.SortingPoint2;
			var line1LowPoint = line1Point1.y > line1Point2.y ? line1Point2 : line1Point1;
			var line1HighPoint = line1Point1.y > line1Point2.y ? line1Point1 : line1Point2;
			var line1slantUpward = line1HighPoint.x > line1LowPoint.x;

			Vector2 line2Point1 = line2.SortingPoint1;
			Vector2 line2Point2 = line2.SortingPoint2;
			var line2LowPoint = line2Point1.y > line2Point2.y ? line2Point2 : line2Point1;
			var line2HighPoint = line2Point1.y > line2Point2.y ? line2Point1 : line2Point2;
			var line2slantUpward = line2HighPoint.x > line2LowPoint.x;

			var comp1 = ComparePointAndLine(line1Point1, line2);
			var comp2 = ComparePointAndLine(line1Point2, line2);
			var oneVStwo = int.MinValue;
			if (comp1 == comp2) //Both points in line 1 are above or below line2
				oneVStwo = comp1;

			var comp3 = ComparePointAndLine(line2Point1, line1);
			var comp4 = ComparePointAndLine(line2Point2, line1);
			var twoVSone = int.MinValue;
			if (comp3 == comp4) //Both points in line 2 are above or below line1
				twoVSone = -comp3;

			if (oneVStwo != int.MinValue && twoVSone != int.MinValue)
			{
				if (oneVStwo == twoVSone) //the two comparisons agree about the ordering
					return oneVStwo;
				return CompareLineCenters(line1, line2);
			}

			if (line1slantUpward == line2slantUpward)
			{
				if (line1slantUpward)
					return line1LowPoint.x > line2LowPoint.x ? 1 : -1;
				return line1LowPoint.x > line2LowPoint.x ? -1 : 1;
			}

			if (oneVStwo != int.MinValue)
				return oneVStwo;
			if (twoVSone != int.MinValue)
				return twoVSone;

			return CompareLineCenters(line1, line2);
		}

		private static int CompareLineCenters(IsoSpriteSorting line1, IsoSpriteSorting line2) =>
			-line1.SortingLineCenterHeight.CompareTo(line2.SortingLineCenterHeight);

		private static int ComparePointAndLine(Vector3 point, IsoSpriteSorting line)
		{
			var pointY = point.y;
			if (pointY > line.SortingPoint1.y && pointY > line.SortingPoint2.y)
				return -1;
			if (pointY < line.SortingPoint1.y && pointY < line.SortingPoint2.y)
				return 1;
			var slope = (line.SortingPoint2.y - line.SortingPoint1.y) / (line.SortingPoint2.x - line.SortingPoint1.x);
			var intercept = line.SortingPoint1.y - slope * line.SortingPoint1.x;
			var yOnLineForPoint = slope * point.x + intercept;
			return yOnLineForPoint > point.y ? 1 : -1;
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

					renderersToSort[j].sortingOrder = value + j - (isBelowZeroSortingOrder ? 1000 : 0) + renderBelowSortingOrder;
				}
			}
		}

		public Bounds2D TheBounds
		{
			get
			{
				if (isMovable || !Application.isPlaying)
				{
					var frameCount = Time.frameCount;
					if (frameCount != lastBoundsCalculatedFrame)
					{
						lastBoundsCalculatedFrame = frameCount;
						RefreshBounds();
					}
				}

				return cachedBounds;
			}
		}

		private void OnDestroy()
		{
			if (Application.isPlaying) Unregister();
		}

		private void Unregister()
		{
			IsoSpriteSortingManager.UnregisterSprite(this);
		}
	}
}
