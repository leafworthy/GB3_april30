using System.Collections.Generic;
using __SCRIPTS;
using UnityEditor;
using UnityEngine;
using VInspector;

[ExecuteInEditMode]
public class WallBuilder : MonoBehaviour
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created

	public GameObject anchorWall;

	

	public Vector2 TileSize = new(12, 6);
	public List<Wall> walls = new();

	public isoDirection wallDirection = isoDirection.NE;
	public isoDirection lastDirection => GetLastDirection();

	private isoDirection GetLastDirection()
	{
		if (walls == null || walls.Count == 0)
		{
			Debug.Log("no last direction");
			return isoDirection.none;
		}
		return walls[^1].direction;
	}

	private int flipX;
	private int flipY;

	public Vector2 NE_NE;
	public Vector2 NE_SE;
	public Vector2 NE_SW;
	public Vector2 NE_NW;

	public Vector2 SE_NE;
	public Vector2 SE_SE;
	public Vector2 SE_SW;
	public Vector2 SE_NW;

	public Vector2 SW_NE;
	public Vector2 SW_SE;
	public Vector2 SW_SW;
	public Vector2 SW_NW;

	public Vector2 NW_NE;
	public Vector2 NW_SE;
	public Vector2 NW_SW;
	public Vector2 NW_NW;

	[Button]
	public void BuildWall()
	{

		if (walls == null) walls = new List<Wall>();
		anchorWall = walls.Count > 0 ? walls[^1].gameObject : null;
	
		if (lastDirection == isoDirection.NW && wallDirection == isoDirection.SE ||
		    lastDirection == isoDirection.SE && wallDirection == isoDirection.NW ||
		    lastDirection == isoDirection.NE && wallDirection == isoDirection.SW ||
		    lastDirection == isoDirection.SW && wallDirection == isoDirection.NE)
		{
			Debug.Log("delete");
			DeleteWall();
			return;
		}

		var newWallPiece = GetWallPiece();
		if (anchorWall == null)
			newWallPiece.transform.localPosition = new Vector3(TileSize.x / 2, TileSize.y * 2);
		else
			newWallPiece.transform.position = (Vector2) anchorWall.transform.position + ToVector2(wallDirection) * TileSize;

		if (wallDirection == isoDirection.NW || wallDirection == isoDirection.SE)
			newWallPiece.transform.localScale = new Vector3(-newWallPiece.transform.localScale.x, newWallPiece.transform.localScale.y, 1);

		newWallPiece.transform.SetParent(gameObject.transform);
		var wall = newWallPiece.GetComponent<Wall>();
		wall.direction = wallDirection;
		walls.Add(wall);
	}

	[Button("NE")]
	public void SetDirectionNE()
	{
		wallDirection = isoDirection.NE;
		Debug.Log("last direction: " + lastDirection + ", new direction: " + wallDirection);
		BuildWall();
	}

	[Button("SE")]
	public void SetDirectionSE()
	{
		wallDirection = isoDirection.SE;
		Debug.Log("last direction: " + lastDirection + ", new direction: " + wallDirection);
		BuildWall();
	}

	[Button("SW")]
	public void SetDirectionSW()
	{
		wallDirection = isoDirection.SW;
		Debug.Log("last direction: " + lastDirection + ", new direction: " + wallDirection);
		BuildWall();
	}

	[Button("NW")]
	public void SetDirectionNW()
	{
		wallDirection = isoDirection.NW;
		Debug.Log("last direction: " + lastDirection + ", new direction: " + wallDirection);
		BuildWall();
	}

	[Button]
	public void DeleteWall()
	{
		if (walls == null || walls.Count == 0) return;

		var lastWall = walls[^1];
		walls.RemoveAt(walls.Count - 1);
		if (lastWall != null) DestroyImmediate(lastWall.gameObject); // Destroy the GameObject, not the component
	}

	private Vector2 ToVector2(isoDirection isoDirection)
	{
		if (isoDirection == isoDirection.NE)
		{
			switch (lastDirection)
			{
				case isoDirection.NE:
					return NE_NE;
				case isoDirection.SE:
					return SE_NE;
				case isoDirection.SW:
					return SW_NE;
				case isoDirection.NW:
					return NW_NE;
			}
		}

		if (isoDirection == isoDirection.SE)
		{
			switch (lastDirection)
			{
				case isoDirection.NE:
					return NE_SE;
				case isoDirection.SE:
					return SE_SE;
				case isoDirection.SW:
					return SW_SE;
				case isoDirection.NW:
					return NW_SE;
			}
		}

		if (isoDirection == isoDirection.SW)
		{
			switch (lastDirection)
			{
				case isoDirection.NE:
					return NE_SW;
				case isoDirection.SE:
					return SE_SW;
				case isoDirection.SW:
					return SW_SW;
				case isoDirection.NW:
					return NW_SW;
			}
		}

		if (isoDirection == isoDirection.NW)
		{
			switch (lastDirection)
			{
				case isoDirection.NE:
					return NE_NW;
				case isoDirection.SE:
					return SE_NW;
				case isoDirection.SW:
					return SW_NW;
				case isoDirection.NW:
					return NW_NW;
			}
		}

		return default;
	}

	private GameObject GetWallPiece() => PrefabUtility.InstantiatePrefab(ASSETS.House.WallPrefab, transform) as GameObject;


	private void DrawTileRectangle(Vector3 position, bool highlight = true)
	{
		Vector2 tileSize = TileSize;

		// Apply offset to push up by the tile height
		Vector3 offsetPosition = position + new Vector3(0, tileSize.y, 0);

		// Calculate the four corners of the isometric tile
		// Doubling both width and height to match the actual wall size (12x12)
		Vector3[] corners = new Vector3[4];
		corners[0] = offsetPosition + new Vector3(0, tileSize.y, 0); // Top (doubled y)
		corners[1] = offsetPosition + new Vector3(tileSize.x, 0, 0); // Right (doubled x)
		corners[2] = offsetPosition + new Vector3(0, -tileSize.y, 0); // Bottom (doubled y)
		corners[3] = offsetPosition + new Vector3(-tileSize.x, 0, 0); // Left (doubled x)

		// Set color based on whether this is a highlight or background tile
		if (highlight)
		{
			// Semi-transparent highlight for existing tiles
			Handles.color = new Color(0.3f, 0.7f, 1f, 0.3f);
			Handles.DrawAAConvexPolygon(corners);

			// Solid outline
			Handles.color = new Color(0.3f, 0.7f, 1f, 0.8f);
		}

		// Draw the outline of the tile
		Handles.DrawLine(corners[0], corners[1]);
		Handles.DrawLine(corners[1], corners[2]);
		Handles.DrawLine(corners[2], corners[3]);
		Handles.DrawLine(corners[3], corners[0]);

		// Draw a small dot at the center for reference
		if (highlight)
		{
			Handles.color = Color.white;
			Handles.DrawWireDisc(offsetPosition, Vector3.forward, 0.2f);
		}
	}



}