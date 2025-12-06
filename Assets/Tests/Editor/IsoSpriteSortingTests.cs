using NUnit.Framework;
using UnityEngine;
using __SCRIPTS.Plugins._ISOSORT;

namespace Tests
{
	public class IsoSpriteSortingTests
	{
		private GameObject CreateTestObject(string name, Vector3 position, Vector3 scale, Vector3 offset, IsoSpriteSorting.SortType sortType = IsoSpriteSorting.SortType.Point)
		{
			var go = new GameObject(name);
			go.transform.position = position;
			go.transform.localScale = scale;

			var sorting = go.AddComponent<IsoSpriteSorting>();
			sorting.SorterPositionOffset = offset;
			sorting.sortType = sortType;
			sorting.isMovable = false;

			// Add a dummy sprite renderer so it's not null
			var sr = go.AddComponent<SpriteRenderer>();
			sorting.renderersToSort.Add(sr);

			sorting.Setup();
			sorting.EnsureCache(force: true);

			return go;
		}

		[TearDown]
		public void TearDown()
		{
			// Clean up all test objects
			var allObjects = Object.FindObjectsOfType<GameObject>();
			foreach (var obj in allObjects)
			{
				Object.DestroyImmediate(obj);
			}
		}

		// ============================================================
		// POINT VS POINT TESTS
		// ============================================================

		[Test]
		public void PointVsPoint_HigherY_RendersFirst()
		{
			// Higher Y = further back = renders first (return -1)
			var obj1 = CreateTestObject("Higher", Vector3.zero, Vector3.one, new Vector3(0, 5, 0));
			var obj2 = CreateTestObject("Lower", Vector3.zero, Vector3.one, new Vector3(0, 2, 0));

			var s1 = obj1.GetComponent<IsoSpriteSorting>();
			var s2 = obj2.GetComponent<IsoSpriteSorting>();

			int result = IsoSpriteSorting.CompareIsoSorters(s1, s2);

			Assert.AreEqual(-1, result, "Higher Y should render first (return -1)");
		}

		[Test]
		public void PointVsPoint_LowerY_RendersLast()
		{
			var obj1 = CreateTestObject("Lower", Vector3.zero, Vector3.one, new Vector3(0, 2, 0));
			var obj2 = CreateTestObject("Higher", Vector3.zero, Vector3.one, new Vector3(0, 5, 0));

			var s1 = obj1.GetComponent<IsoSpriteSorting>();
			var s2 = obj2.GetComponent<IsoSpriteSorting>();

			int result = IsoSpriteSorting.CompareIsoSorters(s1, s2);

			Assert.AreEqual(1, result, "Lower Y should render last (return 1)");
		}

		[Test]
		public void PointVsPoint_SameY_UsesInstanceID()
		{
			var obj1 = CreateTestObject("Same1", Vector3.zero, Vector3.one, new Vector3(0, 3, 0));
			var obj2 = CreateTestObject("Same2", Vector3.zero, Vector3.one, new Vector3(0, 3, 0));

			var s1 = obj1.GetComponent<IsoSpriteSorting>();
			var s2 = obj2.GetComponent<IsoSpriteSorting>();

			int result = IsoSpriteSorting.CompareIsoSorters(s1, s2);

			// Should not be 0 (uses instance ID as tiebreaker)
			Assert.AreNotEqual(0, result, "Same Y should use instance ID tiebreaker");
		}

		// ============================================================
		// SCALE TRANSFORMATION TESTS
		// ============================================================

		[Test]
		public void Scale_DoubleScale_DoublesOffset()
		{
			// With scale 1, offset (0, 2, 0) should result in Y = 2
			var obj1 = CreateTestObject("Normal", Vector3.zero, Vector3.one, new Vector3(0, 2, 0));

			// With scale 2, offset (0, 2, 0) should result in Y = 4
			var obj2 = CreateTestObject("Scaled", Vector3.zero, new Vector3(2, 2, 2), new Vector3(0, 2, 0));

			var s1 = obj1.GetComponent<IsoSpriteSorting>();
			var s2 = obj2.GetComponent<IsoSpriteSorting>();

			// Get cached points
			Vector3 p1 = s1.AsPoint;
			Vector3 p2 = s2.AsPoint;

			Assert.AreEqual(2f, p1.y, 0.01f, "Normal scale should have Y = 2");
			Assert.AreEqual(4f, p2.y, 0.01f, "Double scale should have Y = 4");
		}

		[Test]
		public void Scale_NonUniform_AppliesCorrectly()
		{
			// Scale (1, 3, 1) with offset (0, 2, 0) should give Y = 6
			var obj = CreateTestObject("NonUniform", Vector3.zero, new Vector3(1, 3, 1), new Vector3(0, 2, 0));
			var s = obj.GetComponent<IsoSpriteSorting>();

			Vector3 p = s.AsPoint;

			Assert.AreEqual(6f, p.y, 0.01f, "Y scale of 3 with offset 2 should give Y = 6");
		}

		[Test]
		public void Scale_WithRotation_TransformsCorrectly()
		{
			// Rotate 90 degrees around Z, offset (2, 0, 0) should become (0, 2, 0) in world space
			var obj = CreateTestObject("Rotated", Vector3.zero, Vector3.one, new Vector3(2, 0, 0));
			obj.transform.rotation = Quaternion.Euler(0, 0, 90);

			var s = obj.GetComponent<IsoSpriteSorting>();
			s.EnsureCache(force: true);

			Vector3 p = s.AsPoint;

			Assert.AreEqual(2f, p.y, 0.1f, "90° rotation should transform X offset to Y");
			Assert.AreEqual(0f, p.x, 0.1f, "90° rotation should transform X offset to Y");
		}

		// ============================================================
		// POINT VS LINE TESTS
		// ============================================================

		[Test]
		public void PointVsLine_PointHigherThanLineCenter_RendersFirst()
		{
			// Line from (0, 2) to (10, 2) - horizontal at Y=2
			var lineObj = CreateTestObject("Line", Vector3.zero, Vector3.one, new Vector3(0, 2, 0), IsoSpriteSorting.SortType.Line);
			var line = lineObj.GetComponent<IsoSpriteSorting>();
			line.SorterPositionOffset2 = new Vector3(10, 2, 0);
			line.EnsureCache(force: true);

			// Point at (5, 5) - higher Y than line
			var pointObj = CreateTestObject("Point", Vector3.zero, Vector3.one, new Vector3(5, 5, 0));
			var point = pointObj.GetComponent<IsoSpriteSorting>();

			int result = IsoSpriteSorting.CompareIsoSorters(point, line);

			Assert.AreEqual(-1, result, "Point with higher Y should render first");
		}

		[Test]
		public void PointVsLine_PointLowerThanLineCenter_RendersLast()
		{
			// Line from (0, 5) to (10, 5) - horizontal at Y=5
			var lineObj = CreateTestObject("Line", Vector3.zero, Vector3.one, new Vector3(0, 5, 0), IsoSpriteSorting.SortType.Line);
			var line = lineObj.GetComponent<IsoSpriteSorting>();
			line.SorterPositionOffset2 = new Vector3(10, 5, 0);
			line.EnsureCache(force: true);

			// Point at (5, 2) - lower Y than line
			var pointObj = CreateTestObject("Point", Vector3.zero, Vector3.one, new Vector3(5, 2, 0));
			var point = pointObj.GetComponent<IsoSpriteSorting>();

			int result = IsoSpriteSorting.CompareIsoSorters(point, line);

			Assert.AreEqual(1, result, "Point with lower Y should render last");
		}

		[Test]
		public void PointVsLine_DiagonalLine_LeftEnd()
		{
			// Diagonal line from (0, 2) to (10, 8)
			var lineObj = CreateTestObject("Line", Vector3.zero, Vector3.one, new Vector3(0, 2, 0), IsoSpriteSorting.SortType.Line);
			var line = lineObj.GetComponent<IsoSpriteSorting>();
			line.SorterPositionOffset2 = new Vector3(10, 8, 0);
			line.EnsureCache(force: true);

			// Point at X=2 (near left end)
			// At X=2, line Y = 2 + (8-2) * (2/10) = 3.2
			// Point at Y=5 is higher than 3.2, so should render first
			var pointObj = CreateTestObject("Point", Vector3.zero, Vector3.one, new Vector3(2, 5, 0));
			var point = pointObj.GetComponent<IsoSpriteSorting>();

			int result = IsoSpriteSorting.CompareIsoSorters(point, line);

			Assert.AreEqual(-1, result, "Point higher than line at its X position should render first");
		}

		[Test]
		public void PointVsLine_DiagonalLine_RightEnd()
		{
			// Diagonal line from (0, 2) to (10, 8)
			var lineObj = CreateTestObject("Line", Vector3.zero, Vector3.one, new Vector3(0, 2, 0), IsoSpriteSorting.SortType.Line);
			var line = lineObj.GetComponent<IsoSpriteSorting>();
			line.SorterPositionOffset2 = new Vector3(10, 8, 0);
			line.EnsureCache(force: true);

			// Point at X=8 (near right end)
			// At X=8, line Y = 2 + (8-2) * (8/10) = 6.8
			// Point at Y=5 is lower than 6.8, so should render last
			var pointObj = CreateTestObject("Point", Vector3.zero, Vector3.one, new Vector3(8, 5, 0));
			var point = pointObj.GetComponent<IsoSpriteSorting>();

			int result = IsoSpriteSorting.CompareIsoSorters(point, line);

			Assert.AreEqual(1, result, "Point lower than line at its X position should render last");
		}

		[Test]
		public void PointVsLine_PointOutsideLineRange_UsesNearestEndpoint()
		{
			// Line from (5, 5) to (10, 5) - horizontal
			var lineObj = CreateTestObject("Line", Vector3.zero, Vector3.one, new Vector3(5, 5, 0), IsoSpriteSorting.SortType.Line);
			var line = lineObj.GetComponent<IsoSpriteSorting>();
			line.SorterPositionOffset2 = new Vector3(10, 5, 0);
			line.EnsureCache(force: true);

			// Point at X=2 (left of line range)
			// Should compare against leftmost endpoint Y=5
			var pointObj = CreateTestObject("Point", Vector3.zero, Vector3.one, new Vector3(2, 7, 0));
			var point = pointObj.GetComponent<IsoSpriteSorting>();

			int result = IsoSpriteSorting.CompareIsoSorters(point, line);

			Assert.AreEqual(-1, result, "Point Y=7 > endpoint Y=5, should render first");
		}

		[Test]
		public void PointVsLine_VerticalLine_UsesAverage()
		{
			// Vertical line from (5, 2) to (5, 8)
			var lineObj = CreateTestObject("Line", Vector3.zero, Vector3.one, new Vector3(5, 2, 0), IsoSpriteSorting.SortType.Line);
			var line = lineObj.GetComponent<IsoSpriteSorting>();
			line.SorterPositionOffset2 = new Vector3(5, 8, 0);
			line.EnsureCache(force: true);

			// Point at (3, 7) - Y=7 is higher than line average Y=5
			var pointObj = CreateTestObject("Point", Vector3.zero, Vector3.one, new Vector3(3, 7, 0));
			var point = pointObj.GetComponent<IsoSpriteSorting>();

			int result = IsoSpriteSorting.CompareIsoSorters(point, line);

			Assert.AreEqual(-1, result, "Point Y=7 > line avg Y=5, should render first");
		}

		// ============================================================
		// LINE VS LINE TESTS
		// ============================================================

		[Test]
		public void LineVsLine_BothHorizontal_ComparesByY()
		{
			// Line 1: Y=5
			var line1Obj = CreateTestObject("Line1", Vector3.zero, Vector3.one, new Vector3(0, 5, 0), IsoSpriteSorting.SortType.Line);
			var line1 = line1Obj.GetComponent<IsoSpriteSorting>();
			line1.SorterPositionOffset2 = new Vector3(10, 5, 0);
			line1.EnsureCache(force: true);

			// Line 2: Y=3
			var line2Obj = CreateTestObject("Line2", Vector3.zero, Vector3.one, new Vector3(0, 3, 0), IsoSpriteSorting.SortType.Line);
			var line2 = line2Obj.GetComponent<IsoSpriteSorting>();
			line2.SorterPositionOffset2 = new Vector3(10, 3, 0);
			line2.EnsureCache(force: true);

			int result = IsoSpriteSorting.CompareIsoSorters(line1, line2);

			Assert.AreEqual(-1, result, "Line with higher Y should render first");
		}

		[Test]
		public void LineVsLine_Overlapping_ComparesAtOverlapCenter()
		{
			// Line 1: diagonal from (0, 2) to (10, 8)
			var line1Obj = CreateTestObject("Line1", Vector3.zero, Vector3.one, new Vector3(0, 2, 0), IsoSpriteSorting.SortType.Line);
			var line1 = line1Obj.GetComponent<IsoSpriteSorting>();
			line1.SorterPositionOffset2 = new Vector3(10, 8, 0);
			line1.EnsureCache(force: true);

			// Line 2: diagonal from (5, 8) to (15, 2) - crossing line 1
			var line2Obj = CreateTestObject("Line2", Vector3.zero, Vector3.one, new Vector3(5, 8, 0), IsoSpriteSorting.SortType.Line);
			var line2 = line2Obj.GetComponent<IsoSpriteSorting>();
			line2.SorterPositionOffset2 = new Vector3(15, 2, 0);
			line2.EnsureCache(force: true);

			int result = IsoSpriteSorting.CompareIsoSorters(line1, line2);

			// At X=7.5 (center of overlap 5-10):
			// Line1 Y = 2 + 6*(7.5/10) = 6.5
			// Line2 Y = 8 - 6*((7.5-5)/10) = 6.5
			// Should use tiebreaker (instance ID)
			Assert.AreNotEqual(0, result, "Should have consistent ordering even if Y values are close");
		}

		[Test]
		public void LineVsLine_NoXOverlap_UsesAverageY()
		{
			// Line 1: from (0, 5) to (5, 5)
			var line1Obj = CreateTestObject("Line1", Vector3.zero, Vector3.one, new Vector3(0, 5, 0), IsoSpriteSorting.SortType.Line);
			var line1 = line1Obj.GetComponent<IsoSpriteSorting>();
			line1.SorterPositionOffset2 = new Vector3(5, 5, 0);
			line1.EnsureCache(force: true);

			// Line 2: from (10, 3) to (15, 3) - no X overlap
			var line2Obj = CreateTestObject("Line2", Vector3.zero, Vector3.one, new Vector3(10, 3, 0), IsoSpriteSorting.SortType.Line);
			var line2 = line2Obj.GetComponent<IsoSpriteSorting>();
			line2.SorterPositionOffset2 = new Vector3(15, 3, 0);
			line2.EnsureCache(force: true);

			int result = IsoSpriteSorting.CompareIsoSorters(line1, line2);

			Assert.AreEqual(-1, result, "Line with higher average Y should render first");
		}

		[Test]
		public void LineVsLine_BothVertical_ComparesAverageY()
		{
			// Vertical line 1: X=5, Y from 2 to 8 (avg=5)
			var line1Obj = CreateTestObject("Line1", Vector3.zero, Vector3.one, new Vector3(5, 2, 0), IsoSpriteSorting.SortType.Line);
			var line1 = line1Obj.GetComponent<IsoSpriteSorting>();
			line1.SorterPositionOffset2 = new Vector3(5, 8, 0);
			line1.EnsureCache(force: true);

			// Vertical line 2: X=8, Y from 1 to 5 (avg=3)
			var line2Obj = CreateTestObject("Line2", Vector3.zero, Vector3.one, new Vector3(8, 1, 0), IsoSpriteSorting.SortType.Line);
			var line2 = line2Obj.GetComponent<IsoSpriteSorting>();
			line2.SorterPositionOffset2 = new Vector3(8, 5, 0);
			line2.EnsureCache(force: true);

			int result = IsoSpriteSorting.CompareIsoSorters(line1, line2);

			Assert.AreEqual(-1, result, "Vertical line with higher avg Y should render first");
		}

		// ============================================================
		// COMPLEX SCENARIO TESTS
		// ============================================================

		[Test]
		public void ComplexScenario_ScaledRotatedLine_WithPoint()
		{
			// Create a line, scale it, and rotate it
			var lineObj = CreateTestObject("ScaledLine", new Vector3(0, 0, 0), new Vector3(2, 2, 2),
				new Vector3(0, 1, 0), IsoSpriteSorting.SortType.Line);
			lineObj.transform.rotation = Quaternion.Euler(0, 0, 45);

			var line = lineObj.GetComponent<IsoSpriteSorting>();
			line.SorterPositionOffset2 = new Vector3(2, 1, 0);
			line.EnsureCache(force: true);

			// Create a point above the line's world position
			var pointObj = CreateTestObject("Point", Vector3.zero, Vector3.one, new Vector3(0, 5, 0));
			var point = pointObj.GetComponent<IsoSpriteSorting>();

			int result = IsoSpriteSorting.CompareIsoSorters(point, line);

			// Point should render first since it's at higher Y
			Assert.AreEqual(-1, result, "Point at higher Y should render first even with scaled/rotated line");
		}

		[Test]
		public void ComplexScenario_MultipleObjectsSorting()
		{
			// Create 3 objects at different depths
			var obj1 = CreateTestObject("Back", Vector3.zero, Vector3.one, new Vector3(0, 10, 0));
			var obj2 = CreateTestObject("Middle", Vector3.zero, Vector3.one, new Vector3(0, 5, 0));
			var obj3 = CreateTestObject("Front", Vector3.zero, Vector3.one, new Vector3(0, 2, 0));

			var s1 = obj1.GetComponent<IsoSpriteSorting>();
			var s2 = obj2.GetComponent<IsoSpriteSorting>();
			var s3 = obj3.GetComponent<IsoSpriteSorting>();

			// Back should be less than middle
			Assert.Less(IsoSpriteSorting.CompareIsoSorters(s1, s2), 0);

			// Middle should be less than front
			Assert.Less(IsoSpriteSorting.CompareIsoSorters(s2, s3), 0);

			// Back should be less than front
			Assert.Less(IsoSpriteSorting.CompareIsoSorters(s1, s3), 0);
		}

		// ============================================================
		// EDGE CASE TESTS
		// ============================================================

		[Test]
		public void EdgeCase_ZeroScale_DoesNotCrash()
		{
			// This shouldn't crash, just give zero offset
			var obj = CreateTestObject("ZeroScale", Vector3.zero, Vector3.zero, new Vector3(1, 1, 0));
			var s = obj.GetComponent<IsoSpriteSorting>();

			Assert.DoesNotThrow(() => s.EnsureCache(force: true));
		}

		[Test]
		public void EdgeCase_NegativeScale_TransformsCorrectly()
		{
			// Negative scale should flip the offset
			var obj = CreateTestObject("NegScale", Vector3.zero, new Vector3(1, -1, 1), new Vector3(0, 2, 0));
			var s = obj.GetComponent<IsoSpriteSorting>();

			Vector3 p = s.AsPoint;

			Assert.AreEqual(-2f, p.y, 0.01f, "Negative Y scale should flip offset");
		}

		[Test]
		public void EdgeCase_VeryLongLine_StillWorksCorrectly()
		{
			// Create a very long line
			var lineObj = CreateTestObject("LongLine", Vector3.zero, Vector3.one,
				new Vector3(-1000, 5, 0), IsoSpriteSorting.SortType.Line);
			var line = lineObj.GetComponent<IsoSpriteSorting>();
			line.SorterPositionOffset2 = new Vector3(1000, 5, 0);
			line.EnsureCache(force: true);

			// Point in the middle
			var pointObj = CreateTestObject("Point", Vector3.zero, Vector3.one, new Vector3(0, 7, 0));
			var point = pointObj.GetComponent<IsoSpriteSorting>();

			int result = IsoSpriteSorting.CompareIsoSorters(point, line);

			Assert.AreEqual(-1, result, "Should work with very long lines");
		}
	}
}
