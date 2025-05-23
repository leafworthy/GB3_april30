using UnityEngine;

[ExecuteAlways]
public class BillboardThis : MonoBehaviour
{

    private void LateUpdate()
    {
        if (transform.parent == null)
            return;

        // Preserve world position
        Vector3 worldPos = transform.position;

        // Set rotation to identity (no rotation)
        transform.rotation = Quaternion.identity;

        // Set scale to 1 (no scaling distortion)
        transform.localScale = Vector3.one;

        // Reapply original world position
        transform.position = worldPos;
    }
}
