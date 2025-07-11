#if UNITY_EDITOR
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    //[CreateAssetMenu(fileName = "Sample_Attribute_HideInLine", menuName = "Gaskellgames/GgSamplePage")]
    public class Sample_Attribute_HideInLine : ScriptableObject
    {
        // ---------- HideInLine ----------

        [SerializeField]
        private byte notHidden;

        [SerializeField, HideInLine]
        private byte hideInLine;

        [field: SerializeField, HideInLine]
        private byte HideInLine { get; set; }

    } // class end
}

#endif