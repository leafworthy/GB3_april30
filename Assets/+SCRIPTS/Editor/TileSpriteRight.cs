using UnityEditor;
using UnityEngine;

public class SpriteTiler : EditorWindow
{



    [MenuItem("Tools/Tile Sprite Right")]
    static void TileSpriteRight()
    {
        // Get the currently selected GameObject
        GameObject selected = Selection.activeGameObject;

        if (selected == null)
        {
            EditorUtility.DisplayDialog("No Selection", "Please select a GameObject with SpriteRenderer component(s).", "OK");
            return;
        }

        // Check if the selected object or its children have SpriteRenderers
        if (!HasSpriteRenderers(selected))
        {
            EditorUtility.DisplayDialog("Invalid Selection", "Selected GameObject must have SpriteRenderer component(s) on itself or its children.", "OK");
            return;
        }

        // Record the operation for undo
        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Tile Sprite Right");

        // Find the rightmost sprite in the same parent
        Transform parent = selected.transform.parent;
        GameObject rightmostSprite = FindRightmostObject(parent, selected);

        // Calculate the position for the new sprite
        Vector3 newPosition = CalculateNextPositionForObject(rightmostSprite, true);

        // Create prefab instance or duplicate based on whether the original is a prefab
        GameObject duplicate = CreateTileInstance(selected, parent, newPosition);

        // Record the creation for undo
        Undo.RegisterCreatedObjectUndo(duplicate, "Create Tile");

        // Select the new duplicate so user can keep pressing the shortcut
        Selection.activeGameObject = duplicate;

        Undo.CollapseUndoOperations(undoGroup);

    }



    [MenuItem("Tools/Tile Sprite Left")]
    static void TileSpriteLeft()
    {
        // Get the currently selected GameObject
        GameObject selected = Selection.activeGameObject;

        if (selected == null)
        {
            EditorUtility.DisplayDialog("No Selection", "Please select a GameObject with SpriteRenderer component(s).", "OK");
            return;
        }

        // Check if the selected object or its children have SpriteRenderers
        if (!HasSpriteRenderers(selected))
        {
            EditorUtility.DisplayDialog("Invalid Selection", "Selected GameObject must have SpriteRenderer component(s) on itself or its children.", "OK");
            return;
        }

        // Record the operation for undo
        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Tile Sprite Left");

        // Find the leftmost sprite in the same parent
        Transform parent = selected.transform.parent;
        GameObject leftmostSprite = FindLeftmostObject(parent, selected);

        // Calculate the position for the new sprite
        Vector3 newPosition = CalculateNextPositionForObject(leftmostSprite, false);

        // Create prefab instance or duplicate based on whether the original is a prefab
        GameObject duplicate = CreateTileInstance(selected, parent, newPosition);

        // Record the creation for undo
        Undo.RegisterCreatedObjectUndo(duplicate, "Create Tile");

        // Select the new duplicate so user can keep pressing the shortcut
        Selection.activeGameObject = duplicate;

        Undo.CollapseUndoOperations(undoGroup);

    }



    static bool HasSpriteRenderers(GameObject obj)
    {
        // Check if the object itself has a SpriteRenderer
        if (obj.GetComponent<SpriteRenderer>() != null)
            return true;

        // Check if any children have SpriteRenderers
        SpriteRenderer[] childRenderers = obj.GetComponentsInChildren<SpriteRenderer>();
        return childRenderers.Length > 0;
    }

    static Bounds GetCombinedBounds(GameObject obj)
    {
        SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>();
        if (renderers.Length == 0)
            return new Bounds(obj.transform.position, Vector3.zero);

        Bounds combinedBounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            combinedBounds.Encapsulate(renderers[i].bounds);
        }

        return combinedBounds;
    }

    static GameObject FindRightmostObject(Transform parent, GameObject originalObject)
    {
        GameObject rightmost = originalObject;
        float rightmostX = GetCombinedBounds(originalObject).max.x;

        // If there's no parent, just use the original object
        if (parent == null)
        {
            return rightmost;
        }

        // Check all children of the parent (or all root objects if no parent)
        Transform[] children = parent != null ? parent.GetComponentsInChildren<Transform>() : FindObjectsByType<Transform>( FindObjectsSortMode.None);

        foreach (Transform child in children)
        {
            // Skip the parent itself and objects without sprite renderers
            if (child == parent || !HasSpriteRenderers(child.gameObject)) continue;

            // Compare objects by their rightmost bounds
            float childRightmostX = GetCombinedBounds(child.gameObject).max.x;
            if (childRightmostX > rightmostX)
            {
                rightmostX = childRightmostX;
                rightmost = child.gameObject;
            }
        }

        return rightmost;
    }

    static GameObject FindLeftmostObject(Transform parent, GameObject originalObject)
    {
        GameObject leftmost = originalObject;
        float leftmostX = GetCombinedBounds(originalObject).min.x;

        // If there's no parent, just use the original object
        if (parent == null)
        {
            return leftmost;
        }

        // Check all children of the parent (or all root objects if no parent)
        Transform[] children = parent != null ? parent.GetComponentsInChildren<Transform>() : FindObjectsByType<Transform>( FindObjectsSortMode.None);

        foreach (Transform child in children)
        {
            // Skip the parent itself and objects without sprite renderers
            if (child == parent || !HasSpriteRenderers(child.gameObject)) continue;

            // Compare objects by their leftmost bounds
            float childLeftmostX = GetCombinedBounds(child.gameObject).min.x;
            if (childLeftmostX < leftmostX)
            {
                leftmostX = childLeftmostX;
                leftmost = child.gameObject;
            }
        }

        return leftmost;
    }


    static Vector3 CalculateNextPositionForObject(GameObject obj, bool toRight)
    {
        Bounds objBounds = GetCombinedBounds(obj);
        Vector3 currentPos = obj.transform.position;

        // Get the object's world-space width
        float objectWidth = objBounds.size.x;

        // Calculate new position offset by the object width
        Vector3 newPos = currentPos;
        if (toRight)
        {
            newPos.x += objectWidth;
        }
        else
        {
            newPos.x -= objectWidth;
        }

        return newPos;
    }



    static GameObject CreateTileInstance(GameObject original, Transform parent, Vector3 position)
    {
        GameObject duplicate;

        // Check if the original is a prefab instance
        if (PrefabUtility.IsPartOfAnyPrefab(original))
        {
            // Get the prefab asset
            GameObject prefabAsset = PrefabUtility.GetCorrespondingObjectFromOriginalSource(original);
            if (prefabAsset != null)
            {
                // Create a new prefab instance
                duplicate = (GameObject)PrefabUtility.InstantiatePrefab(prefabAsset, parent);
            }
            else
            {
                // Fallback to regular instantiate if we can't get the prefab asset
                duplicate = Instantiate(original, parent);
            }
        }
        else
        {
            // Not a prefab, use regular instantiate
            duplicate = Instantiate(original, parent);
        }

        // Set the name and position
        duplicate.name = original.name + "_Tile";
        duplicate.transform.position = position;

        return duplicate;
    }



    // Validate menu item - only enable when a sprite is selected
    [MenuItem("Tools/Tile Sprite Right", true)]
    static bool ValidateTileSpriteRight()
    {
        GameObject selected = Selection.activeGameObject;
        return selected != null && HasSpriteRenderers(selected);
    }

    // Validate menu item - only enable when a sprite is selected
    [MenuItem("Tools/Tile Sprite Left", true)]
    static bool ValidateTileSpriteLeft()
    {
        GameObject selected = Selection.activeGameObject;
        return selected != null && HasSpriteRenderers(selected);
    }

    // Validate menu items for isometric tiling
    [MenuItem("Tools/Tile Sprite Isometric Top Right", true)]
    static bool ValidateTileSpriteIsometricTopRight()
    {
        GameObject selected = Selection.activeGameObject;
        return selected != null && HasSpriteRenderers(selected);
    }

    [MenuItem("Tools/Tile Sprite Isometric Top Left", true)]
    static bool ValidateTileSpriteIsometricTopLeft()
    {
        GameObject selected = Selection.activeGameObject;
        return selected != null && HasSpriteRenderers(selected);
    }

    [MenuItem("Tools/Tile Sprite Isometric Bottom Right", true)]
    static bool ValidateTileSpriteIsometricBottomRight()
    {
        GameObject selected = Selection.activeGameObject;
        return selected != null && HasSpriteRenderers(selected);
    }

    [MenuItem("Tools/Tile Sprite Isometric Bottom Left", true)]
    static bool ValidateTileSpriteIsometricBottomLeft()
    {
        GameObject selected = Selection.activeGameObject;
        return selected != null && HasSpriteRenderers(selected);
    }
}
