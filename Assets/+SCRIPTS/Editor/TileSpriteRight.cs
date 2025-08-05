using UnityEngine;
using UnityEditor;

public class SpriteTiler : EditorWindow
{
    [MenuItem("Tools/Tile Sprite Right")]
    static void TileSpriteRight()
    {
        // Get the currently selected GameObject
        GameObject selected = Selection.activeGameObject;

        if (selected == null)
        {
            EditorUtility.DisplayDialog("No Selection", "Please select a GameObject with a SpriteRenderer component.", "OK");
            return;
        }

        // Check if the selected object has a SpriteRenderer
        SpriteRenderer spriteRenderer = selected.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            EditorUtility.DisplayDialog("Invalid Selection", "Selected GameObject must have a SpriteRenderer component.", "OK");
            return;
        }

        // Check if sprite exists
        if (spriteRenderer.sprite == null)
        {
            EditorUtility.DisplayDialog("No Sprite", "SpriteRenderer has no sprite assigned.", "OK");
            return;
        }

        // Record the operation for undo
        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Tile Sprite Right");

        // Find the rightmost sprite in the same parent
        Transform parent = selected.transform.parent;
        GameObject rightmostSprite = FindRightmostSprite(parent, selected);

        // Calculate the position for the new sprite
        Vector3 newPosition = CalculateNextPosition(rightmostSprite);

        // Duplicate the original sprite
        GameObject duplicate = Instantiate(selected, parent);
        duplicate.name = selected.name + "_Tile";
        duplicate.transform.position = newPosition;

        // Record the creation for undo
        Undo.RegisterCreatedObjectUndo(duplicate, "Create Tile");

        // Select the new duplicate so user can keep pressing the shortcut
        Selection.activeGameObject = duplicate;

        Undo.CollapseUndoOperations(undoGroup);

        Debug.Log($"Tiled sprite to position: {newPosition}");
    }

    static GameObject FindRightmostSprite(Transform parent, GameObject originalSprite)
    {
        GameObject rightmost = originalSprite;
        float rightmostX = originalSprite.transform.position.x;

        // If there's no parent, just use the original sprite
        if (parent == null)
        {
            return rightmost;
        }

        // Check all children of the parent (or all root objects if no parent)
        Transform[] children = parent != null ? parent.GetComponentsInChildren<Transform>() : FindObjectsOfType<Transform>();

        foreach (Transform child in children)
        {
            // Skip the parent itself
            if (child == parent) continue;

            // Only consider objects with SpriteRenderer and the same sprite
            SpriteRenderer childSR = child.GetComponent<SpriteRenderer>();
            if (childSR != null && childSR.sprite == originalSprite.GetComponent<SpriteRenderer>().sprite)
            {
                if (child.position.x > rightmostX)
                {
                    rightmostX = child.position.x;
                    rightmost = child.gameObject;
                }
            }
        }

        return rightmost;
    }

    static Vector3 CalculateNextPosition(GameObject sprite)
    {
        SpriteRenderer sr = sprite.GetComponent<SpriteRenderer>();
        Vector3 currentPos = sprite.transform.position;

        // Get the sprite's world-space width
        float spriteWidth = sr.sprite.bounds.size.x * sprite.transform.lossyScale.x;

        // Calculate new position offset by the sprite width
        Vector3 newPos = currentPos;
        newPos.x += spriteWidth;

        return newPos;
    }


    // Validate menu item - only enable when a sprite is selected
    [MenuItem("Tools/Tile Sprite Right", true)]
    static bool ValidateTileSpriteRight()
    {
        GameObject selected = Selection.activeGameObject;
        return selected != null && selected.GetComponent<SpriteRenderer>() != null && selected.GetComponent<SpriteRenderer>().sprite != null;
    }
}
