using __SCRIPTS;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WallBuilder))]
public class WallBuilderEditor : Editor
{
    private WallBuilder wallBuilder;
    private bool showDirectionSettings = false;
    private bool showOffsetSettings = false;
    private readonly Color[] directionColors = new Color[]
    {
        new Color(0.8f, 0.2f, 0.2f, 1f), // NE - Red
        new Color(0.2f, 0.8f, 0.2f, 1f), // SE - Green
        new Color(0.2f, 0.2f, 0.8f, 1f), // SW - Blue
        new Color(0.8f, 0.8f, 0.2f, 1f)  // NW - Yellow
    };

    private void OnEnable()
    {
        wallBuilder = (WallBuilder)target;
        
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space(10);
        DrawTileSizeProperty();
        
        EditorGUILayout.Space(10);
        DrawWallControls();
        
        EditorGUILayout.Space(10);
        DrawOffsetSettings();
        
        EditorGUILayout.Space(10);
        DrawWallsList();

        serializedObject.ApplyModifiedProperties();
        
        // Repaint scene view when inspector changes
        if (GUI.changed)
        {
            SceneView.RepaintAll();
        }
    }

    private void DrawTileSizeProperty()
    {
        EditorGUILayout.LabelField("Tile Settings", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("TileSize"), new GUIContent("Tile Size"));
        EditorGUI.indentLevel--;
    }

    private void DrawWallControls()
    {
        // Current wall direction
        EditorGUILayout.LabelField("Wall Builder Controls", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("wallDirection"), new GUIContent("Current Direction"));
        
        GUILayout.Space(5);
        
        // Direction buttons in a neat grid
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        
        // NE button
        GUI.backgroundColor = wallBuilder.wallDirection == isoDirection.NE ? directionColors[0] : Color.white;
        if (GUILayout.Button("NE", GUILayout.Width(60), GUILayout.Height(30)))
        {
            wallBuilder.SetDirectionNE();
        }
        
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        
        // NW button
        GUI.backgroundColor = wallBuilder.wallDirection == isoDirection.NW ? directionColors[3] : Color.white;
        if (GUILayout.Button("NW", GUILayout.Width(60), GUILayout.Height(30)))
        {
            wallBuilder.SetDirectionNW();
        }
        
        GUILayout.Space(50);
        
        // SE button
        GUI.backgroundColor = wallBuilder.wallDirection == isoDirection.SE ? directionColors[1] : Color.white;
        if (GUILayout.Button("SE", GUILayout.Width(60), GUILayout.Height(30)))
        {
            wallBuilder.SetDirectionSE();
        }
        
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        
        // SW button
        GUI.backgroundColor = wallBuilder.wallDirection == isoDirection.SW ? directionColors[2] : Color.white;
        if (GUILayout.Button("SW", GUILayout.Width(60), GUILayout.Height(30)))
        {
            wallBuilder.SetDirectionSW();
        }
        
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        
        // Reset color
        GUI.backgroundColor = Color.white;
        
        GUILayout.Space(10);
        
        // Build and Delete buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Build Wall", GUILayout.Height(30)))
        {
            wallBuilder.BuildWall();
        }
        
        if (GUILayout.Button("Delete Last Wall", GUILayout.Height(30)))
        {
            wallBuilder.DeleteWall();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUI.indentLevel--;
    }

    private void DrawOffsetSettings()
    {
        showOffsetSettings = EditorGUILayout.Foldout(showOffsetSettings, "Direction Offset Settings", true, EditorStyles.foldoutHeader);
        
        if (showOffsetSettings)
        {
            EditorGUI.indentLevel++;

            // Presets buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set Recommended Offsets"))
            {
                SetRecommendedOffsets();
            }
            
            if (GUILayout.Button("Reset Offsets"))
            {
                ResetOffsets();
            }
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            
            // Draw offset fields in a structured way
            DrawOffsetGroup("From NE", new string[] { "NE_NE", "NE_SE", "NE_SW", "NE_NW" }, 0);
            DrawOffsetGroup("From SE", new string[] { "SE_NE", "SE_SE", "SE_SW", "SE_NW" }, 1);
            DrawOffsetGroup("From SW", new string[] { "SW_NE", "SW_SE", "SW_SW", "SW_NW" }, 2);
            DrawOffsetGroup("From NW", new string[] { "NW_NE", "NW_SE", "NW_SW", "NW_NW" }, 3);
            
            EditorGUI.indentLevel--;
        }
    }

    private void DrawOffsetGroup(string label, string[] properties, int colorIndex)
    {
        GUILayout.Space(5);
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        // Group label
        EditorGUILayout.BeginHorizontal();
        GUI.color = directionColors[colorIndex];
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();
        
        // Offset fields
        for (int i = 0; i < properties.Length; i++)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(properties[i]), new GUIContent("To " + properties[i].Substring(properties[i].IndexOf('_') + 1)));
        }
        
        EditorGUILayout.EndVertical();
    }

    private void DrawWallsList()
    {
        showDirectionSettings = EditorGUILayout.Foldout(showDirectionSettings, "Walls List", true, EditorStyles.foldoutHeader);
        
        if (showDirectionSettings)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("walls"), true);
            EditorGUI.indentLevel--;
        }
    }

    private void InitializeDefaultOffsets()
    {
        // Only set defaults if all values are Vector2.zero
        if (wallBuilder.NE_NE == Vector2.zero && 
            wallBuilder.SE_SE == Vector2.zero && 
            wallBuilder.SW_SW == Vector2.zero && 
            wallBuilder.NW_NW == Vector2.zero)
        {
            SetRecommendedOffsets();
        }
    }

    private void SetRecommendedOffsets()
    {
        Undo.RecordObject(wallBuilder, "Set Recommended Wall Offsets");
        
        // When coming from NE direction
        wallBuilder.NE_NE = new Vector2(1, 0.5f);      // Continue straight
        wallBuilder.NE_SE = new Vector2(0.5f, -0.5f);  // Turn right
        wallBuilder.NE_SW = new Vector2(0, 0);         // 180° turn (handled by deletion)
        wallBuilder.NE_NW = new Vector2(-0.5f, 0.5f);  // Turn left

        // When coming from SE direction
        wallBuilder.SE_NE = new Vector2(0.5f, 0.5f);   // Turn left
        wallBuilder.SE_SE = new Vector2(0, -1f);       // Continue straight
        wallBuilder.SE_SW = new Vector2(-0.5f, -0.5f); // Turn right
        wallBuilder.SE_NW = new Vector2(0, 0);         // 180° turn (handled by deletion)

        // When coming from SW direction
        wallBuilder.SW_NE = new Vector2(0, 0);         // 180° turn (handled by deletion)
        wallBuilder.SW_SE = new Vector2(0.5f, -0.5f);  // Turn left
        wallBuilder.SW_SW = new Vector2(-1f, -0.5f);   // Continue straight
        wallBuilder.SW_NW = new Vector2(-0.5f, 0.5f);  // Turn right

        // When coming from NW direction
        wallBuilder.NW_NE = new Vector2(0.5f, 0.5f);   // Turn right
        wallBuilder.NW_SE = new Vector2(0, 0);         // 180° turn (handled by deletion)
        wallBuilder.NW_SW = new Vector2(-0.5f, -0.5f); // Turn left
        wallBuilder.NW_NW = new Vector2(0, 1f);        // Continue straight
    }
    
    private void ResetOffsets()
    {
        Undo.RecordObject(wallBuilder, "Reset Wall Offsets");
        
        // Reset all offsets to zero
        wallBuilder.NE_NE = wallBuilder.NE_SE = wallBuilder.NE_SW = wallBuilder.NE_NW = Vector2.zero;
        wallBuilder.SE_NE = wallBuilder.SE_SE = wallBuilder.SE_SW = wallBuilder.SE_NW = Vector2.zero;
        wallBuilder.SW_NE = wallBuilder.SW_SE = wallBuilder.SW_SW = wallBuilder.SW_NW = Vector2.zero;
        wallBuilder.NW_NE = wallBuilder.NW_SE = wallBuilder.NW_SW = wallBuilder.NW_NW = Vector2.zero;
    }
    
    // Scene view visualization
    private void OnSceneGUI()
    {
      
        
        // Get the builder's transform position (anchor point)
        Vector3 anchorPosition = wallBuilder.transform.position;
        
        // Always draw the base tile at the anchor point
        DrawTileRectangle(anchorPosition, true);
        
        if (wallBuilder.walls == null || wallBuilder.walls.Count == 0)
            return;
            
        // Draw tiles for each wall based on its index
        for (int i = 0; i < wallBuilder.walls.Count; i++)
        {
            if (wallBuilder.walls[i] == null)
                continue;
                
            // Draw direction indicator for each wall
            DrawDirectionIndicator(wallBuilder.walls[i]);
            
            // Calculate tile position based on index and wall direction
            Vector3 tilePos = CalculateTilePosition(i);
            
            // Draw the tile
            DrawTileRectangle(tilePos);
            
            // Draw index numbers near wall
            Handles.Label(wallBuilder.walls[i].transform.position + Vector3.up * 2f, i.ToString());
        }
        
        // Draw possible next positions if we have walls
        if (wallBuilder.walls.Count > 0)
        {
            // Calculate the position for the next wall based on existing walls
            Vector3 nextBasePos = CalculateTilePosition(wallBuilder.walls.Count - 1);
            
            // Draw preview for each possible direction
            DrawDirectionPreview(nextBasePos, isoDirection.NE, 0);
            DrawDirectionPreview(nextBasePos, isoDirection.SE, 1);
            DrawDirectionPreview(nextBasePos, isoDirection.SW, 2);
            DrawDirectionPreview(nextBasePos, isoDirection.NW, 3);
        }
    }
    
    // Calculate tile position based on wall index
    private Vector3 CalculateTilePosition(int index)
    {
        var isItFlipped = wallBuilder.walls[index].transform.localScale.x < 0;
        var offsetty = new Vector3(wallBuilder.TileSize.x / 2, wallBuilder.TileSize.y * 2);
        if (isItFlipped)
        {
            offsetty.x = -offsetty.x; // Flip the X offset if the wall is flipped
        }
        return wallBuilder.walls[index].transform.position- offsetty;
        if (index < 0 || wallBuilder.walls == null || index >= wallBuilder.walls.Count)
            return wallBuilder.transform.position;
            
        Vector3 anchorPosition = wallBuilder.transform.position;
        Vector2 tileSize = wallBuilder.TileSize;
        
       
        // For index 0, return a position offset from the anchor based on first wall direction
        if (index == 0)
        {
            Wall wall = wallBuilder.walls[0];
            switch (wall.direction)
            {
                case isoDirection.NE:
                    return anchorPosition + new Vector3(tileSize.x, tileSize.y, 0);  // Double X, Half Y
                case isoDirection.SE:
                    return anchorPosition + new Vector3(tileSize.x, -tileSize.y, 0); // Double X, Half Y
                case isoDirection.SW:
                    return anchorPosition + new Vector3(-tileSize.x, -tileSize.y, 0); // Double X, Half Y
                case isoDirection.NW:
                    return anchorPosition + new Vector3(-tileSize.x, tileSize.y, 0); // Double X, Half Y
                default:
                    return anchorPosition;
            }
        }
        
        // For other indices, calculate based on previous wall
        Vector3 prevTilePos = CalculateTilePosition(index - 1);
        Wall currentWall = wallBuilder.walls[index];
        Wall prevWall = wallBuilder.walls[index - 1];
        
        // Get offset based on direction change
        Vector2 offset = GetOffsetForDirectionChange(prevWall.direction, currentWall.direction);
        
        // Apply offset scaled by tile size (doubling X component for wider tiles, using full Y for height)
        return prevTilePos + new Vector3(offset.x * tileSize.x, offset.y * tileSize.y*2, 0);
    }
    
    // Get the offset between tiles based on direction change
    private Vector2 GetOffsetForDirectionChange(isoDirection fromDir, isoDirection toDir)
    {
        if (toDir == isoDirection.NE)
        {
            switch (fromDir)
            {
                case isoDirection.NE: return new Vector2(1, 0);
                case isoDirection.SE: return new Vector2(0, 1);
                case isoDirection.SW: return new Vector2(-1, 0);
                case isoDirection.NW: return new Vector2(0, -1);
            }
        }
        else if (toDir == isoDirection.SE)
        {
            switch (fromDir)
            {
                case isoDirection.NE: return new Vector2(0, -1);
                case isoDirection.SE: return new Vector2(1, 0);
                case isoDirection.SW: return new Vector2(0, 1);
                case isoDirection.NW: return new Vector2(-1, 0);
            }
        }
        else if (toDir == isoDirection.SW)
        {
            switch (fromDir)
            {
                case isoDirection.NE: return new Vector2(-1, 0);
                case isoDirection.SE: return new Vector2(0, -1);
                case isoDirection.SW: return new Vector2(1, 0);
                case isoDirection.NW: return new Vector2(0, 1);
            }
        }
        else if (toDir == isoDirection.NW)
        {
            switch (fromDir)
            {
                case isoDirection.NE: return new Vector2(0, 1);
                case isoDirection.SE: return new Vector2(-1, 0);
                case isoDirection.SW: return new Vector2(0, -1);
                case isoDirection.NW: return new Vector2(1, 0);
            }
        }
        
        return Vector2.zero;
    }
    
 
    
    private void DrawTileRectangle(Vector3 position, bool highlight = true)
    {
        Vector2 tileSize = wallBuilder.TileSize;
        
        // Apply offset to push up by the tile height
        Vector3 offsetPosition = position + new Vector3(0, tileSize.y, 0);
        
        // Calculate the four corners of the isometric tile
        // Doubling both width and height to match the actual wall size (12x12)
        Vector3[] corners = new Vector3[4];
        corners[0] = offsetPosition + new Vector3(0, tileSize.y, 0);                 // Top (doubled y)
        corners[1] = offsetPosition + new Vector3(tileSize.x, 0, 0);                 // Right (doubled x)
        corners[2] = offsetPosition + new Vector3(0, -tileSize.y, 0);                // Bottom (doubled y)
        corners[3] = offsetPosition + new Vector3(-tileSize.x, 0, 0);                // Left (doubled x)
        
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
    
    private void DrawDirectionIndicator(Wall wall)
    {
        if (wall == null)
            return;
            
        Vector3 pos = wall.transform.position;
        float size = 1.5f;
        
        // Get color based on direction
        Color dirColor = Color.white;
        switch (wall.direction)
        {
            case isoDirection.NE: dirColor = directionColors[0]; break;
            case isoDirection.SE: dirColor = directionColors[1]; break;
            case isoDirection.SW: dirColor = directionColors[2]; break;
            case isoDirection.NW: dirColor = directionColors[3]; break;
        }
        
        Handles.color = dirColor;
        
        // Draw direction arrow
        Vector3 dir = Vector3.zero;
        switch (wall.direction)
        {
            case isoDirection.NE: dir = new Vector3(1, 0.5f, 0); break;
            case isoDirection.SE: dir = new Vector3(1, -0.5f, 0); break;
            case isoDirection.SW: dir = new Vector3(-1, -0.5f, 0); break;
            case isoDirection.NW: dir = new Vector3(-1, 0.5f, 0); break;
        }
        
        Handles.DrawLine(pos, pos + dir.normalized * size);
        
        // Draw arrowhead
        Vector3 arrowDir = dir.normalized * size;
        Vector3 right = Quaternion.Euler(0, 0, 30) * -arrowDir.normalized;
        Vector3 left = Quaternion.Euler(0, 0, -30) * -arrowDir.normalized;
        
        Handles.DrawLine(pos + arrowDir, pos + arrowDir + right * 0.5f);
        Handles.DrawLine(pos + arrowDir, pos + arrowDir + left * 0.5f);
    }
    
    private void DrawDirectionPreview(Vector3 basePos, isoDirection direction, int colorIndex)
    {
        isoDirection lastDir = wallBuilder.lastDirection;
        
        // Skip 180° turns
        if ((lastDir == isoDirection.NE && direction == isoDirection.SW) ||
            (lastDir == isoDirection.SE && direction == isoDirection.NW) ||
            (lastDir == isoDirection.SW && direction == isoDirection.NE) ||
            (lastDir == isoDirection.NW && direction == isoDirection.SE))
        {
            return;
        }
        
        // Get offset for the next tile
        Vector2 offset = GetOffsetForDirectionChange(lastDir, direction);
        Vector2 tileSize = wallBuilder.TileSize;

        var isItFlipped = wallBuilder.walls[wallBuilder.walls.Count-1].transform.localScale.x < 0;
        var offsetty = new Vector3(tileSize.x / 2, tileSize.y * 2);
        if (isItFlipped)
        {
            offset.x = -offset.x; // Flip the X offset if the wall is flipped
        }

        
        // Calculate next tile position (doubling X component, full Y for height)
        Vector3 nextTilePos = (Vector2)basePos;
        
        // Calculate wall position (between tiles)
        Vector3 wallPos = (basePos + nextTilePos) ;
        
        // Draw preview
        Handles.color = directionColors[colorIndex];
        
        // Draw the tile preview
        DrawTileRectangle(nextTilePos, true);
        
        // Draw circle at the wall position
        Handles.DrawWireDisc(wallPos, Vector3.forward, 0.5f);
        
        // Draw label for direction
        Handles.Label(wallPos + Vector3.up * 1.5f, direction.ToString());
    }
}