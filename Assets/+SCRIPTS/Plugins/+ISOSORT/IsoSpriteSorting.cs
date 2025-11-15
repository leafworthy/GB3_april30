using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace __SCRIPTS.Plugins._ISOSORT
{
	[ExecuteAlways, Serializable]
	public class IsoSpriteSorting : SerializedMonoBehaviour
	{
		public bool Subtract1000SortingOrder;
		public bool isMovable;
		public int sortingOrderOffset;
		public Collider2D sortingBounds;
		public Vector3 SorterPositionOffset1;
		public Vector3 SorterPositionOffset2;

		public enum SortType
		{
			Point,
			Line
		}

		public SortType sortType = SortType.Point;

		[NonSerialized] private int visibleStaticLastRefreshFrame;
		[NonSerialized] public bool registered = false;
		[NonSerialized] public bool forceSort;
		[NonSerialized] public readonly List<IsoSpriteSorting> staticDependencies = new(2048);
		[NonSerialized] public readonly List<IsoSpriteSorting> inverseStaticDependencies = new(2048);
		[NonSerialized] public readonly List<IsoSpriteSorting> movingDependencies = new(2048);
		[NonSerialized] private readonly List<IsoSpriteSorting> visibleStaticDependencies = new(2048);
		public List<IsoSpriteSorting> VisibleMovingDependencies => movingDependencies;
		public List<IsoSpriteSorting> VisibleStaticDependencies
		{
			get
			{
				if (visibleStaticLastRefreshFrame >= Time.frameCount) return visibleStaticDependencies;
				visibleStaticLastRefreshFrame = Time.frameCount;
				IsoSpriteSortingManager.FilterListByVisibility(staticDependencies, visibleStaticDependencies);
				return visibleStaticDependencies;
			}
		}

		public bool VisibleRenderersToSort => _renderersToSort.Where(t => t != null).Any(t => !t.isVisible);
		public int RenderersToSortCount => _renderersToSort.Count;
		public bool HasRenderers => _renderersToSort.Count > 0;

		private List<SpriteRenderer> _renderersToSort = new();
		private Bounds2D _cachedBounds;
		private Transform _transform;
		private int _lastBoundsCalculatedFrame;
		private int _lastPoint1CalculatedFrame;
		private int _lastPoint2CalculatedFrame;
		private Vector2 _cachedPoint1;
		private Vector2 _cachedPoint2;

		public void SetupStaticCache()
		{
			RefreshBounds();
			RefreshCachedPoint1();
			RefreshCachedPoint2();
		}

		public int RendererSortingOrder
		{
			get => _renderersToSort.Count > 0 ? _renderersToSort[0].sortingOrder : 0;
			set
			{
				for (var j = 0; j < _renderersToSort.Count; ++j)
				{
					if (_renderersToSort[j] == null) continue;
					_renderersToSort[j].sortingOrder = value + j + (Subtract1000SortingOrder ? -1000 : 0) + sortingOrderOffset;
				}
			}
		}
		private void RefreshBounds()
		{
			if (_renderersToSort.Count == 0) GetRenderers();
			if (_renderersToSort.Count <= 0) return;
			_cachedBounds = sortingBounds == null ? AttackUtilities.GetCombinedSpriteBounds(transform) : new Bounds2D(sortingBounds.bounds);
		}

		private void SetTransform()
		{
			if (_transform == null) _transform = transform;
		}

		private Vector2 RefreshPoint(Vector2 offset)
		{
			if (!isMovable && Application.isPlaying || Time.frameCount == _lastPoint2CalculatedFrame) return _transform.position;
			_lastPoint2CalculatedFrame = Time.frameCount;
			SetTransform();
			return offset + (Vector2) _transform.position;
		}

		private void RefreshCachedPoint1()
		{
			_cachedPoint1 = RefreshPoint(SorterPositionOffset1);
		}

		private void RefreshCachedPoint2()
		{
			_cachedPoint2 = RefreshPoint(SorterPositionOffset2);
		}

		public Bounds2D TheBounds
		{
			get
			{
				if (!isMovable && Application.isPlaying || Time.frameCount == _lastBoundsCalculatedFrame) return _cachedBounds;
				_lastBoundsCalculatedFrame = Time.frameCount;
				RefreshBounds();
				return _cachedBounds;
			}
		}
		private Vector3 SortingPoint1
		{
			get
			{
				RefreshCachedPoint1();
				return _cachedPoint1;
			}
		}

		private Vector3 SortingPoint2
		{
			get
			{
				RefreshCachedPoint2();
				return _cachedPoint2;
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
			var isoSorters = (IsoSpriteSorting[]) FindObjectsByType(typeof(IsoSpriteSorting), FindObjectsInactive.Exclude, FindObjectsSortMode.None);
			foreach (var t1 in isoSorters)
			{
				t1.Setup();
				t1.forceSort = true; // Force sorting update for all sprites
			}

			IsoSpriteSortingManager.UpdateSorting();
			SceneView.RepaintAll();
#endif
		}

		public void Setup()
		{
			SetTransform(); //This needs to be here AND in the Awake function
			if (_renderersToSort.Count == 0) GetRenderers();
			IsoSpriteSortingManager.RegisterSprite(this);
		}

		private void Awake()
		{
			SetTransform(); //This needs to be here AND in the setup function
		}

		private void OnEnable()
		{
			Setup();
		}

		private void OnDisable()
		{
			Unregister();
		}

		private void OnDestroy()
		{
			Unregister();
		}

		private void Unregister()
		{
			IsoSpriteSortingManager.UnregisterSprite(this);
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			SetTransform();

			// Only proceed if we have a valid transform
			if (_transform != null)
			{
				if (_renderersToSort.Count == 0) GetRenderers();
				forceSort = true;
				IsoSpriteSortingManager.RegisterSprite(this);
			}
		}

		private void OnDrawGizmos()
		{
			if (Application.isPlaying || _transform == null || !(Time.realtimeSinceStartup - lastGizmosUpdateTime > 0.1f)) return;
			lastGizmosUpdateTime = Time.realtimeSinceStartup;
			forceSort = true;
			SetupStaticCache();
		}

		private float lastGizmosUpdateTime;
		private int renderersToSortCount;
#endif

		public void GetRenderers()
		{
			if (HasRenderers) return;

			_renderersToSort.Clear();
			foreach (var spriteRenderer in GetComponentsInChildren<SpriteRenderer>(true))
			{
				if (spriteRenderer.CompareTag("DontSort")) continue;
				if (!_renderersToSort.Contains(spriteRenderer)) _renderersToSort.Add(spriteRenderer);
			}
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
				return oneVStwo == twoVSone ? //the two comparisons agree about the ordering
					oneVStwo : CompareLineCenters(line1, line2);
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


	}
}
