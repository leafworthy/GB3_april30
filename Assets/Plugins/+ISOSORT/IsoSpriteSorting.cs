using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[ExecuteInEditMode]
public class IsoSpriteSorting : MonoBehaviour
{
	public bool isMovable;
	public bool renderBelowAll;

	[NonSerialized] public bool registered = false;
	[NonSerialized] public bool forceSort;

	public Collider2D sortingBounds;

	[NonSerialized] public readonly List<IsoSpriteSorting> staticDependencies = new(16);
	[NonSerialized] public readonly List<IsoSpriteSorting> inverseStaticDependencies = new(16);
	[NonSerialized] public readonly List<IsoSpriteSorting> movingDependencies = new(8);

	private readonly List<IsoSpriteSorting> visibleStaticDependencies = new(16);

	public int renderBelowSortingOrder = 0;
	private int visibleStaticLastRefreshFrame = 0;
	
	private static IsoSpriteSorting[] isoSorters = new  IsoSpriteSorting[8];

	
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

	public Vector3 SorterPositionOffset = new();
	public Vector3 SorterPositionOffset2 = new();
	public List<Renderer> renderersToSort = new();
	private Bounds2D cachedBounds;
	private int lastBoundsCalculatedFrame = 0;
	public bool renderAboveAll;
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

		if (renderersToSort[0] == null)
		{
			GetRenderers();
		}
		cachedBounds = new Bounds2D(renderersToSort[0].bounds);
	}

	private void RefreshPoint1()
	{
		cachedPoint1 = SorterPositionOffset + t.position;
	}

	private void RefreshPoint2()
	{
		cachedPoint2 = SorterPositionOffset2 + t.position;
	}

	private int lastPoint1CalculatedFrame;
	private Vector2 cachedPoint1;

	private Vector3 SortingPoint1
	{
		get
		{
			if (isMovable)
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
			if (isMovable)
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
			else
				return SortingPoint1;
		}
	}

	private float SortingLineCenterHeight
	{
		get
		{
			if (sortType == SortType.Line)
				return (SortingPoint1.y + SortingPoint2.y) / 2;
			else
			{
				return SortingPoint1.y;
			}
		}
	}

	public static void UpdateSorters()
	{
		//isoSorters = FindObjectsByType<IsoSpriteSorting>( FindObjectsInactive.Include, FindObjectsSortMode.None); //changed this
		//foreach (var t1 in isoSorters)
		//	t1.Setup();

		//IsoSpriteSortingManager.UpdateSorting();
	}


	private void Awake()
	{
		t = transform; //This needs to be here AND in the setup function
	}


	private void OnEnable()
	{
		if (Application.isPlaying) Setup();
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
		GetRenderers();
	}
#endif

	
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
			{
				
				
				renderersToSort.Add(spriteRenderer);
			}
		}
	}

	public void RemoveNulls()
	{
		for (int i = renderersToSort.Count - 1; i >= 0; i--)
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
		{
			return sprite2.SortingPoint1.y.CompareTo(sprite1.SortingPoint1.y);
		}
		else if (sprite1.sortType == SortType.Line && sprite2.sortType == SortType.Line)
			return CompareLineAndLine(sprite1, sprite2);
		else if (sprite1.sortType == SortType.Point && sprite2.sortType == SortType.Line)
			return ComparePointAndLine(sprite1.SortingPoint1, sprite2);
		else if (sprite1.sortType == SortType.Line && sprite2.sortType == SortType.Point)
			return -ComparePointAndLine(sprite2.SortingPoint1, sprite1);
		else
			return 0;
	}

	public static int CompareIsoSortersBelow(IsoSpriteSorting sprite1, IsoSpriteSorting sprite2)
	{
		if (sprite1.renderBelowSortingOrder == sprite2.renderBelowSortingOrder) return 0;
		return sprite1.renderBelowSortingOrder > sprite2.renderBelowSortingOrder ? 1 : -1;
	}

	private static int CompareLineAndLine(IsoSpriteSorting line1, IsoSpriteSorting line2)
	{
		Vector2 line1Point1 = line1.SortingPoint1;
		Vector2 line1Point2 = line1.SortingPoint2;
		var line1LowPoint = (line1Point1.y > line1Point2.y) ? line1Point2 : line1Point1;
		var line1HighPoint = (line1Point1.y > line1Point2.y) ? line1Point1 : line1Point2;
		var line1slantUpward = line1HighPoint.x > line1LowPoint.x;
		
		
		Vector2 line2Point1 = line2.SortingPoint1;
		Vector2 line2Point2 = line2.SortingPoint2;
		var line2LowPoint = (line2Point1.y > line2Point2.y) ? line2Point2 : line2Point1;
		var line2HighPoint = (line2Point1.y > line2Point2.y) ?  line2Point1 : line2Point2;
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
			{
				return (line1LowPoint.x > line2LowPoint.x) ? 1 : -1;
			}
			else
			{
				return (line1LowPoint.x > line2LowPoint.x) ? -1 : 1;
			}
		}
		
		if (oneVStwo != int.MinValue)
			return oneVStwo;
		if (twoVSone != int.MinValue)
			return twoVSone;

			return CompareLineCenters(line1, line2);
	}

	private static int CompareLineCenters(IsoSpriteSorting line1, IsoSpriteSorting line2)
	{
		return -line1.SortingLineCenterHeight.CompareTo(line2.SortingLineCenterHeight);
	}

	private static int ComparePointAndLine(Vector3 point, IsoSpriteSorting line)
	{
		var pointY = point.y;
		if (pointY > line.SortingPoint1.y && pointY > line.SortingPoint2.y)
			return -1;
		else if (pointY < line.SortingPoint1.y && pointY < line.SortingPoint2.y)
			return 1;
		else
		{
			var slope = (line.SortingPoint2.y - line.SortingPoint1.y) / (line.SortingPoint2.x - line.SortingPoint1.x);
			var intercept = line.SortingPoint1.y - slope * line.SortingPoint1.x;
			var yOnLineForPoint = slope * point.x + intercept;
			return yOnLineForPoint > point.y ? 1 : -1;
		}
	}

	public int RendererSortingOrder
	{
		get
		{
			if (renderersToSort.Count > 0)
				return renderersToSort[0].sortingOrder;
			else
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
			if (isMovable)
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
		if (Application.isPlaying)
		{
		
			Unregister();
		}
	}

	private void Unregister()
	{
		IsoSpriteSortingManager.UnregisterSprite(this);
	}
}